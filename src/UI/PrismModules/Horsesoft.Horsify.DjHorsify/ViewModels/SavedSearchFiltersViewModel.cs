using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    public class SavedSearchFiltersViewModel : HorsifyBindableBase
    {
        private IDjHorsifyService _djHorsifyService;
        private IRegionManager _regionManager;
        private IHorsifyDialogService _horsifyDialogService;

        #region Commands

        public ICommand DeleteFilterCommand { get; private set; }
        public ICommand CloseViewCommand { get; private set; }
        public ICommand LoadSavedFilterCommand { get; private set; }
        public ICommand SearchFilterCommand { get; private set; }

        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        #endregion

        public SavedSearchFiltersViewModel(IHorsifyDialogService horsifyDialogService, IDjHorsifyService djHorsifyService, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _djHorsifyService = djHorsifyService;
            _regionManager = regionManager;
            _horsifyDialogService = horsifyDialogService;

            DeleteFilterCommand = new DelegateCommand(OnDeleteFilterConfirm);
            CloseViewCommand = new DelegateCommand(() => _regionManager.RequestNavigate(Regions.ContentRegion, "DjHorsifyView"));
            LoadSavedFilterCommand = new DelegateCommand(OnLoadSavedFilter);
            SearchFilterCommand = new DelegateCommand(OnRunSearchFilter);
            ConfirmationRequest = new InteractionRequest<IConfirmation>();

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
                        _djHorsifyService.SavedFilters = new ObservableCollection<FiltersSearch>(filters.OrderBy(z => z.Name));
                    else
                    {
                        _djHorsifyService.SavedFilters.AddRange(filters.OrderBy(z => z.Name));
                    }

                    this.SavedFilters = _djHorsifyService.SavedFilters;
                }

            });
        }

        private void OnDeleteFilterConfirm()
        {
            _horsifyDialogService.Show("Delete Save DJH options", "Are you sure?", ConfirmationRequest, r =>
            {
                if (r.Confirmed)
                {
                    DeleteFilters();
                }
            });
        }

        private void DeleteFilters()
        {
            bool result = false;
            Task.Run(async () =>
            {
                result = await OnDeleteFilterAsync();
                if (result)
                {
                    Log("Removing search filter from UI list");

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _djHorsifyService.SavedFilters.Remove(SelectedFilter);
                    });
                }
            });
        }

        private Task<bool> OnDeleteFilterAsync()
        {           
            Log($"Deleting saved filters: {SelectedFilter ?.Name}");            
            if (this.SelectedFilter?.Id > 0)
                return _djHorsifyService.DeleteSearchFilterAsync(this.SelectedFilter?.Id);

            return Task.FromResult(false);                
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
            _regionManager.RequestNavigate(Regions.ContentRegion, ViewNames.SearchedSongsView, navParams);
        }
        #endregion
    }
}
