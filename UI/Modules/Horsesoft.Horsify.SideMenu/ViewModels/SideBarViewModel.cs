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

        #region Constructors
        public SideBarViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            Log("Loading SideBar..", Category.Debug, Priority.None);

            SearchButtons = new ObservableCollection<ISearchButtonViewModel>();

            //Composite Menus and MenuItems
            mCreator = new MenuCreator();

            //Builds all menu items from the _rootMenu
            GenerateSearchButtonsForMenuComponent(mCreator._rootMenu);
            //SearchButtonsView = new ListCollectionView(SearchButtons);

            //Updates the buttons when changed.
            //Messenger.Default.Register<AddSearchMenuButtonsMessage>(this, (x) => UpdateSearchButtons(x));
            eventAggregator.GetEvent<OnMenuParentSelectedEvent<IMenuComponent>>()
                .Subscribe(menuComponent =>
                {
                    Log("Menu Parent Selected", Category.Debug, Priority.None);
                    this.UpdateSearchButtons(menuComponent);
                });

            eventAggregator.GetEvent<HorsifySearchCompletedEvent>().Subscribe(() => IsBusy = false);

            #region Commands Setup
            MenuHomeCommand = new DelegateCommand(() =>
            {
                //Build items from the _rootMenu
                SearchButtons.Clear();
                GenerateSearchButtonsForMenuComponent(mCreator._rootMenu);
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
                SelectMenuCommand = new DelegateCommand<SearchButtonViewModel>(SelectMenuAsync);
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
            return new SearchButtonViewModel(menuiterator.Current, _loggerFacade) { SearchTitle = menuiterator.Current.Name,
                    ImagePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Horsify\\Menus\\" + menuiterator.Current.Image };
        }

        /// <summary>
        /// Generates Search Buttons For Menu Component
        /// </summary>
        private void GenerateSearchButtonsForMenuComponent(IMenuComponent menuComponent)
        {
            //Get all items if this is a menu
            if (menuComponent.GetType() == typeof(Menu))
            {
                var menuiterator = menuComponent.GetIterator();
                while (menuiterator.MoveNext())
                {
                    SearchButtons.Add(new SearchButtonViewModel(menuiterator.Current, _loggerFacade)
                    { SearchTitle = menuiterator.Current.Name, ImagePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Horsify\\Menus\\" + menuiterator.Current.Image });
                }
            }
            else
            {
                //TODO: What?
                var meh = menuComponent;
            }
        }

        /// <summary>
        /// Gets all the buttons that are in the root menu
        /// </summary>
        private void GenerateSearchButtonsFromRoot()
        {
            Log($"Generating Root menu", Category.Info, Priority.None);

            var menuiterator = mCreator._rootMenu.GetIterator();
            while (menuiterator.MoveNext())
            {
                //Add the root search buttons
                SearchButtons.Add(new SearchButtonViewModel(menuiterator.Current, _loggerFacade)
                { SearchTitle = menuiterator.Current.Name,
                    ImagePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + 
                    "\\Horsify\\Menus\\" + menuiterator.Current.Image });
            }
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
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    AddSearchButtons(menuComponent);
                    var view = CollectionViewSource.GetDefaultView(SearchButtons);
                    if (view != null)
                    {
                        view.MoveCurrentToPosition(-1);
                        view.Refresh();

                        //view.MoveCurrentToFirst();                        
                    }                        
                    //SearchButtonsView.MoveCurrentToPosition(0);
                    //SearchButtonsView.Refresh();
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
        private void AddSearchButtons(IMenuComponent menuComponent)
        {
            //If in menu generate the child items
            if (menuComponent.Name != "Back")
            {                
                //Clear the current list
                SearchButtons.Clear();

                var menuiterator = menuComponent.GetIterator();

                var buttons = new List<SearchButtonViewModel>();
                //Add back and search buttons
                while (menuiterator.MoveNext())
                {
                    buttons.Add(AssignSearchButtons(menuiterator));                    
                }

                SearchButtons.AddRange(buttons);

                //Assing this menu to the last menu?
                _previousMenu = menuComponent;
            }
            else
            {
                if (menuComponent.Name == "Back")
                {
                    _loggerFacade.Log("Go Back", Category.Info, Priority.None);

                    //If parent null generate from the root menu
                    if (menuComponent.Parent == null) { this.GenerateSearchButtonsFromRoot(); return; }

                    //Clear the current list
                    SearchButtons.Clear();

                    //Get menu items for previous parent menu
                    this.GenerateSearchButtonsForMenuComponent(menuComponent.Parent);
                }
            }
        }

        #endregion

        //TODO Fix SelectMenuAsync

        /// <summary>
        /// Select a menu button
        /// </summary>
        /// <returns></returns>
        private void SelectMenuAsync(SearchButtonViewModel searchButtonViewModel)
        {
            if (IsBusy) { return; }

            var menuComponent = searchButtonViewModel.MenuComponent;
            if (menuComponent.GetType() == typeof(Menu))
            {
                Log("Menu Selected", Category.Debug);
                _eventAggregator.GetEvent<OnMenuParentSelectedEvent<IMenuComponent>>().Publish(menuComponent);
            }
            else if (menuComponent.GetType() == typeof(MenuItem))
            {
                var menuName = menuComponent?.Name;                

                //Return back if menu is Back or IsBusy
                if (menuName == "Back")
                {
                    _eventAggregator.GetEvent<OnMenuParentSelectedEvent<IMenuComponent>>().Publish(menuComponent.Parent);
                    return;
                }                

                //Run the busy searching
                //Todo: Use the command can execute here                
                if (menuComponent?.ExtraSearchType != ExtraSearchType.None)
                {
                    IsBusy = true;
                    var navParams = new NavigationParameters();
                    navParams.Add("extra_search", menuComponent.ExtraSearchType);
                    _regionManager.RequestNavigate(Regions.ContentRegion, "SearchedSongsView", navParams);
                }
                else if (menuComponent?.SearchString == "SEARCH")
                {
                    switch (menuName)
                    {
                        case "A-Z":
                            _regionManager.RequestNavigate("ContentRegion", "AToZSearchView");
                            break;
                        case "SEARCH":
                            _regionManager.RequestNavigate("ContentRegion", "SearchView");
                            break;
                        case "SONG SEARCH":
                            _regionManager.RequestNavigate("ContentRegion", "InstantSearch");
                            break;
                        default:
                            break;
                    }
                }
                else if (menuName == "DJ Horsify" || menuName == "Filter" || menuName == "Database Stats")
                {
                    if (menuName == "DJ Horsify")
                    {
                        _regionManager.RequestNavigate("ContentRegion", "DjHorsifyView");
                    }
                    else if (menuName == "Filter")
                    {
                        _regionManager.RequestNavigate("ContentRegion", "FilterCreatorView");
                    }
                }
                else
                {
                    IsBusy = true;
                    NavigationParameters navParams = NavigationHelper.CreateSearchFilterNavigation(menuComponent.SearchType, menuComponent.Name);
                    _regionManager.RequestNavigate("ContentRegion", "SearchedSongsView", navParams);
                }
            }
        }
    }
}
