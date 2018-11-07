using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Repositories.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Horsesoft.Music.Horsify.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        IHorsifySongService _horsifySongService;

        public PlaylistsController(IHorsifySongService horsifySongService)
        {
            _horsifySongService = horsifySongService;
        }

        // GET: api/Playlists
        [HttpGet]
        public IEnumerable<Playlist> Get()
        {
            return _horsifySongService.GetAllPlaylists();
        }

        // GET: api/Playlists/5
        [HttpGet("{id}", Name = "GetPlaylists")]
        public Playlist Get(int id)
        {
            return _horsifySongService.GetPlaylistById(id);
        }

        [Route("[action]")]
        [HttpPost]
        public Task<IEnumerable<AllJoinedTable>> GetSongs([FromBody] Playlist playlist)
        {
            return _horsifySongService.GetSongsFromPlaylistAsync(playlist);
        }

        // POST: api/Playlists
        [HttpPost]
        public void Post([FromBody] Playlist value)
        {
        }

        // PUT: api/Playlists/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Playlist value)
        {
            _horsifySongService.InsertOrUpdatePlaylists(new Playlist[] { value});
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
