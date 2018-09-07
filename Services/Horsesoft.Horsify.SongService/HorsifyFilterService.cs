using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Horsesoft.Horsify.SongService
{
    public partial class HorsifySongService // Filter Service
    {
        public IEnumerable<Music.Data.Model.Filter> GetFilters()
        {
            return _sqliteRepo.FilterRepository.Get(orderBy: x => x.OrderBy(z => z.Name));
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
