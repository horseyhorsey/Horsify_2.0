using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Data.Model
{
    public partial class Genre
    {
        public Genre()
        {
            Song = new HashSet<Song>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Style { get; set; }

        public ICollection<Song> Song { get; set; }
    }
}
