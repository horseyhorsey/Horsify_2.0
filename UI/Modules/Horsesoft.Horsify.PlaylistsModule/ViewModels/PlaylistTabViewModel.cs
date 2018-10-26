using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Horsesoft.Horsify.PlaylistsModule.ViewModels
{
    public class PlaylistTabViewModel : HorsifyBindableBase
    {
        #region Services
        private IEventAggregator _eventAggregator;
        private IHorsifyPlaylistService _horsifyPlaylistService;
        private IQueuedSongDataProvider _queuedSongDataProvider;
        #endregion

        #region Commands                
        public ICommand AddToQueueCommand { get; set; }
        public ICommand ClearItemsCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }        
        public ICommand SavePlaylistCommand { get; set; }               
        public ICommand PlayItemCommand { get; set; }
        #endregion

        #region Constructors
        public PlaylistTabViewModel(IHorsifyPlaylistService horsifyPlaylistService, IQueuedSongDataProvider queuedSongDataProvider, IEventAggregator eventAggregator, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _horsifyPlaylistService = horsifyPlaylistService;
            _queuedSongDataProvider = queuedSongDataProvider;
            _eventAggregator = eventAggregator;

            //Create playlist item collection and assign the ICollectionView
            PlayListItemViewModels = new ObservableCollection<PlaylistItemViewModel>();
            PlayListItems = new ListCollectionView(PlayListItemViewModels);

            //Hook up to the add to Playlist event. 
            //Only fires if the key of the dictionary is the same as this playlist name
            _eventAggregator.GetEvent<AddToPlaylistEvent>()
                .Subscribe(OnAddToPlayList, ThreadOption.PublisherThread, false,
                x => x.ContainsKey(TabHeader));

            ClearItemsCommand = new DelegateCommand(OnClearItemsCommand);
            SavePlaylistCommand = new DelegateCommand(OnSavePlaylistCommand);
            AddToQueueCommand = new DelegateCommand<PlaylistItemViewModel>(OnAddToQueue);
            RemoveItemCommand = new DelegateCommand<PlaylistItemViewModel>(OnRemoveItem);
            PlayItemCommand = new DelegateCommand<PlaylistItemViewModel>(OnPlayItem);
        }

        #endregion

        #region Properties

        public ICollectionView PlayListItems { get; }

        /// <summary>
        /// Gets the play list item view models. These items are the songs.
        /// </summary>
        /// <value>
        /// The play list item view models.
        /// </value>
        private ObservableCollection<PlaylistItemViewModel> _playlistItemViewModels;
        public ObservableCollection<PlaylistItemViewModel> PlayListItemViewModels
        {
            get { return _playlistItemViewModels; }
            set { SetProperty(ref _playlistItemViewModels, value); }
        }

        private string _name;
        /// <summary>
        /// Gets or sets the name of the Current Playlist
        /// </summary>
        public string TabHeader
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private Playlist _playlist;
        public Playlist Playlist
        {
            get { return _playlist; }
            set { SetProperty(ref _playlist, value); }
        }
        #endregion

        #region Public Methods

        public int GetItemCount()
        {
            return this.PlayListItemViewModels.Count;
        }

        public void AddPlaylistItem(PlaylistItemViewModel playlistItemViewModel)
        {            
            if (!this.PlayListItemViewModels.Any(x => x == playlistItemViewModel))
                this.PlayListItemViewModels.Add(playlistItemViewModel);
        }
        #endregion

        #region Private methods

        internal void ClearItems()
        {
            this.PlayListItemViewModels.Clear();
        }

        /// <summary>
        /// Called when a song is added to the Playlist. Usually from the preparation list.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OnAddToPlayList(Dictionary<string, AllJoinedTable> obj)
        {            
            if (!this.PlayListItemViewModels.Any(x => x.Song == obj[TabHeader]))
            {
                Log($"Adding to playlist");
                this.PlayListItemViewModels.Add(new PlaylistItemViewModel(_loggerFacade)
                {
                    Song = obj[TabHeader]
                });
            }            
        }

        private void OnAddToQueue(PlaylistItemViewModel vm)
        {
            Log($"Adding to queue");
            _queuedSongDataProvider.Add(vm.Song);
        }

        /// <summary>
        /// Clears all the playlist items
        /// </summary>
        private void OnClearItemsCommand()
        {
            Log($"Clearing playlist items");
            PlayListItemViewModels?.Clear();
        }

        /// <summary>
        /// Called when [save playlist command], converts the songs to ids and saves to db
        /// </summary>
        private async void OnSavePlaylistCommand()
        {
            Log($"Saving playlist");
            var songItemsString = PlayListItemViewModels?.Select(x => x.Song.Id + "," + x.PlayedState);

            if (songItemsString != null)
            {
                var jointStr = string.Join(";", songItemsString);

                if (this.Playlist == null)
                {
                    this.Playlist = new Playlist();
                    this.Playlist.Id = 0;
                    this.Playlist.Name = this.TabHeader;
                }
                                    
                this.Playlist.Count = PlayListItemViewModels.Count;
                this.Playlist.Items = jointStr;

                await _horsifyPlaylistService.SavePlaylistAsync(new Playlist[] { this.Playlist });
            }
        }

        /// <summary>
        /// Updates the playlist values. Creates one if null
        /// </summary>
        /// <param name="jointStr">The joint string.</param>
        private void UpdatePlaylistValues(string jointStr)
        {

        }

        private void OnRemoveItem(PlaylistItemViewModel obj)
        {
            if(this.PlayListItemViewModels.Any(x => x == obj))
            {                
                this.PlayListItemViewModels.Remove(obj);
                Log("Removed playlist item");
            }
        }

        private void OnPlayItem(PlaylistItemViewModel obj)
        {            
            _eventAggregator
                .GetEvent<OnMediaPlay<AllJoinedTable>>()
                .Publish(obj.Song);
            Log("Playing playlist item");
        }
        #endregion
    }
}
