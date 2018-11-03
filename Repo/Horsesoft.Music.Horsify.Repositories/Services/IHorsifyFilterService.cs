using Horsesoft.Music.Data.Model;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.Repositories.Services
{
    public interface IHorsifyFilterService
    {
        IEnumerable<Filter> GetFilters();
        Filter GetFilter(int id);
        void InsertFilter(Filter filter);
        void RemoveFilter(Filter filter);
        void UpdateFilter(Filter filter);
    }
}
