using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows.Input;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    /// <summary>
    /// This view model captures the SearchType from user to use in a filter
    /// </summary>
    public class CreateFilterViewModel : BindableBase, INavigationAware
    {
        private IRegionManager _regionManager;
        public ICommand NavigateBackCommand { get; private set; }

        public CreateFilterViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            NavigateBackCommand = new DelegateCommand(OnNavigateBack);
        }

        #region Properties
        private string _searchType;
        public string SelectedSearchType
        {
            get { return _searchType; }
            set
            {
                SetProperty(ref _searchType, value);

                //Convert the string selection to SongFilterType
                if (!string.IsNullOrEmpty(SelectedSearchType))
                    NavigateEditFilterView((SongFilterType)Enum.Parse(typeof(SongFilterType), SelectedSearchType));
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
            var navParams = new NavigationParameters();
            navParams.Add("create_new_filter", selectedSearchType);
            _regionManager.RequestNavigate(Regions.ContentRegion, "EditFilterView", navParams);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SelectedSearchType = null;
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
