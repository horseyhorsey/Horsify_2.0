using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Data.Model
{
    public partial class MusicalKey
    {
        public MusicalKey()
        {
            Song = new HashSet<Song>();
        }

        public long Id { get; set; }
        public string MusicKey { get; set; }

        public ICollection<Song> Song { get; set; }
    }
}
