using System.Collections.Generic;
using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Repositories.Services;
using Microsoft.AspNetCore.Mvc;

namespace Horsesoft.Music.Horsify.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        private IHorsifySongService _horsifySongService;

        public FiltersController(IHorsifySongService horsifySongService)
        {
            _horsifySongService = horsifySongService;
        }

        // GET: api/Filter
        [HttpGet]
        public IEnumerable<Filter> Get()
        {
            return _horsifySongService.GetFilters();
        }

        // GET: api/Filter/5
        [HttpGet("{id}", Name = "Get")]
        public Filter Get(int id)
        {
            return _horsifySongService.GetFilter(id);
        }

        // POST: api/Filter
        [HttpPost]
        public void Post([FromBody] Filter filter)
        {
            _horsifySongService.InsertFilter(filter);
        }

        // PUT: api/Filter/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Filter filter)
        {
            _horsifySongService.UpdateFilter(filter);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{filter}")]
        public void Delete(Filter filter)
        {
            _horsifySongService.RemoveFilter(filter);
        }
    }
}
