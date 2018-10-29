﻿using Horsesoft.Horsify.MediaPlayer.Model;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using System;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{
    public class MediaControlViewModelBase : HorsifyBindableBase
    {
        protected IEventAggregator _eventAggregator;        
        protected IHorsifyMediaController _horsifyMediaController;

        public DelegateCommand PlayPauseCommand { get; set; }        
        public DelegateCommand<object> SeekingStartedCommand { get; set; }
        public DelegateCommand<object> SeekingStoppedCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }

        public MediaControl MediaControlModel { get; set; }

        #region Constructors
        public MediaControlViewModelBase(ILoggerFacade loggerFacade, 
            IHorsifyMediaController horsifyMediaController, 
            IEventAggregator eventAggregator, MediaControl mediaControlModel)
            : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _horsifyMediaController = horsifyMediaController;
            MediaControlModel = mediaControlModel;

            _horsifyMediaController.OnTimeChanged += (currentTime) =>
            {
                if (!mediaControlModel.IsSeeking)
                {
                    if (MediaControlModel.IsPlaying)
                    {
                        MediaControlModel.CurrentSongPosition = currentTime;
                    }                        
                }               
            };

            _horsifyMediaController.OnMediaLoaded += (duration) =>
            {
                MediaControlModel.CurrentSongTime = duration;
                //SongTotalSeconds = CurrentSongTime.TotalSeconds;
            };

            #region Commands
            PlayPauseCommand = new DelegateCommand(OnPlayPause);
            SeekingStartedCommand = new DelegateCommand<object>(OnSeekStarted);
            SeekingStoppedCommand = new DelegateCommand<object>(OnSeekStopped);
            StopCommand = new DelegateCommand(OnStopped);
            #endregion
        }

        protected AllJoinedTable _previousSong;
        protected int? _previousSongRating;        

        private void OnStopped()
        {
            MediaControlModel.IsPlaying = false;
            _horsifyMediaController.Stop();            
            MediaControlModel.CurrentSongPosition = TimeSpan.Zero;
            MediaControlModel.CurrentSongTimeString = $"{MediaControlModel.CurrentSongPosition.TotalSeconds}|{MediaControlModel.CurrentSongTime.TotalSeconds}";
            Log("Media Stopped");
        }

        private void OnSeekStarted(object x)
        {
            MediaControlModel.IsSeeking = true;
        }

        private void OnSeekStopped(object sliderValue)
        {
            var val = (double)sliderValue;
            var length = MediaControlModel.CurrentSongTime.TotalSeconds;
            var pos = (1 / length) * val;
            _horsifyMediaController.SetMediaPosition(pos);
            MediaControlModel.IsSeeking = false;
        }

        #endregion

        #region Private Methods

        private void OnPlayPause()
        {
            MediaControlModel.IsPlaying = _horsifyMediaController.PlayPause(MediaControlModel.IsPlaying);
            Log($"Play/Pause IsPlaying: {MediaControlModel.IsPlaying}");
        }
        #endregion
    }
}
