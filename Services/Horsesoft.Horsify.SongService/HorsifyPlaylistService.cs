using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Horsesoft.Horsify.SongService
{
    public partial class HorsifySongService // Playlist Service
    {
        public IEnumerable<Playlist> GetAllPlaylists()
        {
            return _sqliteRepo.PlaylistRepository.Get();
        }

        public IEnumerable<AllJoinedTable> GetSongsFromPlaylist(Playlist playlist)
        {
            var ids = GetIdsFromPlaylistString(playlist.Items);

            if (ids?.Length > 0)
            {
                return _sqliteRepo.GetAllJoinedTableByIds(ids);
            }

            return null;
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
                        var dbPlaylist = _sqliteRepo.PlaylistRepository.Get(x => x.Name == playlist.Name).FirstOrDefault();
                        if (dbPlaylist != null)
                        {
                            dbPlaylist.Items = playlist.Items;
                            dbPlaylist.Count = playlist.Count;
                            _sqliteRepo.PlaylistRepository.Insert(dbPlaylist);
                            playlist.Id = dbPlaylist.Id;
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
