using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Vlc;
using System;
using System.IO;

namespace Horsesoft.Horsify.ServicesModule
{
    public class HorsifyVlcMediaController : IHorsifyMediaController
    {
        #region Fields
        private static bool _isInitialized = false;
        private IVlcPlayer _vlcPlayer;
        private IVlcPlayer _vlcPlayer2;
        private bool _vlcPlayer2IsInitialized;
        #endregion

        #region Events
        public event OnMediaFinishedEvent OnMediaFinished;
        public event OnMediaLoaded OnMediaLoaded;
        public event OnTimeChanged OnTimeChanged;        
        #endregion

        #region Constructors
        public HorsifyVlcMediaController(string vlcPath)
        {
            _vlcPlayer = new VlcPlayer(vlcPath);
            _vlcPlayer.Init();
            _isInitialized = true;

            _vlcPlayer.MediaFinished += () => OnMediaFinished?.Invoke();
            _vlcPlayer.MediaLoaded += (x) => OnMediaLoaded?.Invoke(x);
            _vlcPlayer.TimeChanged += (x) => OnTimeChanged?.Invoke(x);
        }

        /// <summary>
        /// Todo. Crossfades / Two decks
        /// </summary>
        /// <param name="vlcPath"></param>
        /// <param name="dualmode"></param>
        public HorsifyVlcMediaController(string vlcPath, bool dualmode) : this(vlcPath)
        {            
            if (dualmode)
            {
                _vlcPlayer2 = new VlcPlayer(vlcPath);
                _vlcPlayer2.Init();
                _vlcPlayer2IsInitialized = true;

                _vlcPlayer2.MediaFinished += () => OnMediaFinished?.Invoke();
                _vlcPlayer2.MediaLoaded += (x) => OnMediaLoaded?.Invoke(x);
                _vlcPlayer2.TimeChanged += (x) => OnTimeChanged?.Invoke(x);
            }
        }
        #endregion

        #region Public Methods
        public bool PlayPause(bool isPlaying)
        {
            if (!isPlaying)
            {
                _vlcPlayer.Play();
                return true;
            }
            else
            {
                _vlcPlayer.Pause();
                return false;
            }
        }

        public void SetMedia(Uri file)
        {
            _vlcPlayer.SetMedia(file);
        }

        public void SetMediaPosition(double position)
        {
            _vlcPlayer.SetmediaPosition((float)position);
        }

        public void SetVolume(int currentVolume)
        {
            _vlcPlayer.SetVolume(currentVolume);
        }

        public void Stop()
        {
            _vlcPlayer.Stop();
        }
        #endregion
    }
}
