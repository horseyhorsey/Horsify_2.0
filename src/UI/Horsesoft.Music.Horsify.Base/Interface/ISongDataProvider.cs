using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface ISongDataProvider
    {
        Task ExtraSearch(ExtraSearchType extraSearchType);
        Task<AllJoinedTable> GetSongById(int id);
        Task<AllJoinedTable[]> GetSongs(Playlist playlist);
        Task<AllJoinedTable[]> GetSongsAsync(SearchType searchTypes, string wildCardSearch, short randomAmount = 10, short maxAmount = -1);

        /// <summary>
        /// Searches and populates the <see cref="SearchedSongs"/>
        /// </summary>
        /// <param name="searchTypes">The search types.</param>
        /// <param name="wildCardSearch">The wild card search.</param>
        /// <param name="randomAmount">The random amount.</param>
        Task SearchAsync(SearchType searchTypes, string wildCardSearch, short randomAmount = 10, short maxAmount = -1);

        /// <summary>
        /// Searches with an ISearchFilter
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <param name="randomAmount"></param>
        /// <returns></returns>
        Task SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount = 10, short maxAmount = -1);

        ObservableCollection<AllJoinedTable> SearchedSongs { get; set; }
        AllJoinedTable SelectedSong { get; set; }
        Task<bool> UpdatePlayedSong(AllJoinedTable selectedSong, int? rating = null);
    }
}
