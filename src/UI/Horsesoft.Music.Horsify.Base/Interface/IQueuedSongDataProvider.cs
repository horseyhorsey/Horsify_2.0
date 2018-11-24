using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IQueuedSongDataProvider
    {
        bool Add(AllJoinedTable allJoinedTable);
        ObservableCollection<AllJoinedTable> QueueSongs { get; set; }
        void QueueSongRange(IEnumerable<AllJoinedTable> songs, int amount);
        bool ShuffleEnabled { get; set; }
        void Shuffle();        
    }
}
