using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Events;
using Prism.Commands;
using System.Linq;
using Prism.Regions;
using Horsesoft.Horsify.SearchModule.Model;
using System.Collections.Generic;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Prism.Logging;
using Horsesoft.Music.Horsify.Base.ViewModels;
using System.Collections.ObjectModel;
using Prism.Interactivity.InteractionRequest;
using System.Windows.Data;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class SearchedSongsViewModel : HorsifyBindableBase, INavigationAware
    {
        #region Fields
        private ISongDataProvider _songDataProvider;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IQueuedSongDataProvider _queuedSongDataProvider;
        private IDjHorsifyService _djHorsifyService;
        private ISearchHistoryProvider _searchHistory;
        private IRegionNavigationJournal _journal;        
        #endregion

        #region Commands
        public DelegateCommand<string> RequestViewCommand{ get; private set; }

        #endregion

        #region Requests
        public InteractionRequest<INotification> RequestRandomViewRequest { get; private set; }
        public InteractionRequest<INotification> RequestSortDialogRequest { get; private set; }
        #endregion

        #region Constructors
        public SearchedSongsViewModel(ISongDataProvider songDataProvider,
            IQueuedSongDataProvider queuedSongDataProvider, ISearchHistoryProvider searchHistory,
            IEventAggregator eventAggregator, IRegionManager regionManager, IDjHorsifyService djHorsifyService, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _queuedSongDataProvider = queuedSongDataProvider;
            _djHorsifyService = djHorsifyService;

            _songDataProvider = songDataProvider;
            _searchHistory = searchHistory;
            RecentSearch = new RecentSearch();

            SearchedSongs = _songDataProvider.SearchedSongs;
            SongsListView = new ListCollectionView(_songDataProvider.SearchedSongs);

            SongsListView.CurrentChanged += SongsListView_CurrentChanged;

            _eventAggregator.GetEvent<OnSearchedSongEvent>().Subscribe(async () =>
            {
                await OnSearchedSong();
            });

            //Runs the seach and adds filter to recent searches
            _eventAggregator.GetEvent<OnSearchedSongEvent<ISearchFilter>>()
                .Subscribe(async filter =>
                {
                    await OnSearchedSong(filter);
                }, ThreadOption.UIThread);

            //Dialog requests
            RequestRandomViewRequest = new InteractionRequest<INotification>();
            RequestSortDialogRequest = new InteractionRequest<INotification>();
            RequestViewCommand = new DelegateCommand<string>((viewName) => OnRequestView(viewName));
        }

        #endregion

        #region Properties
        private RecentSearch _recentSearch;
        public RecentSearch RecentSearch
        {
            get { return _recentSearch; }
            set { SetProperty(ref _recentSearch, value); }
        }

        private string _SongResultInfo;
        public string SongResultInfo
        {
            get { return _SongResultInfo; }
            set { SetProperty(ref _SongResultInfo, value); }
        }

        public ObservableCollection<AllJoinedTable> SearchedSongs { get; set; }
        public ICollectionView SongsListView { get; set; }
        #endregion

        #region Navigation

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _journal = navigationContext.NavigationService.Journal;

            if (navigationContext.Parameters.Count() > 0)
            {
                //Convert incoming search filter
                try
                {
                    var filter = navigationContext.Parameters["djhorsify_search"] as SearchFilter;
                    if (filter != null)
                    {
                        this.RecentSearch.ResultCount = 0;
                        _songDataProvider.SearchLikeFiltersAsync(filter, 0)
                            .ContinueWith((x) =>
                            {
                                AddToSearchHistory(filter);
                                _lastSearchFilter = filter;
                                this.RecentSearch.SearchTerm = "DjHorsify ";
                                UpdateSearchHistory(filter);
                                PublishSearchFinished();
                            });

                        return;
                    }


                    filter = navigationContext.Parameters["search_filter"] as SearchFilter;
                    if (filter != null)
                    {
                        if (_searchHistory.RecentSearches.Count > 0)
                        {
                            if (_lastSearchFilter != null)
                            {
                                if (filter.Equals(_lastSearchFilter))
                                {
                                    PublishSearchFinished();
                                    return;
                                }
                            }
                        }

                        this.RecentSearch.ResultCount = 0;
                        _songDataProvider.SearchLikeFiltersAsync(filter, 0)
                            .ContinueWith((x) =>
                            {
                                AddToSearchHistory(filter);
                                _lastSearchFilter = filter;
                                UpdateSearchHistory(filter);
                                PublishSearchFinished();
                            });
                        return;
                    }

                    var searchType = (ExtraSearchType)navigationContext.Parameters["extra_search"];
                    if (searchType != ExtraSearchType.None)
                    {
                        this.RecentSearch.ResultCount = 0;
                        this.RecentSearch.SearchTerm += searchType;

                        //TODO: REmove this
                        //SongsListView.SortDescriptions.Clear();

                        //Just let the query order the songs, the add to search history will clear the last
                        _songDataProvider.ExtraSearch(searchType)
                            .ContinueWith((x) =>
                            {
                                AddToSearchHistory(searchType);
                                PublishSearchFinished();
                            });

                        //this.SortDescription(SongFilterType.TimesPlayed);
                    }

                    SongsListView.SortDescriptions.Clear();
                }
                catch (Exception ex)
                {
                    Log($"{ex.Message}", Category.Exception, Priority.High);
                }
                finally
                {

                }
            }
        }

        private void PublishSearchFinished()
        {
            _eventAggregator.GetEvent<HorsifySearchCompletedEvent>().Publish();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _journal?.GoForward();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Adds to the search filter history lists and RecentSearch
        /// </summary>
        /// <param name="filter">The filter.</param>
        private void AddToSearchHistory(ISearchFilter filter)
        {
            Log($"Adding filter to search history");
            _searchHistory.RecentSearches.Add(new SearchHistory()
            {
                Filter = filter,
            });
        }

        /// <summary>
        /// Adds the extra search type to the search history list.
        /// </summary>
        /// <param name="extraSearchType">Type of the extra search.</param>
        private void AddToSearchHistory(ExtraSearchType extraSearchType)
        {
            var resultCnt = _songDataProvider.SearchedSongs?.Count;
            var history = new SearchHistory();
            history.ResultCount = resultCnt;

            RecentSearch.ResultCount = (int)resultCnt;
            RecentSearch.SearchTerm = extraSearchType.ToString();
        }

        private async Task OnSearchedSong()
        {
            try
            {
                //await _songDataProvider
                await _songDataProvider.SearchAsync(
                    SearchType.Artist, "%Jackson", 5);
            }
            catch (Exception ex)
            {
                Log(ex.Message, Category.Exception);
                System.Windows.MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Called when [searched song]. Adds the result count and filter into history
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        private async Task OnSearchedSong(ISearchFilter filter)
        {
            //Log($"Searching songs..{string.Join(",", filter.Filters.Select(x => x.Filter.SearchTerms))}", Category.Debug);
            try
            {
                await _songDataProvider.SearchLikeFiltersAsync(filter as SearchFilter, 0);
                Log($"Search completed..");

                AddToSearchHistory(filter);
            }
            catch (Exception ex)
            {
                Log($"Search: {filter.Filters.ElementAt(0)?.Filters[0]} - {ex.Message}", Category.Exception);
            }

            UpdateSearchHistory(filter);

            //_regionManager.RequestNavigate("ContentRegion", "SearchedSongsView");
            _eventAggregator.GetEvent<OnNavigateViewEvent<string>>().Publish("SearchedSongsView");
        }

        /// <summary>
        /// Opens up song selection view when CurrentItem changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SongsListView_CurrentChanged(object sender, EventArgs e)
        {
            var song = SongsListView.CurrentItem as AllJoinedTable;
            if (song == null)
            {
                Log("Selected song is null", Category.Warn);
                return;
            }

            OnSongItemSelected(song);
        }

        /// <summary>
        /// Requests for a view control. Eg: RandomSelectView
        /// </summary>
        /// <remarks>
        /// RandomSelectView
        /// </remarks>
        /// <param name="viewName"></param>
        private void OnRequestView(string viewName)
        {
            if (viewName == "RandomSelectView")
            {
                if (SearchedSongs?.Count != null)
                {
                    this.RequestRandomViewRequest.Raise(new Notification { Content = "Notification Message", Title = "Horsify - Random Select" }
                    , r =>
                    {
                        Log("Queuing songs from selection RandomSelectView");

                        RandomSelectOption randomSelectOption = r.Content as RandomSelectOption;
                        if (randomSelectOption?.Amount > 0)
                        {
                            QueueRandomSongs(randomSelectOption);
                            Log("Queued songs from selection RandomSelectView");
                        }
                    });
                }
            }
            else if (viewName == "SortDialogView")
            {
                this.RequestSortDialogRequest.Raise(new Notification { Content = SongsListView, Title = "Sort " },
                    r =>
                    {
                        this.UpdateSortingInfo();
                        Log("Opening sort dialog");
                        Log($"Sort descriptions Count after open dialog: {SongsListView.SortDescriptions.Count}");
                    });
            }
        }

        private void OnSongItemSelected(AllJoinedTable songItem)
        {
            if (songItem != null)
            {
                Log($"Selected song : {songItem.FileLocation}", Category.Debug);
                var navParams = new NavigationParameters();
                navParams.Add("song", songItem);
                _regionManager.RequestNavigate("ContentRegion", "SongSelectedView", navParams);
            }
        }

        private Random _random = new Random();
        private void QueueRandomSongs(RandomSelectOption options)
        {
            if (_songDataProvider.SearchedSongs?.Count > 0)
            {
                Log($"Queuing random songs", Category.Debug);

                var songs = new List<AllJoinedTable>();

                var filtered = _songDataProvider.SearchedSongs
                    .Where(x => x.Rating >= options.RatingLower &&
                                x.Rating <= options.RatingHigher
                    ).ToArray();

                //Pick random songs from filtered
                var filterCount = filtered.Count() - 1;
                if (filterCount > 1)
                {
                    for (int i = 0; i < options.Amount; i++)
                    {
                        //Select random id from the fitlered count.
                        //Only add song if not already exisiting.
                        var id = _random.Next(filterCount);
                        var song = filtered[id];
                        if (!_queuedSongDataProvider.QueueSongs.Contains(song))
                            _queuedSongDataProvider.QueueSongs.Add(song);
                    }
                }
            }
        }

        private void UpdateSearchHistory(ISearchFilter filter)
        {
            var resultCnt = _songDataProvider.SearchedSongs?.Count;
            _searchHistory.RecentSearches.Last().ResultCount = resultCnt;

            Log($"Recent Search history count: {_searchHistory.RecentSearches.Count}");

            RecentSearch.SearchTerm = filter.Filters?.ElementAt(0).Filters[0];
            if (resultCnt != 0)
            {
                RecentSearch.ResultCount = (int)resultCnt;
                Log($"Songs found: {RecentSearch.ResultCount}");

                UpdateSortingInfo();
            }
            else
            {
                RecentSearch.ResultCount = 0;
                Log("No Songs found in search");
                RecentSearch.SearchTerm = filter.Filters?.ElementAt(0).Filters[0];
                SongResultInfo = $"No songs found";
            }

        }

        private void UpdateSortingInfo()
        {
            SongResultInfo = null;
            SongResultInfo += " Sorted by: ";
            foreach (var sortDesc in SongsListView.SortDescriptions)
            {
                SongResultInfo += sortDesc.PropertyName + " " + sortDesc.Direction.ToString();
            }
        }

        #endregion

        #region Sorting

        private SongFilterType _lastSortDescription;
        private ListSortDirection _lastSortDirection = ListSortDirection.Descending;
        private SearchFilter _lastSearchFilter;

        /// <summary>
        /// Sorts the description and sets the last description from the incoming filter type
        /// </summary>
        /// <param name="songFilterType">Type of the song filter.</param>
        private void SortDescription(SongFilterType songFilterType)
        {
            _lastSortDescription = songFilterType;
            UpdateSortDescription();
        }

        private void UpdateSortDescription()
        {
            //TODO: Check if needed anymore
            SongsListView.SortDescriptions.Clear();
            SongsListView.SortDescriptions.Add(new SortDescription()
            { PropertyName = _lastSortDescription.ToString(), Direction = _lastSortDirection });
        }
        #endregion
    }
}
