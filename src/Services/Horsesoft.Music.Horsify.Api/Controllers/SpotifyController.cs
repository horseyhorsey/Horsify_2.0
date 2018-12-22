using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public SpotifyController(IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        [HttpGet]
        public async Task<ChallengeResult> GetTEstAsync()
        {
            var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();

            return Challenge(new AuthenticationProperties() { RedirectUri = "/" }, "Spotify");
            
        }

        [HttpPost]
        public void TestPost(string f, string s, string a)
        {

        }
    }
}