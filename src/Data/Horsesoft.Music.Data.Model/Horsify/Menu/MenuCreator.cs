using System;
using System.Collections.Generic;
using System.IO;
using Horsesoft.Music.Data.Model.Horsify;
using Newtonsoft.Json;

namespace Horsesoft.Music.Data.Model.Menu
{
    public class MenuCreator
    {
        #region Fields
        public IMenuComponent _rootMenu;
        private Menu _searchMenus;
        #endregion

        #region Constructors
        public MenuCreator()
        {
            CreateMenus();
        }
        #endregion

        #region Priavte Methods

        /// <summary>
        /// Builds a Menu with Menus or MenuItems. Adds a back navigation item.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imagePath"></param>
        /// <param name="parentMenu"></param>
        /// <param name="childComponents"></param>
        /// <returns></returns>
        private IMenuComponent BuildMenu(string name, string imagePath, List<IMenuComponent> childComponents)
        {
            // Return a new menu from params
            return new Menu()
            {
                Name = name,
                Image = imagePath,
                MenuComponents = childComponents
            };
        }

        private IMenuComponent CreateDecadeMenu(string menuName, int startYear, int endYear, string imagepath = "", IMenuComponent parentMenu = null)
        {
            var menuItems = new List<IMenuComponent>();
            var searchStartYear = startYear.ToString().Remove(startYear.ToString().Length - 1, 1);

            //Create back button
            menuItems.Add(new MenuItem() { Name = "Back", Parent = parentMenu });

            menuItems.Add(new MenuItem { Name = $"{searchStartYear}%", SearchType = SearchType.Year });
            for (int i = endYear -1; i > startYear -1; i--)
            {                
                menuItems.Add(new MenuItem { Name = $"{i}", SearchType = SearchType.Year });
            }

            return new Menu
            {
                Name = menuName,
                MenuComponents = menuItems,
                Image = imagepath,
                Parent = parentMenu
            };
        }

        /// <summary>
        /// Generates the genres from a custom genres.json
        /// </summary>
        /// <returns>A Menu</returns>
        private IMenuComponent CreateGenres()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
            settings.Converters.Add(new MenuConverter());

            //Convert to JSON
            //string json = JsonConvert.SerializeObject(menu, Formatting.Indented, settings);

            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var h_progData = Path.Combine(appPath, "Horsify", "genres.json");

            return JsonConvert.DeserializeObject<Menu>(System.IO.File.ReadAllText(h_progData), settings);
        }

        private void CreateMenus()
        {
            //Create the root menu
            _rootMenu = new Menu() { Name = "Root" };

            _searchMenus = new Menu()
            {
                Name = "Search",
                MenuComponents = new List<IMenuComponent>()
                {
                    new MenuItem() { Name = "Back", Parent = this._rootMenu },
                    new MenuItem {Name = "A-Z", Image = @"a-z.png", SearchString = "SEARCH"},
                    new MenuItem {Name = "SONG SEARCH", Image = @"", SearchString = "SEARCH"},
                    new MenuItem {Name = "SEARCH", Image = @"", SearchString = "SEARCH"}
                },
                Image = "search.png"
            };

            //Create decades
            Menu menuDecades = CreateDecadesMenu();

            //Genres
            IMenuComponent menuGenres = CreateGenres();

            //History menu, recentyly plyaed - added
            var menuHistory = BuildMenu("History", null, new List<IMenuComponent>()
            {
                new MenuItem() { Name = "Back", Parent = _rootMenu },

                new MenuItem { Name = "Most Played",
                    ExtraSearchType = ExtraSearchType.MostPlayed
                },

                new MenuItem { Name = "Recently Played",
                    ExtraSearchType = ExtraSearchType.RecentlyPlayed
                ,Image = null},

                new MenuItem{
                Name = "Recently Added",
                    ExtraSearchType = ExtraSearchType.RecentlyAdded
                }
            });

            var menuConfig = new Menu
            {
                Name = "Options",
                Image = @"",
                MenuComponents = new List<IMenuComponent>()
                {
                    new MenuItem() { Name = "Back", Parent = this._rootMenu },
                    //new MenuItem {Name = "Database Stats", Image = @""}
                }
            };

            //Add all menus and items to root
            _rootMenu.Add(_searchMenus);
            _rootMenu.Add(menuDecades);
            _rootMenu.Add(menuGenres);
            _rootMenu.Add(menuHistory);
            _rootMenu.Add(menuConfig);
        }

        private Menu CreateDecadesMenu()
        {
            var menuDecades = new Menu
            {
                Name = "Decades",
                Parent = _rootMenu
            };

            menuDecades.MenuComponents = new List<IMenuComponent>();
            menuDecades.MenuComponents.Add(new MenuItem() { Name = "Back", Parent = this._rootMenu });

            var decades = new List<IMenuComponent>
            {
                CreateDecadeMenu("1950s",1950, 1960, @"50s.png", menuDecades),
                CreateDecadeMenu("1960s",1960, 1970, @"60s.png", menuDecades),
                CreateDecadeMenu("1970s",1970, 1980, @"70s.png", menuDecades),
                CreateDecadeMenu("1980s",1980, 1990, @"80s.png", menuDecades),
                CreateDecadeMenu("1990s",1990, 2000, @"90s.png", menuDecades),
                CreateDecadeMenu("2000s",2000, 2010, @"00s.png", menuDecades),
                CreateDecadeMenu("2010s",2010, 2020, @"10s.png", menuDecades),
            };
            decades.Reverse();

            menuDecades.MenuComponents.AddRange(decades);
            return menuDecades;
        }

        //TODO: Fix CUSTOM Genres Repo or remove.
        /*
        /// <summary>
        /// Gets the custom genres from user text files and adds them to a custom menu.
        /// <para/> Sets the <see cref="IMenuComponent.SearchString"/>
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        private List<IMenuComponent> GetCustomGenres(Menu menu)
        {
            var genreListPath = Path.Combine(Environment.CurrentDirectory, "Lists\\Genres");
            if (!Directory.Exists(genreListPath))
                Directory.CreateDirectory(genreListPath);

            var genreRepo = new GenreRepo();
            var genreSearches = genreRepo.GetLists(genreListPath);

            var customMenu = new List<IMenuComponent>();
            var menuItems = new List<MenuItem>();
            menuItems.Add(new MenuItem() { Name = "Back", Parent = menu });
            foreach (var genreSearch in genreSearches)
            {
                var name = genreSearch.Key;
                var genres = genreSearch.Value;

                string searchString = string.Empty;
                foreach (var genre in genres)
                {
                    searchString += $"{genre}|";
                }

                menuItems.Add(new MenuItem { Name = name,
                    SearchString = searchString, SearchType = SearchType.Genre
                });
            }

            customMenu.AddRange(menuItems);

            return customMenu;
        }
        */

        #endregion

    }
}