using System;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify.Audio;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Events;
using Prism.Mvvm;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{
    public class SongPlaying3dViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        private ISongPlayingInfo _songPlayingInfo;

        public SongPlaying3dViewModel(ISongPlayingInfo songPlayingInfo,             
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _songPlayingInfo = songPlayingInfo;

            _eventAggregator
                .GetEvent<OnMediaPlay<AllJoinedTable>>()
            .Subscribe(song =>
            {               
                SelectedSong = song;                            
            });

            //When song time changes
            _eventAggregator.GetEvent<OnMediaTimeChangedEvent<SongTime>>()
            .Subscribe(songTime =>
            {
                if (!this._songPlayingInfo.IsSeeking)
                    CurrentSongTime = songTime.Duration.TotalSeconds;
                CurrentSongPosition = songTime.CurrentSongTime.TotalSeconds;
            });

            //Sets the IsSeeking variable
            _eventAggregator
                .GetEvent<OnMediaSeekEvent<bool>>().Subscribe(x =>
                {
                    _songPlayingInfo.IsSeeking = x;
                });

            _eventAggregator.GetEvent<QueuedJobsCompletedEvent>()
                .Subscribe(() =>
                {
                    ResetSelectedSong();

                }, ThreadOption.UIThread);
        }

        private AllJoinedTable _selectedSong;
        public AllJoinedTable SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                SetProperty(ref _selectedSong, value);  
                
                if (SelectedSong !=null)
                    Rating = (int)_selectedSong.Rating;
            }
        }

        #region Song times
        private int _rating;
        public int Rating
        {
            get { return _rating; }
            set {
                SetProperty(ref _rating, value);      
                
                if (SelectedSong !=null)
                    _selectedSong.Rating = value;
            }
        }

        private double _currentSongTime;
        /// <summary>
        /// Gets or Sets the CurrentSongTime
        /// </summary>
        public double CurrentSongTime
        {
            get { return _currentSongTime; }
            set { SetProperty(ref _currentSongTime, value); }
        }

        private double _currentSongPosition;
        /// <summary>
        /// Gets or Sets the CurrentSongPosition
        /// </summary>
        public double CurrentSongPosition
        {
            get { return _currentSongPosition; }
            set
            {
                SetProperty(ref _currentSongPosition, value);

                if (!_songPlayingInfo.IsSeeking)
                    CurrentSongTimeString = $"{CurrentSongPosition}|{CurrentSongTime}";
            }
        }

        private string _currentSongTimeString;
        /// <summary>
        /// Gets or Sets the CurrentSongTimeString
        /// </summary>
        public string CurrentSongTimeString
        {
            get { return _currentSongTimeString; }
            set { SetProperty(ref _currentSongTimeString, value); }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Resets the selected song by setting the song, positions and time to null
        /// </summary>
        private void ResetSelectedSong()
        {
            this.SelectedSong = null;
            CurrentSongPosition = 0;
            CurrentSongTime = 0;
            CurrentSongTimeString = null;
        }
        #endregion
    }
}
