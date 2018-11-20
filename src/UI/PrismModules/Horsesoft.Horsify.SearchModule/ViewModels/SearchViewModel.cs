using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Helpers;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public interface ISearchViewModel
    {
        ICommand CloseSearchViewCommand { get; set; }
    }

    public class SearchViewModel : HorsifyBindableBase
    {
        private IRegionManager _regionManager;
        #region Commands
        public ICommand CloseSearchViewCommand { get; set; }
        public ICommand RunSearchCommand { get; set; }
        #endregion

        #region Constructors

        public SearchViewModel(IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {

            _regionManager = regionManager;
            CloseSearchViewCommand = new DelegateCommand(() =>
            {
                //eventAggregator.GetEvent<OnNavigateViewEvent<string>>()
                //    .Publish("SearchedSongsView");

                regionManager.RequestNavigate("ContentRegion", "SearchedSongsView");
            });

            RunSearchCommand = new DelegateCommand(OnRunSearch);
        }

        private void OnRunSearch()
        {
            var filter = new SearchFilter(SearchText);
            var navparams = NavigationHelper.CreateSearchFilterNavigation(filter);
            _regionManager.RequestNavigate(Regions.ContentRegion, "SearchedSongsView", navparams);
        }
        #endregion

        private string _searchText;
        /// <summary>
        /// Gets or Sets the SearchText
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }
    }
}
