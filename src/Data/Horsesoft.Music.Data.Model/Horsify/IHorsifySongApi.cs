using System.Collections.Generic;
using System.Threading.Tasks;

namespace Horsesoft.Music.Data.Model.Horsify
{
    public interface IHorsifySongApi
    {
        string BaseAddress { get; set; }
        Task<bool> DeleteFilterAsync(Filter filter);

        Task<bool> DeleteFilterSearchAsync(int? id);

        /// <summary>
        /// Call the songs api extra search, like most played etc <see cref="ExtraSearchType"/>
        /// </summary>
        /// <param name="extraSearchType"></param>
        Task<IEnumerable<AllJoinedTable>> ExtraSearch(ExtraSearchType extraSearchType);

        Task<AllJoinedTable> GetById(int id);

        /// <summary>
        /// Gets the all the distinct entries for a certain table. A-Z
        /// </summary>
        /// <param name="searchType">Type of the search.</param>
        /// <returns></returns>
        IEnumerable<string> GetEntries(SearchType searchType, char firstChar);

        IEnumerable<string> GetEntries(SearchType searchType, string searchTerm, short maxAmount = -1);

        Task<IEnumerable<Filter>> GetFilters();

        Task<IEnumerable<Playlist>> GetPlaylists();

        Task<IEnumerable<FiltersSearch>> GetSavedSearchFiltersAsync();

        Task<IEnumerable<AllJoinedTable>> GetSongsFromPlaylistAsync(Playlist playlist);

        Task<bool> InsertFilterAsync(Filter filter);

        Task InsertOrUpdatePlaylistsAsync(Playlist[] playlists);

        Task<bool> InsertSavedSearchFiltersAsync(FiltersSearch searchFilter);

        /// <summary>
        /// Search for songs with types. Use * for wildcard likes
        /// </summary>
        /// <param name="term"></param>
        /// <param name="searchTypes"></param>
        /// <returns></returns>
        Task<IEnumerable<AllJoinedTable>> SearchAsync(string term, SearchType searchTypes);

        Task<IEnumerable<AllJoinedTable>> SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1);

        Task UpdateFilterAsync(long id, Filter filterToUpdate);

        Task<bool> UpdatePlayedSongAsync(int id, int? rating);
        Task<bool> UpdateSavedSearchFiltersAsync(FiltersSearch filter);
    }
}
