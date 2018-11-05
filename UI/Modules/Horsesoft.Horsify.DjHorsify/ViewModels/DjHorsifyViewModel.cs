using Horsesoft.Horsify.DjHorsify.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Helpers;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    public interface IDjHorsifyViewModel
    {
        DjHorsifyOption DjHorsifyOption { get; set; }
        ICollectionView IncludedFiltersView { get; set; }
        ICollectionView IncludedAndFiltersView { get; set; }        
        ICollectionView ExcludedFiltersView { get; set; }
    }

    public class DjHorsifyViewModel : HorsifyBindableBase, INavigationAware, IDjHorsifyViewModel
    {
        private IRegionManager _regionManager;
        private IDjHorsifyService _djHorsifyService;

        #region Commands        
        public ICommand CreateFilterCommand { get; set; }
        public ICommand EditFilterCommand { get; set; }
        public ICommand RunSearchCommand { get; set; }
        public ICommand RunSingleSearchCommand { get; set; }        
        #endregion

        public DjHorsifyViewModel(IDjHorsifyService djHorsifyService, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _regionManager = regionManager;

            _djHorsifyService = djHorsifyService;
            DjHorsifyOption = _djHorsifyService.DjHorsifyOption as DjHorsifyOption;

            try
            {
                _djHorsifyService.GetDatabaseFiltersAsync().Wait();

                GenerateHorsifyFilters();
            }
            catch (Exception ex)
            {
                Log($"Error Loading DjHorsify : {ex.Message}", Category.Exception);
                throw;
            }

            #region Setup filtering
            AvailableFiltersView = new MyCollectionView(this.HorsifyFilters);
            AvailableFiltersView.Filter = (o) =>
            {
                var item = o as DjHorsifyFilterModel;
                if (item.SearchAndOrOption == SearchAndOrOption.None)
                {
                    if (DjHorsifyOption.SelectedFilters.Any(x => x == item))
                    {
                        DjHorsifyOption.SelectedFilters.Remove(item);
                    }
                    return true;
                }

                return false;
            };

            IncludedFiltersView = new MyCollectionView(this.HorsifyFilters);
            IncludedFiltersView.Filter = (o) =>
            {
                var item = o as DjHorsifyFilterModel;
                if (item.SearchAndOrOption == SearchAndOrOption.Or)
                {
                    if (!DjHorsifyOption.SelectedFilters.Any(x => x == item))
                    {
                        DjHorsifyOption.SelectedFilters.Add(item);
                    }
                    return true;
                }

                return false;
            };

            IncludedAndFiltersView = new MyCollectionView(this.HorsifyFilters);
            IncludedAndFiltersView.Filter = (o) =>
            {
                var item = o as DjHorsifyFilterModel;
                if (item.SearchAndOrOption == SearchAndOrOption.And)
                {
                    if (!DjHorsifyOption.SelectedFilters.Any(x => x == item))
                    {
                        DjHorsifyOption.SelectedFilters.Add(item);
                    }
                    return true;
                }
                return false;
            };

            ExcludedFiltersView = new MyCollectionView(this.HorsifyFilters);
            ExcludedFiltersView.Filter = (o) =>
            {
                var item = o as DjHorsifyFilterModel;
                if (item.SearchAndOrOption == SearchAndOrOption.Not)
                {
                    if (!DjHorsifyOption.SelectedFilters.Any(x => x == item))
                    {
                        DjHorsifyOption.SelectedFilters.Add(item);
                    }
                    return true;
                }
                return false;
            }; 
            #endregion

            #region Commands
            CreateFilterCommand = new DelegateCommand(OnCreateFilter);
            EditFilterCommand = new DelegateCommand<DjHorsifyFilterModel>(OnEditFilter);
            RunSearchCommand = new DelegateCommand(OnRunSearch);
            RunSingleSearchCommand = new DelegateCommand<DjHorsifyFilterModel>(OnRunSearch);
            #endregion            
        }

        private void GenerateHorsifyFilters()
        {
            var horsifyFilters = GenerateHorsifyFilters(_djHorsifyService.Filters.ToArray());
            if (horsifyFilters?.Count() > 0)
            {
                //If null create the collection and populate it, otherwise addrange
                if (HorsifyFilters == null)
                {
                    HorsifyFilters = new ObservableCollection<DjHorsifyFilterModel>(horsifyFilters);                    
                }
                else
                {
                    HorsifyFilters.AddRange(horsifyFilters);
                }                
            }            
        }

        /// <summary>
        /// Generates the horsify filters from database filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        private IEnumerable<DjHorsifyFilterModel> GenerateHorsifyFilters(Music.Data.Model.Filter[] filters)
        {
            var horsifyFilters = new List<DjHorsifyFilterModel>();
            foreach (var filter in filters)
            {
                var horsifyFilter = HorsifyFilter.GetFilterFromString(filter.SearchTerms, filter);
                horsifyFilter.FileName = filter.Name;
                horsifyFilter.Id = (int)filter.Id;

                horsifyFilters.Add(new DjHorsifyFilterModel(horsifyFilter));
            }

            return horsifyFilters;
        }

        #region Properties
        //public ICollectionView AvailableFiltersView { get; set; }
        public ICollectionView IncludedFiltersView { get; set; }
        public ICollectionView ExcludedFiltersView { get; set; }
        public ICollectionView IncludedAndFiltersView { get; set; }        

        private DjHorsifyOption _djHorsifyOption;
        public DjHorsifyOption DjHorsifyOption
        {
            get { return _djHorsifyOption; }
            set { SetProperty(ref _djHorsifyOption, value); }
        }

        private ObservableCollection<DjHorsifyFilterModel> _horsifyFilters;
        public ObservableCollection<DjHorsifyFilterModel> HorsifyFilters
        {
            get { return _horsifyFilters; }
            set { SetProperty(ref _horsifyFilters, value); }
        }

        public ICollectionView AvailableFiltersView { get; }
        #endregion

        #region Private Methods
        private void OnEditFilter(DjHorsifyFilterModel filter)
        {
            Log($"Editing previous filter: {filter?.FileName}");

            var navParams = new NavigationParameters();
            navParams.Add("edit_filter", filter);
            _regionManager.RequestNavigate(Regions.ContentRegion, "EditFilterView", navParams);
        }

        private void OnRunSearch()
        {
            Log("Running search view DJHorsify", Category.Debug);

            try
            {
                var searchFilter = _djHorsifyService.GenerateSearchFilter(this.DjHorsifyOption);

                if (searchFilter == null)
                    throw new NullReferenceException("Generating search filter from Dj Horsify option failed.");

                //if (searchFilter?.Filters?.Count() <= 0)
                //    return;

                var navParams = new NavigationParameters();
                navParams.Add("djhorsify_search", searchFilter);
                _regionManager.RequestNavigate(Regions.ContentRegion, "SearchedSongsView", navParams);
            }
            catch (Exception ex)
            {
                Log($"Fail to run search from DJ Horsify : {ex.Message}", Category.Exception);
                throw;
            }
        }

        private void OnRunSearch(DjHorsifyFilterModel filter)
        {
            if (filter != null)
            {
                Log("Running single search view DJHorsify", Category.Debug);
                var navParams = NavigationHelper.CreateSearchFilterNavigation(filter);
                _regionManager.RequestNavigate(Regions.ContentRegion, "SearchedSongsView", navParams);
            }
        }

        private void OnCreateFilter()
        {
            Log("Create filter DJHorsify", Category.Info);
            _regionManager.RequestNavigate(Regions.ContentRegion, "CreateFilterView");
        }

        #endregion

        #region Navigate
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var filter = navigationContext.Parameters["add_new_filter"] as DjHorsifyFilterModel;
            if (filter != null)
            {
                Log("add_new_filter", Category.Debug);
                Music.Data.Model.Filter dbFilter = CreateDbFilter(filter);
                try
                {
                    var result = _djHorsifyService.AddFilter(dbFilter);
                    if (result)
                        this.HorsifyFilters.Add(filter as DjHorsifyFilterModel);
                    else
                        Log("Couldn't create service filters", Category.Debug);
                }
                catch (Exception ex)
                {
                    Log($"Couldn't create service filters. {ex.Message}", Category.Exception);
                }
                finally
                {

                }
            }
            else
            {
                filter = navigationContext.Parameters["edited_filter"] as DjHorsifyFilterModel;
                if (filter != null)
                {
                    Music.Data.Model.Filter dbFilter = CreateDbFilter(filter);
                    var result = _djHorsifyService.UpdateFilter(dbFilter);
                    if (!result)
                        Log("Couldn't update service filters", Category.Debug);
                    else
                    {                        
                        var existingFilter = this.HorsifyFilters.FirstOrDefault(x => x.Id == filter.Id);
                        if (existingFilter != null)
                        {
                            existingFilter.FileName = filter.FileName;
                            existingFilter.Filters = filter.Filters;
                            existingFilter.SearchAndOrOption = filter.SearchAndOrOption;
                            existingFilter.SearchType = filter.SearchType;
                        }

                        Log($"Filter {dbFilter.Name} saved successfully", Category.Info);
                    }                       
                        
                }
            }
        }

        private static Music.Data.Model.Filter CreateDbFilter(DjHorsifyFilterModel filter)
        {
            var searchTerm = $"{filter.SearchType.ToString()}:";
            var jointFilters = string.Join(";", filter.Filters);
            searchTerm += jointFilters;
            var dbFilter = new Music.Data.Model.Filter
            {
                Name = filter.FileName,
                SearchTerms = searchTerm,
                Id = filter.Id
            };
            return dbFilter;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        } 
        #endregion
    }
}
