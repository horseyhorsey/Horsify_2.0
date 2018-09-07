using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{
    public class MediaElementViewModel : HorsifyBindableBase
    {
        private IEventAggregator _eventAggregator;
        private ISongDataProvider _songDataProvider;
        private IMediaPlayer _mediaPlayer;

        public MediaElementViewModel(ISongDataProvider songDataProvider, IEventAggregator eventAggregator, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _songDataProvider = songDataProvider;

            _eventAggregator.GetEvent<OnMediaPlay<AllJoinedTable>>()
            .Subscribe(async song =>
            {
                Log($"MediaElement: Loading file async: {song?.FileLocation}", Category.Info, Priority.Medium);

                try
                {
                    await SetSongLocationAndSourceAsync(song);
                }
                catch (Exception ex)
                {
                    Log($"Error loading file: {ex.Message}", Category.Exception);
                    _eventAggregator.GetEvent<SkipQueueEvent>().Publish();
                }

            }, ThreadOption.UIThread);

            _eventAggregator.GetEvent<QueuedJobsCompletedEvent>()
            .Subscribe(() =>
            {
                _loggerFacade.Log($"Queued Jobs Completed", Category.Debug, Priority.Medium);
                this.AudioSource = null;

            }, ThreadOption.UIThread);            
        }

        #region Properties

        private Uri _audioSource = null;
        public Uri AudioSource
        {
            get
            {
                return _audioSource;
            }
            set
            {
                SetProperty(ref _audioSource, value);
            }
        }

        private AllJoinedTable _previousSong;
        private int? _previousSongRating;
        #endregion

        public Task SetSongLocationAndSourceAsync(AllJoinedTable allJoinedTable)
        {
            return SetAudioSourceAsync(allJoinedTable);
        }

        private async Task SetAudioSourceAsync(AllJoinedTable allJoinedTable)
        {
            var mediaFile = new Uri(allJoinedTable.FileLocation, UriKind.Absolute);
            if (!System.IO.File.Exists(mediaFile.LocalPath))
                throw new FileNotFoundException($"Media file not found: {mediaFile.LocalPath}");

            AudioSource = mediaFile;

            //Update file tags
            if (_previousSong != null)
            {        
                if (_previousSongRating != null)
                {
                    if (_previousSong.Rating != _previousSongRating)
                    {
                        await UpdatePlayedSongAsync(_previousSong, _previousSong.Rating);
                    }
                    else { await UpdatePlayedSongAsync(_previousSong); }
                }
                else { await UpdatePlayedSongAsync(_previousSong); }
            }

            _previousSong = allJoinedTable;
            _previousSongRating = _previousSong.Rating;
        }

        private Task UpdatePlayedSongAsync(AllJoinedTable selectedSong, int? rating = null)
        {
            return _songDataProvider.UpdatePlayedSong(selectedSong, rating);
        }
    }
}
