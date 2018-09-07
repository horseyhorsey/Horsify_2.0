using Horsesoft.Music.Data.Model.Import;
using Horsesoft.Music.Horsify.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Horsesoft.Music.Engine.Import
{
    public interface IFileImport
    {
        void CancelImport();

        bool CancelPending { get; set; }

        IEnumerable<Data.Model.File> GetFilesFromDatabase();
        IEnumerable<Data.Model.File> GetFilesWithoutYear();
        IEnumerable<Data.Model.File> GetUntaggedFilesFromDatabase();

        /// <summary>
        /// Imports the file asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        Task<Data.Model.File> ImportFileAsync(string fileName, bool getHash);

        /// <summary>
        /// Imports the files asynchronous.
        /// </summary>
        /// <param name="dirName">Name of the dir.</param>
        /// <returns></returns>
        Task<IEnumerable<Data.Model.File>> ImportFilesAsync(string dirName, bool getHashes);

        event Action<string> OnFileImported;

        Task<bool> SaveFileToDatabase(Data.Model.File file);
        Task<bool> SaveFilesToDatabase(IEnumerable<Data.Model.File> files);
    }

    public class FileImport : IFileImport
    {
        /// <summary>
        /// The import options. If we're ok to import Wav, MP3, FLAC etc
        /// </summary>
        SongImportType _importOptions;
        private IHorsifyDataRepo _horsifyDataRepo;
        public bool CancelPending { get; set; }
        private CancellationTokenSource _cts = new CancellationTokenSource();
        public event Action<string> OnFileImported;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileImport"/> class.
        /// </summary>
        /// <param name="importOptions">The import file options. MP£, WAV etc</param>
        public FileImport()
        {
            _horsifyDataRepo = new HorsifyDataSqliteRepo();
            _importOptions = SongImportType.All;
        }

        #endregion

        #region Public Methods
        public void CancelImport()
        {
            try
            {
                if (_cts.Token != null)
                    _cts?.Cancel();
            }
            catch { }
        }

        public IEnumerable<Data.Model.File> GetFilesFromDatabase()
        {
            return _horsifyDataRepo.FileRepository
                .Get(includeProperties: "Song,Song.Artist,Song.Album,Song.Genre,Song.Discog,Song.Label");
        }

        public IEnumerable<Data.Model.File> GetUntaggedFilesFromDatabase()
        {
            return _horsifyDataRepo.GetUntaggedFiles();
        }

        public IEnumerable<Data.Model.File> GetFilesWithoutYear()
        {
            return _horsifyDataRepo.SongRepository.Get(
                filter: x => x.Year == null,
                includeProperties: "File")
                .Select(x => x.File);
        }

        /// <summary>
        /// Imports a media file asynchronously
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="getHash"></param>
        /// <returns></returns>
        public async Task<Data.Model.File> ImportFileAsync(string fileName, bool getHash = false)
        {
            return await Task.Run(() =>
             {
                 var e = Path.GetExtension(fileName).ToUpper();
                 if (e == ".MP3" || e == ".FLAC" || e == ".OGG" || e == ".WAV" || e == ".WMA")
                 {
                     if (_importOptions.HasFlag(SongImportType.All))
                         return CreateFile(fileName, getHash);

                     if (_importOptions.HasFlag(SongImportType.MP3))
                         return CreateFile(fileName, getHash);

                     if (!_importOptions.HasFlag(SongImportType.WAV))
                         return CreateFile(fileName, getHash);

                     if (!_importOptions.HasFlag(SongImportType.FLAC))
                         return CreateFile(fileName, getHash);

                     if (!_importOptions.HasFlag(SongImportType.WMA))
                         return CreateFile(fileName, getHash);
                 }

                 return null;
             });
        }

        public async Task<IEnumerable<Data.Model.File>> ImportFilesAsync(string dirName, bool getHashes = false)
        {
            var dirFiles = await GetFilesAsync(dirName, "*.*");
            var files = new List<Data.Model.File>();

            _cts = new CancellationTokenSource();

            return await Task.Run(() =>
            {
                try
                {
                    var options = new ParallelOptions()
                    {
                        CancellationToken = _cts.Token,
                        MaxDegreeOfParallelism = System.Environment.ProcessorCount
                    };

                    Parallel.ForEach(dirFiles, options,
                    x =>
                    {
                        var dirFile = ImportFileAsync(x, getHashes).Result;

                        if (dirFile != null)
                        {
                            files.Add(dirFile);
                        }

                        options.CancellationToken.ThrowIfCancellationRequested();
                    });
                }
                catch (OperationCanceledException)
                {
                    return null;
                }
                catch (Exception)
                {

                    throw;
                }

                return files;
            });
        }

        public async Task<bool> SaveFileToDatabase(Data.Model.File file)
        {
            if (_horsifyDataRepo != null)
            {
                try
                {
                    //IHorsifySongService  client = new HorsifySongServiceReference.HorsifySongServiceClient();                    
                    _horsifyDataRepo.FileRepository.Insert(file);
                    var uow = (IUnitOfWork)_horsifyDataRepo;
                    await uow.SaveAsync();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves the files to database. Creates a new empty song for each entry.
        /// Runs checks if existing entries.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        public async Task<bool> SaveFilesToDatabase(IEnumerable<Data.Model.File> files)
        {
            int i = 0;
            var uow = (IUnitOfWork)_horsifyDataRepo;
            _cts = new CancellationTokenSource();

            if (_horsifyDataRepo != null)
            {
                //Add check for existing, faster if not checking every file coming in.
                bool checkExisting = true;
                if (_horsifyDataRepo.FileRepository.GetById((long)1) == null)
                    checkExisting = false;

                try
                {
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            if (CancelPending)
                            {
                                CancelPending = false;
                                break;
                            }

                            bool canAddFile = true;
                            if (checkExisting)
                            {
                                //Check for existing file in database
                                var existingFile =
                                    _horsifyDataRepo.FileRepository.Get(
                                        x => x.DriveVolume == file.DriveVolume && x.FileName == file.FileName).FirstOrDefault();

                                ////Add if not already there
                                if (existingFile != null)
                                {
                                    canAddFile = false;
                                    OnFileImported?.Invoke($"SKIPPED: {file.ToString()}");
                                }
                            }

                            if (canAddFile)
                            {
                                _horsifyDataRepo.FileRepository.Insert(file);

                                OnFileImported?.Invoke($"IMPORTED: {file.ToString()}");

                                i++;

                                if (i > 2000)
                                {
                                    await uow.SaveAsync();
                                    i = 0;
                                }
                            }
                        }
                    }

                    await uow.SaveAsync();
                }
                catch (Exception ex)
                {
                    OnFileImported?.Invoke($"ERROR: {ex.Message}");
                }
                finally
                {
                    _cts.Dispose();
                    CancelPending = false;
                }

            }

            return true;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Creates the file by getting the seperate paths and a hash for the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private Data.Model.File CreateFile(string fileName, bool getHash = false)
        {
            var hash = getHash ? GetFileHash(fileName) : null;
            var directory = Path.GetDirectoryName(fileName);
            return new Data.Model.File
            {
                DriveVolume = directory.Remove(3, directory.Length - 3),
                Folder = directory.Replace(directory.Remove(3, directory.Length - 3), ""),
                FileName = Path.GetFileName(fileName),
                Hash = hash
            };
        }

        public async Task<IEnumerable<string>> GetFilesAsync(string path, string searchPattern)
        {
            return await Task.Run(() =>
            {
                return GetAllFiles(path, searchPattern);
            });
        }

        IEnumerable<String> GetAllFiles(string path, string searchPattern)
        {
            return Directory.EnumerateFiles(path, searchPattern)
                .Union(Directory.EnumerateDirectories(path)
                .SelectMany(d =>
                {
                    try
                    {
                        return GetAllFiles(d, searchPattern);
                    }
                    catch (UnauthorizedAccessException) { return Enumerable.Empty<string>(); }
                    catch (Exception) { return Enumerable.Empty<string>(); }
                }));
        }

        /// <summary>
        /// Gets the file hash.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private string GetFileHash(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] retVal = md5.ComputeHash(file);
                string sb = string.Empty;

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb += retVal[i].ToString("x2");
                }

                return sb;
            }
        }
        #endregion
    }
}
