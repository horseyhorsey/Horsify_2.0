using Horsesoft.Horsify.DjHorsify.Model;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Helpers;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.Model;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

    /*
//TODO: Musical key: This is will have to be separated
Set a starting key
Song Amount (for DJ sets)
Set whether to get all major/minor from starting key or include both
Set to choose whether user can pick the same key instead of moving around each song.
Set to just pick songs in that key.
Rules:
Should go round the open key clock picking songs, clockwise.
    - Can only go to the relative Major/Minor Key , eg not diagonal...can jump two spaces
    - Speeds must be similar with a 1-2 BPM range
*/

    public class DjHorsifyViewModel : HorsifyBindableBase, INavigationAware, IDjHorsifyViewModel
    {
        private IRegionManager _regionManager;
        private IDjHorsifyService _djHorsifyService;

        #region Commands        
        public ICommand CreateFilterCommand { get; set; }
        public ICommand ClearSelectionsCommand { get; set; }
        public ICommand EditFilterCommand { get; set; }
        public ICommand RunSearchCommand { get; set; }
        public ICommand RunSingleSearchCommand { get; set; }
        public ICommand SaveFilterCommand { get; set; }        
        public ICommand ShowSavedFiltersCommand { get; set; }
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
                //Generate the the filtering views collection
                CreateFilterViews();
            }
            catch (Exception ex)
            {
                Log($"Error Loading DjHorsify : {ex.Message}", Category.Exception);
                throw;
            }

            #region Commands
            CreateFilterCommand = new DelegateCommand(OnCreateFilter);
            ClearSelectionsCommand = new DelegateCommand(ClearSelections);
            EditFilterCommand = new DelegateCommand<DjHorsifyFilterModel>(OnEditFilter);
            RunSearchCommand = new DelegateCommand(OnRunSearch);
            RunSingleSearchCommand = new DelegateCommand<DjHorsifyFilterModel>(OnRunSearch);
            SaveFilterCommand = new DelegateCommand(OnSaveFilter);
            ShowSavedFiltersCommand = new DelegateCommand(OnShowSavedFilters);
            #endregion
        }

        #region Properties              

        private ICollectionView _availableFiltersView;
        public ICollectionView AvailableFiltersView
        {
            get { return _availableFiltersView; }
            set { SetProperty(ref _availableFiltersView, value); }
        }

        private ICollectionView _IncludedFiltersView;
        public ICollectionView IncludedFiltersView
        {
            get { return _IncludedFiltersView; }
            set { SetProperty(ref _IncludedFiltersView, value); }
        }

        private ICollectionView _excludedFiltersView;
        public ICollectionView ExcludedFiltersView
        {
            get { return _excludedFiltersView; }
            set { SetProperty(ref _excludedFiltersView, value); }
        }

        private ICollectionView _includedAndFiltersView;
        public ICollectionView IncludedAndFiltersView
        {
            get { return _includedAndFiltersView; }
            set { SetProperty(ref _includedAndFiltersView, value); }
        }

        private DjHorsifyOption _djHorsifyOption;
        public DjHorsifyOption DjHorsifyOption
        {
            get { return _djHorsifyOption; }
            set { SetProperty(ref _djHorsifyOption, value); }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates all Collections for AND/OR/NOT and Available
        /// </summary>
        private void CreateFilterViews()
        {

            AvailableFiltersView = new MyCollectionView(_djHorsifyService.HorsifyFilters);
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

            IncludedFiltersView = new MyCollectionView(_djHorsifyService.HorsifyFilters);
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

            IncludedAndFiltersView = new MyCollectionView(_djHorsifyService.HorsifyFilters);
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

            ExcludedFiltersView = new MyCollectionView(_djHorsifyService.HorsifyFilters);
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
        }

        /// <summary>
        /// Generates the horsify filters from database filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        private IEnumerable<DjHorsifyFilterModel> GenerateHorsifyFilters(IEnumerable<Music.Data.Model.Filter> filters)
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

        /// <summary>
        /// 
        /// </summary>
        private void GenerateHorsifyFilters()
        {
            var horsifyFilters = GenerateHorsifyFilters(_djHorsifyService.Filters);
            if (horsifyFilters?.Count() > 0)
            {
                //If null create the collection and populate it, otherwise addrange
                if (_djHorsifyService.HorsifyFilters == null)
                {
                    _djHorsifyService.HorsifyFilters = new ObservableCollection<DjHorsifyFilterModel>(horsifyFilters);
                }
                else
                {
                    _djHorsifyService.HorsifyFilters.AddRange(horsifyFilters);
                }
            }
        }

        private void ClearSelections()
        {
            foreach (var item in _djHorsifyService.HorsifyFilters.Where(x => x.SearchAndOrOption != SearchAndOrOption.None))
            {
                item.SearchAndOrOption = SearchAndOrOption.None;
            }
        }

        private void OnEditFilter(DjHorsifyFilterModel filter)
        {
            Log($"Editing previous filter: {filter?.FileName}");

            var navParams = new NavigationParameters();
            navParams.Add("edit_filter", filter);
            _regionManager.RequestNavigate(Regions.ContentRegion, "EditFilterView", navParams);
        }

        /// <summary>
        /// Runs a search with all the DJ Horsify Options
        /// </summary>
        private void OnRunSearch()
        {
            Log("Running search view DJHorsify", Category.Debug);
            if (!this.DjHorsifyOption.IsOptionsValid())
            {
                Log("The DJ Horsify Options are not enabled or incorrect to run searches", Category.Warn);
                return;
            }

            try
            {
                //Reset the selected keys if not enabled and have any selected.
                if (!this.DjHorsifyOption.HarmonicEnabled)
                    this.DjHorsifyOption.SelectedKeys = Music.Data.Model.Import.OpenKeyNotation.None;

                var searchFilter = _djHorsifyService.GenerateSearchFilter(this.DjHorsifyOption);
                if (searchFilter == null)
                    throw new NullReferenceException("Generating search filter from Dj Horsify option failed.");

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

        /// <summary>
        /// This is for the single searches on each filter box
        /// </summary>
        /// <param name="filter"></param>
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

        private void RefreshFilterViews()
        {
            Log("Refreshing filters.");
            AvailableFiltersView.Refresh();
            IncludedFiltersView.Refresh();
            IncludedAndFiltersView.Refresh();
            ExcludedFiltersView.Refresh();
        }

        private void OnSaveFilter()
        {
            _regionManager.RequestNavigate("ContentRegion", "SaveSearchFilterView");
        }

        private void OnShowSavedFilters()
        {
            _regionManager.RequestNavigate("ContentRegion", "SavedSearchFiltersView");
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

                var existingFilter = this._djHorsifyService.Filters.FirstOrDefault(x => x.Name == filter.FileName);
                bool update = existingFilter == null ? false: true;
                try
                {
                    bool result = false;
                    Task.Run(async () =>
                    {
                        if (update)
                        {
                            Log($"Updating filter {dbFilter.Name}");
                            existingFilter.SearchTerms = dbFilter.SearchTerms;
                            result = _djHorsifyService.UpdateFilter(dbFilter);
                        }
                        else
                        {
                            Log($"Adding new filter {dbFilter.Name}");
                            result = await _djHorsifyService.AddFilterAsync(dbFilter);
                        }                            
                            
                    }).Wait();

                    if (result)
                    {
                        Log("Successfully added filter to database");
                        _djHorsifyService.HorsifyFilters.Add(filter);                        
                        CreateFilterViews();
                    }
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

                return;
            }

            filter = navigationContext.Parameters["edited_filter"] as DjHorsifyFilterModel;
            if (filter != null)
            {
                Music.Data.Model.Filter dbFilter = CreateDbFilter(filter);
                var result = _djHorsifyService.UpdateFilter(dbFilter);
                if (!result)
                    Log("Couldn't update service filters", Category.Debug);
                else
                {
                    var existingFilter = _djHorsifyService.HorsifyFilters.FirstOrDefault(x => x.Id == filter.Id);
                    if (existingFilter != null)
                        RefreshFilterViews();
                    //if (existingFilter != null)
                    //{
                    //    existingFilter.FileName = filter.FileName;
                    //    existingFilter.Filters = filter.Filters;
                    //    existingFilter.SearchAndOrOption = filter.SearchAndOrOption;
                    //    existingFilter.SearchType = filter.SearchType;
                    //}

                    Log($"Filter {dbFilter.Name} saved successfully", Category.Info);
                }
                return;
            }

            filter = navigationContext.Parameters["delete_filter"] as DjHorsifyFilterModel;
            if (filter != null)
            {
                Music.Data.Model.Filter dbFilter = CreateDbFilter(filter);
                bool result = false;
                Task.Run(async () =>
                {
                    result = await _djHorsifyService.DeleteFilterAsync(dbFilter);
                }).Wait();

                if (result)
                {
                    var inListFilter = _djHorsifyService.Filters.FirstOrDefault(x => x.Name == dbFilter.Name);
                    if (inListFilter != null)
                    {                        
                        var deleted = _djHorsifyService.Filters.Remove(inListFilter);
                        filter.SearchAndOrOption = SearchAndOrOption.None;
                        _djHorsifyService.HorsifyFilters.Remove(filter);
                        Log($"Deleted filter from database: {deleted}");
                    }
                    else
                    {
                        Log($"Couldn't find filter to remove");
                    }

                }
                else
                    Log("Couldn't delete filter from database", Category.Warn);

                return;
            }

            

            var savedfilter = navigationContext.Parameters["load_filter"] as FiltersSearch;
            if (savedfilter != null)
            {
                ClearSelections();
                Log("Loading saved filter into DJ Horsify");

                var searchFilter = savedfilter.ConvertToSearchFilter();
                if (searchFilter != null)
                {
                    //Set ranges
                    this.DjHorsifyOption.BpmRange = searchFilter.BpmRange;
                    this.DjHorsifyOption.RatingRange = searchFilter.RatingRange;

                    //Set up music keys TODO: not updating UI after load
                    if (!searchFilter.MusicKeys.Contains("None"))
                    {
                        this.DjHorsifyOption.SelectedKeys = DjHorsifyOption.ConvertKeys(searchFilter.MusicKeys);
                        this.DjHorsifyOption.HarmonicEnabled = true;
                    }
                    else
                        this.DjHorsifyOption.HarmonicEnabled = false;

                    //Set amount
                    if (savedfilter.RandomAmount.Value > 0)
                        this.DjHorsifyOption.Amount = savedfilter.RandomAmount.Value;

                    if (searchFilter?.Filters?.Count() > 0)
                    {
                        //Go over each filename in filters and set the SearchAndOrOption type making the list update.
                        foreach (var item in searchFilter?.Filters)
                        {
                            var existingFilter = _djHorsifyService.HorsifyFilters.FirstOrDefault(x => x.FileName == item.FileName);
                            if (existingFilter == null)
                                Log("Couldn't find existing filter whilst loading saved.");
                            else
                                existingFilter.SearchAndOrOption = item.SearchAndOrOption;
                        }
                    }
                    
                    return;
                }

                Log("Couldn't load saved filter", Category.Warn);
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
