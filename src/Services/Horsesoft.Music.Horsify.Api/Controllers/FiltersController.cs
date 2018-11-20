using System.Collections.Generic;
using System.Linq;
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
        [HttpDelete("{name}")]
        public bool Delete(string name)
        {
            var repo = _horsifySongService.GetRepo();
            var filter = (repo.FilterRepository.Get(x => x.Name == name))?.ElementAt(0);
            if (filter != null)
            {
                _horsifySongService.RemoveFilter(filter);
                return true;
            }

            return false;
        }
    }
}
