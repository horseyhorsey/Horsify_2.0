using System;

namespace Horsesoft.Vlc
{
    public class Events
    {
        public delegate void MediaFinishedEvent();
        public delegate void MediaLoadedEvent(TimeSpan duration);        
        public delegate void TimeChangedEvent(TimeSpan currentTime);
    }
}
