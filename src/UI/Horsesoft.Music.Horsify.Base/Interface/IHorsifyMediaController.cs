using System;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IHorsifyMediaController
    {
        event OnMediaFinishedEvent OnMediaFinished;
        event OnMediaLoaded OnMediaLoaded;
        event OnTimeChanged OnTimeChanged;

        bool PlayPause(bool isPlaying);
        void SetMedia(Uri file);
        void SetMediaPosition(double position);
        void SetVolume(int currentVolume);
        void Stop();
    }
}
