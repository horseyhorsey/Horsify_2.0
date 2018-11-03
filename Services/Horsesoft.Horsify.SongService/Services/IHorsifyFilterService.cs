using Horsesoft.Music.Data.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace Horsesoft.Horsify.SongService.Services
{
    public interface IHorsifyFilterService
    {
        IEnumerable<Filter> GetFilters();

        
        void InsertFilter(Filter filter);

        
        void RemoveFilter(Filter filter);

        
        void UpdateFilter(Filter filter);
    }
}
