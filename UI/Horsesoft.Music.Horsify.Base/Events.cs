using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify.Audio;
using Horsesoft.Music.Data.Model.Menu;
using Prism.Events;
using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.Base
{

    public class OnAddToQueueEvent<T> : PubSubEvent<IEnumerable<AllJoinedTable>> { }

    /// <summary>
    /// Previously named SearchSongsMessage in MvvmLightVersion
    /// </summary>
    /// <seealso cref="Prism.Events.PubSubEvent" />
    public class OnSearchedSongEvent : PubSubEvent { }
    public class OnSearchedSongEvent<T> : PubSubEvent<T> { }
    public class OnNavigateViewEvent<T> : PubSubEvent<string> { }
    public class OnMenuParentSelectedEvent<T> : PubSubEvent<IMenuComponent> { }
    public class OnSwitchSkinEvent<TSkin> : PubSubEvent<string> { }

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
    public class OnAdvanceQueue : PubSubEvent { }
    /// <summary>
    /// Should be a QueueItemViewModel
    /// </summary>
    /// <seealso cref="Prism.Events.PubSubEvent{System.Object}" />
    public class OnPlayQueuedSongEvent : PubSubEvent<object> { }
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

    #region Media Elements

    public class OnMediaChangeVolumeEvent<TUpDown> : PubSubEvent<string> { }
    public class OnMediaPlayPauseEvent<T> : PubSubEvent<bool> { }
    public class OnMediaPlay<T> : PubSubEvent<AllJoinedTable> { }
    public class OnMediaStopped : PubSubEvent { }

    //SongTimeChangedMessage
    public class OnMediaTimeChangedEvent<T> : PubSubEvent<SongTime> { }    
    public class OnMediaChangedVolumeEvent<T> : PubSubEvent<double> { }
    public class OnMediaSetPositionEvent<T> : PubSubEvent<TimeSpan> { }
    public class OnMediaSeekEvent<T> : PubSubEvent<bool> { }


    /// <summary>
    /// Let view models know the search is completed
    /// </summary>
    public class HorsifySearchCompletedEvent : PubSubEvent { }

    /// <summary>
    /// Increments or decrements volume
    /// </summary>
    /// <seealso cref="Prism.Events.PubSubEvent{System.String}" />
    public class ChangeVolumeEvent : PubSubEvent<string> { }

    /// <summary>
    /// Sets the volume
    /// </summary>
    /// <seealso cref="Prism.Events.PubSubEvent{System.Double}" />
    public class SetVolumeEvent : PubSubEvent<double> { }    

    /// <summary>
    /// The current volume
    /// </summary>
    /// <seealso cref="Prism.Events.PubSubEvent{System.Double}" />
    public class VolumeChangedEvent : PubSubEvent<double> { }

    #endregion
}
