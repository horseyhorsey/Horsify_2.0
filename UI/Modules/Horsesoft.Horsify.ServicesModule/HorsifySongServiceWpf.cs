using System.Collections.Generic;
using System.Threading.Tasks;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Repositories.Services;

namespace Horsesoft.Horsify.ServicesModule
{
    public class HorsifySongServiceWpf : IHorsifySongService
    {
        public int Add(File file)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetAllFromTableAsStrings(SearchType searchType, string search, short maxAmount = -1)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Playlist> GetAllPlaylists()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Playlist>> GetAllPlaylistsAsync()
        {
            throw new System.NotImplementedException();
        }

        public File GetById(long value)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Filter> GetFilters()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AllJoinedTable> GetMostPlayed()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AllJoinedTable>> GetMostPlayedAsync()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AllJoinedTable> GetRecentlyAdded()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AllJoinedTable>> GetRecentlyAddedAsync()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AllJoinedTable> GetRecentlyPlayed()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AllJoinedTable>> GetRecentlyPlayedAsync()
        {
            throw new System.NotImplementedException();
        }

        public AllJoinedTable GetSongById(int value)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AllJoinedTable> GetSongsFromPlaylist(Playlist playlist)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AllJoinedTable>> GetSongsFromPlaylistAsync(Playlist playlist)
        {
            throw new System.NotImplementedException();
        }

        public int GetTotals(string type = "Song")
        {
            throw new System.NotImplementedException();
        }

        public void InsertFilter(Filter filter)
        {
            throw new System.NotImplementedException();
        }

        public void InsertOrUpdatePlaylists(IEnumerable<Playlist> playlists)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertOrUpdatePlaylistsAsync(Playlist[] playlists)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveFilter(Filter filter)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AllJoinedTable> SearchLike(SearchType searchTypes, string wildCardSearch, short randomAmount = 0, short maxAmount = -1)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AllJoinedTable>> SearchLikeAsync(SearchType searchTypes, string wildCardSearch, short randomAmount, short maxAmount)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<AllJoinedTable> SearchLikeFilters(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AllJoinedTable>> SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount, short maxAmount)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateFilter(Filter filter)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdatePlayedSong(long songId, int? rating = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdatePlayedSongAsync(int id, int? rating)
        {
            throw new System.NotImplementedException();
        }
    }
}
