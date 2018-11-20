using Horsesoft.Horsify.SearchModule.Model;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class InstantSearchViewModel : HorsifySongPlayBindableBase
    {
        private ISongDataProvider _songDataProvider;
        private IHorsifySongApi _horsifySongApi;
        private IEventAggregator _eventAggregator;
        private static DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public InstantSearchViewModel(ISongDataProvider songDataProvider, IHorsifySongApi horsifySongApi,
            IEventAggregator eventAggregator, IQueuedSongDataProvider queuedSongDataProvider, ILoggerFacade loggerFacade) : base(queuedSongDataProvider, loggerFacade)
        {
            _songDataProvider = songDataProvider;
            _horsifySongApi = horsifySongApi;
            _eventAggregator = eventAggregator;

            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;

            SearchModel.PropertyChanged += SearchModel_PropertyChanged;

            ShowSearchKeyboardViewCommand = new DelegateCommand<object>(OnShowSearchKeyboard);
        }

        private void OnShowSearchKeyboard(object showSearch)
        {
            bool show = false;
            bool.TryParse(showSearch.ToString(), out show);
            SearchKeyboardVisible = show;
        }

        private bool _searchKeyboardVisible = true;
        public bool SearchKeyboardVisible
        {
            get { return _searchKeyboardVisible; }
            set { SetProperty(ref _searchKeyboardVisible, value); }
        }

        protected override void OnPlay(AllJoinedTable song = null)
        {
            if (song != null)
            {
                AllJoinedTable fullSong = null;
                Task.Run(async () =>
               {
                   fullSong = await GetSong(song);
               }).Wait();

                Log($"Playing song: {fullSong.FileLocation}");
                _eventAggregator.GetEvent<OnMediaPlay<AllJoinedTable>>()
                .Publish(fullSong);
            }
        }

        private Task<AllJoinedTable> GetSong(AllJoinedTable allJoinedTable)
        {
            return _songDataProvider.GetSongById(allJoinedTable.Id);
        }


        //TODO: Put this back in
        /// <summary>
        /// Gets the song from a searchstring. Searchstring must begin with an ID and be split with pipe char. |
        /// </summary>
        /// <param name="searchString">The search string.</param>
        /// <returns></returns>
        //private AllJoinedTable GetSong(string searchString)
        //{
        //    if (string.IsNullOrWhiteSpace(searchString))
        //    {
        //        Log($"Play song in instant search empty.", Category.Warn);
        //        return null;
        //    }

        //    int id = 0;
        //    if (searchString.Contains("|"))
        //    {
        //        id = Convert.ToInt32(searchString.Split('|')[0]);
        //    }

        //    return _songDataProvider.GetSongById(id);
        //}

        protected override void OnQueueSong(AllJoinedTable song = null)
        {
            if (song != null)
            {
                AllJoinedTable fullSong = null;
                Task.Run(async () =>
                {
                    fullSong = await GetSong(song);
                }).Wait();

                Log($"Queueing song: {fullSong.FileLocation}");                
                base.QueueSong(fullSong);
            }
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Get songs, stop timer and populate collection            
            _dispatcherTimer.Stop();
            var results = _horsifySongApi.GetEntries(Music.Data.Model.Horsify.SearchType.Title, SearchModel.SearchText, 15);
            SearchModel.Results.Clear();
            SearchModel.AllJoinedTables.Clear();
            SearchModel.Results.AddRange(results);

            //.Select(z => $"{z.Id}|{z.Artist?.Name}|{z.Title}|{z.Album?.Title}|{z.Year}|{z.ImageLocation}|{z.Rating}")

            foreach (var song in SearchModel.Results)
            {
                var fields = song.Split('|');
                long year = 0;
                var ajonedSong = new AllJoinedTable()
                {
                    Id = Convert.ToInt32(fields[0]),
                    Artist = fields[1],
                    Title = fields[2],
                    Album = fields[3],
                    Year = long.TryParse(fields[4], out year) ? year : 0,
                    ImageLocation = fields[5],
                    Rating = Convert.ToInt32(fields[6])
                };

                SearchModel.AllJoinedTables.Add(ajonedSong);
            }

            //foreach (var item in SearchModel.Results)
            //{
            //    var fileds = item.Split('|');
            //}

            //SearchModel.AllJoinedTables
        }

        #region Commands
        public ICommand ShowSearchKeyboardViewCommand { get; set; }
        #endregion

        #region Properties
        private SearchModel _searchModel = new SearchModel();
        public SearchModel SearchModel
        {
            get { return _searchModel; }
            set { SetProperty(ref _searchModel, value); }
        }

        private int _caretIndex;
        public int CursorPosition
        {
            get { return _caretIndex; }
            set { SetProperty(ref _caretIndex, value); }
        }
        #endregion

        #region Private Methods
        private void SearchModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SearchText")
            {
                var sText = SearchModel.SearchText;
                if (sText?.Length > 2)
                {
                    //Do the search after timer
                    if (_dispatcherTimer.IsEnabled)
                        _dispatcherTimer.Stop();

                    _dispatcherTimer.Start();
                }
                else
                {
                    SearchModel.Results.Clear();
                }
            }
        }
        #endregion
    }
}
