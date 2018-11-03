using Horsesoft.Music.Horsify.Base.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Horsesoft.Music.Data.Model;
using Prism.Logging;

namespace Horsesoft.Horsify.ServicesModule
{
    public class HorsifyPlaylistService : Horsesoft.Music.Horsify.Base.Interface.IPlaylistService
    {
        #region Services
        private readonly ILoggerFacade _loggerFacade;
        #endregion

        #region Constructor
        public HorsifyPlaylistService(ILoggerFacade loggerFacade)
        {
            _loggerFacade = loggerFacade;
            Playlists = new List<Playlist>();
        }
        #endregion

        #region Properties
        public List<Music.Data.Model.Playlist> Playlists { get; set; }
        #endregion

        #region Public Methods
        public Task<IEnumerable<AllJoinedTable>> GetSongs(Playlist playlist)
        {
            _loggerFacade.Log($"Getting Playlist songs from service {playlist.Items}", Category.Debug, Priority.Low);
            //return Task.Run(() =>   _horsifySongService.GetSongsFromPlaylistAsync((int)playlist.Id));
            return null;
        }

        public async Task UpdateFromDatabaseAsync()
        {
            try
            {
                _loggerFacade.Log($"Get all playlists from database", Category.Debug, Priority.Low);

                IEnumerable<Playlist> playlists = null; //await _horsifySongService.GetAllPlaylistsAsync();

                Playlists.Clear();
                if (playlists != null)
                {
                    Playlists.AddRange(playlists);
                }
            }
            catch (System.Exception ex)
            {
                _loggerFacade.Log($"Get playlists failed: {ex.Message}", Category.Exception, Priority.Low);
                throw;
            }
        }

        public Task SavePlaylistAsync(Playlist[] playlists)
        {
            _loggerFacade.Log($"Saving playlists", Category.Info, Priority.Low);
            _loggerFacade.Log($"playlist count {playlists.Length}", Category.Debug, Priority.Low);

            return null;
            //return _horsifySongService.InsertOrUpdatePlaylistsAsync(playlists);
        }
        #endregion
    }
}
