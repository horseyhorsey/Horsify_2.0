using Horsesoft.Horsify.SearchModule.Model;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Helpers;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class InstantSearchViewModel : HorsifySongPlayBindableBase, INavigationAware
    {
        private ISongDataProvider _songDataProvider;
        private IHorsifySongApi _horsifySongApi;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private static DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public InstantSearchViewModel(ISongDataProvider songDataProvider, IHorsifySongApi horsifySongApi, IRegionManager regionManager,
            IEventAggregator eventAggregator, IQueuedSongDataProvider queuedSongDataProvider, ILoggerFacade loggerFacade)
            : base(queuedSongDataProvider, eventAggregator, loggerFacade)
        {
            _songDataProvider = songDataProvider;
            _horsifySongApi = horsifySongApi;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;

            SearchModel.PropertyChanged += SearchModel_PropertyChanged;

            ShowSearchKeyboardViewCommand = new DelegateCommand<object>(OnShowSearchKeyboard);

            SearchResults = new ListCollectionView(SearchModel.AllJoinedTables);
            SearchResults.CurrentChanged += SearchResults_CurrentChanged;
        }

        #region Commands
        public ICommand ShowSearchKeyboardViewCommand { get; set; }
        #endregion

        #region Properties
        private SearchModel _searchModel = new SearchModel();
        public SearchModel SearchModel
        {
            get { return _searchModel; }
            set { SetProperty(ref _searchModel, value); }
        }

        private int _caretIndex;
        public int CursorPosition
        {
            get { return _caretIndex; }
            set { SetProperty(ref _caretIndex, value); }
        }

        public ICollectionView SearchResults { get; set; }

        private bool _searchKeyboardVisible = true;
        public bool SearchKeyboardVisible
        {
            get { return _searchKeyboardVisible; }
            set { SetProperty(ref _searchKeyboardVisible, value); }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Navigates to the song selected view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchResults_CurrentChanged(object sender, EventArgs e)
        {
            Log("Selected song...");
            try
            {
                if (SearchResults.CurrentItem != null)
                {
                    var song = SearchResults.CurrentItem as AllJoinedTable;
                    Log($"Navigating to song from InstantSearch : {song.Id} - {song.Artist} - {song.Title}", Category.Debug);
                    _regionManager.RequestNavigate(Regions.ContentRegion,
                        ViewNames.SongSelectedView, NavigationHelper.CreateSongNavigation(song));
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, Category.Exception);
            }
        }

        private void OnShowSearchKeyboard(object showSearch)
        {
            bool show = false;
            bool.TryParse(showSearch.ToString(), out show);
            SearchKeyboardVisible = show;
        }

        protected override void OnPlay(AllJoinedTable song = null)
        {
            if (song != null)
            {
                AllJoinedTable fullSong = null;
                Task.Run(async () =>
                {
                    fullSong = await GetSong(song);
                }).Wait();

                Log($"Playing song: {fullSong.FileLocation}");
                _eventAggregator.GetEvent<OnMediaPlay<AllJoinedTable>>()
                .Publish(fullSong);
            }
        }

        private Task<AllJoinedTable> GetSong(AllJoinedTable allJoinedTable)
        {
            return _songDataProvider.GetSongById(allJoinedTable.Id);
        }

        protected override void OnQueueSong(AllJoinedTable song = null)
        {
            if (song != null)
            {
                AllJoinedTable fullSong = null;
                Task.Run(async () =>
                {
                    fullSong = await GetSong(song);
                }).Wait();

                Log($"Queueing song: {fullSong.FileLocation}");
                base.QueueSong(fullSong);
            }
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Get songs, stop timer and populate collection            
            _dispatcherTimer.Stop();
            SearchModel.AllJoinedTables.Clear();

            RunSearch();
        }

        private void RunSearch()
        {
            var arr = new string[] { "*" + SearchModel.SearchText + "*" };
            IEnumerable<AllJoinedTable> results = null;
            Task.Run(async () =>
            {
                results = await _horsifySongApi.SearchLikeFiltersAsync(new SearchFilter(arr, SearchModel.SelectedSearchType), maxAmount: 100);
            }).ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var sType = SearchModel.SelectedSearchType;
                    if (sType == SearchType.Album)
                    {
                        SearchModel.AllJoinedTables.AddRange(results.OrderBy(x => x.Artist).ThenBy(x => x.Album));
                    }
                    else if (sType == SearchType.Artist)
                    {
                        SearchModel.AllJoinedTables.AddRange(results.OrderBy(x => x.Artist));
                    }
                    else
                    {
                        SearchModel.AllJoinedTables.AddRange(results.OrderBy(x => x.Title));
                    }
                });
            });
        }

        private void SearchModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SearchText")
            {
                var sText = SearchModel.SearchText;
                if (sText?.Length > 2)
                {
                    //Do the search after timer
                    if (_dispatcherTimer.IsEnabled)
                        _dispatcherTimer.Stop();

                    _dispatcherTimer.Start();
                }
                else
                {
                    SearchModel.AllJoinedTables.Clear();
                }
            }
        }
        #endregion

        #region Navigation
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {            
            try
            {
                Log("Navigating from InstantSearch.");
                //this.CursorPosition = 0;
                //this.SearchModel.SearchText = string.Empty;
                //this.SearchModel.AllJoinedTables.Clear();
            }
            catch (Exception ex)
            {
                Log($"Navigating from InstantSearch. {ex.Message}", Category.Exception);
            }
        }
        #endregion
    }
}
