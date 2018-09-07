using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Logging;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Horsesoft.Horsify.PlaylistsModule.ViewModels
{
    public class PlaylistsViewModel : HorsifyBindableBase
    {
        #region Services
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IUnityContainer _unityContainer;
        private IHorsifyPlaylistService _horsifyPlaylistService;
        #endregion

        #region Commands/Requests
        public ICommand CloseTabCommand { get; set; }
        public DelegateCommand HelpWindowCommand { get; set; }
        public InteractionRequest<INotification> HelpNotificationRequest { get; set; } 
        #endregion

        #region Constructors
        public PlaylistsViewModel(IHorsifyPlaylistService horsifyPlaylistService, IEventAggregator eventAggregator, 
            IRegionManager regionManager, IUnityContainer unityContainer, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _unityContainer = unityContainer;
            _horsifyPlaylistService = horsifyPlaylistService;

            PlayListViewModels = new ObservableCollection<PlaylistTabViewModel>();
            OpenPlayListViewModels = new ObservableCollection<PlaylistTabViewModel>();

            #region Commands
            CreatePlaylistCommand = new DelegateCommand<string>(OnCreatePlaylist);
            OpenSavedPlaylistCommand = new DelegateCommand<PlaylistTabViewModel>(OnOpenSavedPlaylist);

            #endregion

            CreatePlayList("Preparation Playlist", true);

            _horsifyPlaylistService.UpdateFromDatabaseAsync().GetAwaiter().OnCompleted(() =>
            {
                OnPlaylistsUpdated();
            });

            LoadedViewCommand = new DelegateCommand<PlaylistTabViewModel>(async (x) => await OnViewLoaded(x));
            CloseTabCommand = new DelegateCommand<PlaylistTabViewModel>(OnCloseTab);

            #region Notification for help
            HelpNotificationRequest = new InteractionRequest<INotification>();
            HelpWindowCommand = new DelegateCommand(RaiseHelpNotification);
            #endregion
        } 
        #endregion

        #region Commands
        public ICommand CreatePlaylistCommand { get; set; }
        public ICommand OpenSavedPlaylistCommand { get; set; }
        public ICommand LoadedViewCommand { get; set; }
        #endregion

        #region Properties
        public ObservableCollection<PlaylistTabViewModel> PlayListViewModels { get; set; }
        public ObservableCollection<PlaylistTabViewModel> OpenPlayListViewModels { get; set; }
        public string Title { get; private set; }
        #endregion

        #region Private Methods

        private PlaylistTabViewModel CreateTabViewModel(Playlist playlist)
        {
            var vm = ResolveNewTabModelFromContainer();
            vm.TabHeader = playlist.Name;
            vm.Playlist = playlist;

            return vm;
        }

        private PlaylistTabViewModel ResolveNewTabModelFromContainer()
        {
            return _unityContainer.Resolve<PlaylistTabViewModel>();
        }

        private void CreatePlayList(string playListName, bool addToTabsRegion = false)
        {
            PlaylistTabViewModel vm = null;

            if (!PlayListViewModels.Any(x => x.TabHeader == playListName))
            {
                vm = ResolveNewTabModelFromContainer();
                vm.TabHeader = playListName;
                PlayListViewModels.Add(vm);
            }
            else
            {

            }

            if (addToTabsRegion)
            {
                OpenPlayListViewModels.Add(vm);
                _lastOpenedTab = vm;
            }
        }

        private void OnCreatePlaylist(string playlistName)
        {
            if (string.IsNullOrWhiteSpace(playlistName)) return;

            if (playlistName.Length > 5)
                CreatePlayList(playlistName);
        }

        private void OnOpenSavedPlaylist(PlaylistTabViewModel obj)
        {
            OnOpenPlaylist(obj);
        }

        private PlaylistTabViewModel _lastOpenedTab = null;
        private void OnOpenPlaylist(PlaylistTabViewModel playlistTabViewModel)
        {
            //var views = _regionManager.Regions[Regions.PlaylistTabsRegion].Views;

            if (!OpenPlayListViewModels.Any(x => x == playlistTabViewModel))
            {
                OpenPlayListViewModels.Add(playlistTabViewModel);
            }

            //Reset and select tab
            if (_lastOpenedTab != null)
                _lastOpenedTab.IsSelected = false;

            playlistTabViewModel.IsSelected = true;
            _lastOpenedTab = playlistTabViewModel;

        }

        private void RaiseHelpNotification()
        {
            HelpNotificationRequest.Raise(new Notification { Content = "Notification Message", Title = "Notification" }, r => Title = "Good to go");
        }

        private void OnCloseTab(PlaylistTabViewModel vm)
        {
            if (vm != null)
            {
                this.OpenPlayListViewModels.Remove(vm);
                vm.ClearItems();
            }
        }

        private async Task OnViewLoaded(PlaylistTabViewModel x)
        {
            if (x == null) return;
            if (x.GetItemCount() > 0) return;

            if (x.TabHeader == "Preparation Playlist")
                return;

            if (x.Playlist != null)
            {

                if (x.Playlist.Count > 0)
                {
                    //Get the songs from service asyync
                    var playlistSongs = await _horsifyPlaylistService.GetSongs(x.Playlist);

                    //Add item view models for all the returned songs
                    foreach (var song in playlistSongs)
                    {
                        x.AddPlaylistItem(new PlaylistItemViewModel(_loggerFacade)
                        {
                            Song = song,
                            PlayedState = 0
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the playlists from the database
        /// </summary>
        private void OnPlaylistsUpdated()
        {
            foreach (var playlist in _horsifyPlaylistService.Playlists)
            {
                PlayListViewModels.Add(CreateTabViewModel(playlist));
            }
        }

        #endregion
    }
}
