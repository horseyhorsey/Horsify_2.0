using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IDjHorsifyService
    {
        bool AddFilter(Filter filter);

        IDjHorsifyOption DjHorsifyOption { get; set; }

        ObservableCollection<Filter> Filters { get; }

        void GetDatabaseFilters();

        /// <summary>
        /// Gets the songs using the DjHorsifyOption
        /// </summary>
        /// <returns></returns>
        IEnumerable<AllJoinedTable> GetSongs(IDjHorsifyOption djHorsifyOption);

        Task<IEnumerable<AllJoinedTable>> GetSongsAsync(IDjHorsifyOption djHorsifyOption);

        bool UpdateFilter(Music.Data.Model.Filter dbFilter);

        SearchFilter GenerateSearchFilter(IDjHorsifyOption djHorsifyOption);
    }
}
