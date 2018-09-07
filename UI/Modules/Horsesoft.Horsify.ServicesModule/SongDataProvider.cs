using Horsesoft.Horsify.ServicesModule.HorsifyService;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Engine.Tagging;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.ServicesModule
{
    public class SongDataProvider : ISongDataProvider
    {
        #region Fields
        private IHorsifySongService _horsifySongService;
        private ILoggerFacade _loggerFacade; 
        #endregion

        #region Constructors
        public SongDataProvider(IHorsifySongService horsifySongService, ILoggerFacade loggerFacade)
        {
            _horsifySongService = horsifySongService;
            _loggerFacade = loggerFacade;

            SearchedSongs = new ObservableCollection<AllJoinedTable>();
        } 
        #endregion

        #region Properties
        public ObservableCollection<AllJoinedTable> SearchedSongs { get; set; }

        public AllJoinedTable SelectedSong { get; set; }
        #endregion

        #region Public Methods
        public async Task ExtraSearch(ExtraSearchType extraSearchType)
        {
            ResetResults();
            IEnumerable<AllJoinedTable> songs = null;

            switch (extraSearchType)
            {
                case ExtraSearchType.None:
                    break;
                case ExtraSearchType.MostPlayed:
                    songs = await _horsifySongService.GetMostPlayedAsync();
                    break;
                case ExtraSearchType.RecentlyAdded:
                    songs = await _horsifySongService.GetRecentlyAddedAsync();
                    break;
                case ExtraSearchType.RecentlyPlayed:
                    songs = await _horsifySongService.GetRecentlyPlayedAsync();
                    break;
                default:
                    break;
            }

            if (songs?.Count() > 0)
            {
                SearchedSongs.AddRange(songs);
            }
        }

        public AllJoinedTable GetSongById(int id)
        {
            return _horsifySongService.GetSongById(id);
        }

        /// <summary>
        /// Gets the songs from a playlist
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <returns></returns>
        public Task<AllJoinedTable[]> GetSongs(Playlist playlist)
        {
            return _horsifySongService.GetSongsFromPlaylistAsync(playlist);
        }

        public Task<AllJoinedTable[]> GetSongsAsync(SearchType searchTypes, string wildCardSearch, short randomAmount = 10, short maxAmount = -1)
        {
            return _horsifySongService.SearchLikeAsync(searchTypes, wildCardSearch, randomAmount, maxAmount);
        }

        public async Task SearchAsync(SearchType searchTypes, string wildCardSearch, short randomAmount = 10, short maxAmount = -1)
        {
            if (SearchedSongs == null)
                SearchedSongs = new ObservableCollection<Music.Data.Model.AllJoinedTable>();
            else
                SearchedSongs.Clear();

            var results = await this.GetSongsAsync(searchTypes, wildCardSearch, randomAmount, maxAmount);
            if (results?.Count() > 0)
            {
                foreach (var item in results)
                {
                    SearchedSongs.AddRange(results);
                }
            }

        }

        public async Task SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1)
        {
            ResetResults();

            var results = await _horsifySongService.SearchLikeFiltersAsync(searchFilter, randomAmount, maxAmount);
            if (results?.Count() > 0)
            {
                SearchedSongs.AddRange(results);
            }
        }

        public async Task<bool> UpdatePlayedSong(AllJoinedTable selectedSong, int? rating = null)
        {
            bool fileTagResult = false;
            ISongTagger tagger = new SongTaggerTagLib();

            if (rating != null && rating > 0)
            {
                _loggerFacade.Log($"Song provider - Updating played song", Category.Debug, Priority.Medium);
                fileTagResult = tagger.UpdateFileTag(selectedSong.FileLocation, (byte)rating);
                if (!fileTagResult)
                {
                    _loggerFacade.Log($"Failed to update song. {selectedSong?.FileLocation}", Category.Exception, Priority.Medium);
                    return false;
                }
            }

            return await _horsifySongService.UpdatePlayedSongAsync(selectedSong.Id, rating);
        }
        #endregion

        #region Private Methods
        private void ResetResults()
        {
            if (SearchedSongs == null)
                SearchedSongs = new ObservableCollection<Music.Data.Model.AllJoinedTable>();
            else
                SearchedSongs.Clear();
        } 
        #endregion
    }
}
