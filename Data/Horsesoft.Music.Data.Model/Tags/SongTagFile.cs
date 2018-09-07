using Horsesoft.Music.Data.Model.Import;

namespace Horsesoft.Music.Data.Model.Tags
{
    public class SongTagFile
    {
        public string Album { get; set; }
        public string Artist { get; set; }
        public int BitRate { get; set; }
        public int? Bpm { get; set; }
        public string Comment { get; set; }
        public string Country { get; set; }        
        public int? DiscogReleaseId { get; set; }
        public string Duration { get; set; }
        public string FilePath { get; set; }
        public string Genre { get; set; }
        public byte[] ImageData { get; set; }
        public string Label { get; set; }
        public string MusicalKey { get; set; }
        public Rating Rating { get; set; }
        public string Title { get; set; }
        public int? TrackNumber { get; set; }
        public int? Year { get; set; }
        public string ImageLocation { get; set; }        
    }
}