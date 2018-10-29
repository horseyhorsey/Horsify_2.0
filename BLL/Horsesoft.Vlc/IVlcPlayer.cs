using System;
using static Horsesoft.Vlc.Events;

namespace Horsesoft.Vlc
{
    public interface IVlcPlayer
    {
        event MediaLoadedEvent MediaLoaded;
        event MediaFinishedEvent MediaFinished;
        event TimeChangedEvent TimeChanged;

        void Init();
        void Init(string[] options);
        /// <summary>
        /// Plays the media
        /// </summary>
        /// <returns>Whether playing or not</returns>
        bool Play();
        /// <summary>
        /// Pauses the media
        /// </summary>
        /// <returns>Whether paused or not</returns>
        bool Pause();
        void SetMedia(Uri file);
        void SetmediaPosition(float position);
        void SetVolume(int volume);
        void Stop();
    }
}
