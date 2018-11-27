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
using System.Windows;
using Horsesoft.Music.Horsify.Base.Helpers;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class SearchedSongsViewModel : HorsifySongPlayBindableBase, INavigationAware, IConfirmNavigationRequest
    {
        #region Fields
        private IDjHorsifyService _djHorsifyService;
        private IVoiceControl _voiceControl;
        private IEventAggregator _eventAggregator;
        private IRegionNavigationJournal _journal;
        private IRegionManager _regionManager;
        private ISearchHistoryProvider _searchHistory;
        private ISongDataProvider _songDataProvider;
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
            IQueuedSongDataProvider queuedSongDataProvider, ISearchHistoryProvider searchHistory, IVoiceControl voiceControl,
            IEventAggregator eventAggregator, IRegionManager regionManager, IDjHorsifyService djHorsifyService, ILoggerFacade loggerFacade) : base(queuedSongDataProvider, eventAggregator, loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            _djHorsifyService = djHorsifyService;
            _voiceControl = voiceControl;
            _voiceControl.VoiceCommandSent += _voiceControl_VoiceCommandSent;

            _songDataProvider = songDataProvider;
            _searchHistory = searchHistory;
            RecentSearch = new RecentSearch();

            SearchedSongs = _songDataProvider.SearchedSongs;
            SongsListView = new ListCollectionView(_songDataProvider.SearchedSongs);

            SongsListView.CurrentChanged += SongsListView_CurrentChanged;

            //Dialog requests
            RequestRandomViewRequest = new InteractionRequest<INotification>();            
            RequestSortDialogRequest = new InteractionRequest<INotification>();
            RequestViewCommand = new DelegateCommand<string>((viewName) => OnRequestView(viewName));
        }

        private void _voiceControl_VoiceCommandSent(string voicetext)
        {
            Log(voicetext);

            if (voicetext == "horsify search")
            {
                HorsifyVoiceActive = true;
                Log($"{voicetext} - activated");
                VoiceHelper = "Horsify Search";
            }          
            else if(voicetext == "horsify play")
            {
                HorsifyVoiceActive = true;
                Log($"{voicetext} - activated");
                VoiceHelper = "What's the Song Id?";
            }
            else if (voicetext == "horsify queue")
            {
                HorsifyVoiceActive = true;
                Log($"{voicetext} - activated");
                VoiceHelper = "Queue Song Id...";
            }
            else
            {
                if (_voiceControl.Command == VoiceCommand.Search)
                {
                    _regionManager.RequestNavigate(Regions.ContentRegion, ViewNames.SearchedSongsView, NavigationHelper.CreateSearchFilterNavigation(
                        new SearchFilter($"%{voicetext}%")));

                    HorsifyVoiceActive = false;
                }        
                else if (_voiceControl.Command == VoiceCommand.Play)
                {
                    var songId = 0;
                    int.TryParse(voicetext, out songId);

                    if (songId > 0)
                    {
                        var songToPlay = this.SearchedSongs
                        .FirstOrDefault((x) => x.Id == songId);

                        if (songToPlay != null)
                        {
                            Log($"Playing song {voicetext}");
                            _eventAggregator
                                .GetEvent<OnMediaPlay<AllJoinedTable>>().Publish(songToPlay);
                        }
                        else
                        {
                            Log($"Couldn't find song to play: {voicetext}");
                        }
                    }

                    HorsifyVoiceActive = false;
                }
                else if (_voiceControl.Command == VoiceCommand.Queue)
                {
                    var songId = 0;
                    int.TryParse(voicetext, out songId);

                    if (songId > 0)
                    {
                        var songToPlay = this.SearchedSongs
                        .FirstOrDefault((x) => x.Id == songId);

                        if (songToPlay != null)
                        {
                            Log($"Queueing song {voicetext}");
                            _queuedSongDataProvider.Add(songToPlay);
                        }
                        else
                        {
                            Log($"Couldn't find song to queue: {voicetext}");
                        }
                    }

                    HorsifyVoiceActive = false;
                }
            }
        }



        #endregion

        #region Properties

        private int _lastPostion;
        private SearchFilter _lastSearchFilter;
        private AllJoinedTable _lastSelectedSong;
        private bool _randomDialogOpen;
        private RecentSearch _recentSearch;
        private string _SongResultInfo;

        private bool _sortDialogOpen;

        public RecentSearch RecentSearch
        {
            get { return _recentSearch; }
            set { SetProperty(ref _recentSearch, value); }
        }
        public ObservableCollection<AllJoinedTable> SearchedSongs { get; set; }

        public string SongResultInfo
        {
            get { return _SongResultInfo; }
            set { SetProperty(ref _SongResultInfo, value); }
        }
        public ICollectionView SongsListView { get; set; }        

        private bool _isBusy;
        /// <summary>
        /// Gets or Sets the IsBusy flag
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private bool _voiceActive;
        /// <summary>
        /// Gets or Sets the IsBusy flag
        /// </summary>
        public bool HorsifyVoiceActive
        {
            get { return _voiceActive; }
            set { SetProperty(ref _voiceActive, value); }
        }

        private string _voiceHelper;
        /// <summary>
        /// Gets or Sets the IsBusy flag
        /// </summary>
        public string VoiceHelper
        {
            get { return _voiceHelper; }
            set { SetProperty(ref _voiceHelper, value); }
        }
        #endregion

        #region Navigation

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            //var views = _regionManager.Regions[Regions.ContentRegion].ActiveViews;
            //var viewName = (views?.FirstOrDefault())?.ToString();
            //var result = viewName.Contains(ViewNames.SearchedSongsView) ? false : true;
            //Log($"Can navigate? {result}");            

            continuationCallback(CanNavigateFrom());
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //_journal?.GoForward();
            //TODO: Close dialogs when leaving view or make the dialogs as views
            //if (_sortDialogOpen || _randomDialogOpen)
            //{
            //    Log("Cannot Navigate away with dialogs open");
            //    return;
            //}            
        }

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

                        IsBusy = true;
                        this.RecentSearch.ResultCount = 0;
                        _songDataProvider.SearchLikeFiltersAsync(filter, 0)
                            .ContinueWith((x) =>
                            {
                                AddToSearchHistory(filter);
                                _lastSearchFilter = new SearchFilter();
                                _lastSearchFilter.BpmRange.IsEnabled = filter.BpmRange.IsEnabled;
                                _lastSearchFilter.BpmRange.Low = filter.BpmRange.Low;
                                _lastSearchFilter.BpmRange.Hi = filter.BpmRange.Hi;
                                _lastSearchFilter.RatingRange.IsEnabled = filter.RatingRange.IsEnabled;
                                _lastSearchFilter.RatingRange.Low = filter.RatingRange.Low;
                                _lastSearchFilter.RatingRange.Hi = filter.RatingRange.Hi;
                                _lastSearchFilter.Filters = filter.Filters;
                                _lastSearchFilter.MusicKeys = filter.MusicKeys;
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
                        
                        RunSearchFilter(filter);
                        return;
                    }

                    var searchType = (ExtraSearchType)navigationContext.Parameters["extra_search"];
                    if (searchType != ExtraSearchType.None)
                    {
                        //Don't run the same search again if navigating to it
                        if (RecentSearch.SearchTerm == searchType.ToString())
                            return;

                        this.RecentSearch.ResultCount = 0;
                        this.RecentSearch.SearchTerm += searchType;

                        IsBusy = true;
                        //Just let the query order the songs, the add to search history will clear the last
                        _songDataProvider.ExtraSearch(searchType)
                            .ContinueWith((x) =>
                            {
                                AddToSearchHistory(searchType);
                                PublishSearchFinished();
                            });
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

        private void RunSearchFilter(SearchFilter filter)
        {
            IsBusy = true;

            this.RecentSearch.ResultCount = 0;
            _songDataProvider.SearchLikeFiltersAsync(filter, 0)
                .ContinueWith((x) =>
                {
                    AddToSearchHistory(filter);
                    _lastSearchFilter = filter;
                    UpdateSearchHistory(filter);
                    PublishSearchFinished();
                });
        }

        private bool CanNavigateFrom()
        {
            return (!_randomDialogOpen && !_sortDialogOpen);
        }

        private void PublishSearchFinished()
        {
            IsBusy = false;
            _eventAggregator.GetEvent<HorsifySearchCompletedEvent>().Publish();
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

        private void OnRequestRandomSelect(string viewName)
        {
            _randomDialogOpen = true;
            Log($"Opening {viewName}");

            this.RequestRandomViewRequest.Raise(
                new Notification { Content = "Notification Message", Title = "Horsify - Random Select" }
            , r =>
            {
                Log("Queuing songs from selection RandomSelectView");
                _randomDialogOpen = false;
                //TODO: A better way to re-enable the side bar on completion
                _eventAggregator.GetEvent<HorsifySearchCompletedEvent>().Publish();
                RandomSelectOption randomSelectOption = r.Content as RandomSelectOption;
                if (randomSelectOption?.Amount > 0)
                {
                    QueueRandomSongs(randomSelectOption);
                    Log("Queued songs from selection RandomSelectView");
                }
            });
        }

        private void OnRequestSortView(string viewName)
        {
            _sortDialogOpen = true;
            Log($"Opening {viewName}");

            OpenSortingView();
        }

        /// <summary>
        /// Opens the Sorting View / Vm
        /// //TODO: A better way to re-enable the side bar on completion
        /// </summary>
        private void OpenSortingView()
        {            
            _eventAggregator.GetEvent<HorsifySearchCompletedEvent>().Publish();
            SongsListView.CurrentChanged -= SongsListView_CurrentChanged;
            this.RequestSortDialogRequest.Raise(new Notification { Content = null, Title = "Sort " }, r => { OnSortViewClosed(r);});
        }

        /// <summary>
        /// Retrives the Sortdescription from the SortViewDialog. Appies to <see cref="SongListView"/>
        /// </summary>
        /// <param name="r"></param>
        private void OnSortViewClosed(INotification r)
        {
            if (r.Content != null)
            {
                //Get the sortdescription from dialog
                var sortDescription = (SortDescription)r.Content;
                if (sortDescription.PropertyName != null)
                {
                    var sortDescriptions = SongsListView.SortDescriptions;
                    if (sortDescriptions != null)
                    {
                        if (sortDescriptions.Count > 0)
                        {
                            //Clear the previous sort decs if not null
                            var sortDesc = sortDescriptions.FirstOrDefault(x => x.PropertyName == sortDescription.PropertyName);
                            if (sortDesc != null)
                                sortDescriptions.Clear();
                        }

                        //Add the desc and assign changed event
                        SongsListView.SortDescriptions.Add(sortDescription);
                        SongsListView.CurrentChanged += SongsListView_CurrentChanged;
                    }

                    this.UpdateSortingInfo();

                    Log($"Sort descriptions Count after open dialog: {SongsListView.SortDescriptions.Count}");
                }
                else
                {
                    Log("Sort property name is null", Category.Exception);
                }
            }

            //Disable opened
            _sortDialogOpen = false;
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
            if (SearchedSongs?.Count > 0)
            {
                if (!CanNavigateFrom())
                    return;

                switch (viewName)
                {
                    case ViewNames.RandomSelectView:
                        OnRequestRandomSelect(viewName);
                        break;
                    case ViewNames.SortDialogView:
                        OnRequestSortView(viewName);
                        break;
                    default:
                        break;
                }
            }
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
            _eventAggregator.GetEvent<OnNavigateViewEvent<string>>().Publish(ViewNames.SearchedSongsView);
        }

        private void OnSongItemSelected(AllJoinedTable songItem)
        {
            Log($"Selected song : {songItem.FileLocation}", Category.Debug);
            var navParams = new NavigationParameters();
            navParams.Add("song", songItem);
            _regionManager.RequestNavigate(Regions.ContentRegion, ViewNames.SongSelectedView, navParams);
            _lastSelectedSong = songItem;
        }

        private void QueueRandomSongs(RandomSelectOption options)
        {
            if (_songDataProvider.SearchedSongs?.Count > 0)
            {
                Log($"random songs from songs count: {_songDataProvider.SearchedSongs?.Count}", Category.Debug);

                IEnumerable<AllJoinedTable> filtered = null;
                if (options.RatingRange.IsEnabled)
                {
                    Log("Rating random selected");
                    filtered = _songDataProvider.SearchedSongs
                   .Where(x => x.Rating >= options.RatingRange.Low &&
                               x.Rating <= options.RatingRange.Hi
                   );
                }
                else
                {
                    Log("Rating random not selected");
                    filtered = _songDataProvider.SearchedSongs;
                }

                Log($"Queuing {options.Amount} random");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _queuedSongDataProvider.QueueSongRange(filtered, options.Amount);
                });
                
            }
        }

        /// <summary>
        /// Opens up song selection view when CurrentItem changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SongsListView_CurrentChanged(object sender, EventArgs e)
        {
            //Need to track position in the case of another view selecting the item for us on focus.
            var currPos = (sender as ICollectionView).CurrentPosition;
            if (_lastPostion != currPos)
            {
                _lastPostion = currPos;
                var song = SongsListView.CurrentItem as AllJoinedTable;
                if (song == null)
                {
                    Log("Selected song is null", Category.Warn);
                    return;
                }

                OnSongItemSelected(song);                
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
    }
}
