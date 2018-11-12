using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Repositories.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Horsesoft.Music.Horsify.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private IHorsifySongService _horsifySongService;

        public StatsController(IHorsifySongService horsifySongService)
        {
            _horsifySongService = horsifySongService;
        }
        // GET: api/Stats
        [HttpGet]
        public Stat Get()
        {
            var repo = _horsifySongService.GetRepo();
            return new Stat()
            {
                SongCount = repo.SongRepository.Get().Count(),
                AlbumCount = repo.AlbumRepository.Get().Count(),
                Untagged = repo.GetUntaggedFiles().Count(),
                Unrated = repo.SongRepository.Get(x => x.Rating == 0).Count()
            };
        }

        // GET: api/Stats/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Stats
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT: api/Stats/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
