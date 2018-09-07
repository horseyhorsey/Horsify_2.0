using Horsesoft.Music.Data.Model;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;

namespace Horsesoft.Horsify.SongService.Services
{
    [ServiceContract]
    public interface IHorsifyPlaylistService
    {
        [OperationContract]
        IEnumerable<Playlist> GetAllPlaylists();

        [OperationContract]
        IEnumerable<AllJoinedTable> GetSongsFromPlaylist(Playlist playlist);

        /// <summary>
        /// Inserts the or updates playlist if it has an existing id
        /// </summary>
        /// <param name="playlist">The playlist to insert or update</param>
        [OperationContract]
        void InsertOrUpdatePlaylists(IEnumerable<Playlist> playlists);
    }
}
