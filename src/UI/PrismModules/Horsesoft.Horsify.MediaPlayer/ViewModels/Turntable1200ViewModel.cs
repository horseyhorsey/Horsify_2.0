using Horsesoft.Horsify.MediaPlayer.Model;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Events;
using Prism.Logging;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{
    public class Turntable1200ViewModel : MediaControlViewModelBase
    {
        #region Constructors
        public Turntable1200ViewModel(ILoggerFacade loggerFacade, IHorsifyMediaController horsifyMediaController, IEventAggregator eventAggregator, MediaControl mediaControl) : base(loggerFacade, horsifyMediaController, eventAggregator, mediaControl)
        {
        }
        #endregion

        #region Properties
        private bool _turntableChecked;
        /// <summary>
        /// This is the toggle button for the turntable : TODO: Check if can use isPaused instead
        /// </summary>
        public bool TurnTableChecked
        {
            get { return _turntableChecked; }
            set { SetProperty(ref _turntableChecked, value); }
        }
        #endregion
    }
}
