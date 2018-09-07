using Horsesoft.Horsify.ServicesModule.HorsifyService;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.ServicesModule
{
    public class DjHorsifyService : IDjHorsifyService
    {
        private IHorsifySongService _horsifySongService;
        public IDjHorsifyOption DjHorsifyOption { get; set; }

        private ILoggerFacade _loggerFacade;
        private Music.Data.Model.Filter[] _dbFilters;

        public DjHorsifyService(IDjHorsifyOption djHorsifyOption, IHorsifySongService horsifySongService, ILoggerFacade loggerFacade)
        {
            _horsifySongService = horsifySongService;
            DjHorsifyOption = djHorsifyOption;
            _loggerFacade = loggerFacade;

            GetDatabaseFilters();
        }

        public ObservableCollection<Filter> Filters { get; private set; }        

        #region IDjHorsifyService Methods

        /// <summary>
        /// Adds the filter with the running song service
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public bool AddFilter(Music.Data.Model.Filter filter)
        {
            try
            {
                _horsifySongService.InsertFilter(filter);
            }
            catch (System.Exception ex)
            {
                _loggerFacade.Log($"AddFilter - {ex.Message}", Category.Exception, Priority.Medium);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the database filters and converts to Horsify Filters
        /// </summary>
        public void GetDatabaseFilters()
        {
            try
            {
                _dbFilters = _horsifySongService.GetFilters();
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

        /// <summary>
        /// Gets the songs using the DjHorsifyOption
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AllJoinedTable> GetSongs(IDjHorsifyOption djHorsifyOption)
        {

            var searchFilter = GenerateSearchFilter(djHorsifyOption);

            return _horsifySongService.SearchLikeFilters(searchFilter, (short)djHorsifyOption.Amount, -1);
        }

        public SearchFilter GenerateSearchFilter(IDjHorsifyOption djHorsifyOption)
        {
            IList<HorsifyFilter> horsifyFilters = new List<HorsifyFilter>();
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

        public Task<IEnumerable<AllJoinedTable>> GetSongsAsync(IDjHorsifyOption djHorsifyOption)
        {
            if (djHorsifyOption == null)
                djHorsifyOption = this.DjHorsifyOption;

            return Task.Run(() => GetSongs(djHorsifyOption));
        }

        #endregion

        #region Private Methods

        public bool UpdateFilter(Music.Data.Model.Filter dbFilter)
        {
            try
            {
                //TODO: Tidy up, doesn't need all this work going on
                var filterToUpdate = _dbFilters.FirstOrDefault(x => x.Id == dbFilter.Id);
                filterToUpdate.SearchTerms = dbFilter.SearchTerms;
                filterToUpdate.Name = dbFilter.Name;
                //TODO: ID already being tracked in database
                _horsifySongService.UpdateFilter(filterToUpdate);

                //Get db filter
                var f = this.Filters.FirstOrDefault(x => x.Id == filterToUpdate.Id);
                //Create horsify filter
                var newFilter = Music.Data.Model.Horsify.HorsifyFilter.GetFilterFromString(filterToUpdate.SearchTerms, f);

                newFilter.FileName = filterToUpdate.Name;
                newFilter.Filters = newFilter.Filters;
                newFilter.Id = (int)dbFilter.Id;

                this.Filters.Remove(f);
                this.Filters.Add(f);

                return true;
            }
            catch (System.Exception ex)
            {
                _loggerFacade.Log($"UpdateFilter - {ex.Message}", Category.Exception, Priority.Medium);
                return false;
            }
        }

        #endregion
    }
}
