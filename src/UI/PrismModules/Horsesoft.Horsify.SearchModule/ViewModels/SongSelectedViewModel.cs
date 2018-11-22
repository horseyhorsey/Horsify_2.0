using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Helpers;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public interface ISongSelectedViewModel
    {
        ICommand PlayCommand { get; set; }        
        ICommand QueueSongsCommand { get; set; }
        ICommand SearchSongsCommand { get; set; }
        AllJoinedTable SelectedSong { get; set; }
    }

    public class SongSelectedViewModel : HorsifyBindableBase, ISongSelectedViewModel, INavigationAware
    {
        #region Services
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IQueuedSongDataProvider _queuedSongDataProvider;
        private IRegionNavigationJournal _journal; 
        #endregion

        #region Commands
        public ICommand GoBackCommand { get; set; }
        public ICommand PlayCommand { get; set; }
        public ICommand QueueSongsCommand { get; set; }
        public ICommand SearchSongsCommand { get; set; }
        public ICommand AddToPlayListCommand { get; private set; }
        #endregion

        #region Constructor
        public SongSelectedViewModel(IQueuedSongDataProvider queuedSongDataProvider, IEventAggregator eventAggregator, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _queuedSongDataProvider = queuedSongDataProvider;

            GoBackCommand = new DelegateCommand(OnGoBack);
            PlayCommand = new DelegateCommand(OnPlay);
            QueueSongsCommand = new DelegateCommand(OnQueueSong);
            SearchSongsCommand = new DelegateCommand<string>(OnSearchSongs);            
            AddToPlayListCommand = new DelegateCommand<AllJoinedTable>(OnAddToPlayListCommand);
        }

        #endregion        

        #region Properties
        private AllJoinedTable _selectedSong;
        public AllJoinedTable SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                SetProperty(ref _selectedSong, value);
            }
        }
        #endregion

        #region Navigation Aware

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            var songItem = navigationContext.Parameters["song"] as AllJoinedTable;
            //Check if the song isn't the same otherwise allow this as a target
            if (songItem != null)
                return SelectedSong != null && SelectedSong.FileLocation == songItem.FileLocation;
            else
                return true;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _journal = navigationContext.NavigationService.Journal;

            var songItem = navigationContext.Parameters["song"] as AllJoinedTable;
            if (songItem != null)
            {                
                SelectedSong = songItem;
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Called when [go back] to show the SearchedSongsView
        /// </summary>
        private void OnGoBack()
        {
            Log($"Navigating back from {_journal?.CurrentEntry.Uri}");
            _journal?.GoBack();
        }

        private void OnPlay()
        {
            Log("Song selected play");
            OnGoBack();

            _eventAggregator.GetEvent<OnMediaPlay<AllJoinedTable>>()
                .Publish(SelectedSong);
        }

        /// <summary>
        /// Called when [queue song]. Adds to the queue provider.
        /// </summary>
        private void OnQueueSong()
        {
            _queuedSongDataProvider.QueueSongs.Add(SelectedSong);
        }

        private void OnSearchSongs(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (SelectedSong != null)
                {
                    try
                    {
                        Log("Creating search...");
                        SearchType searchType = (SearchType)Enum.Parse(typeof(SearchType), str);
                        string searchTerm = SelectedSong.GetType().GetProperty(str).GetValue(SelectedSong).ToString();

                        NavigationParameters navParams = NavigationHelper.CreateSearchFilterNavigation(searchType, searchTerm); 
                        _regionManager.RequestNavigate("ContentRegion", "SearchedSongsView", navParams);
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message, Category.Exception);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [add to play list command] which adds the song to the current Preparation Playlist
        /// </summary>
        /// <param name="song">The song.</param>
        private void OnAddToPlayListCommand(AllJoinedTable song)
        {
            _eventAggregator.GetEvent<AddToPlaylistEvent>().Publish(new Dictionary<string, AllJoinedTable>()
            {
                { "Preparation Playlist", song }
            });
        }

        #endregion
    }
}
