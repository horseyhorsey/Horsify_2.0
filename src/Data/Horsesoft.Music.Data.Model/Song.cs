using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horsesoft.Music.Data.Model
{
    public partial class Song
    {
        public long Id { get; set; }
        public string AddedDate { get; set; }
        public long? AlbumId { get; set; }
        public long? ArtistId { get; set; }
        public long? BitRate { get; set; }
        public long? Bpm { get; set; }
        public string Comment { get; set; }
        public string Country { get; set; }
        public long? DiscogId { get; set; }
        public long FileId { get; set; }
        public long? GenreId { get; set; }
        public string ImageLocation { get; set; }
        public string IsDamaged { get; set; }
        public long? LabelId { get; set; }
        public string LastPlayed { get; set; }
        public long? MusicalKeyId { get; set; }
        public string Nsfw { get; set; }
        public long? Rating { get; set; }
        public string Time { get; set; }
        public long? TimesPlayed { get; set; }
        public string Title { get; set; }
        public long? Track { get; set; }
        public long? Year { get; set; }

        public Album Album { get; set; }
        public Artist Artist { get; set; }
        public Discog Discog { get; set; }
        public File File { get; set; }
        public Genre Genre { get; set; }
        public Label Label { get; set; }
        public MusicalKey MusicalKey { get; set; }
    }
}
