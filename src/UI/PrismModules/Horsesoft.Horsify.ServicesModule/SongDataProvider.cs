﻿using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.ServicesModule
{
    public class SongDataProvider : ISongDataProvider
    {
        #region Fields
        private ILoggerFacade _loggerFacade;
        private IHorsifySongApi _horsifySongApi;
        #endregion

        #region Constructors
        public SongDataProvider(ILoggerFacade loggerFacade, IHorsifySongApi horsifySongApi)
        {
            _horsifySongApi = horsifySongApi;
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

            songs = await _horsifySongApi.ExtraSearch(extraSearchType);

            if (songs?.Count() > 0)
            {
                SearchedSongs.AddRange(songs);
            }
        }

        public Task<AllJoinedTable> GetSongById(int id)
        {
            return _horsifySongApi.GetById(id);
        }

        /// <summary>
        /// Gets the songs from a playlist
        /// </summary>
        /// <param name="playlist">The playlist.</param>
        /// <returns></returns>
        public Task<IEnumerable<AllJoinedTable>> GetSongsAsync(Playlist playlist)
        {
            return _horsifySongApi.GetSongsFromPlaylistAsync(playlist);            
        }

        public Task<IEnumerable<AllJoinedTable>> GetSongsAsync(SearchType searchTypes, string wildCardSearch, short randomAmount = 10, short maxAmount = -1)
        {
            return _horsifySongApi.SearchAsync(wildCardSearch, searchTypes);
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

            var results = await _horsifySongApi.SearchLikeFiltersAsync(searchFilter, randomAmount, maxAmount);

            if (results?.Count() > 0)
            {
                SearchedSongs.AddRange(results);                
            }
        }

        public async Task<bool> UpdatePlayedSong(AllJoinedTable selectedSong, int? rating = null)
        {         
            if (rating != null && rating > 0)
            {
                _loggerFacade?.Log($"Changing song rating ID: {selectedSong.Id}, Rating: {rating}", Category.Debug, Priority.Medium);
            }
            else
            {
                _loggerFacade?.Log($"Updating played song: {selectedSong.Id}", Category.Debug, Priority.Medium);
            }

            return await _horsifySongApi.UpdatePlayedSongAsync(selectedSong.Id, rating);
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
