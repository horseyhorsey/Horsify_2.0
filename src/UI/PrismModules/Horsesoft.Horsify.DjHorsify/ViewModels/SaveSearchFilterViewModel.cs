using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    /// <summary>
    /// View model to save DJ Horsify selection filters
    /// </summary>
    public class SaveSearchFilterViewModel : HorsifyBindableBase
    {
        #region Commands
        public ICommand CloseViewCommand { get; private set; }
        public ICommand SaveFilterCommand { get; private set; }
        #endregion

        private IDjHorsifyService _djHorsifyService;
        private IRegionManager _regionManager;

        #region Constructors
        public SaveSearchFilterViewModel(IDjHorsifyService djHorsifyService, IEventAggregator eventAggregator, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _djHorsifyService = djHorsifyService;
            _regionManager = regionManager;
            OnScreenKeyboardViewModel = new OnScreenKeyboardViewModel();

            CloseViewCommand = new DelegateCommand(OnCloseView);
            SaveFilterCommand = new DelegateCommand(async () => await OnSaveFilterAsync());
        }

        #endregion

        #region Properties
        private OnScreenKeyboardViewModel _OnScreenKeyboardViewModel;
        /// <summary>
        /// Gets or Sets the OnScreenKeyboardViewModel
        /// </summary>
        public OnScreenKeyboardViewModel OnScreenKeyboardViewModel
        {
            get { return _OnScreenKeyboardViewModel; }
            set { SetProperty(ref _OnScreenKeyboardViewModel, value); }
        }
        #endregion

        #region Private Methods

        private void OnCloseView()
        {
            _regionManager.RequestNavigate(Regions.ContentRegion, "DjHorsifyView");
        }

        private async Task OnSaveFilterAsync()
        {
            if (_djHorsifyService.SavedFilters == null)
                _djHorsifyService.SavedFilters = new System.Collections.ObjectModel.ObservableCollection<FiltersSearch>();

            //Check if already have a savedfilter and update it
            var filter = _djHorsifyService.SavedFilters.FirstOrDefault(x => x.Name == OnScreenKeyboardViewModel.SearchText);
            if (filter != null)
            {
                var id  = filter.Id;
                Log($"Attempting to update FiltersSearch for : {filter.Name}");
                var tempFilter = CreateFilterSearch();

                filter.Id = id;
                filter.SearchFilterContent = tempFilter.SearchFilterContent;
                filter.MaxAmount = tempFilter.MaxAmount;
                filter.RandomAmount = tempFilter.RandomAmount;

                bool result = await _djHorsifyService.UpdateSearchFilterAsync(filter);
                if (result)
                {
                    Log($"FilterSearch updated: {filter.Name}");
                    OnCloseView();
                }
                else
                {
                    Log($"Failed to update saved filter {filter?.Name}");
                }
            }
            else //Insert a new saved filter
            {
                Log("Attempting to save FiltersSearch");
                FiltersSearch newFilter = CreateFilterSearch();
                Log("Generated searchfilter");

                Log("Adding filter Async");                
                var result = await _djHorsifyService.AddSavedSearchFilterAsync(newFilter);
                if (result)
                {
                    _djHorsifyService.SavedFilters.Add(newFilter);
                    Log($"FilterSearch added to database under: {newFilter.Name}");
                    OnCloseView();
                }
                else
                {
                    Log($"Failed to add saved filter {newFilter?.Name}");
                }

            }
        }

        private FiltersSearch CreateFilterSearch()
        {
            var searchFilter = _djHorsifyService.GenerateSearchFilter(_djHorsifyService.DjHorsifyOption);

            var content = Newtonsoft.Json.JsonConvert.SerializeObject(searchFilter);
            var newFilter = new FiltersSearch()
            {
                MaxAmount = -1,
                RandomAmount = _djHorsifyService.DjHorsifyOption.Amount,
                Name = OnScreenKeyboardViewModel.SearchText,
                SearchFilterContent = content
            };
            return newFilter;
        }
        #endregion
    }
}
