using System.Collections.Generic;
using System;
using System.IO;
using Horsesoft.Music.Data.Model.Horsify;

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

        #region Support Methods

        private void CreateMenus()
        {
            //Create the root menu
            _rootMenu = new Menu() { Name = "Root" };

            _searchMenus = new Menu() { Name = "Search",
                MenuComponents = new List<IMenuComponent>()
                {
                    new MenuItem() { Name = "Back", Parent = this._rootMenu },
                    new MenuItem {Name = "A-Z", Image = @"", SearchString = "SEARCH"},
                    new MenuItem {Name = "SONG SEARCH", Image = @"", SearchString = "SEARCH"},
                    new MenuItem {Name = "SEARCH", Image = @"", SearchString = "SEARCH"}
                }
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

        #endregion

        #region Builders

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

        #endregion

        #region Menu builders

        private IMenuComponent CreateDecadeMenu(string menuName, int startYear, int endYear, string imagepath = "", IMenuComponent parentMenu = null)
        {
            var menuItems = new List<IMenuComponent>();
            var searchStartYear = startYear.ToString().Remove(startYear.ToString().Length - 1, 1);

            //Create back button
            menuItems.Add(new MenuItem() { Name = "Back", Parent = parentMenu });

            menuItems.Add(new MenuItem { Name = $"{searchStartYear}%", SearchType = SearchType.Year });
            for (int i = startYear; i < endYear; i++)
            {
                if (string.IsNullOrWhiteSpace(imagepath))
                    menuItems.Add(new MenuItem { Name = $"{i}", SearchType = SearchType.Year });
                else
                {
                    menuItems.Add(new MenuItem { Name = $"{i}", SearchType = SearchType.Year });
                }

            }

            return new Menu
            {
                Name = menuName,
                MenuComponents = menuItems,
                Image = imagepath,
                Parent = parentMenu
            };
        }

        private IMenuComponent CreateGenres()
        {
            var menu = new Menu
            {                
                Name = "Genre",
            };

            var menuComponents = new List<IMenuComponent>
                {
                    //Create back navigation
                    new MenuItem() { Name = "Back", Parent = this._rootMenu },

                    //BuildMenu("Custom","",GetCustomGenres(menu)),
                    BuildMenu("Abstract", @"abstract.png",GetAbstractMenuItems(menu)),
                    BuildMenu("Acoustic", @"acoustic.png", GetAcousticMenuItems(menu)),
                    BuildMenu("Country", @"country.png",GetCountryMenuItems(menu)),

                    //Dance menu goes two deep
                    CreateDanceMenu(menu),
                    BuildMenu("Disco", @"disco.png", GetDiscoMenuItems(menu)),
                    new MenuItem{Name = "Downtempo%",SearchType = SearchType.Genre, Image = @"downtempo.png"},
                    BuildMenu("Funk", @"Funk.png", GetFunkMenuItems(menu)),
                    BuildMenu("Hip Hop", @"hip_hop.png", GetHipHopMenuItems(menu)),
                    BuildMenu("Jazz", @"Jazz.png", GetJazzMenuItems(menu)),
                    BuildMenu("Pop",@"pop.png", GetPopMenuItems(menu)),
                    BuildMenu("Reggae",@"reggae.png", GetReggaeItems(menu)),
                    BuildMenu("Rock", @"rock.png", GetRockItems(menu)),
                    BuildMenu("Rock & Roll",@"rockroll.png", GetRockAndRollItems(menu)),
                    BuildMenu("Soul",@"soul.png", GetSoulItems(menu)),
                    new MenuItem{Name = "Soundtrack%",SearchType = SearchType.Genre,Image = @"soundtrack.png" },
                };

            menu.MenuComponents = menuComponents;

            return menu;
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

        private IMenuComponent CreateDanceMenu(Menu menu)
        {
            //build dance menu
            var danceMenu = BuildMenu("Dance", @"dance.png", new List<IMenuComponent>());
            danceMenu.Parent = menu;

            var menuComponents = new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu},
                    BuildMenu("Breakbeat",@"Breakbeat.png",GetBreakBeatItems(danceMenu)),
                    BuildMenu("Drum & Bass, Jungle", @"junglednb.png", GetDnbItems(danceMenu)),
                    BuildMenu("Dubstep",  @"dubstep.png", GetDubStepItems(danceMenu)),
                    BuildMenu("Electronic", @"electronic.png", GetElectronicItems(danceMenu)),
                    BuildMenu("Garage", @"garage.png", GetGarageItems(menu)),
                    BuildMenu("House", @"house.png", GetHouseItems(menu)),
                    BuildMenu("Techno", @"techno.png", GetTechnoItems(menu)),
            };

            //Update the Parent for each menu
            menuComponents.ForEach(x =>
            {
                danceMenu.Add(x);
            });

            return danceMenu;
        }

        private List<IMenuComponent> GetAbstractMenuItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "Abstract%", Image = @"abstract.png", SearchType = SearchType.Genre },
                    new MenuItem { Name = "Ambient%", Image = @"ambient.png",SearchType = SearchType.Genre },
                    new MenuItem { Name = "Breaks%",Image = @"breaks.png", SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Trip Hop%", Image = @"triphop.png",SearchType = SearchType.Genre },
                };
        }

        private List<IMenuComponent> GetAcousticMenuItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Acoustic%", Image = @"Acoustic.png", SearchType = SearchType.Genre },
                };
        }        

        private List<IMenuComponent> GetBreakBeatItems(IMenuComponent danceMenu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = danceMenu },
                    new MenuItem { Name = "Breakbeat", Image = @"breakbeat.png",SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Breakbeat, Jungle%" ,Image = @"breakbeatjungle.png",SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Breakbeat, Hardcore%",Image = @"breakbeathardcore.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "Hardcore%",SearchType = SearchType.Genre,Image = @"hardcore.png" },
                    new MenuItem { Name = "%Happy Hardcore%", Image = @"happyhardcore.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Breakbeat, House%", Image = @"breakbeathouse.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Breakbeat, Techno%", Image = @"breakbeattechno.png",SearchType = SearchType.Genre  },
                };
        }

        private List<IMenuComponent> GetCountryMenuItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Country%" , SearchType = SearchType.Genre,Image = @"country.png"}
                };
        }

        private List<IMenuComponent> GetDnbItems(IMenuComponent menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Drum & Bass%", SearchType = SearchType.Genre, Image = "dnb.png", },
                    new MenuItem { Name = "%Drum & Bass, Breakcore%", SearchType = SearchType.Genre, Image = @"breakcore.png" },
                    new MenuItem { Name = "%Drum & Bass, Breaks%", SearchType = SearchType.Genre, Image = "dnbbreaks.png", },
                    new MenuItem { Name = "%Drum & Bass, Drumfunk%", SearchType = SearchType.Genre, Image = "dnbdrumfunk.png", },
                    new MenuItem { Name = "%Drum & Bass, Halfstep%", SearchType = SearchType.Genre, Image = "halfstep.png", },
                    new MenuItem { Name = "%Intelligent%", SearchType = SearchType.Genre, Image = "intelligent.png", },
                    new MenuItem { Name = "%Jazzstep%", SearchType = SearchType.Genre, Image = "jazzstep.png" },
                    new MenuItem { Name = "%Jungle%", SearchType = SearchType.Genre, Image = "jungle.png" },
                    new MenuItem { Name = "%Jungle, Ragga%", SearchType = SearchType.Genre, Image = "jungle_ragga.png" },
                    new MenuItem { Name = "%Drum & Bass, Jump Up%", SearchType = SearchType.Genre, Image = "jumpup.png" },
                    new MenuItem { Name = "%Liquid%", SearchType = SearchType.Genre, Image = "liquid.png" },
                    new MenuItem { Name = "%Drum & Bass, Minimal%", SearchType = SearchType.Genre, Image = "minimal.png" },
                    new MenuItem { Name = "%Neurofunk%", SearchType = SearchType.Genre, Image = "Neurofunk.png" },
                    new MenuItem { Name = "%Drum & Bass, Technoid%", SearchType = SearchType.Genre, Image = "technoid.png" },
                    new MenuItem { Name = "%Techstep%", SearchType = SearchType.Genre, Image = "Techstep.png" },
                    new MenuItem { Name = "%Drum & Bass, Trance%", SearchType = SearchType.Genre, Image = "dnbtrance.png" },
                };
        }

        /// <summary>
        /// Creates all sub genres of disco
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        private List<IMenuComponent> GetDiscoMenuItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Disco%",Image = @"disco.png",SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Disco, Boogie%", Image = @"discoboogie.png" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Disco, Dance-Pop%",Image = @"disco-dance-pop.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Disco, Electro%", Image = @"disco-electro.png",SearchType = SearchType.Genre   },
                    new MenuItem { Name = "%Disco, Funk%",Image = @"disco-funk.png",SearchType = SearchType.Genre   },
                    new MenuItem { Name = "%Disco, Soul%", Image = @"disco-soul.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Italo%", Image = @"italo_disco.png",SearchType = SearchType.Genre  },
                };
        }

        private List<IMenuComponent> GetDubStepItems(IMenuComponent menu)
        {
            return new List<IMenuComponent>
            {
                new MenuItem() { Name = "Back", Parent = menu },
                new MenuItem { Name = "%Dubstep%" , Image=@"dubstep.png",SearchType = SearchType.Genre }
            };
        }

        private List<IMenuComponent> GetElectronicItems(IMenuComponent menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Trance%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Techno",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Pop Club Remixes%",SearchType = SearchType.Genre  }
                };
        }

        private List<IMenuComponent> GetGarageItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Garage%", Image = @"garage.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%UK Garage%", Image = @"uk_garage.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Speed garage%", Image = @"speedgarage.png",SearchType = SearchType.Genre  }
                };
        }

        private List<IMenuComponent> GetHouseItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%House%" , Image = @"house.png",SearchType = SearchType.Genre},
                    new MenuItem { Name = "%House, Acid%" , Image = @"House, Acid.png",SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Breakbeat, House%", Image = @"breakbeathouse.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%House, Deep House%", Image = @"deephouse.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%House, Electro%", Image = @"houseelectro.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%House, Euro House%", Image = @"houseeurohouse.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Hard House%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%House, Ghetto%",Image = @"House, Ghetto.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Hip-House%" ,Image = @"Hip-House.png",SearchType = SearchType.Genre },
                    new MenuItem { Name = "%House, Pop%",Image = @"House, Pop.png" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%House, Progressive%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%House, Techno%" ,SearchType = SearchType.Genre },
                };
        }

        private List<IMenuComponent> GetFunkMenuItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "Funk", Image = @"funk.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "Funk,%", Image = @"funk.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Funk, Soul%", Image = @"funk_soul.png" ,SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Jazz-Funk%", Image = @"jazz_funk.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%P.Funk%", Image = @"p-funk.png",SearchType = SearchType.Genre }
                };
        }

        private List<IMenuComponent> GetHipHopMenuItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "Hip Hop%", SearchType = SearchType.Genre, Image=@"hip_hop.png" },
                    new MenuItem { Name = "Electro%", SearchType = SearchType.Genre, Image=@"electro.png"},
                    new MenuItem { Name = "%Hip Hop, British%", SearchType = SearchType.Genre, Image=@"hiphop_uk.png"},
                    new MenuItem { Name = "%Hip Hop, Gangsta%", SearchType = SearchType.Genre, Image=@"gangsta.png"  },
                    new MenuItem { Name = "%Conscious%", SearchType = SearchType.Genre, Image=@"hiphop_concious.png"  },
                    new MenuItem { Name = "%Grime%", SearchType = SearchType.Genre, Image=@"grime.png"},
                };
        }

        private List<IMenuComponent> GetJazzMenuItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "Jazz" , SearchType = SearchType.Genre,Image = @"jazz.png"},
                    new MenuItem {Name = "Fusion",SearchType = SearchType.Genre, Image=@"jazz_fusion.png"},
                    new MenuItem {Name = "%Future Jazz%",SearchType = SearchType.Genre, Image=@"jazz_future.png"},
                    new MenuItem { Name = "Swing" ,SearchType = SearchType.Genre,  Image=@"jazz_swing.png"}
                };
        }

        private List<IMenuComponent> GetPopMenuItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Pop%", Image = @"pop.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Brit Pop%", Image = @"britpop.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Hip Hop, Pop%", Image = @"hiphop_pop.png",SearchType = SearchType.Genre }  ,
                    new MenuItem { Name = "%New Wave", Image= @"new_wave.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%R&B,%", Image= @"rnb.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Synth-Pop%", Image=@"synth_pop.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Synthwave%", Image = @"synthwave.png" ,SearchType = SearchType.Genre },
                };
        }

        private List<IMenuComponent> GetReggaeItems(Menu menu)
        {
            return new List<IMenuComponent>
                {

                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "Reggae,%", SearchType = SearchType.Genre,Image = @"reggae.png" },
                    new MenuItem { Name = "Reggae, Roots%", SearchType = SearchType.Genre  },
                    new MenuItem { Name = "Reggae%", SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Lovers Rock%", SearchType = SearchType.Genre  },
                    new MenuItem { Name = "Reggae, Dub%", SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Ska%", SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Calypso%", SearchType = SearchType.Genre  },
                    new MenuItem { Name = "Dancehall%", SearchType = SearchType.Genre  },
                };
        }

        private List<IMenuComponent> GetRockAndRollItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Rock & Roll,%",Image = @"rockroll.png" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Doo Wop%" ,SearchType = SearchType.Genre }  ,
                    new MenuItem { Name = "%Rhythm & Blues" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Rockabilly%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Rock & Roll, Vocal%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Surf%" ,SearchType = SearchType.Genre },
                };
        }

        private List<IMenuComponent> GetRockItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "%Rock", Image = @"rock.png",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Alternative Rock%" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Brit Pop%" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Classic Rock%" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Folk Rock%" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Glam%" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Grunge%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Hard Rock%" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Heavy Metal%" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Indie%" ,SearchType = SearchType.Genre },
                    new MenuItem { Name = "%Leftfield%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Punk%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Rock, Pop%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Rockabilly%",SearchType = SearchType.Genre  },
                    new MenuItem { Name = "%Soft Rock%",SearchType = SearchType.Genre  },
                };
        }        

        private List<IMenuComponent> GetSoulItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "Soul" , SearchType = SearchType.Genre,Image = @"soul.png"},
                    new MenuItem {Name = "Ballad%",SearchType = SearchType.Genre,Image = @"ballad.png"},
                    new MenuItem { Name = "%Northern Soul%" ,SearchType = SearchType.Genre,Image = @"northern_soul.png"},
                    new MenuItem { Name = "%Motown%", SearchType = SearchType.Genre ,Image = @"motown.png"}
                };
        }

        private List<IMenuComponent> GetTechnoItems(Menu menu)
        {
            return new List<IMenuComponent>
                {
                    new MenuItem() { Name = "Back", Parent = menu },
                    new MenuItem { Name = "Techno" , SearchType = SearchType.Genre,Image = @"techno.png"},
                    new MenuItem { Name = "Gabba" , SearchType = SearchType.Genre,Image = @"gabba.png"},
                    new MenuItem { Name = "Gabber%" , SearchType = SearchType.Genre,Image = @"gabber.png"},
                    new MenuItem { Name = "%Breakbeat, Techno%",SearchType = SearchType.Genre},
                    new MenuItem { Name = "%Techno, Acid%",SearchType = SearchType.Genre},                    
                    new MenuItem { Name = "%Techno, Euro House%" ,SearchType = SearchType.Genre},
                    new MenuItem { Name = "%Techno, Tribal House%" ,SearchType = SearchType.Genre},                    
                    new MenuItem { Name = "%Trance, Techno%" ,SearchType = SearchType.Genre},                    
                };
        }        

        #endregion

    }
}