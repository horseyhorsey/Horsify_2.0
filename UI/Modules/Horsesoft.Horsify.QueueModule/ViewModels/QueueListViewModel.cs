using Horsesoft.Horsify.ServicesModule.Extensions;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
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
            _eventAggregator.GetEvent<SkipQueueEvent>().Subscribe(async () => await OnSkipQueueAsync(), ThreadOption.UIThread);

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

        private void OnClearQueue()
        {            
            if (QueueItems?.Count > 0)
            {
                Log("Clearing Queue", Category.Debug);
                QueueItems.Clear();
            }                
        }

        /// <summary>
        /// Shuffles the local queue items list
        /// </summary>
        private void OnShuffleQueue()
        {
            QueueItems.Shuffle();
        }

        private async Task OnSkipQueueAsync()
        {
            Log("Skipping queue");

            if (!this._skipQueueEventRunning)
            {
                try
                {
                    this._skipQueueEventRunning = true;
                    bool queueIsEmpty = QueueItems.Count == 0;

                    if (QueueItems.Count < 2)
                    {
                        //DJ HORsify - Queue
                        IEnumerable<AllJoinedTable> songs = null;
                        if (_djHorsifyService.DjHorsifyOption.IsEnabled && _djHorsifyService.DjHorsifyOption.Amount > 0)
                        {
                            //User didn't select any filters
                            if (_djHorsifyService.DjHorsifyOption.SelectedFilters?.Count <= 0)
                            {
                                Log("Running DJ Horsify no filters.", Category.Warn);

                                //Back out if no other option is enabled
                                if (!_djHorsifyService.DjHorsifyOption.BpmRange.IsEnabled && !_djHorsifyService.DjHorsifyOption.RatingRange.IsEnabled &&
                                    _djHorsifyService.DjHorsifyOption.SelectedKeys == Music.Data.Model.Import.OpenKeyNotation.None)
                                {
                                    Log("DJ Horsify enabled but no options or filters selected.", Category.Warn);
                                    return;
                                }
                            }

                            Log("Refilling queue from DJ Horsify");

                            //Get DjHorsify Songs
                            songs = await _djHorsifyService.GetSongsAsync(null);
                            if (songs != null)
                                _queuedSongDataProvider.QueueSongs.AddRange(songs);

                            //Start playing a song if queue was empty when DJ Horsify is run
                            if (queueIsEmpty)
                            {
                                PlayQueuedSong();
                                return;
                            }
                        }
                    }

                    //Publish queued songs
                    if (queueIsEmpty) _eventAggregator.GetEvent<QueuedJobsCompletedEvent>().Publish();
                    else
                    {
                        Log("Queue not empty. Attempting to play next song");
                        PlayQueuedSong();
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
        }


        /// <summary>
        /// Plays the next queued song if there are songs available in queue
        /// </summary>
        private void PlayQueuedSong()
        {
            if (QueueItems.Count > 0)
            {
                bool shuffle = _queuedSongDataProvider.ShuffleEnabled;
                Log($"Queue items available: {QueueItems.Count}");
                Log($"Shuffle Enabled: {shuffle}");
                if (!shuffle || QueueItems.Count == 1)
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PlayQueuedSong(vm);
                        OnRemoveSong(vm);
                    });
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
