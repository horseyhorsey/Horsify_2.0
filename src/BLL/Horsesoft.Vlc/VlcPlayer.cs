using System;
using System.IO;
using Vlc.DotNet.Core;
using static Horsesoft.Vlc.Events;

namespace Horsesoft.Vlc
{
    public class VlcPlayer : IVlcPlayer
    {
        private const string VLC_X86 = @"C:\Program Files (x86)\VideoLan\VLC";

        private VlcMediaPlayer _vlcMediaPlayer;
        private DirectoryInfo libDirectory;
        
        #region Constructors
        public VlcPlayer()
        {
        }

        /// <summary>
        /// Initializes new instance of the Player. Finds common VLC install path
        /// </summary>
        /// <param name="vlcPath"></param>
        public VlcPlayer(string vlcPath = null)
        {            
            if (!string.IsNullOrWhiteSpace(vlcPath))
            {
                libDirectory = new DirectoryInfo(vlcPath);
            }
            else
            {
                libDirectory = new DirectoryInfo(IntPtr.Size == 4 ? VLC_X86 : VLC_X86.Replace(" (x86)", ""));
            }

            CheckVlcPath(libDirectory.FullName);
        }

        #endregion

        #region Events
        public event TimeChangedEvent TimeChanged;
        public event MediaLoadedEvent MediaLoaded;
        public event MediaFinishedEvent MediaFinished; 
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes VLC 
        /// </summary>
        public void Init()
        {
            if (_vlcMediaPlayer == null)
                _vlcMediaPlayer = new VlcMediaPlayer(libDirectory);

            _vlcMediaPlayer.TimeChanged += _vlcMediaPlayer_TimeChanged;
            _vlcMediaPlayer.EncounteredError += _vlcMediaPlayer_EncounteredError;
            _vlcMediaPlayer.EndReached += _vlcMediaPlayer_EndReached;
            //TODO: Log from VLC
            //_vlcMediaPlayer.Log += _vlcMediaPlayer_Log;
        }

        /// <summary>
        /// Initializes VLC specified CLI options.
        /// </summary>
        /// <param name="options">The VLC command line options.</param>
        public void Init(string[] options)
        {
            _vlcMediaPlayer = new VlcMediaPlayer(libDirectory, options);
            this.Init();            
        }

        /// <summary>
        /// Plays the media
        /// </summary>
        /// <returns>If playing after invoking play</returns>
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
            var media = _vlcMediaPlayer.SetMedia(file);
            if (media != null)
            {
                Play();
                media.Parse();
                MediaLoaded?.Invoke(media.Duration);
            }
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
        #endregion

        #region Private Methods

        private static void CheckVlcPath(string vlcPath)
        {
            if (!Directory.Exists(vlcPath))
                throw new DirectoryNotFoundException(
                    $@"VLC directory not found. VLC Installation is missing or 
set a directory to VLC in the configuration at {vlcPath.Replace(@"VideoLAN\VLC", @"Horsify\Horsify Jukebox.exe.config")}");
        }

        private void _vlcMediaPlayer_EndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            MediaFinished?.Invoke();
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

        #endregion
    }
}
