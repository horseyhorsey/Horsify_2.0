using Horsesoft.Music.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Repositories.Services
{
            
    public interface IHorsifyPlaylistService
    {        
        IEnumerable<Playlist> GetAllPlaylists();
        IEnumerable<AllJoinedTable> GetSongsFromPlaylist(Playlist playlist);
        /// <summary>
        /// Inserts the or updates playlist if it has an existing id
        /// </summary>
        /// <param name="playlist">The playlist to insert or update</param>        
        void InsertOrUpdatePlaylists(IEnumerable<Playlist> playlists);
    }
    public partial class HorsifySongService  // Playlist Service
    {
        public IEnumerable<Playlist> GetAllPlaylists()
        {
            return _sqliteRepo.PlaylistRepository.Get();
        }

        public IEnumerable<AllJoinedTable> GetSongsFromPlaylist(Playlist playlist)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdatePlaylists(IEnumerable<Playlist> playlists)
        {
            if (playlists != null)
            {                
                foreach (var playlist in playlists)
                {
                    if (playlist.Id != 0)
                    {
                        var dbPlaylist = _sqliteRepo.PlaylistRepository.GetById(playlist.Id);
                        dbPlaylist.Items = playlist.Items;
                        dbPlaylist.Count = playlist.Count;
                        _sqliteRepo.PlaylistRepository.Update(dbPlaylist);
                    }
                    else
                    {
                        //Check if playlist name already exists and update that one
                        var dbPlaylist = _sqliteRepo.PlaylistRepository.Get(x => x.Name == playlist.Name).FirstOrDefault();
                        if (dbPlaylist != null)
                        {
                            dbPlaylist.Items = playlist.Items;
                            dbPlaylist.Count = playlist.Count;
                            _sqliteRepo.PlaylistRepository.Update(dbPlaylist);                            
                        }
                        // New playlist.
                        else
                        {
                            _sqliteRepo.PlaylistRepository.Insert(playlist);                            
                        }
                    }
                }

                ((IUnitOfWork)_sqliteRepo).Save();
            }            
        }

        public Task InsertOrUpdatePlaylistsAsync(Playlist[] playlists)
        {
            return Task.Run(() => InsertOrUpdatePlaylists(playlists));
        }

        private int[] GetIdsFromPlaylistString(string dbString)
        {
            var split = dbString.Split(';', ',');
            if (split.Length > 0)
            {
                var modded = new List<int>();
                for (int i = 0; i < split.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        modded.Add(Convert.ToInt32(split[i]));
                    }
                }

                return modded.ToArray() ;
            }

            return null;
        }
    }
}
