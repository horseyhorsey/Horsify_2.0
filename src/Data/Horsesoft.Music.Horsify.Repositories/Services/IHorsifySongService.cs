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

        Task<bool> DeleteFilterSearchAsync(int id);

        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
        Task<IEnumerable<FiltersSearch>> GetFilterSearchesAsync();
        Task<IEnumerable<AllJoinedTable>> GetMostPlayedAsync();        
        Task<IEnumerable<AllJoinedTable>> GetRecentlyAddedAsync();
        Task<IEnumerable<AllJoinedTable>> GetRecentlyPlayedAsync();
        HorsifyDataSqliteRepo GetRepo();
        Task<IEnumerable<AllJoinedTable>> GetSongsFromPlaylistAsync(Playlist playlist);

        Task<bool> InsertFilterSearchAsync(FiltersSearch filtersSearch);
        Task InsertOrUpdatePlaylistsAsync(Playlist[] playlists);

        Task<IEnumerable<AllJoinedTable>> SearchLikeAsync(SearchType searchTypes, string wildCardSearch, short randomAmount, short maxAmount);
        Task<IEnumerable<AllJoinedTable>> SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount, short maxAmount);
        
        Task UpdateFilterSearchAsync(FiltersSearch filtersSearch);        
        Task<bool> UpdatePlayedSongAsync(int id, int? rating);

        Task DeletePlaylistAsync(int id);
    }
}
