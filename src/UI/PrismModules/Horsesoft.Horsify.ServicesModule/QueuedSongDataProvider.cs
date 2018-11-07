using System.Collections.ObjectModel;
using System.Linq;
using Horsesoft.Horsify.ServicesModule.Extensions;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base.Interface;

namespace Horsesoft.Horsify.ServicesModule
{
    public class QueuedSongDataProvider : IQueuedSongDataProvider
    {
        public ObservableCollection<AllJoinedTable> QueueSongs { get; set; }

        public bool ShuffleEnabled { get; set; }

        public QueuedSongDataProvider()
        {
            QueueSongs = new ObservableCollection<AllJoinedTable>();
        }

        public bool Add(AllJoinedTable allJoinedTable)
        {
            if (!QueueSongs.Any(x => x == allJoinedTable))
                QueueSongs.Add(allJoinedTable);
            else
                return false;

            return true;
        }

        public void Shuffle()
        {
            QueueSongs.Shuffle();
        }
    }
}
