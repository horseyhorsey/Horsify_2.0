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
    public class FiltersSearchController : ControllerBase
    {
        private IHorsifySongService _horsifySongService;

        public FiltersSearchController(IHorsifySongService horsifySongService)
        {
            _horsifySongService = horsifySongService;
        }

        // GET: api/FiltersSearch
        [HttpGet]
        public Task<IEnumerable<FiltersSearch>> Get()
        {
            return _horsifySongService.GetFilterSearchesAsync();
        }

        // GET: api/FiltersSearch/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/FiltersSearch
        [HttpPost]
        public Task<bool> Post([FromBody] FiltersSearch filtersSearch)
        {
            return _horsifySongService.InsertFilterSearchAsync(filtersSearch);
        }

        //// PUT: api/FiltersSearch/5
        [HttpPut("{id}")]
        public Task Put(int id, [FromBody] FiltersSearch filtersSearch)
        {
            return _horsifySongService.UpdateFilterSearchAsync(filtersSearch);
        }

        //// DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public Task<bool> Delete(int id)
        {
            return _horsifySongService.DeleteFilterSearchAsync(id);
        }
    }
}
