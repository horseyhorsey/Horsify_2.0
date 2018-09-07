using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Import;
using Horsesoft.Music.Data.Model.Tags;
using Horsesoft.Music.Engine.Tagging;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Horsesoft.Music.Engine.Import
{
    /// <summary>
    /// Gets, Updates and saves tag information from databased files.
    /// </summary>
    public interface ITagImport
    {
        void CancelTagging();
        SongTagFile GetTagInformation(Data.Model.File file, TagOption options = TagOption.All);
        SongTagFile GetTagInformation(string fileName, TagOption options = TagOption.All);
        event Action<string> OnTagUpdated;
        Task<Dictionary<string, string>> UpdateAllTags(IEnumerable<Data.Model.File> files, TagOption tagOption = TagOption.All);
        void UpdateDbSongTag(SongTagFile taggedSong, int id);
        void SaveChanges();        
    }

    /// <summary>
    /// Gets, Updates and saves tag information from databased files.
    /// </summary>
    public class TagImport : ITagImport
    {
        private ISongTagger _songTagger;
        private IHorsifyDataRepo _horsifyDataRepo;
        private IHorsifySettings _horsifySettings;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        public bool CancelPending { get; set; }
        public event Action<string> OnTagUpdated;

        #region Constructors
        public TagImport(IHorsifySettings horsifySettings)
        {
            _songTagger = new SongTaggerId3();
            _horsifyDataRepo = new HorsifyDataSqliteRepo();
            _horsifySettings = horsifySettings;
        } 
        #endregion

        public void CancelTagging()
        {
            try
            {
                if (_cts.Token != null)
                    _cts?.Cancel();
            }
            catch { }
        }

        public SongTagFile GetTagInformation(Data.Model.File file, TagOption options = TagOption.All)
        {
            var songTag = GetTagInformation(file.ToString(), options);        

            return songTag;
        }

        /// <summary>
        /// Gets the tag information by invoking <see cref="SongTagger.PopulateSongTag(string, TagOption)"/>
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public SongTagFile GetTagInformation(string fileName, TagOption options = TagOption.All)
        {
            return _songTagger.PopulateSongTag(fileName, options);
        }

        public async Task<Dictionary<string, string>> UpdateAllTags(IEnumerable<Data.Model.File> files, TagOption tagOption = TagOption.All)
        {
            _cts = new CancellationTokenSource();

            //Make sure Directories exist for artwork
            CreateArtworkDirectories();

            return await Task.Run(() =>
            {
                int i = 0;
                var failedFiles = new Dictionary<string, string>();

                foreach (var file in files)
                {
                    try
                    {
                        if (_cts.IsCancellationRequested)
                            throw new OperationCanceledException();

                        try
                        {
                            //Get tag and save to Db, Add to failed here.
                            var taggedSong = GetTagInformation(file, tagOption);

                            //Create an image
                            SaveImageFromTag(taggedSong);

                            if (taggedSong != null)
                            {
                                //Save image                        
                                _horsifyDataRepo.UpdateDbSongTag(taggedSong, (int)file.Id);

                                OnTagUpdated?.Invoke($"Tag update: {taggedSong.Title}");

                                if (i > 250)
                                {
                                    i = -1;
                                    ((IUnitOfWork)_horsifyDataRepo).Save();
                                }
                                else
                                    i++;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            failedFiles.Add(file.ToString(), ex.Message);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                   
                }

            ((IUnitOfWork)_horsifyDataRepo).Save();

                return failedFiles;
            },_cts.Token);
        }

        /// <summary>
        /// Creates the artwork directories needed for storing cover art
        /// </summary>
        private void CreateArtworkDirectories()
        {
            var coverArtDir = Path.Combine(_horsifySettings.HorsifyPath, "CoverArt");
            var artDirs = new List<string>
            {
                Path.Combine(coverArtDir, "discog"),
                Path.Combine(coverArtDir, "album"),
                Path.Combine(coverArtDir, "song")
            };

            foreach (var dir in artDirs)
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// Saves the image from a tag. <see cref="SongFileTag"/>
        /// </summary>
        /// <param name="taggedSong">The tagged song.</param>
        private void SaveImageFromTag(SongTagFile taggedSong)
        {
            var coverArtDir = Path.Combine(_horsifySettings.HorsifyPath, "CoverArt");
            SongImageToImage(taggedSong, coverArtDir);
        }

        public void SaveChanges()
        {
            ((IUnitOfWork)_horsifyDataRepo).Save();
        }

        /// <summary>
        /// Updates the database song tag.
        /// </summary>
        /// <param name="taggedSong">The tagged song.</param>
        /// <param name="id">The identifier.</param>
        public void UpdateDbSongTag(SongTagFile taggedSong, int id)
        {
            _horsifyDataRepo.UpdateDbSongTag(taggedSong, id);
        }

        #region Private Methods
        /// <summary>
        /// Save an image for the given song if it doesn't exist. 
        /// Checks discogs releaseId, album name & artist - title for images and saves in this order if available to Art/discog, Art/Album, Art/song
        /// </summary>
        /// <param name="song">A song with image data</param>
        private bool SongImageToImage(SongTagFile song, string artworkFolder)
        {
            if (song.ImageData?.Length > 0)
            {
                string discogImagePath = Path.Combine(artworkFolder, $"discog\\{song.DiscogReleaseId}.jpg");


                var art = song.Artist?.Replace("\"", "");
                var title = song.Title?.Replace("\"", "");
                var album = song.Album?.Replace("\"", "");
                string albumImageFile = Path.Combine(artworkFolder, $@"album\\{art} - {album}.jpg");
                string songImageFile = Path.Combine(artworkFolder, $@"song\\{art} - {title} - {album}.jpg");

                if (System.IO.File.Exists(discogImagePath))
                {
                    song.ImageLocation = discogImagePath;
                    return false;
                }
                if (System.IO.File.Exists(albumImageFile))
                {
                    song.ImageLocation = albumImageFile;
                    return false;
                }
                if (System.IO.File.Exists(songImageFile))
                {
                    song.ImageLocation = songImageFile;
                    return false;
                }

                using (MemoryStream ms = new MemoryStream(song.ImageData))
                {
                    //Check if we have a discogs ID and save under that ID.jpg
                    if (song.DiscogReleaseId !=null && song.DiscogReleaseId != 0)
                    {
                        song.ImageLocation = discogImagePath;
                        //Only save the image if we don't have it already.
                        if (!System.IO.File.Exists(discogImagePath))
                        {
                            SaveImage(ms, discogImagePath);
                            return true;
                        }
                    }
                    else if (song.Album != string.Empty)
                    {
                        song.ImageLocation = albumImageFile;

                        if (!System.IO.File.Exists(albumImageFile))
                        {
                            SaveImage(ms, albumImageFile);
                            return true;
                        }
                    }
                    else if (song.Title != string.Empty)
                    {
                        song.ImageLocation = songImageFile;

                        if (!System.IO.File.Exists(songImageFile))
                        {
                            SaveImage(ms, songImageFile);
                            return true;
                        }
                    }

                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Save song image from memory stream
        /// </summary>
        /// <param name="ms">Memory stream of image data</param>
        /// <param name="imageFile">File to save to</param>
        private void SaveImage(MemoryStream ms, string imageFile)
        {
            if (!System.IO.File.Exists(imageFile))
            {
                try
                {
                    var img = Image.FromStream(ms);
                    img.Save(imageFile);
                }
                catch { }
            }
        }
        #endregion
    }
}
