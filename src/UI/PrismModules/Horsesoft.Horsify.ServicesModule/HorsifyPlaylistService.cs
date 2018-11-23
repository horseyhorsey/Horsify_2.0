using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Horsesoft.Music.Data.Model;
using Prism.Logging;
using Horsesoft.Music.Data.Model.Horsify;

namespace Horsesoft.Horsify.ServicesModule
{
    public class HorsifyPlaylistService : Horsesoft.Music.Horsify.Base.Interface.IPlaylistService
    {
        #region Services
        private readonly IHorsifySongApi _horsifySongApi;
        private readonly ILoggerFacade _loggerFacade;
        #endregion

        #region Constructor
        public HorsifyPlaylistService(IHorsifySongApi horsifySongApi, ILoggerFacade loggerFacade)
        {
            _horsifySongApi = horsifySongApi;
            _loggerFacade = loggerFacade;
            Playlists = new List<Playlist>();
        }
        #endregion

        #region Properties
        public List<Music.Data.Model.Playlist> Playlists { get; set; }
        #endregion

        #region Public Methods

        public Task<bool> DeletePlaylistAsync(int id)
        {
            return _horsifySongApi.DeletePlaylistAsync(id);
        }

        public Task<IEnumerable<AllJoinedTable>> GetSongs(Playlist playlist)
        {
            _loggerFacade.Log($"Getting Playlist songs from service {playlist.Items}", Category.Debug, Priority.Low);
            return _horsifySongApi.GetSongsFromPlaylistAsync(playlist);
        }

        public async Task UpdateFromDatabaseAsync()
        {
            try
            {
                _loggerFacade.Log($"Get all playlists from database", Category.Debug, Priority.Low);

                IEnumerable<Playlist> playlists = await _horsifySongApi.GetPlaylists();

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

            return _horsifySongApi.InsertOrUpdatePlaylistsAsync(playlists);
        }
        #endregion
    }
}
