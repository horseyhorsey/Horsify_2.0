using Horsesoft.Horsify.DjHorsify.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.Model;
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
        private IDjHorsifyService _djHorsifyService;

        public ICollectionView AvailableSearchTerms { get; set; }

        #region Commands
        public ICommand AddSearchTermCommand { get; set; }
        public ICommand CloseViewCommand { get; set; }
        public ICommand DeleteFilterCommand { get; set; }        
        public ICommand RemoveSearchTermCommand { get; set; }
        public ICommand SaveFilterCommand { get; set; }
        #endregion

        #region Constructors
        public EditFilterViewModel(IDjHorsifyService djHorsifyService, IRegionManager regionManager, ILoggerFacade loggerFacade):base(loggerFacade)
        {
            _regionManager = regionManager;
            _djHorsifyService = djHorsifyService;

            SearchTerms = new ObservableCollection<string>();
            AvailableSearchTerms = new ListCollectionView(SearchTerms);

            AddSearchTermCommand = new DelegateCommand(OnAddSearchTerm);
            CloseViewCommand = new DelegateCommand(OnCancel);
            DeleteFilterCommand = new DelegateCommand(OnDeleteFilter);
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
            if (CurrentSearchTerm?.Length > 0)
            {
                if (!SearchTerms.Any(x => x == CurrentSearchTerm))
                    SearchTerms.Add(CurrentSearchTerm);
            }
        }

        /// <summary>
        /// Sends a delete_filter request to DjHorsifyView to delete the filter.
        /// </summary>
        private void OnDeleteFilter()
        {
            //Create params based on whether we were editing or not
            var navParams = new NavigationParameters();
            Log("Adding NEW Filter");
            //searchFilter.Id = -1;
            navParams.Add("delete_filter", this.CurrentFilter);
            _regionManager.RequestNavigate(Regions.ContentRegion, "DjHorsifyView", navParams);
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
                if (this.CurrentFilter.FileName?.Length > 0)
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
            Log("saving Filter: ");
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
                    Log("Adding NEW Filter");
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
                

                var djhModel = filter as DjHorsifyFilterModel;
                if (djhModel != null)
                {
                    var existingDjh =_djHorsifyService.HorsifyFilters.FirstOrDefault(x => x.FileName == djhModel.FileName);
                    if (existingDjh != null)
                    {
                        Log("Trying to create a new filter when already exists, loading that instead.");                        
                        this.CurrentFilter = SetEditPreviousFilter(existingDjh);
                        this.CurrentFilter.SearchType = djhModel.SearchType;
                        return;
                    }
                        

                    this.IsEditingFilter = false;
                    
                    SelectedSearchType = (SongFilterType)djhModel.SearchType;
                    CurrentFilter = djhModel;
                    SearchTerms.Clear();
                    Log($"Created new filter", Category.Warn, Priority.Medium);
                    return;
                }

                Log($"Failed creating new filter from DJHModel", Category.Warn, Priority.Medium);
            }

            filter = navigationContext.Parameters["edit_filter"];
            if (filter != null)
            {                
                this.CurrentFilter = SetEditPreviousFilter(filter);
                if (this.CurrentFilter != null)
                    this.lastFilter = this.CurrentFilter;
            }
        }

        private DjHorsifyFilterModel SetEditPreviousFilter(object filter)
        {
            Log($"Loading existing filter", Category.Debug, Priority.Medium);
            this.IsEditingFilter = true;
            var model = filter as DjHorsifyFilterModel;            
            if (model != null)
            {                
                this.SelectedSearchType = (SongFilterType)model.SearchType;
                this.SearchTerms.Clear();
                this.SearchTerms.AddRange(model.Filters);
            }
            else { Log($"Loading existing filter failed", Category.Warn, Priority.Medium); }

            return model;
        }
        #endregion
    }
}
