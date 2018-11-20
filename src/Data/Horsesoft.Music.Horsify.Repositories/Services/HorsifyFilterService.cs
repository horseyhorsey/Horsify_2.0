using Horsesoft.Music.Data.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Repositories.Services
{
    public partial class HorsifySongService // Filter Service
    {
        public IEnumerable<Music.Data.Model.Filter> GetFilters()
        {
            return _sqliteRepo.FilterRepository.Get(orderBy: x => x.OrderBy(z => z.Name));
        }

        public Task<IEnumerable<FiltersSearch>> GetFilterSearchesAsync()
        {
            return Task.Run(() =>  _sqliteRepo.FiltersSearchRepository.Get());
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

        public async Task<bool> DeleteFilterSearchAsync(int id)
        {
            try
            {
                _sqliteRepo.FiltersSearchRepository.Delete(id);
                await ((IUnitOfWork)_sqliteRepo).SaveAsync();
                return true;
            }
            catch { return false;}
            
        }

        public Task<bool> InsertFilterSearchAsync(FiltersSearch filtersSearch)
        {
            try
            {
                Task.Run(() =>
                {
                    _sqliteRepo.FiltersSearchRepository.Insert(filtersSearch);
                    ((IUnitOfWork)_sqliteRepo).Save();

                    return true;
                });
            }
            catch (System.Exception)
            {
                
            }

            return Task.FromResult(false);
        }

        public Task UpdateFilterSearchAsync(FiltersSearch filtersSearch)
        {
            _sqliteRepo.FiltersSearchRepository.Update(filtersSearch);
            return ((IUnitOfWork)_sqliteRepo).SaveAsync();
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
