using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using System.Collections.Generic;
using System.ServiceModel;

namespace Horsesoft.Horsify.SongService
{
    [ServiceContract]
    public interface IHorsifySearchService
    {
        [OperationContract]
        IEnumerable<AllJoinedTable> SearchLike(SearchType searchTypes, string wildCardSearch, short randomAmount = 0, short maxAmount = -1);

        [OperationContract]
        IEnumerable<AllJoinedTable> SearchLikeFilters(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1);

        [OperationContract]
        IEnumerable<AllJoinedTable> GetMostPlayed();

        [OperationContract]
        IEnumerable<AllJoinedTable> GetRecentlyAdded();

        [OperationContract]
        IEnumerable<AllJoinedTable> GetRecentlyPlayed();

        [OperationContract]
        IEnumerable<string> GetAllFromTableAsStrings(SearchType searchType, string search, short maxAmount = -1);
    }
}
