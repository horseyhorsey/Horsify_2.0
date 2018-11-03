using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.SongService
{
    public partial class HorsifySongService // Playlist Service
    {
        public IEnumerable<Playlist> GetAllPlaylists()
        {
            return _sqliteRepo.PlaylistRepository.Get();
        }
        public Task<IEnumerable<Playlist>> GetAllPlaylistsAsync()
        {
            return Task.Run(() => GetAllPlaylists());
        }

        public IEnumerable<AllJoinedTable> GetSongsFromPlaylist(int id)
        {
            var playlist = _sqliteRepo.PlaylistRepository.GetById(id);
            if (playlist == null) return null;

            var ids = GetIdsFromPlaylistString(playlist.Items);
            if (ids?.Length > 0)
            {
                return _sqliteRepo.GetAllJoinedTableByIds(ids);
            }

            return null;
        }

        public Task<IEnumerable<AllJoinedTable>> GetSongsFromPlaylistAsync(int id)
        {
            return Task.Run(() => GetSongsFromPlaylist(id));
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
