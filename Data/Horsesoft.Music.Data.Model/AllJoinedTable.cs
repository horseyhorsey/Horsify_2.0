using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;

namespace Horsesoft.Music.Data.Model
{
    [DataContract]
    public partial class AllJoinedTable
    {
        #region Song Props
        [DataMember]
        public int Id { get; set; } = 1;

        [DataMember]
        public int? Rating { get; set; }

        [DataMember]
        public long? Year { get; set; }

        [DataMember]
        public string Artist { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Album { get; set; }

        [DataMember]
        public string MusicKey { get; set; }

        [DataMember]
        public long? Bpm { get; set; }

        [DataMember]
        public long? BitRate { get; set; }

        [DataMember]
        public long? Track { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Genre { get; set; }

        [DataMember]
        public long? DiscogId { get; set; }

        [DataMember]
        public int? ReleaseId { get; set; }

        [DataMember]
        public string ImageLocation { get; set; }

        [DataMember]
        public int? AddedDate { get; set; }

        [DataMember]
        public string IsDamaged { get; set; }

        [DataMember]
        public int? LastPlayed { get; set; }

        [DataMember]
        public long? TimesPlayed { get; set; }

        [DataMember]
        public bool? NSFW { get; set; }

        [DataMember]
        public string FileLocation { get; set; }
        #endregion

        public AllJoinedTable()
        {
            Id = 1;
        }
    }

    public class AllJoinedTableExtended : AllJoinedTable
    {
        public string SongImage { get; set; } = null;
        public bool ChangesMade { get; set; }
        private object SpareImage { get; set; }

        /// <summary>
        /// Gets the safe filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public string GetSafeFilename(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        /// <summary>
        /// Gets the image location for song. Go's by DiscogId, Album, Artist + title
        /// </summary>
        /// <param name="artworkFolder">The artwork folder.</param>
        public void GetImageLocation(string artworkFolder)
        {
            if (string.IsNullOrWhiteSpace(ImageLocation) || !System.IO.File.Exists(Path.Combine(artworkFolder, ImageLocation)))
            {
                SongImage = Path.Combine(artworkFolder, "ho.jpg");
            }
            else
                SongImage = artworkFolder + "\\" + ImageLocation;
        }

        /// <summary>
        /// Concatenates the Artwork folder to the 
        /// </summary>
        /// <param name="song"></param>
        /// <param name="artworkFolder"></param>
        public void GetImageLocation(AllJoinedTable song, string artworkFolder)
        {
            if (string.IsNullOrWhiteSpace(song.ImageLocation) || !System.IO.File.Exists(Path.Combine(artworkFolder, song.ImageLocation)))
            {
                SongImage = Path.Combine(artworkFolder, "ho.jpg");
            }
            else
                SongImage = artworkFolder + "\\" + song.ImageLocation;
        }

        // Use the WPF BitmapImage class to load and 
        // resize the bitmap. NOTE: Only 32bpp formats are supported correctly.
        // Support for additional color formats is left as an exercise
        // for the reader. For more information, see documentation for ColorConvertedBitmap.
        //BitmapImage LoadImage(string filename)
        //{
        //    BitmapImage bitmapImage = new BitmapImage();
        //    bitmapImage.BeginInit();
        //    bitmapImage.UriSource = new Uri(filename);
        //    //bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        //    bitmapImage.DecodePixelHeight = 75;
        //    bitmapImage.DecodePixelWidth = 75;
        //    bitmapImage.EndInit();
        //    //bitmapImage.Format = forma
        //    //int size = (int)(bitmapImage.Height * bitmapImage.Width);
        //    //int stride = (int)bitmapImage.Width * 4;
        //    //byte[] dest = new byte[stride * tilePixelHeight];

        //    return bitmapImage;
        //}
    }
}
