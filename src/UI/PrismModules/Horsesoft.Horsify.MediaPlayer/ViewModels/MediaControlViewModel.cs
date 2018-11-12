using Horsesoft.Horsify.MediaPlayer.Model;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Events;
using Prism.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{    
    /// <summary>
    /// The mini media control in now playing panel.
    /// </summary>
    /// <seealso cref="Horsesoft.Horsify.MediaPlayer.ViewModels.MediaControlViewModelBase" />
    public class MediaControlViewModel : MediaControlViewModelBase
    {
        private ISongDataProvider _songDataProvider;

        public MediaControlViewModel(IEventAggregator eventAggregator, ILoggerFacade loggerFacade, 
            IHorsifyMediaController horsifyMediaController, ISongDataProvider songDataProvider, MediaControl mediaControl) 
            : base(loggerFacade, horsifyMediaController, eventAggregator, mediaControl)
        {
            _songDataProvider = songDataProvider;

            #region Events
            //Update SelectedSong
            _eventAggregator.GetEvent<OnMediaPlay<AllJoinedTable>>()
            .Subscribe(song => { OnSongChangedLoaded(song); }, ThreadOption.UIThread);

            //Queue completed
            _eventAggregator.GetEvent<QueuedJobsCompletedEvent>().Subscribe(() =>
            {
                _loggerFacade.Log($"Queued Jobs Completed", Category.Debug, Priority.Medium);
                ResetSelectedSong();
            }, ThreadOption.UIThread);


            #region Events
            _horsifyMediaController.OnTimeChanged += (currentTime) => { OnTimeChanged(MediaControlModel, currentTime); };
            _horsifyMediaController.OnMediaFinished += OnMediaFinsished;
            _horsifyMediaController.OnMediaLoaded += (duration) => { MediaControlModel.CurrentSongTime = duration; };
            #endregion

            #endregion
        }

        #region Private Methods

        /// <summary>
        /// Sends a Skip queue event.
        /// </summary>
        private void OnMediaFinsished()
        {
            //Application.Current.Dispatcher.Invoke(() => );            
            _eventAggregator.GetEvent<SkipQueueEvent>().Publish();
        }

        private void OnSongChangedLoaded(AllJoinedTable song)
        {
            Log($"MediaElement: Loading file async: {song?.FileLocation}", Category.Info, Priority.Medium);

            try
            {
                var mediaFile = new Uri(song.FileLocation, UriKind.Absolute);
                if (!System.IO.File.Exists(mediaFile.LocalPath))
                    throw new FileNotFoundException($"Media file not found: {mediaFile.LocalPath}");

                MediaControlModel.SelectedSong = song;
                //_horsifyMediaController.SetMedia(song.FileLocation);
                _horsifyMediaController.SetMedia(mediaFile);

                //Update file tags
                if (_previousSong != null && _previousSong != MediaControlModel.SelectedSong)
                    UpdateFileTags(); 

                _previousSong = MediaControlModel.SelectedSong;
                _previousSongRating = _previousSong.Rating;

                MediaControlModel.IsPlaying = true;
            }
            catch (Exception ex)
            {
                Log($"Error loading file: {ex.Message}", Category.Exception);
                MediaControlModel.SelectedSong = null;
                //_eventAggregator.GetEvent<SkipQueueEvent>().Publish();
            }

        }


        private void OnTimeChanged(MediaControl mediaControlModel, TimeSpan currentTime)
        {
            if (!mediaControlModel.IsSeeking)
            {
                if (MediaControlModel.IsPlaying)
                {
                    MediaControlModel.CurrentSongPosition = currentTime;
                }
            }
        }

        /// <summary>
        /// Resets the selected song and time. 
        /// </summary>
        private void ResetSelectedSong()
        {
            if (!MediaControlModel.IsPlaying)
                MediaControlModel.SelectedSong = null;
                MediaControlModel.CurrentSongTimeString = null;
        }

        private void UpdateFileTags()
        {
            if (_previousSong != null)
            {
                if (_previousSong.Rating != null)
                {
                    if (_previousSong.Rating != _previousSongRating)
                    {
                        UpdatePlayedSong(_previousSong, _previousSong.Rating);
                    }
                    else { UpdatePlayedSong(_previousSong); }
                }
                else { UpdatePlayedSong(_previousSong); }

                _previousSong = MediaControlModel.SelectedSong;
                _previousSongRating = _previousSong.Rating;
            }
        }

        private Task<bool> UpdatePlayedSong(AllJoinedTable selectedSong, int? rating = null)
        {
            return _songDataProvider.UpdatePlayedSong(selectedSong, rating);
                //_loggerFacade?.Log($"Failed to update song. {filePath}", Category.Exception, Priority.Medium);
        }
        #endregion
    }
}
