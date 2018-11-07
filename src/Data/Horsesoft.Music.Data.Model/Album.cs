using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Data.Model
{
    public partial class Album
    {
        public Album()
        {
            Song = new HashSet<Song>();
        }

        public long Id { get; set; }
        public string Title { get; set; }

        public ICollection<Song> Song { get; set; }
    }
}
