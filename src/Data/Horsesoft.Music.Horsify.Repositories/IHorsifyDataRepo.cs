using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Data.Model.Tags;
using Horsesoft.Music.Data.Sqlite.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Repositories
{
    public static class ShuffleExtension
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public interface IHorsifyDataRepo
    {
        #region Repos
        IGenericRepository<AllJoinedTable> AllJoinedTableRepository { get; }
        IGenericRepository<Album> AlbumRepository { get; }
        IGenericRepository<Artist> ArtistRepository { get; }
        IGenericRepository<Discog> DiscogRepository { get; }
        IGenericRepository<Data.Model.File> FileRepository { get; }
        IGenericRepository<Data.Model.Filter> FilterRepository { get; }
        IGenericRepository<FiltersSearch> FiltersSearchRepository { get; }
        IGenericRepository<Genre> GenreRepository { get; }
        IGenericRepository<Label> LabelRepository { get; }
        IGenericRepository<MusicalKey> MusicalKeyRepository { get; }
        IGenericRepository<Playlist> PlaylistRepository { get; }
        IGenericRepository<Song> SongRepository { get; }
        IGenericRepository<Status> StatusKeyRepository { get; }
        #endregion

        /// <summary>
        /// Searches the given tables with a wildcard like sequence. <para />
        /// </summary>        
        /// <param name="sqlliteSearchString">The search string. A Select where Like clause</param>
        /// <returns></returns>
        IEnumerable<AllJoinedTable> ExecuteSearchLike(string sqlliteSearchString, short maxAmount = -1);
        IEnumerable<AllJoinedTable> ExecuteSearchLike(string sqlliteSearchString, short randomAmount = 10, short maxAmount = -1);
        IEnumerable<AllJoinedTable> GetMostPlayed();
        IEnumerable<AllJoinedTable> GetRecentlyAdded();
        IEnumerable<AllJoinedTable> GetRecentlyPlayed();
        /// <summary>
        /// Gets the songs from saved filters. These filters are currently for exact matches.
        /// </summary>
        /// <param name="searchFilter">The search filter. <see cref="ISearchFilter"/></param>
        /// <returns></returns>
        IQueryable<AllJoinedTable> GetSongsFromSavedFilters(ISearchFilter searchFilter);
        IEnumerable<Data.Model.File> GetUntaggedFiles();
        IEnumerable<AllJoinedTable> GetAllJoinedTableByIds(int[] ids);

        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <returns>True if the database was just created</returns>
        Task<bool> InitializeDatabase(string installDir, string tableToCheck = "Song");

        string SearchLike(SearchType searchType, string searchString);
        string SearchLike(SearchFilter searchFilter);

        void UpdateDbSongTag(SongTagFile songTag, int fileId);
    }

    /// <summary>
    /// This class implements the interface for the horsify repositories.
    /// </summary>
    /// <seealso cref="Horsesoft.Music.Horsify.Repository.UnitOfWork" />
    /// <seealso cref="Horsesoft.Music.Horsify.Repository.IHorsifyRepositories" />
    public abstract class HorsifyDataRepo : UnitOfWork, IHorsifyDataRepo
    {
        public HorsifyDataRepo()
        {            
        }

        /// <summary>
        /// Initializes the database. Checks if the Song table exists from the 'SqlliteMaster' tables
        /// </summary>
        /// <returns>
        /// True if the database was just created
        /// </returns>
        public async Task<bool> InitializeDatabase(string installDir, string tableToCheck = "Song")
        {
            var sqliteDbFile = $@"{installDir}\Horsify.db";
            using (SqliteConnection conn = new SqliteConnection($@"Datasource={sqliteDbFile}"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                var table_name = tableToCheck;
                cmd.CommandText = $"SELECT name from sqlite_master where type='table' and name='{table_name}';";
                var reader = await cmd.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    conn.Close();
                    conn.Open();
                    cmd.CommandText = CreateInitialDbSql();
                    var result = await cmd.ExecuteNonQueryAsync();
                    reader.Close();
                    return true;
                }
            };

            using (SqliteConnection conn = new SqliteConnection($@"Datasource={sqliteDbFile}"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                //Create playlist table
                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS Filter (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE, SearchTerms TEXT);";
                cmd.CommandText = $"CREATE TABLE IF NOT EXISTS FiltersSearch(Id INTEGER PRIMARY KEY NOT NULL, Name STRING NOT NULL UNIQUE, MaxAmount INTEGER, RandomAmount INTEGER, SearchFilterContent STRING NOT NULL); ";

                var playlistResult = await cmd.ExecuteNonQueryAsync();
                return false;
            }            
        }

        /// <summary>
        /// Executes a SQL WHERE Syntax on the AllJoinedTables.
        /// </summary>        
        /// <param name="sqlliteSearchString">The search string.</param>
        /// <returns></returns>
        public IEnumerable<AllJoinedTable> ExecuteSearchLike(string sqlliteSearchString, short maxAmount = -1)
        {
            var searchTerm = $"SELECT DISTINCT * FROM AllJoinedTables WHERE {sqlliteSearchString}";

            if (maxAmount > 0)
                searchTerm += $" limit {maxAmount}";

            var foundSongs = ((HorsifyContext)_context)
                .AllJoinedTables
                .FromSql(searchTerm)
                .OrderByDescending(x => x.Rating);

            return foundSongs.AsEnumerable();
        }

        /// <summary>
        /// Executes a SQL WHERE Syntax on the AllJoinedTables with a random statement attached from the incoming amount.
        /// </summary>
        /// <param name="sqlliteSearchString"></param>
        /// <param name="randomAmount"></param>
        /// <returns></returns>
        public IEnumerable<AllJoinedTable> ExecuteSearchLike(string sqlliteSearchString, short randomAmount = 10, short maxAmount = -1)
        {
            if (randomAmount <= 0)
            {
                return ExecuteSearchLike(sqlliteSearchString, maxAmount);
            }

            var searchTerm = $"SELECT * from AllJoinedTables WHERE {sqlliteSearchString} order by random() limit {randomAmount}";

            var foundSongs = ((HorsifyContext)_context)
                .AllJoinedTables
                .FromSql(searchTerm)
                .OrderByDescending(x => x.Rating);

            return foundSongs.AsEnumerable();
        }

        public IEnumerable<AllJoinedTable> GetAllJoinedTableByIds(int[] ids)
        {
            var searchString = string.Empty;
            for (int i = 0; i < ids.Length; i++)
            {
                if (i == ids.Length - 1)
                    searchString += $"Id = {ids[i]}";
                else
                    searchString += $"Id = {ids[i]} OR ";

            }

            var searchTerm = $"SELECT * from AllJoinedTables WHERE {searchString}";

            var foundSongs = ((HorsifyContext)_context)
                .AllJoinedTables
                .FromSql(searchTerm)
                .OrderByDescending(x => x.Rating);

            return foundSongs.AsEnumerable();
        }

        public IEnumerable<AllJoinedTable> GetRecentlyAdded()
        {
            var searchTerm = $"SELECT * FROM AllJoinedTables order by AddedDate desc Limit 500";

            var foundSongs = ((HorsifyContext)_context)
                .AllJoinedTables
                .FromSql(searchTerm);

            return foundSongs.AsEnumerable();
        }

        public IEnumerable<AllJoinedTable> GetRecentlyPlayed()
        {
            var searchTerm = $"SELECT * FROM AllJoinedTables Where LastPlayed not null order by LastPlayed desc Limit 500";

            var foundSongs = ((HorsifyContext)_context)
                .AllJoinedTables
                .FromSql(searchTerm);

            return foundSongs.AsEnumerable();
        }

        public IEnumerable<AllJoinedTable> GetMostPlayed()
        {
            var searchTerm = $"SELECT * FROM AllJoinedTables WHERE TimesPlayed > 0 order by TimesPlayed desc, LastPlayed desc Limit 500";

            var foundSongs = ((HorsifyContext)_context)
                .AllJoinedTables
                .FromSql(searchTerm);

            return foundSongs.AsEnumerable();
        }

        /// <summary>
        /// Gets the untagged files from files with no song entry and songs with no title set
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Data.Model.File> GetUntaggedFiles()
        {
            //HACK: NO GOOD
            var noSongs = FileRepository.Get(filter: x => x.Song.Count == 0);

            //var untaggedFiles = SongRepository
            //    .Get(x => string.IsNullOrWhiteSpace(x.Title),
            //    includeProperties: "File")
            //    .Select(z => z.File);

            //return noSongs.Union(untaggedFiles).Distinct();
            return noSongs.Distinct();
        }

        /// <summary>
        /// Searches the sqlite like. Takes in flags of <see cref="SearchType"/> <para/>
        /// This invokes this classes <see cref="SearchLike"/>
        /// </summary>
        /// <param name="searchType">Can be flags.</param>
        /// <param name="searchString">The search string with wildcards delimited with '|'. eg %Jackson%|%Jack%</param>
        /// <returns></returns>
        public string SearchLike(SearchType searchType, string searchString)
        {
            if (searchString?.Length < 2)
            {
                throw new NullReferenceException("Search terms cannot be null");
            }

            //The tables and search Terms to search
            var tables = string.Empty;
            var searchTerms = string.Empty;
            var searchTermsList = new List<string>();

            if (searchString.Contains("|"))
            {
                var splitTerms = searchString.Split('|');
                searchTermsList.AddRange(splitTerms);
            }
            else
            {
                searchTermsList.Add(searchString);
            }

            //Search all tables if set.
            if (searchType == SearchType.All)
            {
                //Get all enum types excluding all
                var enumName = Enum.GetNames(typeof(SearchType)).Where(x => x != "All");
                var cnt = enumName.Count();
                int b = 0;
                foreach (var search in searchTermsList)
                {
                    if (b !=0)
                        tables += " OR ";

                    //Replace the single quote with double if in search string
                    var finalSearch = search.Replace("'", "''");
                    for (int i = 0; i < cnt; i++)
                    {
                        tables += $"{enumName.ElementAt(i)} LIKE '{finalSearch}' ";
                        if (i < cnt - 1)
                            tables += " OR ";
                    }

                    b++;
                }

                searchTerms = tables;
            }
            else
            {
                //Search for more than one type
                int b = 0;
                int c = 0;
                var searchTypes = searchType.ToString().Split(',');
                if (searchTypes.Length > 1)
                {                    
                    foreach (var s_stype in searchTypes)
                    {
                        b = 0;
                        foreach (var search in searchTermsList)
                        {
                            //Adds year == 2018                    
                            searchTerms += $"({s_stype} LIKE '{search.Replace("'", "''")}')";

                            if (c != searchTypes.Length - 1)
                                searchTerms += $" OR ";

                            if (b != searchTermsList.Count - 1)
                                searchTerms += $" OR ";
                            b++;
                        }

                        c++;
                    }
                }
                else
                {
                    searchTerms += " (";

                    b = 0;
                    foreach (var search in searchTermsList)
                    {
                        //Adds year == 2018                    
                        searchTerms += $"{searchType} LIKE '{search.Replace("'", "''")}'";
                        if (b != searchTermsList.Count - 1)
                            searchTerms += $" OR ";
                        b++;
                    }
                    searchTerms += ") ";
                }                
            }            

            return searchTerms;
        }

        public string SearchLike(SearchFilter searchFilter)
        {
            var searchTerms = string.Empty;

            //Get a Bpm constraint
            var bpmRange = searchFilter.BpmRange;
            if (bpmRange != null && bpmRange.IsEnabled)
            {
                if (bpmRange.Low == bpmRange.Hi)
                    searchTerms += $"(Bpm == {bpmRange.Hi}) AND ";
                else
                    searchTerms += GetRangeFilter("Bpm", bpmRange);
            }

            //Get a Rating constraint
            var ratingRange = searchFilter.RatingRange;
            if (ratingRange != null && ratingRange.IsEnabled)
            {
                if (ratingRange.Low == ratingRange.Hi)
                    searchTerms += $"(Rating == {ratingRange.Hi}) AND ";
                else
                {
                    searchTerms += GetRangeFilter("Rating", ratingRange);
                }
            }

            var musicKeys = searchFilter.MusicKeys;
            if (!string.IsNullOrWhiteSpace(musicKeys) && musicKeys != "None")
            {
                //AND MusicKey LIKE 'Fm' OR MusicKey LIKE 'Gm'
                var keys = musicKeys.Split(',');

                searchTerms += "(";
                for (int ii = 0; ii < keys.Length; ii++)
                {
                    if (ii < keys.Length - 1)
                        searchTerms += $"MusicKey == '{keys[ii].Trim()}' OR ";
                    else
                        searchTerms += $"MusicKey == '{keys[ii].Trim()}') AND ";
                }
            }
            
            //Build up include and exclude string                
            string includeOrString = string.Empty, excludeString = string.Empty, includeAndString = string.Empty;

            //Build lookup items from included and excluded
            var includeFilterLookUp = searchFilter.Filters?
                .ToLookup(x => x.SearchAndOrOption, x => x);
            if (includeFilterLookUp != null)
            {
                foreach (var filter in includeFilterLookUp)
                {
                    //var includedFilters = filter.Where(x => x.SearchAndOrOption == SearchAndOrOption.Include)
                    //    .SelectMany(x => x.Filters).ToList();
                    //var excludedFilters = filter.Where(x => x.SearchAndOrOption == SearchAndOrOption.Exclude) 
                    //    .SelectMany(x => x.Filters).ToList();                
                    
                    if (filter.Key == SearchAndOrOption.Or || filter.Key == SearchAndOrOption.None)
                    {
                        foreach (var filterItem in filter)
                        {
                            includeOrString += SearchLike(filterItem.SearchType, filterItem.Filters.Aggregate((curr, next) => $"{curr}|{next}"));
                        }
                        includeOrString = includeOrString.Replace($")  (", " OR ");
                    }
                    else if (filter.Key == SearchAndOrOption.And)
                    {
                        foreach (var filterItem in filter)
                        {
                            includeAndString += SearchLike(filterItem.SearchType, filterItem.Filters.Aggregate((curr, next) => $"{curr}|{next}"));
                        }

                        includeAndString = includeAndString.Replace($")  (", " OR ");

                    }
                    else if (filter.Key == SearchAndOrOption.Not)
                    {
                        foreach (var filterItem in filter)
                        {
                            excludeString += SearchLike(filterItem.SearchType, filterItem.Filters.Aggregate((curr, next) => $"{curr}|{next}"));
                        }
                        excludeString = excludeString.Replace($")  (", " AND ")
                            .Replace("LIKE", "NOT LIKE")
                            .Replace("OR", "AND");
                    };
                }            
            }

            //OR
            if (!string.IsNullOrWhiteSpace(includeOrString))
            {
                searchTerms += $"({includeOrString})" ;
            }

            //NOT
            if (!string.IsNullOrEmpty(excludeString))
            {
                if (!string.IsNullOrEmpty(includeOrString))
                    searchTerms += " AND " + excludeString;
                else
                {
                    searchTerms += excludeString;
                }
            }

            //AND
            if (!string.IsNullOrEmpty(includeAndString))
            {
                if (!string.IsNullOrWhiteSpace(includeOrString) || !string.IsNullOrWhiteSpace(excludeString))
                    searchTerms += " AND " + includeAndString;               
                else
                    searchTerms += includeAndString;
            }                

            return searchTerms.Replace("AND  AND", "AND");
        }        

        /// <summary>
        /// Gets the songs from saved filters. These filters are currently for exact matches. //TODO: Is this used anymore????
        /// </summary>
        /// <param name="searchFilter">The search filter. <see cref="ISearchFilter" /></param>
        /// <returns></returns>
        public IQueryable<AllJoinedTable> GetSongsFromSavedFilters(ISearchFilter searchFilter)
        {
            IQueryable<AllJoinedTable> songs = null;
            var filter = searchFilter;

            //Filter by RATINGS
            if (filter.RatingRange != null)
            {
                songs = this.AllJoinedTableRepository.Get(x => x.Rating >= filter.RatingRange.Low && x.Rating <= filter.RatingRange.Hi).AsQueryable();
            }

            //Filter BPMs
            if (filter.BpmRange != null)
            {
                if (songs == null)
                    songs = this.AllJoinedTableRepository.Get(x => x.Bpm >= filter.BpmRange.Low && x.Bpm <= filter.BpmRange.Hi).AsQueryable();
                else
                    songs = songs.Where(x => x.Bpm >= filter.BpmRange.Low && x.Bpm <= filter.BpmRange.Hi);
            }

            //Apply all filters
            if (filter.Filters?.Count() > 0)
            {
                //Create expressions from the filters and search each one
                var expressionList = BuildExpressionList(filter);
                foreach (var expression in expressionList)
                {
                    if (songs == null)
                    {
                        songs = this.AllJoinedTableRepository.Get(expression).AsQueryable();
                    }
                    else
                    {
                        songs = songs.Where(expression);
                    }
                }
            }

            return songs;
        }

        public void UpdateDbSongTag(SongTagFile songTag, int fileId)
        {
            Song song = new Song
            {
                FileId = fileId,
                //Update standrad fields
                AddedDate = ConvertUnixTime(DateTime.Now).ToString(),
                BitRate = songTag.BitRate,
                Bpm = songTag.Bpm,
                Comment = songTag.Comment,
                Country = songTag.Country,
                ImageLocation = songTag.ImageLocation,
                Rating = (int)songTag.Rating,
                Title = songTag.Title,
                Time = songTag.Duration,
                Track = songTag.TrackNumber,
                Year = songTag.Year
            };

            UpdateExtraTables(songTag, song);

            SongRepository.Insert(song);
        }

        private long ConvertUnixTime(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }

        #region Private Methods
        private string CreateInitialDbSql()
        {
            var s = Assembly.GetCallingAssembly();
            using (var resource = s
                .GetManifestResourceStream("Horsesoft.Music.Horsify.Repositories.SQL.CreateHorsifySqliteDb.sql"))
            {
                if (resource != null)
                {
                    using (var tr = new StreamReader(resource))
                    {
                        var line = "";
                        var lines = "";
                        while ((line = tr.ReadLine()) != null)
                        {
                            lines += line + Environment.NewLine;
                        }

                        return lines;
                    }
                }
                else
                    return string.Empty;
            }
        }

        private List<Expression<Func<AllJoinedTable, bool>>> BuildExpressionList(ISearchFilter filter)
        {
            var expressionList = new List<Expression<Func<AllJoinedTable, bool>>>();
            foreach (var currentFilter in filter.Filters)
            {
                expressionList.Add(GetExpressionTree(currentFilter.SearchType, currentFilter.Filters));
            }

            return expressionList;
        }

        /// <summary>
        /// Gets an AND clause on a range (low => high)....used for ratings and bpm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="rangeFilterOption"></param>
        /// <returns></returns>
        private string GetRangeFilter<T>(string name, RangeFilterOption<T> rangeFilterOption)
        {
            return $"({name} >= {rangeFilterOption.Low} AND {name} <= {rangeFilterOption.Hi}) AND ";
        }

        private Expression<Func<AllJoinedTable, bool>> GetExpressionTree(SearchType searchType, IEnumerable<string> searchStrings)
        {
            switch (searchType)
            {
                case SearchType.All:
                    break;
                case SearchType.Album:
                    return ExpressionBuilder.BuildOrExpressionTree<AllJoinedTable, string>(searchStrings, song => song.Album);
                case SearchType.Artist:
                    return ExpressionBuilder.BuildOrExpressionTree<AllJoinedTable, string>(searchStrings, song => song.Artist);
                case SearchType.Bpm:
                    //return ExpressionBuilder.BuildOrExpressionTree<AllJoinedTable, long?>(searchStrings, song => song.Bpm);
                    break;
                case SearchType.Genre:
                    return ExpressionBuilder.BuildOrExpressionTree<AllJoinedTable, string>(searchStrings, song => song.Genre);
                case SearchType.Label:
                    return ExpressionBuilder.BuildOrExpressionTree<AllJoinedTable, string>(searchStrings, song => song.Label);
                case SearchType.Year:
                    break;
                default:
                    break;
            }

            return null;
        }
        #endregion

        #region Private Add/Udpdate Methods

        /// <summary>
        /// Updates the extra tables using the methods in this class, eg <see cref="AddOrGetAlbumId(SongTagFile, Song)"/>
        /// </summary>
        /// <param name="songTag">The song tag.</param>
        /// <param name="song">The song.</param>
        private void UpdateExtraTables(SongTagFile songTag, Song song)
        {
            //Add artist or assign Id
            AddOrGetArtistId(songTag, song);
            AddOrGetAlbumId(songTag, song);
            AddOrGetLabelId(songTag, song);
            AddOrGetDiscogsId(songTag, song);
            AddOrGetGenreId(songTag, song);
            AddOrGetMusicKeyId(songTag, song);
        }

        private void AddOrGetMusicKeyId(SongTagFile songTag, Song song)
        {
            if (!string.IsNullOrEmpty(songTag.MusicalKey))
            {
                var musicalKey = MusicalKeyRepository.Get(x => x.MusicKey == songTag.MusicalKey).FirstOrDefault();
                if (musicalKey == null)
                {
                    song.MusicalKey = new MusicalKey() { MusicKey = songTag.MusicalKey };
                    MusicalKeyRepository.Insert(song.MusicalKey);
                    this.Save();
                }
                else
                {
                    song.MusicalKeyId = musicalKey.Id;
                }
            }
        }

        private void AddOrGetArtistId(SongTagFile songTag, Song song)
        {
            if (songTag.Artist != null)
            {
                var artist = ArtistRepository.Get(x => x.Name == songTag.Artist).FirstOrDefault();
                if (artist == null)
                {
                    song.Artist = new Artist() { Name = songTag.Artist };
                    ArtistRepository.Insert(song.Artist);
                    this.Save();
                }
                else
                {
                    song.ArtistId = artist.Id;
                }
            }
        }

        private void AddOrGetAlbumId(SongTagFile songTag, Song song)
        {
            if (songTag.Album != null)
            {
                var album = AlbumRepository.Get(x => x.Title == songTag.Album).FirstOrDefault();
                if (album == null)
                {
                    song.Album = new Album() { Title = songTag.Album };
                    AlbumRepository.Insert(song.Album);
                    this.Save();
                }
                else
                {
                    song.AlbumId = album.Id;
                }
            }
        }

        private void AddOrGetLabelId(SongTagFile songTag, Song song)
        {
            if (songTag.Label != null)
            {
                var label = LabelRepository.Get(x => x.Name == songTag.Label).FirstOrDefault();
                if (label == null)
                {
                    song.Label = new Label() { Name = songTag.Label };
                    LabelRepository.Insert(song.Label);
                    this.Save();
                }
                else
                {
                    song.LabelId = label.Id;
                }
            }
        }

        private void AddOrGetDiscogsId(SongTagFile songTag, Song song)
        {
            if (songTag.DiscogReleaseId != null && songTag.DiscogReleaseId > 0)
            {
                var discog = DiscogRepository.Get(x => x.ReleaseId == songTag.DiscogReleaseId).FirstOrDefault();
                if (discog == null)
                {
                    song.Discog = new Discog() { ReleaseId = (int)songTag.DiscogReleaseId };
                    DiscogRepository.Insert(song.Discog);
                    this.Save();
                }
                else
                {
                    song.DiscogId = discog.Id;
                }
            }
        }

        private void AddOrGetGenreId(SongTagFile songTag, Song song)
        {
            if (songTag.Genre != null)
            {
                var genre = GenreRepository.Get(x => x.Name == songTag.Genre).FirstOrDefault();
                if (genre == null)
                {
                    song.Genre = new Genre() { Name = songTag.Genre };
                    GenreRepository.Insert(song.Genre);
                    this.Save();
                }
                else
                {
                    song.GenreId = genre.Id;
                }
            }
        }

        #endregion

        #region Repositories

        private IGenericRepository<AllJoinedTable> _allJoinedTableRepo;
        public IGenericRepository<AllJoinedTable> AllJoinedTableRepository
        {
            get
            {
                if (_allJoinedTableRepo == null)
                    //_allJoinedTableRepo = new AllJoinedTableRepo((HorsifyContext)_context);
                    _allJoinedTableRepo = new GenericRepository<AllJoinedTable>(_context);

                return _allJoinedTableRepo;
            }
        }

        private IGenericRepository<Album> _albumRepository;
        public IGenericRepository<Album> AlbumRepository
        {
            get
            {
                if (_albumRepository == null)
                    _albumRepository = new GenericRepository<Album>(_context);

                return _albumRepository;
            }
        }

        private IGenericRepository<Artist> _artistRepository;
        public IGenericRepository<Artist> ArtistRepository
        {
            get
            {
                if (_artistRepository == null)
                    _artistRepository = new GenericRepository<Artist>(_context);

                return _artistRepository;
            }
        }

        private IGenericRepository<Discog> _discogRepository;
        public IGenericRepository<Discog> DiscogRepository
        {
            get
            {
                if (_discogRepository == null)
                    _discogRepository = new GenericRepository<Discog>(_context);

                return _discogRepository;
            }
        }

        private IGenericRepository<Data.Model.File> _fileRepository;
        public IGenericRepository<Data.Model.File> FileRepository
        {
            get
            {
                if (_fileRepository == null)
                    _fileRepository = new GenericRepository<Data.Model.File>(_context);

                return _fileRepository;
            }
        }

        private IGenericRepository<Data.Model.Filter> _filterRepository;
        public IGenericRepository<Data.Model.Filter> FilterRepository
        {
            get
            {
                if (_filterRepository == null)
                    _filterRepository = new GenericRepository<Data.Model.Filter>(_context);

                return _filterRepository;
            }
        }

        private IGenericRepository<Genre> _genreRepository;
        public IGenericRepository<Genre> GenreRepository
        {
            get
            {
                if (_genreRepository == null)
                    _genreRepository = new GenericRepository<Genre>(_context);

                return _genreRepository;
            }
        }

        private IGenericRepository<Label> _labelRepository;
        public IGenericRepository<Label> LabelRepository
        {
            get
            {
                if (_labelRepository == null)
                    _labelRepository = new GenericRepository<Label>(_context);

                return _labelRepository;
            }
        }

        private IGenericRepository<MusicalKey> _musicalKeyRepository;
        public IGenericRepository<MusicalKey> MusicalKeyRepository
        {
            get
            {
                if (_musicalKeyRepository == null)
                    _musicalKeyRepository = new GenericRepository<MusicalKey>(_context);

                return _musicalKeyRepository;
            }
        }

        private IGenericRepository<Data.Model.Playlist> _playlistRepository;
        public IGenericRepository<Data.Model.Playlist> PlaylistRepository
        {
            get
            {
                if (_playlistRepository == null)
                    _playlistRepository = new GenericRepository<Data.Model.Playlist>(_context);

                return _playlistRepository;
            }
        }

        private IGenericRepository<Song> _songRepository;
        public IGenericRepository<Song> SongRepository
        {
            get
            {
                if (_songRepository == null)
                    _songRepository = new GenericRepository<Song>(_context);

                return _songRepository;
            }
        }

        private IGenericRepository<Status> _statusKeyRepository;
        public IGenericRepository<Status> StatusKeyRepository
        {
            get
            {
                if (_statusKeyRepository == null)
                    _statusKeyRepository = new GenericRepository<Status>(_context);

                return _statusKeyRepository;
            }
        }

        private IGenericRepository<FiltersSearch> _filtersSearchRepository;
        public IGenericRepository<FiltersSearch> FiltersSearchRepository
        {
            get
            {
                if (_filtersSearchRepository == null)
                    _filtersSearchRepository = new GenericRepository<FiltersSearch>(_context);

                return _filtersSearchRepository;
            }
        }

        #endregion
    }
}


