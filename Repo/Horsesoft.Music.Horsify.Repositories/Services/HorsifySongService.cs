using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;

namespace Horsesoft.Music.Horsify.Repositories.Services
{
    public partial class HorsifySongService : IHorsifySongService
    {
        private IHorsifyDataRepo _sqliteRepo;

        public HorsifySongService()
        {
            //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            //TODO: Can be removed?
            //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            //SQLitePCL.Batteries.Init();
            _sqliteRepo = new HorsifyDataSqliteRepo();
        }

        public Task<IEnumerable<Playlist>> GetAllPlaylistsAsync()
        {
            return Task.Run(() => this.GetAllPlaylists());
        }

        public Task<IEnumerable<AllJoinedTable>> GetSongsFromPlaylistAsync(int id)
        {
            return null;
        }

        public Task<IEnumerable<AllJoinedTable>> SearchLikeAsync(SearchType searchTypes, string wildCardSearch, short randomAmount, short maxAmount)
        {
            return Task.Run(() => SearchLike(searchTypes, wildCardSearch, randomAmount, maxAmount));
        }

        public Task<IEnumerable<AllJoinedTable>> SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount, short maxAmount)
        {
            return Task.Run(() => this.SearchLikeFilters(searchFilter, randomAmount, maxAmount));
        }

        public Task<bool> UpdatePlayedSongAsync(int id, int? rating)
        {
            return Task.Run(() => UpdatePlayedSong(id, rating));
        }
    }

    /// <summary>
    /// File Service
    /// </summary>
    public partial class HorsifySongService
    {
        public int Add(File file)
        {
            return 0;
        }

        public File GetById(long value)
        {
            return _sqliteRepo.FileRepository.GetById(value);
        }
    }

    /// <summary>
    /// Search Service
    /// </summary>
    public partial class HorsifySongService
    {
        public IEnumerable<AllJoinedTable> GetMostPlayed()
        {
            return _sqliteRepo.GetMostPlayed();
        }
        public Task<IEnumerable<AllJoinedTable>> GetMostPlayedAsync()
        {
            return Task.Run(() => GetMostPlayed());
        }

        public IEnumerable<AllJoinedTable> GetRecentlyAdded()
        {
            return _sqliteRepo.GetRecentlyAdded();
        }
        public Task<IEnumerable<AllJoinedTable>> GetRecentlyAddedAsync()
        {
            return Task.Run(() => GetRecentlyAdded());
        }

        public IEnumerable<AllJoinedTable> GetRecentlyPlayed()
        {
            return _sqliteRepo.GetRecentlyPlayed();
        }
        public Task<IEnumerable<AllJoinedTable>> GetRecentlyPlayedAsync()
        {
            return Task.Run(() => GetRecentlyPlayed());
        }

        public AllJoinedTable GetSongById(int value)
        {
            var song = _sqliteRepo.AllJoinedTableRepository.GetById(value);
            return song;
        }

        public int GetTotals(string type = "Song")
        {
            if (type == "Song")
            {
                return _sqliteRepo.SongRepository.Get().Count();
            }
            else if (type == "File")
            {
                return _sqliteRepo.FileRepository.Get().Count();
            }

            return 0;
        }

        /// <summary>
        /// Searches with like wildcards from all tables given in the searchtypes flags. <para/>
        /// Search wild cards joined with ; eg "%Noisia%|%Jackson"
        /// </summary>
        /// <param name="searchTypes">The search types.</param>
        /// <param name="wildCardSearch">The wild card search.</param>
        /// <returns></returns>
        public IEnumerable<AllJoinedTable> SearchLike(SearchType searchTypes, string wildCardSearch, short randomAmount = 0, short maxAmount = -1)
        {
            wildCardSearch = ReplaceWildcards(wildCardSearch);
            var sqlStr = _sqliteRepo.SearchLike(searchTypes, wildCardSearch);

            if (randomAmount > 0)
                return _sqliteRepo.ExecuteSearchLike(sqlStr, randomAmount);

            return _sqliteRepo.ExecuteSearchLike(sqlStr, maxAmount);
        }

        private static string ReplaceWildcards(string wildCardSearch)
        {
            wildCardSearch = wildCardSearch.Replace('*', '%');
            return wildCardSearch;
        }

        //public Task<IEnumerable<AllJoinedTable>> SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1)
        //{
        //    return Task.Run(() => SearchLikeFilters(searchFilter, randomAmount, maxAmount));
        //}

        public IEnumerable<AllJoinedTable> SearchLikeFilters(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1)
        {
            var sqlStr = _sqliteRepo.SearchLike(searchFilter);
            sqlStr = ReplaceWildcards(sqlStr);

            if (sqlStr.EndsWith("AND "))
            {
                sqlStr = sqlStr.Remove(sqlStr.Length - 4, 4);
            }

            if (randomAmount > 0)
                return _sqliteRepo.ExecuteSearchLike(sqlStr, randomAmount, maxAmount);

            return _sqliteRepo.ExecuteSearchLike(sqlStr, maxAmount);
        }

        /// <summary>
        /// Searches the like.
        /// </summary>
        /// <param name="searchType">Type of the search.</param>
        /// <param name="search">The search. A single starting char</param>
        /// <returns></returns>
        public IEnumerable<string> GetAllFromTableAsStrings(SearchType searchType, string search,short maxAmount =-1)
        {
            switch (searchType)
            {
                case SearchType.All:
                    break;
                case SearchType.Album:
                    return _sqliteRepo.AlbumRepository
                        .Get(x => x.Title.ToUpper().StartsWith(search.ToUpper()),
                        orderBy: x => x.OrderBy(z => z.Title))
                        .Select(z => z.Title);
                case SearchType.Artist:
                    if (maxAmount > 0)
                    {
                        return _sqliteRepo.ArtistRepository
                        .Get(x => x.Name.ToUpper().Contains(search.ToUpper()),
                        orderBy: x => x.OrderBy(z => z.Name))
                        .Select(z => z.Name)
                        .Take(maxAmount);
                    }                    
                    return _sqliteRepo.ArtistRepository
                        .Get(x => x.Name.ToUpper().StartsWith(search.ToUpper()),
                        orderBy: x => x.OrderBy(z => z.Name))
                        .Select(z => z.Name);
                case SearchType.Bpm:
                    break;
                case SearchType.FileLocation:
                    break;
                case SearchType.Genre:
                    return _sqliteRepo.GenreRepository
                        .Get(x => x.Name.ToUpper().StartsWith(search.ToUpper()),
                        orderBy: x => x.OrderBy(z => z.Name))
                        .Select(z => z.Name);
                case SearchType.Label:
                    return _sqliteRepo.LabelRepository
                        .Get(x => x.Name.ToUpper().StartsWith(search.ToUpper()),
                        orderBy: x => x.OrderBy(z => z.Name))
                        .Select(z => z.Name);
                case SearchType.Title:
                    if (maxAmount > 0)
                    {
                        return _sqliteRepo.SongRepository
                            .Get(x => x.Title.ToUpper().Contains(search.ToUpper()),
                                orderBy: x => x.OrderBy(z => z.Title),
                                includeProperties: "Artist,Album")
                                .Select(z => $"{z.Id}|{z.Artist?.Name}|{z.Title}|{z.Album?.Title}|{z.Year}|{z.ImageLocation}|{z.Rating}")
                                .Take(maxAmount);
                    }
                    return _sqliteRepo.SongRepository
                        .Get(x => x.Title.ToUpper().StartsWith(search.ToUpper()),
                        orderBy: x => x.OrderBy(z => z.Title))
                        .Select(z => z.Title)
                        .Take(maxAmount);
                case SearchType.Year:
                    break;
                default:
                    break;
            }

            return null;
        }
    }

    public partial class HorsifySongService // Update song
    {
        /// <summary>
        /// Updates the played song count, and last played date and rating.
        /// </summary>
        /// <param name="songId">The song identifier.</param>
        /// <param name="rating">The rating.</param>
        /// <returns></returns>
        public bool UpdatePlayedSong(long songId, int? rating = null)
        {
            var dbSong = _sqliteRepo.SongRepository.GetById(songId);
            if (dbSong == null)
                return false;

            dbSong.LastPlayed = ConvertUnixTime(DateTime.Now).ToString();

            if (dbSong.TimesPlayed == null)
                dbSong.TimesPlayed = 1;
            else
            {
                dbSong.TimesPlayed = dbSong.TimesPlayed + 1;
            }
                        
            if (rating != null)
                dbSong.Rating = rating;

            try
            {
                _sqliteRepo.SongRepository.Update(dbSong);
                var repo = (IUnitOfWork)_sqliteRepo;
                repo.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }                   
        }

        private long ConvertUnixTime(DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }
    }

}
