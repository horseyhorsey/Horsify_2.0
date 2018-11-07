using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Data.Model
{
    public partial class Label
    {
        public Label()
        {
            Song = new HashSet<Song>();
        }

        public long Id { get; set; }
        public long? DiscogsId { get; set; }
        public string Name { get; set; }

        public ICollection<Song> Song { get; set; }
    }
}
