using Horsesoft.Music.Data.Model;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.SongService.Services
{
    
    public interface IHorsifyPlaylistService
    {        
        IEnumerable<Playlist> GetAllPlaylists();
        Task<IEnumerable<Playlist>> GetAllPlaylistsAsync();
        IEnumerable<AllJoinedTable> GetSongsFromPlaylist(Playlist playlist);

        /// <summary>
        /// Inserts the or updates playlist if it has an existing id
        /// </summary>
        /// <param name="playlist">The playlist to insert or update</param>        
        void InsertOrUpdatePlaylists(IEnumerable<Playlist> playlists);
    }
}
