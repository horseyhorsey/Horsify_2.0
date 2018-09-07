using Horsesoft.Music.Data.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace Horsesoft.Horsify.SongService.Services
{
    [ServiceContract]
    public interface IHorsifyFilterService
    {
        [OperationContract]
        IEnumerable<Filter> GetFilters();

        [OperationContract]
        void InsertFilter(Filter filter);

        [OperationContract]
        void RemoveFilter(Filter filter);

        [OperationContract]
        void UpdateFilter(Filter filter);
    }
}
