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
using System.Linq;
using System.Windows.Input;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class AToZSearchViewModel : HorsifyBindableBase
    {
        private IHorsifySongApi _horsifySongApi;
        private IRegionManager _regionManager;

        public ICommand ClearSelectedCommand { get; set; }
        public ICommand SelectResultItemCommand { get; set; }        
        public ICommand RunSearchCommand { get; set; }
        public ICommand RemoveSelectedFilter { get; set; }

        public AToZSearchViewModel(IHorsifySongApi horsifySongApi, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _horsifySongApi = horsifySongApi;
            _regionManager = regionManager;

            SelectResultItemCommand = new DelegateCommand<object[]>(OnSelectResultItem);

            ClearSelectedCommand = new DelegateCommand(OnClearSelected);

            RemoveSelectedFilter = new DelegateCommand<object[]>(OnRemoveSelected);
            RunSearchCommand = new DelegateCommand(OnRunSearch);
        }

        private void OnRemoveSelected(object[] selectedFilters)
        {
            if (selectedFilters?.Length > 0)
            {
                var filter = selectedFilters[0] as HorsifyFilter;
                Log("Removing selected filter:" + filter.SearchType, Category.Debug);
                this.SelectedFilters.Remove(filter);
            }
        }

        private SearchFilter _searchFilter;
        private void OnRunSearch()
        {
            Log("Running A-Z search: ", Category.Debug);
            _searchFilter = new SearchFilter();
            _searchFilter.Filters = new List<HorsifyFilter>(SelectedFilters);

            Log($"Selected Filters count: {SelectedFilters.Count}", Category.Debug);

            var navParams = NavigationHelper.CreateSearchFilterNavigation(_searchFilter);
            Log($"Navigating search view: ", Category.Debug);
            _regionManager.RequestNavigate(Regions.ContentRegion, ViewNames.SearchedSongsView, navParams);

            SearchResults.Clear();
            this.SelectedCharachter = null;
        }

        private void OnClearSelected()
        {
            this.SelectedFilters.Clear();
            this.SelectedCharachter = null;
        }

        //Final Search filter
        //SearchFilter filter = new SearchFilter();

        private void OnSelectResultItem(object[] selectedItems)
        {
            if (selectedItems?.Length > 0)
            {
                var result = (string)selectedItems[0];

                if (!SelectedFilters.Any(x => x.Filters.Contains(result)))
                {
                    var filter = new HorsifyFilter()
                    {
                        SearchType = SearchType,
                        Filters = new System.Collections.Generic.List<string>()
                        {
                            result
                        }
                    };

                    SelectedFilters.Add(filter);
                    //this.SelectedFilters.Add(result);            
                }
            }
        }

        private char?_selectedChar;
        public char? SelectedCharachter
        {
            get { return _selectedChar; }
            set
            {
                var changed = SetProperty(ref _selectedChar, value);

                if (SelectedCharachter != null)
                {
                    if (changed)
                    {
                        SearchResults.Clear();

                        if (SearchType == SearchType.All)
                            SearchType = SearchType.Artist;

                        SearchResults.AddRange(PopulateTableResults());                        
                    }
                }                
            }
        }

        private IEnumerable<string> PopulateTableResults()
        {
            return _horsifySongApi.GetEntries(SearchType, (char)SelectedCharachter);
        }

        private string _selectedSearchType;
        public string SelectedSearchType
        {
            get { return _selectedSearchType; }
            set
            {
                bool changed = SetProperty(ref _selectedSearchType, value);
                if (changed)
                {
                    SearchType type = SearchType.Artist;
                    Enum.TryParse(SelectedSearchType, out type);
                    SearchType = type;
                    SelectedCharachter = null;
                    SearchResults.Clear();
                }
            }
        }

        private SearchType _searchType = SearchType.Artist;
        public SearchType SearchType
        {
            get { return _searchType; }
            set
            {
                bool changed = SetProperty(ref _searchType, value);

                //if (changed)
                //    SelectedSearchType = null;
            }
        }

        private ObservableCollection<HorsifyFilter> _selectedFilters = new ObservableCollection<HorsifyFilter>();
        public ObservableCollection<HorsifyFilter> SelectedFilters
        {
            get { return _selectedFilters; }
            set { SetProperty(ref _selectedFilters, value); }
        }

        private ObservableCollection<string> _searchResults = new ObservableCollection<string>();
        public ObservableCollection<string> SearchResults
        {
            get { return _searchResults; }
            set { SetProperty(ref _searchResults, value); }
        }
    }
}
