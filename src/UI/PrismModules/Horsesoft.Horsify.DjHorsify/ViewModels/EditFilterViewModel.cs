using Horsesoft.Horsify.DjHorsify.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    public class EditFilterViewModel : HorsifyBindableBase, INavigationAware
	{
        private IRegionManager _regionManager;
        public ICollectionView AvailableSearchTerms { get; set; }

        #region Commands
        public ICommand AddSearchTermCommand { get; set; }
        public ICommand CancelCommand { get; set; }        
        public ICommand RemoveSearchTermCommand { get; set; }
        public ICommand SaveFilterCommand { get; set; }
        #endregion

        #region Constructors
        public EditFilterViewModel(IRegionManager regionManager, ILoggerFacade loggerFacade):base(loggerFacade)
        {
            _regionManager = regionManager;

            SearchTerms = new ObservableCollection<string>();
            AvailableSearchTerms = new ListCollectionView(SearchTerms);

            AddSearchTermCommand = new DelegateCommand(OnAddSearchTerm);
            CancelCommand = new DelegateCommand(OnCancel);
            RemoveSearchTermCommand = new DelegateCommand(OnRemoveSearchTerm);
            SaveFilterCommand = new DelegateCommand(OnSaveFilter);
        }

        #endregion

        #region Properties

        private DjHorsifyFilterModel lastFilter;

        private DjHorsifyFilterModel _currentFilter;
        public DjHorsifyFilterModel CurrentFilter
        {
            get { return _currentFilter; }
            set { SetProperty(ref _currentFilter, value); }
        }

        private string _currentSearchTerm;
        public string CurrentSearchTerm
        {
            get { return _currentSearchTerm; }
            set { SetProperty(ref _currentSearchTerm, value); }
        }

        private SongFilterType _searchType;
        public SongFilterType SelectedSearchType
        {
            get { return _searchType; }
            set
            {
                SetProperty(ref _searchType, value);
            }
        }

        private ObservableCollection<string> _searchTerms;        
        public ObservableCollection<string> SearchTerms
        {
            get { return _searchTerms; }
            set { SetProperty(ref _searchTerms, value); }
        }

        private bool IsEditingFilter;
        #endregion

        #region Private Methods

        private void OnCancel()
        {
            Log("Returning to DJHorsify");
            _regionManager.RequestNavigate(Regions.ContentRegion, "DjHorsifyView");
        }

        private void OnAddSearchTerm()
        {
            if (CurrentSearchTerm?.Length > 3)
            {
                if (!SearchTerms.Any(x => x == CurrentSearchTerm))
                    SearchTerms.Add(CurrentSearchTerm);
            }
        }

        private void OnRemoveSearchTerm()
        {
            var selectedSearchTerm = AvailableSearchTerms.CurrentItem as string;
            if (selectedSearchTerm != null)
            {
                if (SearchTerms.Any(x => x == selectedSearchTerm))
                    SearchTerms.Remove(selectedSearchTerm);
            }
        }

        private void OnSaveFilter()
        {
            try
            {
                if (this.CurrentFilter.FileName?.Length > 2)
                {
                    SaveFilter();
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message, Category.Exception, Priority.High);
                throw;
            }
        }

        private void SaveFilter()
        {
            if (this.SearchTerms.Count > 0)
            {                
                //Create the filters or clear existing
                if (this.CurrentFilter.Filters == null)
                    this.CurrentFilter.Filters = new System.Collections.Generic.List<string>();
                else                    
                    this.CurrentFilter.Filters.Clear();
                
                //Add filters and search type
                this.CurrentFilter.Filters.AddRange(SearchTerms);
                this.CurrentFilter.SearchType = (SearchType)Enum.Parse(typeof(SearchType), SelectedSearchType.ToString());

                //Create params based on whether we were editing or not
                var navParams = new NavigationParameters();
                if (!this.IsEditingFilter)
                {
                    //searchFilter.Id = -1;
                    navParams.Add("add_new_filter", this.CurrentFilter);
                }
                else
                {
                    //searchFilter.Filter = lastFilter.;
                    navParams.Add("edited_filter", this.CurrentFilter);
                }

                _regionManager.RequestNavigate(Regions.ContentRegion, "DjHorsifyView", navParams);
            }
        }

        #endregion

        #region Navigation Interface
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        /// <summary>
        /// Opens Create or Edit Filter Views
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var filter = navigationContext.Parameters["create_new_filter"];
            if (filter != null)
            {
                Log($"Creating new filter", Category.Debug, Priority.Medium);
                this.IsEditingFilter = false;                
                SearchTerms.Clear();
                SelectedSearchType = (SongFilterType)filter;
                this.CurrentFilter = new DjHorsifyFilterModel();
                this.lastFilter = this.CurrentFilter;
                return;
            }

            filter = navigationContext.Parameters["edit_filter"];
            if (filter != null)
            {
                this.IsEditingFilter = true;
                Log($"Loading existing filter", Category.Debug, Priority.Medium);                

                this.CurrentFilter = filter as DjHorsifyFilterModel;
                if (this.CurrentFilter != null)
                {
                    this.lastFilter = this.CurrentFilter;                                        
                    this.SelectedSearchType = (SongFilterType)this.CurrentFilter.SearchType;
                    this.SearchTerms.Clear();
                    this.SearchTerms.AddRange(this.CurrentFilter.Filters);
                }
                else { Log($"Loading existing filter failed", Category.Warn, Priority.Medium); }
            }
        }
        #endregion
    }
}
