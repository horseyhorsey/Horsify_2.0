using Horsesoft.Horsify.QueueModule.Views;
using Horsesoft.Horsify.ServicesModule.Extensions;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Horsesoft.Horsify.QueueModule.ViewModels
{
    public class QueueListViewModel : HorsifyBindableBase, INavigationAware
    {
        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;
        private IDjHorsifyService _djHorsifyService;
        private IQueuedSongDataProvider _queuedSongDataProvider;
        public ICollectionView QueuedCollectionView { get; set; }

        #region Commands
        public ICommand PlaySongCommand { get; set; }
        public DelegateCommand<QueueItemViewModel> RemoveSongFromQueueCommand { get; set; }
        #endregion

        #region Constructors
        public QueueListViewModel(IQueuedSongDataProvider queuedSongDataProvider,
            IUnityContainer unityContainer, IEventAggregator eventAggregator, IRegionManager regionManager, IDjHorsifyService djHorsifyService, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _queuedSongDataProvider = queuedSongDataProvider;
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
            _regionManager = regionManager;
            _djHorsifyService = djHorsifyService;

            QueueItems = new ObservableCollection<QueueItemViewModel>();
            QueuedCollectionView = CollectionViewSource.GetDefaultView(QueueItems);

            _queuedSongDataProvider.QueueSongs.CollectionChanged += QueueSongs_CollectionChanged;

            PlaySongCommand = new DelegateCommand<QueueItemViewModel>(PlayQueueItem);
            RemoveSongFromQueueCommand = new DelegateCommand<QueueItemViewModel>(OnRemoveSong);

            #region Queue Control Events

            _eventAggregator.GetEvent<ClearQueueEvent>().Subscribe(OnClearQueue, ThreadOption.UIThread);
            _eventAggregator.GetEvent<ShuffleQueueEvent>().Subscribe(OnShuffleQueue, ThreadOption.UIThread);
            _eventAggregator.GetEvent<SkipQueueEvent>()
                .Subscribe(
                async () => 
                {
                    if (!this._skipQueueEventRunning)
                    {                        
                        try
                        {
                            this._skipQueueEventRunning = true;
                            
                            PlayQueuedSong();

                            if (QueueItems.Count < 2)
                            {
                                bool queueIsEmpty = QueueItems.Count == 0;
                                var songs = await SkipQueueAsync();
                                _queuedSongDataProvider.QueueSongs.AddRange(songs);

                                //Start playing if queue was empty
                                if (queueIsEmpty)
                                {
                                    PlayQueuedSong();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(ex.Message, Category.Exception);
                        }
                        finally
                        {
                            this._skipQueueEventRunning = false;
                        }                        
                    }
                },
                ThreadOption.PublisherThread);

            #endregion

        }

        #endregion

        #region Properties
        private bool _shuffleEnabled;
        public bool ShuffleEnabled
        {
            get { return _shuffleEnabled; }
            set { SetProperty(ref _shuffleEnabled, value); }
        }

        private ObservableCollection<QueueItemViewModel> _queueItems;
        public ObservableCollection<QueueItemViewModel> QueueItems
        {
            get { return _queueItems; }
            set { SetProperty(ref _queueItems, value); }
        }

        private Random _randomShuffle = new Random();
        private bool _skipQueueEventRunning;
        #endregion

        #region Private methods

        /// <summary>
        /// Plays the next queued song if there are songs available in queue
        /// </summary>
        private void PlayQueuedSong()
        {
            if (QueueItems.Count > 0)
            {
                if (!_queuedSongDataProvider.ShuffleEnabled || QueueItems.Count == 1)
                {
                    var vm = QueueItems[0];
                    PlayQueueItem(vm);
                }
                //Shuffle selection
                else
                {
                    var _queueCount = QueueItems.Count - 1;
                    var id = _randomShuffle.Next(QueueItems.Count);
                    var vm = QueueItems[id];
                    PlayQueueItem(vm);
                }
            }
        }

        private void QueueSongs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                try
                {
                    var song = e.NewItems[0] as AllJoinedTable;
                    if (song != null)
                    {
                        QueueItemViewModel queueItemVm = _unityContainer.Resolve(typeof(QueueItemViewModel)) as QueueItemViewModel;
                        queueItemVm.QueuedSong = song;
                        QueueItems.Add(queueItemVm);
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message, Category.Exception);
                    throw;
                }
            }
        }

        private void OnPlayQueuedSong()
        {
            //_evenAggregator.GetEvent<OnPlayQueuedSongEvent>().Publish(this);
        }

        private void OnClearQueue()
        {
            Log("Clearing Queue", Category.Debug);

            if (QueueItems.Count > 0)
                QueueItems.Clear();
        }

        /// <summary>
        /// Shuffles the local queue items list
        /// </summary>
        private void OnShuffleQueue()
        {
            QueueItems.Shuffle();
        }

        private Task<IEnumerable<AllJoinedTable>> SkipQueueAsync()
        {
            try
            {
                var queueCount = QueueItems.Count;
                if (queueCount < 2)
                {
                    //Get more songs with Dj Horsify
                    if (_djHorsifyService.DjHorsifyOption.SelectedFilters.Count > 0)
                    {
                        if (_djHorsifyService.DjHorsifyOption.IsEnabled && _djHorsifyService.DjHorsifyOption.Amount > 0)
                        {
                            return _djHorsifyService.GetSongsAsync(null);
                        }
                    }
                }

                if (queueCount == 0)
                {
                    _eventAggregator.GetEvent<QueuedJobsCompletedEvent>().Publish();
                }
            }
            catch (Exception ex)
            {
                Log($"Error caused by skipping queue: {ex.Message}");                
            }
            finally
            {                
            }

            return null;
        }

        private void PlayQueuedSong(QueueItemViewModel vm)
        {
            Log("Sending Media Play", Category.Debug);

            _eventAggregator.GetEvent<OnMediaPlay<AllJoinedTable>>()
            .Publish(vm.QueuedSong);
        }

        private void PlayQueueItem(QueueItemViewModel vm)
        {
            if (vm != null)
            {
                Log("Sending Media Play", Category.Debug);

                try
                {
                    PlayQueuedSong(vm);
                    OnRemoveSong(vm);
                }
                catch (Exception ex)
                {
                    Log(ex.Message, Category.Exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// Called when [remove song]. Removes passed in song from queue
        /// </summary>
        /// <param name="queueItemViewModel">The queue item view model.</param>
        private void OnRemoveSong(QueueItemViewModel queueItemViewModel)
        {
            Log("Removing song from queue", Category.Debug);

            try
            {
                RemoveQueuedSong(queueItemViewModel);
            }
            catch (Exception ex)
            {
                Log(ex.Message, Category.Exception);
                throw;
            }
        }

        /// <summary>
        /// Removes the queued song from the provider and the local collection
        /// </summary>
        /// <param name="queueItemViewModel">The queue item view model.</param>
        private void RemoveQueuedSong(QueueItemViewModel queueItemViewModel)
        {
            _queuedSongDataProvider.QueueSongs.Remove(queueItemViewModel.QueuedSong);
            QueueItems.Remove(queueItemViewModel);
        } 
        #endregion

        #region Navigation Unused?
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var song = navigationContext.Parameters["queueSong"] as AllJoinedTable;
            if (song != null)
            {
                IRegion region = _regionManager.Regions["QueueList"];
                //region.Add(_unityContainer.Resolve(typeof(QueueItem)));
                //_queuedSongDataProvider.QueueSongs.Add(song);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        #endregion
    }
}
