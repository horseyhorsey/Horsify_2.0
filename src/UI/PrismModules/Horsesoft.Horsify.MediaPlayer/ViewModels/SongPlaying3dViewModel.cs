using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.Model;
using Prism.Events;
using Prism.Logging;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{
    public class SongPlaying3dViewModel : MediaControlViewModelBase
    {

        #region Constructors
        public SongPlaying3dViewModel(ILoggerFacade loggerFacade, IHorsifyMediaController horsifyMediaController, IEventAggregator eventAggregator, MediaControl mediaControl) 
            : base(loggerFacade, horsifyMediaController, eventAggregator, mediaControl)
        {
        } 
        #endregion

        #region Properties

        private int _rating;
        public int Rating
        {
            get { return _rating; }
            set {
                SetProperty(ref _rating, value);      
                
                if (MediaControlModel.SelectedSong !=null)
                    MediaControlModel.SelectedSong.Rating = value;
            }
        }

        #endregion

    }
}
