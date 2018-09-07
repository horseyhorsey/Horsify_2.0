using System;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IMediaPlayer
    {
        void Pause();
        void Play();
        void SetPosition(int framePos);
        void Stop();

        TimeSpan GetCurrentTime();
        TimeSpan GetCurrentPosition();
    }
}
