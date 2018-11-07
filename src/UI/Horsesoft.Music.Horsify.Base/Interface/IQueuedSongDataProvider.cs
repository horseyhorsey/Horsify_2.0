using Horsesoft.Music.Data.Model;
using System.Collections.ObjectModel;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IQueuedSongDataProvider
    {
        bool Add(AllJoinedTable allJoinedTable);

        ObservableCollection<AllJoinedTable> QueueSongs { get; set; }

        bool ShuffleEnabled { get; set; }

        void Shuffle();
    }
}
