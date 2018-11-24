using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Horsesoft.Horsify.ServicesModule.Extensions;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base.Interface;

namespace Horsesoft.Horsify.ServicesModule
{
    public class QueuedSongDataProvider : IQueuedSongDataProvider
    {
        private Random _random = new Random();

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

        public void QueueSongRange(IEnumerable<AllJoinedTable> songs, int amount)
        {            
            var filterCount = songs.Count() - 1;
            if (filterCount > 1)
            {
                _random = new Random();
                for (int i = 0; i < amount; i++)
                {
                    //Select random id from the fitlered count.
                    //Only add song if not already exisiting.
                    var id = _random.Next(filterCount);
                    var song = songs.ElementAt(id);
                    if (!QueueSongs.Contains(song))
                    {
                        QueueSongs.Add(song);                        
                    }
                }
            }
        }
    }
}
