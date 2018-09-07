using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Data.Model
{
    public partial class File
    {
        public File()
        {
            Song = new HashSet<Song>();
        }

        public long Id { get; set; }
        public string DriveVolume { get; set; }
        public string FileName { get; set; }
        public string Folder { get; set; }
        public string Hash { get; set; }

        public ICollection<Song> Song { get; set; }

        public override string ToString()
        {
            return $"{DriveVolume}{Folder}\\{FileName}";
        }
    }
}
