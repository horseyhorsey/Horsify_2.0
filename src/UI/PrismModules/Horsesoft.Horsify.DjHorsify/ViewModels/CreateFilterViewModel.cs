using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Linq;
using System.Windows.Input;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    /// <summary>
    /// This view model captures the SearchType from user to use in a filter
    /// </summary>
    public class CreateFilterViewModel : BindableBase, INavigationAware
    {
        private IRegionManager _regionManager;

        public DelegateCommand CreateFilterCommand { get; private set; }
        public ICommand NavigateBackCommand { get; private set; }

        public CreateFilterViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            CreateFilterCommand = new DelegateCommand(
                () => NavigateEditFilterView(this.SelectedSearchType),
                () => IsFilterNameValid());
            NavigateBackCommand = new DelegateCommand(OnNavigateBack);
        }

        private bool IsFilterNameValid()
        {
            if (this.FilterName?.Count() > 2)
                return true;

            return false;
        }

        #region Properties
        private SongFilterType _searchType;
        public SongFilterType SelectedSearchType
        {
            get { return _searchType; }
            set
            {
                SetProperty(ref _searchType, value);
            }
        }

        private string _filterName;
        public string FilterName
        {
            get { return _filterName; }
            set
            {
                if (SetProperty(ref _filterName, value))
                {
                    this.CreateFilterCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Navigates the edit filter view. Passing in create_new_filter into params
        /// </summary>
        /// <param name="selectedSearchType">Type of the selected search.</param>
        private void NavigateEditFilterView(SongFilterType selectedSearchType)
        {
            var djhModel = new DjHorsifyFilterModel() { FileName = this.FilterName, SearchType = (SearchType)SelectedSearchType };
            var navParams = new NavigationParameters();
            navParams.Add("create_new_filter", djhModel);
            _regionManager.RequestNavigate(Regions.ContentRegion, "EditFilterView", navParams);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SelectedSearchType = SongFilterType.Genre;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        private void OnNavigateBack()
        {
            //On the presumption that we have come from the Dj Horsify View
            _regionManager
                .RequestNavigate("ContentRegion", "DjHorsifyView");
        } 
        #endregion
    }
}
