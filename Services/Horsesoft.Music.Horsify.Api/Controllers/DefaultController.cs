using Horsesoft.Music.Horsify.Repositories;
using Horsesoft.Music.Horsify.Repositories.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Horsesoft.Music.Horsify.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IHorsifySongService _horsifySongService;

        public FileController(IHorsifySongService horsifySongService)
        {
            _horsifySongService = horsifySongService;
        }

        // GET: api/Default/5
        [HttpGet("{id}", Name = "GetFile")]
        public Data.Model.File Get(int id)
        {
            //var file = _horsifySongService.GetById(id);

            var file = _horsifySongService.GetById(id);

            return file;
        }

        // POST: api/Default
        [Authorize]
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Default/5
        [Authorize]
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [Authorize]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
