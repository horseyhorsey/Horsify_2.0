using Horsesoft.Music.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Horsesoft.Music.Horsify.Repositories.Services
{
    public partial class HorsifySongService // Filter Service
    {
        public IEnumerable<Music.Data.Model.Filter> GetFilters()
        {
            return _sqliteRepo.FilterRepository.Get(orderBy: x => x.OrderBy(z => z.Name));
        }

        public Filter GetFilter(int id)
        {
            return _sqliteRepo.FilterRepository.GetById((long)id);
        }

        public void InsertFilter(Filter filter)
        {
            _sqliteRepo.FilterRepository.Insert(filter);
            ((IUnitOfWork)_sqliteRepo).Save();
        }

        public void RemoveFilter(Filter filter)
        {
            _sqliteRepo.FilterRepository.Delete(filter);
            ((IUnitOfWork)_sqliteRepo).Save();
        }

        public void UpdateFilter(Filter filter)
        {
            _sqliteRepo.FilterRepository.Update(filter);
            ((IUnitOfWork)_sqliteRepo).Save();
        }
    }
}
