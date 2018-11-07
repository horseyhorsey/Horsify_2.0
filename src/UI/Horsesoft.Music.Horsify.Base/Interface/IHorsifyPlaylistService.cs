using Horsesoft.Music.Data.Model;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IPlaylistService
    {
        List<Playlist> Playlists { get; set; }

        Task<IEnumerable<AllJoinedTable>> GetSongs(Playlist playlist);

        Task SavePlaylistAsync(Playlist[] playlist);

        Task UpdateFromDatabaseAsync();
    }
}
