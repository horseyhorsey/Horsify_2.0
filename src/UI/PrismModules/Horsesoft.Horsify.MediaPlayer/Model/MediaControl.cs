using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Logging;
using Prism.Mvvm;
using System;

namespace Horsesoft.Horsify.MediaPlayer.Model
{
    public class MediaControl : BindableBase
    {
        public MediaControl()
        {
        }

        #region Properties

        private AllJoinedTable _currentSong;
        public AllJoinedTable SelectedSong
        {
            get { return _currentSong; }
            set { SetProperty(ref _currentSong, value); }
        }

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { SetProperty(ref _isPlaying, value); }
        }

        private bool _isSeeking;
        public bool IsSeeking
        {
            get { return _isSeeking; }
            set { SetProperty(ref _isSeeking, value); }
        }

        private TimeSpan currentSongTime;
        /// <summary>
        /// Gets or Sets the CurrentSongTime
        /// </summary>
        public TimeSpan CurrentSongTime
        {
            get { return currentSongTime; }
            set { SetProperty(ref currentSongTime, value); }
        }

        private TimeSpan _currentSongPosition;
        /// <summary>
        /// Gets or Sets the CurrentSongPosition
        /// </summary>
        public TimeSpan CurrentSongPosition
        {
            get { return _currentSongPosition; }
            set
            {
                SetProperty(ref _currentSongPosition, value);

                if (IsPlaying)
                    CurrentSongTimeString = $"{CurrentSongPosition.TotalSeconds}|{CurrentSongTime.TotalSeconds}";
            }
        }

        private string _currentSongTimeString;
        public string CurrentSongTimeString
        {
            get { return _currentSongTimeString; }
            set { SetProperty(ref _currentSongTimeString, value); }
        }
        #endregion

        public void Clear()
        {
            IsPlaying = false;
            this.SelectedSong = null;
            CurrentSongTimeString = null;
            CurrentSongTimeString = null;
        }
    }
}
