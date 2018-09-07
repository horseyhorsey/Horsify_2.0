using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Import;
using Horsesoft.Music.Data.Model.Tags;
using Id3;
using Id3.Frames;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Horsesoft.Music.Engine.Tagging
{
    public class SongTaggerId3 : SongTagger
    {
        #region Public Methods
        public override SongTagFile PopulateSongTag(string fileName, TagOption tagOption = TagOption.All)
        {
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            using (Mp3Stream mp3Stream = new Mp3Stream(fileStream, Mp3Permissions.ReadWrite))
            {
                var id3Tag = GetTagFromStream(mp3Stream);

                if (id3Tag != null)
                {
                    var song = new SongTagFile();
                    song.Duration = mp3Stream.Audio.Duration.ToString();
                    song.BitRate = mp3Stream.Audio.Bitrate;
                    return CreateSongTag(song, id3Tag, tagOption);
                }                
            }

            return null;
        }
        #endregion

        #region Private Static Methods
        private static void GetAlbum(SongTagFile song, Id3Tag tag)
        {
            //set standard issue props
            if (tag.Album.IsAssigned)
            {
                song.Album = tag.Album.Value.CleanString();
            }
        }
        private static void GetArtist(SongTagFile song, Id3Tag tag)
        {
            if (tag.Artists.IsAssigned)
            {
                song.Artist = tag.Artists.Value.CleanString();
            }
        }
        private static void GetBpm(SongTagFile song, Id3Tag tag)
        {
            song.Bpm = tag.BeatsPerMinute.AsInt;
        }
        private static void GetComments(SongTagFile song, Id3Tag tag)
        {
            //Get comment from list
            if (tag.Comments.Count > 0)
                song.Comment = tag.Comments[0].Comment.CleanString();
        }
        private static void GetGenre(SongTagFile song, Id3Tag tag)
        {
            if (tag.Genre.IsAssigned)
            {
                song.Genre = tag.Genre.Value.CleanString();
            }
        }
        private static void GetLabelPublisher(SongTagFile song, Id3Tag tag)
        {
            if (tag.Publisher.IsAssigned)
            {
                song.Label = tag.Publisher.Value.CleanString();
            }
        }
        private static void GetPictureBytes(SongTagFile song, Id3Tag tag)
        {
            //Extract the picture to bytes.
            if (tag.Pictures?.Count > 0)
            {
                song.ImageData = tag.Pictures[0].PictureData;
            }
        }
        /// <summary>
        /// Gets the rating from an Unknown frame. POPM
        /// </summary>
        /// <remarks>
        /// http://id3.org/id3v2.4.0-frames POPM
        /// </remarks>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        private static void GetRating(SongTagFile song, Id3.Id3Tag tag)
        {
            var unknownFrames = (tag.Frames.Where(x => x.GetType() == typeof(UnknownFrame)));
            foreach (var item in unknownFrames)
            {
                var s = item as UnknownFrame;
                if (s.Id == "POPM")
                {
                    //Byte Length 35 - if it includes a playcount
                    //Byte Length 31 - No Playcount - Rating should be at the end of the array
                    var f = s.Encode();

                    //Get rating byte depending on length
                    byte[] ratings = new byte[1];
                    if (f.Length == 31)
                        ratings[0] = f[30];
                    else if (f.Length == 35)
                        ratings[0] = f[f.Length - 5];

                    song.Rating = ConvertRating(ratings[0]);
                    return;
                }
            }
        }
        private static void GetTitle(SongTagFile song, Id3Tag tag)
        {
            song.Title = tag.Title.IsAssigned ? tag.Title.Value.CleanString() : string.Empty;
        }
        private static void GetTrackNumber(SongTagFile song, Id3Tag tag)
        {
            song.TrackNumber = tag.Track.AsInt ?? 1;
        }
        private static void GetYear(SongTagFile song, Id3Tag tag)
        {
            //Get year from DateTime
            if (tag.Year.AsDateTime.HasValue)
                song.Year = tag.Year.AsDateTime.HasValue ? tag.Year.AsDateTime.Value.Year : 0;
        }
        #endregion

        #region Private Methods

        private static Rating ConvertRating(byte rating)
        {
            if (rating >= 255)
                return Rating.Rank5;
            else if (rating >= 196)
                return Rating.Rank4;
            else if (rating >= 128)
                return Rating.Rank3;
            else if (rating >= 64)
                return Rating.Rank2;
            else if (rating >= 1)
                return Rating.Rank1;

            return Rating.None;
        }

        /// <summary>
        /// Parses Id3 to create a song model
        /// </summary>
        /// <param name="songFile">song file path</param>
        /// <returns>new song</returns>
        private SongTagFile CreateSongTag(SongTagFile song, Id3Tag tag, TagOption options = TagOption.All)
        {
            //If that tag returned null return
            if (tag == null)
                return song;

            GetTrackNumber(song, tag);
            GetArtist(song, tag);
            GetTitle(song, tag);
            GetAlbum(song, tag);
            GetYear(song, tag);
            GetRating(song, tag);
            GetBpm(song, tag);
            GetGenre(song, tag);
            GetComments(song, tag);
            GetMusicalKey(song, tag);

            //ARTWORK
            if (options.HasFlag(TagOption.All) || options.HasFlag(TagOption.Artwork))
            {
                GetPictureBytes(song, tag);
            }
            //Country
            if (options.HasFlag(TagOption.All) || options.HasFlag(TagOption.Country))
            {
                song.Country = GetCountry(tag);
            }
            //DISCOGS
            if (options.HasFlag(TagOption.All) || options.HasFlag(TagOption.Discog))
            {
                GetDiscogsId(song, tag);
            }
            //LABEL
            if (options.HasFlag(TagOption.All) || options.HasFlag(TagOption.Label))
            {
                GetLabelPublisher(song, tag);
            }

            return song;
        }

        /// <summary>
        /// Gets the country name from the file tag
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        private string GetCountry(Id3.Id3Tag tag)
        {
            Id3Frame countryFrame = tag.Frames.Where(x => x.ToString().ToUpper().Contains("DISCOGS_COUNTRY\0﻿")).FirstOrDefault();
            if (countryFrame != null)
            {
                return countryFrame.ToString().ToUpper().Replace("DISCOGS_COUNTRY\0﻿", "");
            }
            else
            {
                countryFrame = tag.Frames.Where(x => x.ToString().ToUpper().Contains("COUNTRY\0﻿")).FirstOrDefault();
                if (countryFrame != null)
                {
                    return countryFrame.ToString().ToUpper().Replace("COUNTRY\0﻿", "");
                }
            }

            return null;
        }
        /// <summary>
        /// Gets (parses) the discog identifier from an <see cref="Id3.Id3Tag"/>
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        private Discog GetDiscogId(Id3.Id3Tag tag)
        {
            string _discogsId = "0";

            Id3Frame discogsFrame = tag.Frames.Where(x => x.ToString().ToUpper().Contains("DISCOGS_RELEASE_ID\0﻿")).FirstOrDefault();
            Discog discog = new Discog();
            if (discogsFrame != null)
                _discogsId = discogsFrame.ToString().ToUpper().Replace("DISCOGS_RELEASE_ID\0﻿", "");
            else
            {
                //Try get release Id other search
                discogsFrame = tag.Frames.Where(x => x.ToString().ToUpper().Contains("DISCOGSID\0")).FirstOrDefault();
                if (discogsFrame != null)
                    _discogsId = discogsFrame.ToString().ToUpper().Replace("DISCOGSID\0", "");
                else
                {
                    discogsFrame = tag.Frames.Where(x => x.ToString().ToUpper().Contains("DISCOGS-ID\0")).FirstOrDefault();

                    if (discogsFrame == null)
                        return discog;

                    _discogsId = discogsFrame.ToString().ToUpper().Replace("DISCOGS-ID\0", "");
                }
            }

            if (!string.IsNullOrEmpty(_discogsId))
            {
                int i;
                int.TryParse(_discogsId, out i);

                if (i != 0)
                {
                    discog.ReleaseId = i;
                }
                else
                    discog.ReleaseId = 0;
            }

            return discog;
        }

        private void GetDiscogsId(SongTagFile song, Id3Tag tag)
        {
            long discogId = GetDiscogId(tag).ReleaseId;
            if (discogId > 0)
            {
                song.DiscogReleaseId = (int)discogId;
            }
        }

        private void GetMusicalKey(SongTagFile song, Id3Tag tag)
        {
            song.MusicalKey = GetMusicKey(tag).Replace("þ", "");
        }

        private string GetMusicKey(Id3Tag tag)
        {
            var unknownFrames = (tag.Frames.Where(x => x.GetType() == typeof(UnknownFrame)));
            foreach (var item in unknownFrames)
            {
                var s = item as UnknownFrame;
                if (s.Id == "TKEY")
                {
                    var f = s.Encode();
                    var len = f.Length;
                    string gg = Encoding.Default.GetString(f).CleanString();
                    var convertedKey = GetOpenKeyNotation(gg);
                    return gg;
                }
            }

            return string.Empty;
        }

        private string GetOpenKeyNotation(string keyString)
        {
            OpenKeyNotation key = OpenKeyNotation.None;
            Enum.TryParse(keyString, out key);

            //We found the correct enum string.
            if (key != OpenKeyNotation.None)
                return key.ToString();

            //Convert a string into enum
            key = ConvertStringToMusicKey(keyString);
            if (key == OpenKeyNotation.None)
                return null;

            return key.ToString();
        }

        /// <summary>
        /// Tries to convert known music strings to OpenKey Enum
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns></returns>
        private OpenKeyNotation ConvertStringToMusicKey(string keyString)
        {
            switch (keyString)
            {
                #region Minor Keys
                case "1m":
                case "8A":
                case "A m":
                case "Amin":
                    return OpenKeyNotation.Am;
                case "2m":
                case "9A":
                case "E m":
                case "Emin":
                    return OpenKeyNotation.Em;
                case "3m":
                case "10A":
                case "B m":
                case "Bmin":
                    return OpenKeyNotation.Bm;
                case "4m":
                case "11A":
                case "F#min":
                case "F#m":
                    return OpenKeyNotation.Gbm;
                case "5m":
                case "12A":
                case "C#m":
                    return OpenKeyNotation.Dbm;
                case "6m":
                case "1A":
                case "G#m":
                    return OpenKeyNotation.Abm;
                case "7m":
                case "2A":
                case "D#m":
                    return OpenKeyNotation.Ebm;
                case "8m":
                case "3A":
                case "A#m":
                    return OpenKeyNotation.Bbm;
                case "9m":
                case "4A":
                case "F m":
                case "Fmin":
                    return OpenKeyNotation.Fm;
                case "10m":
                case "5A":
                case "C m":
                case "Cmin":
                    return OpenKeyNotation.Cm;
                case "11m":
                case "6A":
                case "G m":
                case "Gmin":
                    return OpenKeyNotation.Gm;
                case "12m":
                case "7A":
                case "D m":
                case "Dmin":
                    return OpenKeyNotation.Dm;
                #endregion

                #region Major Keys
                case "1d":
                case "8B":
                case "Cmaj":
                    return OpenKeyNotation.C;
                case "2d":
                case "9B":
                case "Gmaj":
                    return OpenKeyNotation.G;
                case "3d":
                case "10B":
                case "Dmaj":
                    return OpenKeyNotation.D;
                case "4d":
                case "11B":
                case "Amaj":
                    return OpenKeyNotation.A;
                case "5d":
                case "12B":
                case "Emaj":
                    return OpenKeyNotation.E;
                case "6d":
                case "1B":
                case "Bmaj":
                    return OpenKeyNotation.B;
                case "7d":
                case "2B":
                case "F#":
                case "F#maj":
                    return OpenKeyNotation.Gb;
                case "8d":
                case "3B":
                case "C#":
                case "C#maj":
                    return OpenKeyNotation.Db;
                case "9d":
                case "4B":
                case "G#":
                case "G#maj":
                    return OpenKeyNotation.Ab;
                case "10d":
                case "5B":
                case "D#":
                case "D#maj":
                    return OpenKeyNotation.Eb;
                case "11d":
                case "6B":
                case "A#":
                case "A#maj":
                    return OpenKeyNotation.Bb;
                case "12d":
                case "7B":
                case "Fmaj":
                    return OpenKeyNotation.F; 
                #endregion

                default:
                    return OpenKeyNotation.None;
            }
        }

        /// <summary>
        /// Retrieves either ID3V2 or Id3V1 tag
        /// </summary>
        /// <param name="mp3Stream">The MP3 stream.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        private Id3Tag GetTagFromStream(Mp3Stream mp3Stream)
        {
            Id3.Id3Tag tag = null;            

            //Exit, we have no tags
            if (!mp3Stream.HasTags)
                return tag;

            //Get the file start tag which is ID3v2
            tag = mp3Stream.GetTag(Id3TagFamily.FileStartTag);
            if (tag == null)
            {                
                tag = mp3Stream.GetTag(Id3TagFamily.FileEndTag);

                if (tag == null)
                    throw new NullReferenceException($"Couldn't parse tag for file");
            }

            //Get the file start tag which is ID3v2
            else if (!tag.Year.IsAssigned)
            {
                var tagForYear = mp3Stream.GetTag(Id3TagFamily.FileEndTag);
                if (tagForYear != null)
                {
                    if (tagForYear.Year.IsAssigned)
                    {
                        var year = tagForYear.Year.AsDateTime.Value;
                        tag.Year.Value = Convert.ToString(year.Year);
                    }
                }
            }
                
            return tag;
        }
        #endregion
    }
}
