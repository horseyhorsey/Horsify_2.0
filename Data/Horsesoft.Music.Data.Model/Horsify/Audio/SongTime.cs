using System;

namespace Horsesoft.Music.Data.Model.Horsify.Audio
{
    public class SongTime
    {
        public TimeSpan CurrentSongTime;
        public TimeSpan Duration;

        public SongTime()
        {

        }

        public SongTime(TimeSpan currentTime, TimeSpan duration)
        {
            CurrentSongTime = currentTime;
            Duration = duration;
        }
    }
}
