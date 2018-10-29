using System;
using System.IO;
using Vlc.DotNet.Core;
using static Horsesoft.Vlc.Events;

namespace Horsesoft.Vlc
{
    public class VlcPlayer : IVlcPlayer
    {
        private VlcMediaPlayer _vlcMediaPlayer;

        //private DirectoryInfo libDirectory =
        //    new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "libvlc", "win-x86"));

        private DirectoryInfo libDirectory =
            new DirectoryInfo(Path.Combine(@"C:\Program Files (x86)\VideoLan\VLC"));

        public VlcPlayer()
        {
        }

        public event TimeChangedEvent TimeChanged;
        public event MediaLoadedEvent MediaLoaded;

        /// <summary>
        /// Initializes VLC 
        /// </summary>
        public void Init()
        {
            _vlcMediaPlayer = new VlcMediaPlayer(libDirectory);
            _vlcMediaPlayer.TimeChanged += _vlcMediaPlayer_TimeChanged;

            _vlcMediaPlayer.EncounteredError += _vlcMediaPlayer_EncounteredError;

            _vlcMediaPlayer.Log += _vlcMediaPlayer_Log;
        }

        private void _vlcMediaPlayer_EncounteredError(object sender, VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            
        }

        private void _vlcMediaPlayer_Log(object sender, VlcMediaPlayerLogEventArgs e)
        {
            
        }

        private void _vlcMediaPlayer_TimeChanged(object sender, VlcMediaPlayerTimeChangedEventArgs e)
        {
            var time = TimeSpan.FromMilliseconds(e.NewTime);
            TimeChanged?.Invoke(time);
        }

        /// <summary>
        /// Initializes VLC specified CLI options.
        /// </summary>
        /// <param name="options">The VLC command line options.</param>
        public void Init(string[] options)
        {
            _vlcMediaPlayer = new VlcMediaPlayer(libDirectory, options);
            _vlcMediaPlayer.TimeChanged += _vlcMediaPlayer_TimeChanged;
        }

        public bool Play()
        {
            _vlcMediaPlayer.Play();

            return _vlcMediaPlayer.IsPlaying();
        }

        public bool Pause()
        {
            if (_vlcMediaPlayer.IsPausable() && _vlcMediaPlayer.IsPlaying())
            {
                _vlcMediaPlayer.Pause();
                return true;
            }

            return false;
        }

        public void SetMedia(Uri file)
        {
            //var media = _vlcMediaPlayer.SetMedia(@"file:///" + file);
            var media = _vlcMediaPlayer.SetMedia(file);
            if (media != null)
            {
                Play();
                media.Parse();
                MediaLoaded?.Invoke(media.Duration);
            }

            //_vlcMediaPlayer.Play(@"file:///" + file);
            //this.MediaLoaded?.Invoke(e.NewMedia.Duration);
        }

        public void SetmediaPosition(float position)
        {
            _vlcMediaPlayer.Position = position;
        }

        public void SetVolume(int volume)
        {
            _vlcMediaPlayer.Audio.Volume = volume;            
        }

        public void Stop()
        {
            _vlcMediaPlayer.Stop();
        }
    }
}
