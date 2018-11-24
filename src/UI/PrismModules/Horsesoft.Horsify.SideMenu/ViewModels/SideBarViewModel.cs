using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Data.Model.Menu;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Helpers;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Horsesoft.Horsify.SideMenu.ViewModels
{
    public interface ISideBarViewModel
    {
        ICommand MenuHomeCommand { get; set; }
        ICommand OpenSearchCommand { get; set; }
        ICommand NavigateDjHorsifyCommand { get; set; }
        ICollectionView SearchButtonsView { get; set; }
    }

    public class SideBarViewModel : SearchListBaseViewModel, ISideBarViewModel
    {
        #region Commands
        public ICommand MenuHomeCommand { get; set; }
        public ICommand NavigateDjHorsifyCommand { get; set; }
        public ICommand OpenSearchCommand { get; set; }
        public ICommand NavigateCommand { get; set; }
        public ICommand SelectMenuCommand { get; set; }
        #endregion

        #region Menu Items
        private MenuCreator mCreator;
        private IMenuComponent _previousMenu;
        public ICollectionView SearchButtonsView { get; set; }
        #endregion

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private readonly string _menuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Horsify\\Menus\\";

        #region Constructors
        public SideBarViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            Log("Loading SideBar..", Category.Debug, Priority.None);

            //Composite Menus and MenuItems
            mCreator = new MenuCreator();

            //Builds all menu items from the _rootMenu
            SearchButtons = new ObservableCollection<ISearchButtonViewModel>(GenerateSearchButtonsForMenuComponent(mCreator._rootMenu));

            eventAggregator.GetEvent<HorsifySearchCompletedEvent>().Subscribe(() => IsBusy = false);

            #region Commands Setup
            MenuHomeCommand = new DelegateCommand(() =>
            {
                //Build items from the _rootMenu
                SearchButtons.Clear();
                SearchButtons.AddRange(GenerateSearchButtonsForMenuComponent(mCreator._rootMenu));
            });

            NavigateDjHorsifyCommand = new DelegateCommand(() => _regionManager.RequestNavigate(Regions.ContentRegion, "DjHorsifyView"));
            //OPEN SEARCH
            OpenSearchCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<OnNavigateViewEvent<string>>()
                .Publish("InstantSearch");
            });

            NavigateCommand = new DelegateCommand<string>(OnNavigateView);

            if (SelectMenuCommand == null)
            {
                SelectMenuCommand = new DelegateCommand<SearchButtonViewModel>(async (sbm)=> await SelectMenuAsync(sbm));
            }
            #endregion
        }

        #endregion

        #region Support Methods

        /// <summary>
        /// Creates a SearchbuttonViewModel
        /// </summary>
        /// <param name="menuiterator"></param>
        private SearchButtonViewModel AssignSearchButtons(IEnumerator<IMenuComponent> menuiterator)
        {
            return new SearchButtonViewModel(menuiterator.Current, _loggerFacade)
            {
                SearchTitle = menuiterator.Current.Name,
                ImagePath   =  _menuPath + menuiterator.Current.Image
            };
        }

        /// <summary>
        /// Generates Search Buttons For Menu Component
        /// </summary>
        private IList<SearchButtonViewModel> GenerateSearchButtonsForMenuComponent(IMenuComponent menuComponent)
        {
            //Get all items if this is a menu
            if (menuComponent.GetType() == typeof(Menu))
            {
                var menuiterator = menuComponent.GetIterator();
                return CreateMenuButtons(menuiterator);
            }

            Log("Not type of Menu", Category.Warn);
            return null;
        }

        private IList<SearchButtonViewModel> CreateMenuButtons(IEnumerator<IMenuComponent> iterator)
        {
            IList<SearchButtonViewModel> buttons = new List<SearchButtonViewModel>();
            while (iterator.MoveNext())
            {
                string imgLocation =  _menuPath + iterator.Current.Image;
                //imgLocation = File.Exists(imgLocation) ? imgLocation : null;
                buttons.Add(new SearchButtonViewModel(iterator.Current, _loggerFacade)
                {
                    SearchTitle = iterator.Current.Name,
                    ImagePath = imgLocation
                });
            }

            return buttons;
        }

        /// <summary>
        /// Gets all the buttons that are in the root menu
        /// </summary>
        private IList<SearchButtonViewModel> GenerateSearchButtonsFromRoot()
        {
            Log($"Generating Root menu", Category.Info, Priority.None);

            var menuiterator = mCreator._rootMenu.GetIterator();
            var buttons = CreateMenuButtons(menuiterator);
            return buttons;
        }

        private void OnNavigateView(string viewName)
        {
            Log($"Navigating: {viewName}", Category.Info, Priority.None);
            _regionManager.RequestNavigate(Regions.ContentRegion, viewName);
        }

        /// <summary>
        /// Updates the UI's search buttons
        /// </summary>
        /// <param name="x"></param>
        private void UpdateSearchButtons(IMenuComponent menuComponent)
        {
            Log("Updating menu items.", Category.Debug, Priority.None);

            var searchButtonVms = AddSearchButtons(menuComponent);

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    SearchButtons.Clear();
                    foreach (var item in searchButtonVms)
                    {
                        SearchButtons.Add(item);
                    }
                    
                }
                catch (Exception ex)
                {
                    Log(ex.Message, Category.Exception, Priority.High);
                    throw;
                }
            });
        }

        /// <summary>
        /// Get previous menu, create a back button and child items
        /// </summary>
        /// <param name="x"></param>
        private IList<SearchButtonViewModel> AddSearchButtons(IMenuComponent menuComponent)
        {
            //If in menu generate the child items
            if (menuComponent.Name != "Back")
            {
                var menuiterator = menuComponent.GetIterator();
                var buttons = new List<SearchButtonViewModel>();
                //Add back and search buttons
                while (menuiterator.MoveNext())
                {
                    buttons.Add(AssignSearchButtons(menuiterator));
                }
                                
                //Assing this menu to the last menu?
                _previousMenu = menuComponent;

                return buttons;
            }
            else
            {
                if (menuComponent.Name == "Back")
                {
                    Log("Go Back", Category.Info, Priority.None);

                    //If parent null generate from the root menu
                    if (menuComponent.Parent == null) { return GenerateSearchButtonsFromRoot(); }

                    //Get menu items for previous parent menu
                    return this.GenerateSearchButtonsForMenuComponent(menuComponent.Parent);

                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Select a menu button
        /// </summary>
        /// <returns></returns>
        private void SelectMenu(SearchButtonViewModel searchButtonViewModel)
        {
            if (IsBusy) { Log($"Searching busy..."); return; }

            var menuComponent = searchButtonViewModel.MenuComponent;
            if (menuComponent.GetType() == typeof(Menu))
            {
                Log("Menu Selected", Category.Debug);

                //Check if has children and return if has children
                if (menuComponent.GetChild(0) != null)
                {                    
                    this.UpdateSearchButtons(menuComponent);
                    return;
                }
            }

            //Treat as MenuItem
            var menuName = menuComponent?.Name;
            //Return back if menu is Back or IsBusy
            if (menuName == "Back")
            {
                if (menuComponent.Parent.Name == "Root")
                    this.UpdateSearchButtons(this.mCreator._rootMenu);
                else
                    this.UpdateSearchButtons(menuComponent.Parent);
                return;
            }


            NavigationParameters navParams = null;
            string viewName = string.Empty;
            if (menuComponent?.ExtraSearchType != ExtraSearchType.None)
            {
                IsBusy = true;
                Log($"Navigating SearchedSongs with extra search type");
                navParams = new NavigationParameters();
                navParams.Add("extra_search", menuComponent.ExtraSearchType);
                viewName = ViewNames.SearchedSongsView;
            }
            else if (menuComponent?.SearchString == "SEARCH")
            {
                switch (menuName)
                {
                    case "A-Z":
                        viewName = ViewNames.AToZSearchView;
                        break;
                    case "SEARCH":
                        viewName = ViewNames.SearchView;
                        break;
                    case "SONG SEARCH":
                        viewName = ViewNames.InstantSearch;
                        break;
                    default:
                        break;
                }
            }
            else if (menuName == "DJ Horsify" || menuName == "Filter" || menuName == "Database Stats")
            {
                if (menuName == "DJ Horsify")
                {
                    viewName = ViewNames.DjHorsifyView;
                }
                else if (menuName == "Filter")
                {                    
                    viewName = ViewNames.FilterCreatorView;
                }
            }
            else
            {                
                navParams = NavigationHelper
                    .CreateSearchFilterNavigation(menuComponent.SearchType, 
                    menuComponent.SearchString != null ? menuComponent.SearchString : menuComponent.Name);

                viewName = ViewNames.SearchedSongsView;                
            }

            Log($"Navigating{viewName}");
            if (navParams != null)
            {
                Log($"View params available, setting sidebar busy.");
                IsBusy = true;
            }

            //Request nav on the main thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                _regionManager.RequestNavigate(Regions.ContentRegion, viewName, navParams);                
            });

        }

        private async Task SelectMenuAsync(SearchButtonViewModel sbm)
        {
            Log($"Selected menu {sbm?.SearchTitle}");
            
            await Task.Run(() =>
            {
                this.SelectMenu(sbm);
            });
        }
    }
}
