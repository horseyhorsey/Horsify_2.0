using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace Horsesoft.Horsify.QueueModule.ViewModels
{
    public interface IQueueControlViewModel
    {
        ICommand ClearCommand { get; set; }
        ICommand SkipCommand { get; set; }
        ICommand ShuffleCommand { get; set; }
    }

    public class QueueControlViewModel : BindableBase, IQueueControlViewModel
    {
        private IEventAggregator _eventAggregator;
        private IQueuedSongDataProvider _queuedSongDataProvider;

        #region Commands
        public ICommand ClearCommand { get; set; }
        public ICommand SkipCommand { get; set; }
        public ICommand ShuffleCommand { get; set; }
        #endregion

        public QueueControlViewModel(IQueuedSongDataProvider queuedSongDataProvider, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _queuedSongDataProvider = queuedSongDataProvider;

            ClearCommand = new DelegateCommand(OnClearQueue);
            SkipCommand = new DelegateCommand(OnSkipQueue);
            ShuffleCommand = new DelegateCommand(OnShuffleQueue);
        }

        private bool _shuffleEnabled;
        public bool ShuffleEnabled
        {
            get { return _shuffleEnabled; }
            set
            {
                SetProperty(ref _shuffleEnabled, value);
                _queuedSongDataProvider.ShuffleEnabled = value;
            }
        }

        #region Private Methods
        private void OnShuffleQueue()
        {
            var evt = _eventAggregator.GetEvent<ShuffleQueueEvent>();
            evt.Publish();
        }

        private void OnSkipQueue()
        {
            var evt = _eventAggregator.GetEvent<SkipQueueEvent>();
            evt.Publish();
        }

        private void OnClearQueue()
        {
            var evt = _eventAggregator.GetEvent<ClearQueueEvent>();
            evt.Publish();
        }
        #endregion
    }
}
