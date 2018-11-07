using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.Repositories.Services
{

    public interface IHorsifySearchService
    {
        
        IEnumerable<AllJoinedTable> SearchLike(SearchType searchTypes, string wildCardSearch, short randomAmount = 0, short maxAmount = -1);

        
        IEnumerable<AllJoinedTable> SearchLikeFilters(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1);

        
        IEnumerable<AllJoinedTable> GetMostPlayed();

        
        IEnumerable<AllJoinedTable> GetRecentlyAdded();

        
        IEnumerable<AllJoinedTable> GetRecentlyPlayed();

        
        IEnumerable<string> GetAllFromTableAsStrings(SearchType searchType, string search, short maxAmount = -1);
    }
}
