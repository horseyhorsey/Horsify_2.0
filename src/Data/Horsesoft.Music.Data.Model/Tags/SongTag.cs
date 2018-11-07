using System;

namespace Horsesoft.Music.Data.Model.Tags
{
    [Serializable]
    public class SongTag
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public int Year { get; set; }
        public string Label { get; set; }
        public string Genre { get; set; }
        public decimal Bpm { get; set; }
        public uint BitRate { get; set; }
        public string Comment { get; set; }
        public string Key { get; set; }
        public int Ranking { get; set; }
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public string DriveVolume { get; set; }

        public override string ToString() => Artist + " - " + Title + " - " + Year;
    }
}
