using Horsesoft.Horsify.MediaPlayer.Model;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Events;
using Prism.Logging;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{
    public class SongPlaying3dViewModel : MediaControlViewModelBase
    {
        private ISongPlayingInfo _songPlayingInfo;

        #region Constructors
        public SongPlaying3dViewModel(ISongPlayingInfo songPlayingInfo, ILoggerFacade loggerFacade, IHorsifyMediaController horsifyMediaController, IEventAggregator eventAggregator, MediaControl mediaControl) : base(loggerFacade, horsifyMediaController, eventAggregator, mediaControl)
        {
            _songPlayingInfo = songPlayingInfo;

            //TODO Remove me
            //_eventAggregator.GetEvent<OnMediaTimeChangedEvent<SongTime>>()
            //.Subscribe(songTime =>
            //{
            //    if (!this._songPlayingInfo.IsSeeking)
            //        CurrentSongTime = songTime.Duration.TotalSeconds;
            //    CurrentSongPosition = songTime.CurrentSongTime.TotalSeconds;
            //});

            //Sets the IsSeeking variable
            _eventAggregator
                .GetEvent<OnMediaSeekEvent<bool>>().Subscribe(x =>
                {
                    _songPlayingInfo.IsSeeking = x;
                });

            //Fire when queue jobs complete
            _eventAggregator.GetEvent<QueuedJobsCompletedEvent>().Subscribe(() =>
            { ResetSelectedSong();}, ThreadOption.UIThread);
        } 
        #endregion

        #region Song times
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

        #region Private Methods

        /// <summary>
        /// Resets the selected song by setting the song, positions and time to null
        /// </summary>
        private void ResetSelectedSong()
        {
            //TODO uncomment Song positions
            MediaControlModel.SelectedSong = null;
            //CurrentSongPosition = 0;
            //CurrentSongTime = 0;
            MediaControlModel.CurrentSongTimeString = null;
        }
        #endregion
    }
}
