using System.Collections.Generic;
using System.Threading.Tasks;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;

namespace Horsesoft.Music.Horsify.Repositories.Services
{
    public interface IHorsifySongService : IHorsifyFileService, IHorsifySearchService, IHorsifyTagService, IHorsifyFilterService, IHorsifyPlaylistService
    {
        AllJoinedTable GetSongById(int value);

        int GetTotals(string type = "Song");                
        Task<IEnumerable<AllJoinedTable>> GetMostPlayedAsync();
        Task<IEnumerable<AllJoinedTable>> GetRecentlyAddedAsync();
        Task<IEnumerable<AllJoinedTable>> GetRecentlyPlayedAsync();
        Task<bool> UpdatePlayedSongAsync(int id, int? rating);
        Task<IEnumerable<AllJoinedTable>> SearchLikeAsync(SearchType searchTypes, string wildCardSearch, short randomAmount, short maxAmount);
        Task<IEnumerable<AllJoinedTable>> SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount, short maxAmount);
        Task InsertOrUpdatePlaylistsAsync(Playlist[] playlists);
        Task<IEnumerable<AllJoinedTable>> GetSongsFromPlaylistAsync(Playlist playlist);
        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
    }
}
