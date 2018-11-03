using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Repositories.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IEnumerable<AllJoinedTable> Search(string term, SearchType searchTypes = SearchType.All)
        {
            return _horsifySongService.SearchLike(searchTypes, term);
        }

        public Task<IEnumerable<AllJoinedTable>> SearchFilter(string[] filters = null, int[] rating = null,
            int[] bpm = null, short randomAmount = 0, short maxAmount = -1)
        {
            SearchFilter sf = null;

            if (filters?.Length > 0)
            {
                sf = new SearchFilter(filters);
            }
            else
                sf = new SearchFilter();

            if (rating != null)
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

        public IEnumerable<AllJoinedTable> MostPlayed()
        {
            return _horsifySongService.GetMostPlayed();                
        }

        public IEnumerable<AllJoinedTable> PlayedRecent()
        {
            return _horsifySongService.GetRecentlyPlayed();
        }

        public IEnumerable<AllJoinedTable> RecentAdded()
        {
            return _horsifySongService.GetRecentlyAdded();
        }


        //IEnumerable<AllJoinedTable>

        // POST: api/HorsifySongs
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/HorsifySongs/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
