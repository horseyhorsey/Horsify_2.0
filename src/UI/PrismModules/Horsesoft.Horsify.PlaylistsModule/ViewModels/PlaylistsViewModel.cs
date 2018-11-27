using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Horsesoft.Horsify.PlaylistsModule.ViewModels
{
    public class PlaylistsViewModel : HorsifyBindableBase, INavigationAware
    {
        #region Services
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IUnityContainer _unityContainer;
        private IPlaylistService _horsifyPlaylistService;
        #endregion

        #region Commands/Requests        
        public ICommand CloseAllTabsCommand { get; set; }
        public ICommand CloseTabCommand { get; set; }
        public ICommand CreatePlaylistCommand { get; set; }
        public ICommand DeletePlaylistCommand { get; set; }
        public ICommand OpenSavedPlaylistCommand { get; set; }
        public DelegateCommand HelpWindowCommand { get; set; }
        public InteractionRequest<INotification> HelpNotificationRequest { get; set; }
        #endregion

        #region Constructors
        public PlaylistsViewModel(IPlaylistService horsifyPlaylistService, IEventAggregator eventAggregator,
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

            DeletePlaylistCommand = new DelegateCommand(async () => await OnDeletePlaylistAsync());
            CloseAllTabsCommand = new DelegateCommand(OnCloseTabs);
            CloseTabCommand = new DelegateCommand<PlaylistTabViewModel>(OnCloseTab);

            #region Notification for help
            HelpNotificationRequest = new InteractionRequest<INotification>();
            HelpWindowCommand = new DelegateCommand(RaiseHelpNotification);
            #endregion
        }

        #endregion

        #region Properties
        public ObservableCollection<PlaylistTabViewModel> PlayListViewModels { get; set; }
        public ObservableCollection<PlaylistTabViewModel> OpenPlayListViewModels { get; set; }

        private PlaylistTabViewModel _selectedTab;
        public PlaylistTabViewModel SelectedTab
        {
            get { return _selectedTab; }
            set { SetProperty(ref _selectedTab, value); }
        }


        public string Title { get; private set; }
        private PlaylistTabViewModel _lastOpenedTab = null;
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
                Log($"Created playlist - {playListName}");
            }

            if (addToTabsRegion)
            {
                Log("Adding to tab region");
                OpenPlayListViewModels.Add(vm);
                _lastOpenedTab = vm;
            }
        }

        private void OnCreatePlaylist(string playlistName)
        {
            if (string.IsNullOrWhiteSpace(playlistName)) return;

            if (playlistName == "Preparation Playlist")
            {
                Log("Cannot save preparation lists", Category.Warn);
                return;
            }

            if (playlistName.Length > 3)
                CreatePlayList(playlistName);
            else
            {
                Log("Playlist names must be greater than 3 chars long", Category.Warn);
            }
        }

        private void OnOpenSavedPlaylist(PlaylistTabViewModel obj)
        {
            OnOpenPlaylist(obj);
        }        

        private async void OnOpenPlaylist(PlaylistTabViewModel playlistTabViewModel)
        {
            try
            {
                //var existingTab = OpenPlayListViewModels.FirstOrDefault(x => x.Playlist?.Name == playlistTabViewModel?.Playlist?.Name);
                //if (existingTab != null)
                //{
                //    Log($"Opening existing tab", Category.Debug);
                //    OpenPlayListViewModels.Add(playlistTabViewModel);
                //    await OnViewLoaded(playlistTabViewModel);
                //}
                //else
                //{
                //    Log($"Adding to Tab", Category.Debug);
                //    OpenPlayListViewModels.Add(playlistTabViewModel);
                //}                

                if (!OpenPlayListViewModels.Any(x => x == playlistTabViewModel))
                {
                    Log($"Opening existing: {playlistTabViewModel.Playlist?.Name}", Category.Debug);
                    OpenPlayListViewModels.Add(playlistTabViewModel);
                    await OnViewLoaded(playlistTabViewModel);
                }
                else if(playlistTabViewModel?.Playlist?.Id == 0)
                {
                    Log("Adding a not saved playlist");
                    OpenPlayListViewModels.Add(playlistTabViewModel);
                }

                SelectedTab = playlistTabViewModel;
            }
            catch (System.Exception ex)
            {
                Log(ex.Message, Category.Exception);
            }

        }

        private void RaiseHelpNotification()
        {
            HelpNotificationRequest.Raise(new Notification { Content = "Notification Message", Title = "Notification" }, r => Title = "Good to go");
        }

        /// <summary>
        /// Closes the tab from the TabControl
        /// </summary>
        /// <param name="vm">The vm.</param>
        private void OnCloseTab(PlaylistTabViewModel vm)
        {
            try
            {                
                if (SelectedTab != null)
                {
                    //User could push close button on tab and not actually be selected so set to incoming VM
                    if (SelectedTab != vm)
                        SelectedTab = vm;

                    if (SelectedTab.Playlist != null)
                    {
                        Log("Clearing tab items.");
                        //Clear the songs
                        SelectedTab.ClearItems();
                    }

                    //Remove the tab
                    OpenPlayListViewModels.Remove(SelectedTab);
                    Log($"Closed - {SelectedTab?.Playlist?.Name}");
                }
            }
            catch (System.Exception ex)
            {
                Log($"Close tab error: {ex.Message}");
            }
        }

        /// <summary>
        /// Closes all the OpenPlayListViewModels
        /// </summary>
        private void OnCloseTabs()
        {
            try
            {
                foreach (var openTab in PlayListViewModels.Where(x => x.TabHeader != "Preparation Playlist"))
                {
                    OnCloseTab(openTab);
                }

                Log("Closed all tabs.");
            }
            catch (Exception ex)
            {

                Log(ex.Message);
            }
        }

        //TODO: Delete Playlists
        private async Task OnDeletePlaylistAsync()
        {
            try
            {
                var tab = SelectedTab;
                int id = 0;
                if (tab.Playlist != null)
                {
                    Log("Getting local id");
                    id = (int)tab.Playlist?.Id;
                }

                if (id > 0)
                {
                    Log($"Deleting playlist: {id} : {tab.Playlist.Name}");
                    bool result = await _horsifyPlaylistService.DeletePlaylistAsync(id);
                    if (result)
                    {
                        //Remove tab item
                        Log($"Playlist deleted: {id} : {tab.Playlist.Name}");
                        OnCloseTab(tab);
                        this.PlayListViewModels.Remove(tab);
                    }
                }
                else
                {
                    Log("Cannot delete playlist with no id. Must be saved first.", Category.Warn);
                    OnCloseTab(tab);
                    this.PlayListViewModels.Remove(tab);
                }
            }
            catch (Exception ex)
            {
                Log($"{ex.Message}", Category.Exception);
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
            try
            {
                foreach (var playlist in _horsifyPlaylistService.Playlists)
                {
                    PlayListViewModels.Add(CreateTabViewModel(playlist));
                }

                Log("Playlists updated");
            }
            catch (Exception ex)
            {
                Log(ex.Message, Category.Exception);
            }

            

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            return;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            return;
        }

        #endregion
    }
}
