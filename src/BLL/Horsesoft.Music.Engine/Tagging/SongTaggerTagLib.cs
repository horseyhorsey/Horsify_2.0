using Horsesoft.Music.Data.Model.Import;
using Horsesoft.Music.Data.Model.Tags;
using System;
using TagLib.Id3v2;

namespace Horsesoft.Music.Engine.Tagging
{
    /// <summary>
    /// Not a completed class. Use <see cref="SongTaggerId3"/>
    /// </summary>
    /// <seealso cref="Horsesoft.Music.Engine.Tagging.SongTagger" />
    public class SongTaggerTagLib : SongTagger
    {
        public override SongTagFile PopulateSongTag(string fileName, TagOption options = TagOption.All)
        {
            //Can we scan discogs
            var discogOption = options.HasFlag(TagOption.Discog);
            //var tagged = Tagger.GetMp3Tag(fileName);
            return CreateSongModelWithTagLib(fileName);
        }

        public override bool UpdateFileTag(string fileName, byte rating)
        {
            return SetFileRating(fileName, rating);
        }

        /// <summary>
        /// Saves a traktor rating into an mp3. The Traktor rating should be picked up by all media players...
        /// </summary>
        /// <param name="songFile">The song file.</param>
        /// <param name="rating">The rating.</param>
        /// <returns></returns>
        private static bool SetFileRating(string songFile, byte rating)
        {
            try
            {
                TagLib.File track = TagLib.File.Create(songFile);
                var tag = GetSongTag(songFile, track);

                var usr = "horsify@horsify.uk";                
                PopularimeterFrame popmFrame = PopularimeterFrame.Get((Tag)tag, usr, true);
                popmFrame.Rating = rating;
                track.Save();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected SongTagFile CreateSongModelWithTagLib(string songFile)
        {
            //var hash = this.GetFileHash(songFile);
            TagLib.Tag tag = null;
            using (TagLib.File track = TagLib.File.Create(songFile))
            {
                tag = GetSongTag(songFile, track);

                if (tag != null)
                {
                    return BuildSongFromTag(tag, track);
                }
                else
                {
                    return null;
                }
            }
        }

        private static TagLib.Tag GetSongTag(string songFile, TagLib.File track)
        {
            TagLib.Tag tag;
            if (System.IO.Path.GetExtension(songFile.ToUpper()) == ".FLAC")
                tag = track.GetTag(TagLib.TagTypes.FlacMetadata);
            else
                tag = track.GetTag(TagLib.TagTypes.Id3v2);
            return tag;
        }

        private SongTagFile BuildSongFromTag(TagLib.Tag tag, TagLib.File track)
        {
            var song = new SongTagFile();
            
            song.Album = tag.Album;
            song.Artist = tag.FirstPerformer;
            song.Bpm = (int?)tag.BeatsPerMinute;
            song.Label = tag.Publisher;
            song.Genre = tag.FirstGenre;            
            song.Title = tag.Title;
            song.Year = (int)tag.Year;
            song.ImageData = tag.Pictures[0].Data.Data;
            song.Comment = tag.Comment;
            song.Country = tag.Country;
            song.MusicalKey = tag.TKey;
            song.TrackNumber = (int?)tag.Track;

            //Set discogs
            int discogsId = 0;
            try
            {
                discogsId = Convert.ToInt32(tag.DiscogsId);
                song.DiscogReleaseId = discogsId;
            }
            catch { }

            song.Duration = track.Properties.Duration.ToString("hh':'mm':'ss");
            song.BitRate = track.Properties.AudioBitrate;            
            return song;
        }
    }
}
