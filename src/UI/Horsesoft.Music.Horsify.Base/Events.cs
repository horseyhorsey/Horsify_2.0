using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify.Audio;
using Horsesoft.Music.Data.Model.Menu;
using Prism.Events;
using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.Base
{
    public class OnNavigateViewEvent<T> : PubSubEvent<string> { }

    public class StartScanSongsEvent : PubSubEvent { }
    public class StopScanSongsEvent : PubSubEvent { }
    public class ScanSongsEndedEvent : PubSubEvent { }

    #region Shutdown / Minimize
    public class MinimizeEvent : PubSubEvent
    {
        public void Publish(object onMinimizeExecuted)
        {
            throw new NotImplementedException();
        }
    }
    public class ShutdownEvent : PubSubEvent { }
    #endregion

    #region Queue Events
    public class ClearQueueEvent : PubSubEvent { }
    public class ShuffleQueueEvent : PubSubEvent { }
    public class SkipQueueEvent : PubSubEvent { }
    public class QueuedJobsCompletedEvent : PubSubEvent { }
    #endregion

    #region Playlist Events    
    /// <summary>
    /// Adds to a playlist. The name of the playlist should be sent in
    /// </summary>
    /// <seealso cref="Prism.Events.PubSubEvent{System.Collections.Generic.Dictionary{System.String, Horsesoft.Music.Data.Model.AllJoinedTable}}" />
    public class AddToPlaylistEvent : PubSubEvent<Dictionary<string, AllJoinedTable>> { }
    #endregion

    #region Media Prism Events
    public class OnMediaPlay<T> : PubSubEvent<AllJoinedTable> { }
    public class OnMediaChangedVolumeEvent<T> : PubSubEvent<T> { }    
    #endregion

    #region Vlc events
    public delegate void OnMediaFinishedEvent();
    public delegate void OnMediaLoaded(TimeSpan duration);
    public delegate void OnTimeChanged(TimeSpan duration);
    #endregion

    /// <summary>
    /// Let view models know the search is completed
    /// </summary>
    public class HorsifySearchCompletedEvent : PubSubEvent { }


}
