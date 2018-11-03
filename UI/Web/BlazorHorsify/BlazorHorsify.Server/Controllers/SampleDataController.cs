using Horsesoft.Horsify.DotnetServiceHost;
using Horsesoft.Music.Data.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorHorsify.Server.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        //private IHorsifySongService _horsifySongService;

        public SampleDataController()
        {            
            //IHorsifySongService horsifySongService
            //_horsifySongService = horsifySongService;
        }

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };        

        [HttpGet("[action]")]
        public async Task<IEnumerable<AllJoinedTable>> WeatherForecasts()
        {

            //var song = Class1.horsifySongService.GetByIdAsync(564);

            //var song = _horsifySongService.GetById(5252);
            //var songs = await Class1.horsifySongService.GetSongByIdAsync(333);

            return null;
            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //});
        }
    }
}
