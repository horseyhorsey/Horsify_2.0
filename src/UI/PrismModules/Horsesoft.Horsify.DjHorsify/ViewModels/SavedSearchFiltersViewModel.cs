using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    public class SavedSearchFiltersViewModel : HorsifyBindableBase
    {
        private IDjHorsifyService _djHorsifyService;
        private IRegionManager _regionManager;

        #region Commands

        public ICommand DeleteFilterCommand { get; private set; }
        public ICommand ExitViewCommand { get; private set; }
        public ICommand LoadSavedFilterCommand { get; private set; }
        public ICommand SearchFilterCommand { get; private set; } 
        #endregion

        public SavedSearchFiltersViewModel(IDjHorsifyService djHorsifyService, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _djHorsifyService = djHorsifyService;
            _regionManager = regionManager;

            DeleteFilterCommand = new DelegateCommand(async ()=> await OnDeleteFilterCommand());
            ExitViewCommand = new DelegateCommand(() => _regionManager.RequestNavigate(Regions.ContentRegion, "DjHorsifyView"));
            LoadSavedFilterCommand = new DelegateCommand(OnLoadSavedFilter);
            SearchFilterCommand = new DelegateCommand(OnRunSearchFilter);

            InitSavedFilters();
        }

        #region Properties
        private ObservableCollection<FiltersSearch> _savedFilters;
        public ObservableCollection<FiltersSearch> SavedFilters
        {
            get { return _savedFilters; }
            set { SetProperty(ref _savedFilters, value); }
        }

        private FiltersSearch _selectedFilter;
        public FiltersSearch SelectedFilter
        {
            get { return _selectedFilter; }
            set { SetProperty(ref _selectedFilter, value); }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Get saved filters from database
        /// </summary>
        private void InitSavedFilters()
        {            
            IEnumerable<FiltersSearch> filters = null;
            Task.Run(async () =>
            {
                filters = await _djHorsifyService.GetSavedSearchFilters();

            }).ContinueWith(x =>
            {
                if (filters != null)
                {
                    if (_djHorsifyService.SavedFilters == null)
                        _djHorsifyService.SavedFilters = new ObservableCollection<FiltersSearch>(filters);
                    else
                    {
                        _djHorsifyService.SavedFilters.AddRange(filters);
                    }

                    this.SavedFilters = _djHorsifyService.SavedFilters;
                }

            });
        }

        private async Task OnDeleteFilterCommand()
        {
            Log($"Deleting saved filters: {SelectedFilter ?.Name}");
            bool result = false;
            if (this.SelectedFilter?.Id > 0)
                result = await _djHorsifyService.DeleteFilterAsync(this.SelectedFilter?.Id);

            if (result)
            {
                Log("Removing search filter from UI list");
                _djHorsifyService.SavedFilters.Remove(SelectedFilter);
            }
                
        }

        private void OnLoadSavedFilter()
        {
            if (SelectedFilter == null)
            {
                Log("Cannot run search on null selection", Category.Warn);
                return;
            }

            var navParams = new NavigationParameters();
            navParams.Add("load_filter", SelectedFilter);
            _regionManager.RequestNavigate(Regions.ContentRegion, "DjHorsifyView", navParams);
        }

        /// <summary>
        /// Navigates to SearchedSongsView with the selected <see cref="FiltersSearch"/>
        /// </summary>
        private void OnRunSearchFilter()
        {
            if (SelectedFilter == null)
            {
                Log("Cannot run search on null selection", Category.Warn);
                return;
            }

            var navParams = new NavigationParameters();
            navParams.Add("djhorsify_search", SelectedFilter.ConvertToSearchFilter());
            _regionManager.RequestNavigate(Regions.ContentRegion, "SearchedSongsView", navParams);
        }
        #endregion
    }
}
