using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.Model;
using Prism.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.ServicesModule
{
    public class DjHorsifyService : IDjHorsifyService
    {
        #region Fields
        private IHorsifySongApi _horsifySongApi;
        private ILoggerFacade _loggerFacade; 
        #endregion

        public DjHorsifyService(IDjHorsifyOption djHorsifyOption, IHorsifySongApi horsifySongApi, ILoggerFacade loggerFacade)
        {            
            DjHorsifyOption = djHorsifyOption;
            _horsifySongApi = horsifySongApi;
            _loggerFacade = loggerFacade;            
        }

        #region Properties
        public IDjHorsifyOption DjHorsifyOption { get; set; }
        private IEnumerable<Filter> _dbFilters;
        public ObservableCollection<Filter> Filters { get; private set; }
        public ObservableCollection<DjHorsifyFilterModel> HorsifyFilters { get; set; }
        public ObservableCollection<FiltersSearch> SavedFilters { get; set; }
        #endregion

        #region Public Methods        

        /// <summary>
        /// Adds the filter with the running song service
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public async Task<bool> AddFilterAsync(Filter filter)
        {
            try
            {
                if (await _horsifySongApi.InsertFilterAsync(filter))
                {
                    this.Filters.Add(filter);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                _loggerFacade.Log($"AddFilter - {ex.Message}", Category.Exception, Priority.Medium);
                return false;
            }

            return true;
        }

        public Task<bool> AddSavedSearchFilterAsync(FiltersSearch searchFilter)
        {
            return _horsifySongApi.InsertSavedSearchFiltersAsync(searchFilter);
        }

        public Task<bool> DeleteFilterAsync(Filter filter)
        {
            return _horsifySongApi.DeleteFilterAsync(filter);
        }

        public SearchFilter GenerateSearchFilter(IDjHorsifyOption djHorsifyOption)
        {
            IList<HorsifyFilter> horsifyFilters = new List<HorsifyFilter>();
            if (djHorsifyOption.SelectedFilters?.Count > 0)
            {
                foreach (var item in djHorsifyOption.SelectedFilters)
                {
                    var filter = new HorsifyFilter()
                    {
                        FileName = item.FileName,
                        Filters = item.Filters,
                        Id = item.Id,
                        SearchAndOrOption = item.SearchAndOrOption,
                        SearchType = item.SearchType
                    };

                    horsifyFilters.Add(filter);
                }

                return new SearchFilter()
                {
                    BpmRange = djHorsifyOption.BpmRange,
                    Filters = horsifyFilters,
                    RatingRange = djHorsifyOption.RatingRange,
                    MusicKeys = djHorsifyOption.SelectedKeys.ToString()
                };
            }

            return new SearchFilter()
            {
                BpmRange = djHorsifyOption.BpmRange,
                RatingRange = djHorsifyOption.RatingRange,
                MusicKeys = djHorsifyOption.SelectedKeys.ToString()
            };

        }

        /// <summary>
        /// Gets the database filters and converts to Horsify Filters
        /// </summary>
        public async Task GetDatabaseFiltersAsync()
        {
            try
            {
                _dbFilters = await _horsifySongApi.GetFilters();
                if (Filters == null)
                {
                    if (_dbFilters != null)
                    {
                        Filters = new ObservableCollection<Filter>(_dbFilters);
                    }
                }
                else
                {
                    Filters.Clear();
                }
            }
            catch (System.Exception ex)
            {
                _loggerFacade.Log($"{ex.Message}", Category.Exception, Priority.Medium);
                System.Windows.MessageBox.Show("Horsify service not running or failed to query.");
            }

        }

        public Task<IEnumerable<FiltersSearch>> GetSavedSearchFilters()
        {
            return _horsifySongApi.GetSavedSearchFiltersAsync();
        }

        /// <summary>
        /// Gets the songs using the DjHorsifyOption
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AllJoinedTable> GetSongs(IDjHorsifyOption djHorsifyOption)
        {
            var searchFilter = GenerateSearchFilter(djHorsifyOption);

            return _horsifySongApi.SearchLikeFiltersAsync(searchFilter, (short)djHorsifyOption.Amount, (short)djHorsifyOption.Amount).Result;

        }

        public Task<IEnumerable<AllJoinedTable>> GetSongsAsync(IDjHorsifyOption djHorsifyOption)
        {
            if (djHorsifyOption == null)
                djHorsifyOption = this.DjHorsifyOption;

            return Task.Run(() => GetSongs(djHorsifyOption));
        }

        public bool UpdateFilter(Music.Data.Model.Filter dbFilter)
        {
            try
            {
                var filterToUpdate = Filters.FirstOrDefault(x => x.Id == dbFilter.Id);
                filterToUpdate.SearchTerms = dbFilter.SearchTerms;
                filterToUpdate.Name = dbFilter.Name;

                //Update filter
                _horsifySongApi.UpdateFilterAsync(dbFilter.Id, filterToUpdate);           

                return true;
            }
            catch (System.Exception ex)
            {
                _loggerFacade.Log($"UpdateFilter - {ex.Message}", Category.Exception, Priority.Medium);
                return false;
            }
        }

        public Task<bool> UpdateSearchFilterAsync(FiltersSearch filter)
        {
            return _horsifySongApi.UpdateSavedSearchFiltersAsync(filter);
        }

        public Task<bool> DeleteSearchFilterAsync(int? id)
        {
            return _horsifySongApi.DeleteFilterSearchAsync(id);
        }

        #endregion
    }
}
