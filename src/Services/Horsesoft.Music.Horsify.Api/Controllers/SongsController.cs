using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Engine.Tagging;
using Horsesoft.Music.Horsify.Repositories.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

//[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace Horsesoft.Music.Horsify.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        IHorsifySongService _horsifySongService;

        public SongsController(IHorsifySongService horsifySongService)
        {
            _horsifySongService = horsifySongService;
        }

        // GET: api/Songs/5
        [HttpGet("{id}", Name = "Song")]
        public AllJoinedTable GetById(int id)
        {
            return _horsifySongService.GetSongById(id);
        }

        // GET: api/Songs/Search/*jackson*
        [HttpGet]
        public IEnumerable<AllJoinedTable> Search(string term, SearchType searchTypes = SearchType.All)
        {
            return _horsifySongService.SearchLike(searchTypes, term);
        }

        [HttpGet]
        public Task<IEnumerable<AllJoinedTable>> SearchFilter(string[] filters, int[] rating = null, int[] bpm = null, short randomAmount = 0, short maxAmount = -1)
        {
            SearchFilter sf = new SearchFilter(filters);

            if (rating?.Length > 1)
            {
                sf.RatingRange = new RangeFilterOption<byte>((byte)rating[0], (byte)rating[1]);
                sf.RatingRange.IsEnabled = true;
            }

            if (bpm?.Length > 1)
            {
                sf.BpmRange = new RangeFilterOption<byte>((byte)bpm[0], (byte)bpm[1]);
                sf.BpmRange.IsEnabled = true;
            }

            return _horsifySongService.SearchLikeFiltersAsync(sf, randomAmount, maxAmount);
        }

        [HttpGet]
        public IEnumerable<AllJoinedTable> MostPlayed()
        {
            return _horsifySongService.GetMostPlayed();                
        }

        [HttpGet]
        public IEnumerable<AllJoinedTable> PlayedRecent()
        {
            return _horsifySongService.GetRecentlyPlayed();
        }

        [HttpGet]
        public IEnumerable<AllJoinedTable> RecentAdded()
        {
            return _horsifySongService.GetRecentlyAdded();            
        }

        [HttpGet]
        public IEnumerable<string> GetStringEntries(SearchType searchType, string search, short maxAmount = -1)
        {
            return _horsifySongService.GetAllFromTableAsStrings(searchType, search, maxAmount);
        }

        [HttpGet]
        public Task<bool> Update(int id, int? rating)
        {
            var dbSong = _horsifySongService.GetSongById(id);
            if (dbSong == null)
                return Task.FromResult(false);

            ISongTagger tagger = new SongTaggerTagLib();
            var file = Path.Combine(dbSong.FileLocation);
            var fileTagResult = tagger.UpdateFileTag(file, (byte)rating);

            if (fileTagResult && Path.GetExtension(file).ToLower() != ".flac")
                return _horsifySongService.UpdatePlayedSongAsync(id, rating);

            return Task.FromResult(false);
        }

        [HttpGet]
        public Task<IEnumerable<AllJoinedTable>> SongsInPlaylist(Playlist playlist)
        {
            return _horsifySongService.GetSongsFromPlaylistAsync(playlist);
        }        

        //IEnumerable<AllJoinedTable>

        // POST: api/HorsifySongs
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/HorsifySongs/5
        [HttpGet]
        public void UpdateSong(int id, int? rating)
        {
            _horsifySongService.UpdatePlayedSong(id, rating);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
