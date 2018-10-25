using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Events;
using System.Windows.Input;
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

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class SearchedSongsViewModel : HorsifyBindableBase, INavigationAware
    {        
        private ISongDataProvider _songDataProvider;        
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IQueuedSongDataProvider _queuedSongDataProvider;
        private IDjHorsifyService _djHorsifyService;
        private ISearchHistoryProvider _searchHistory;
        private IRegionNavigationJournal _journal;
        public ICollectionView SongsListView { get; set; }

        #region Commands
        public DelegateCommand<AllJoinedTable> SongItemSelectedCommand
        { get; private set; }
        public DelegateCommand GetRandomSongsCommand
        { get; private set; }        
        #endregion

        #region Commands To Move
        public DelegateCommand<object> SelectedSortDescriptionCommand
        { get; private set; }
        public DelegateCommand<object> SelectedSortDirectionCommand
        { get; private set; }
        #endregion

        #region Constructor
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
        
            //SongsListView = CollectionViewSource.GetDefaultView(SearchedSongs);
            //SongsListView.CurrentChanged += (s, e) =>
            //{
            //    var selectedSong = SongsListView.CurrentItem as AllJoinedTable;
            //    _songDataProvider.SelectedSong = selectedSong;
            //};
            //SongsListView.SortDescriptions.Add(new SortDescription() { PropertyName = "Rating", Direction = ListSortDirection.Descending });

            SongItemSelectedCommand = new DelegateCommand<AllJoinedTable>(OnSongItemSelected);
            SelectedSortDescriptionCommand = new DelegateCommand<object>(OnSelectedSortDescription);
            SelectedSortDirectionCommand = new DelegateCommand<object>(OnSelectedSortDirection);

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

            GetRandomSongsCommand = new DelegateCommand(() =>
            {
                if(SearchedSongs?.Count != null)
                {
                    if (Amount > 0)
                    {
                        QueueRandomSongs(Amount);
                    }
                }
            });
        }

        #endregion

        #region Properties
        private RecentSearch _recentSearch;
        public RecentSearch RecentSearch
        {
            get { return _recentSearch; }
            set { SetProperty(ref _recentSearch, value); }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private int _ratingLower = 196;
        public int RatingLower
        {
            get { return _ratingLower; }
            set { SetProperty(ref _ratingLower, value); }
        }

        private int _ratngUpper = 255;
        public int RatingUpper
        {
            get { return _ratngUpper; }
            set { SetProperty(ref _ratngUpper, value); }
        }

        public ObservableCollection<AllJoinedTable> SearchedSongs { get; set; }
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
                }
                catch (Exception ex)
                {
                    Log($"{ex.Message}", Category.Exception, Priority.High);
                    throw;
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

            _regionManager.RequestNavigate("ContentRegion", "SearchedSongsView");
            //_eventAggregator.GetEvent<OnNavigateViewEvent<string>>().Publish();
        }

        private void UpdateSearchHistory(ISearchFilter filter)
        {
            var resultCnt = _songDataProvider.SearchedSongs?.Count;
            _searchHistory.RecentSearches.Last().ResultCount = resultCnt;

            Log($"Recent Search history count: {_searchHistory.RecentSearches.Count}");

            if (resultCnt != 0)
            {
                RecentSearch.ResultCount = (int)resultCnt;
                RecentSearch.SearchTerm = filter.Filters.ElementAt(0).Filters[0];
            }
            else
            {
                RecentSearch.ResultCount = 0;
                RecentSearch.SearchTerm = "No songs for: " + filter.Filters.ElementAt(0).Filters[0];
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
        private void QueueRandomSongs(int amount)
        {
            if (_songDataProvider.SearchedSongs?.Count > 0)
            {
                Log($"Queuing random songs", Category.Debug);

                var songs = new List<AllJoinedTable>();
                if (RatingLower > RatingUpper)
                    RatingUpper = RatingLower;

                var filtered = _songDataProvider.SearchedSongs
                    .Where(x => x.Rating >= this.RatingLower && 
                                x.Rating <= this.RatingUpper
                    ).ToArray();

                //Pick random songs from filtered
                var filterCount = filtered.Count() - 1;
                if (filterCount > 1)
                {
                    for (int i = 0; i < amount; i++)
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
        #endregion

        #region Sorting

        private SongFilterType _lastSortDescription;
        private ListSortDirection _lastSortDirection = ListSortDirection.Descending;
        private SearchFilter _lastSearchFilter;

        private void OnSelectedSortDirection(object obj)
        {
            if (obj != null)
            {
                ListSortDirection sortDirection = ListSortDirection.Descending;
                if ((string)obj == "Ascending")
                {
                    sortDirection = ListSortDirection.Ascending;
                }

                if (sortDirection != _lastSortDirection)
                {
                    _lastSortDirection = sortDirection;
                    UpdateSortDescription();
                }
            }
        }

        private void OnSelectedSortDescription(object obj)
        {
            var searchType = (obj as object[])?[0];
            if (searchType != null)
            {
                SortDescription((SongFilterType)searchType);
            }
        }

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
            SongsListView.SortDescriptions.Clear();
            SongsListView.SortDescriptions.Add(new SortDescription()
            { PropertyName = _lastSortDescription.ToString(), Direction = _lastSortDirection });
        }
        #endregion
    }
}
