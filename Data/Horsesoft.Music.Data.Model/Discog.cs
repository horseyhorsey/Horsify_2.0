using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Data.Model
{
    public partial class Discog
    {
        public Discog()
        {
            Song = new HashSet<Song>();
        }

        public long Id { get; set; }
        public long ReleaseId { get; set; }

        public ICollection<Song> Song { get; set; }
    }
}
