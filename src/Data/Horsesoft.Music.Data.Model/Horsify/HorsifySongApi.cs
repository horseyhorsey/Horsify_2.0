using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Horsesoft.Music.Data.Model.Horsify
{
    public class HorsifySongApi : IHorsifySongApi
    {
        public string BaseAddress { get; set; }
        private HttpClient _client;

        public HorsifySongApi(string address)
        {
            BaseAddress = address;

            _client = new HttpClient();            
        }

        public async Task<IEnumerable<AllJoinedTable>> ExtraSearch(ExtraSearchType extraSearchType)
        {
            HttpResponseMessage response = null;

            switch (extraSearchType)
            {
                case ExtraSearchType.None:
                    return null;
                case ExtraSearchType.MostPlayed:
                    response = await GetResponse(@"api/songs/mostplayed");
                    break;
                case ExtraSearchType.RecentlyAdded:
                    response = await GetResponse(@"api/songs/recentadded");
                    break;
                case ExtraSearchType.RecentlyPlayed:
                    response = await GetResponse(@"api/songs/playedrecent");
                    break;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<AllJoinedTable>>(result);
            }

            return null;
        }

        public async Task<IEnumerable<AllJoinedTable>> SearchAsync(string term, SearchType searchTypes)
        {
            term = $"api/songs/search?term={term.Replace("&","&amp;")}";
            term = searchTypes != SearchType.All ? $@"{term}?{searchTypes}" : term;

            var response = await GetResponse(term);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<AllJoinedTable>>(result);
            }

            return null;
        }

        public async Task<IEnumerable<AllJoinedTable>> SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1)
        {
            //TODO MusicKeys query

            //Apply filters if any available
            string term = $"api/songs/searchFilter?";
            var filters = searchFilter.Filters;            
            if (filters?.Count() > 0)
            {
                for (int i = 0; i < filters.Count(); i++)
                {
                    var filter = filters.ElementAt(0);
                    term += $"filters[{i}]={filter.Filters[0].Replace("%","*").Replace(" ","%20%").Replace("&", "%26%")}";
                }                
            }

            if (term.Contains("filters"))
                term += "&";

            //Apply rating query
            if (searchFilter.RatingRange != null && searchFilter.RatingRange.IsEnabled)
            {
                term += $"rating[0]={searchFilter.RatingRange.Low}&rating[1]={searchFilter.RatingRange.Hi}";
            }

            //Apply bpm query
            if (searchFilter.BpmRange != null && searchFilter.BpmRange.IsEnabled)
            {
                if (searchFilter.RatingRange != null && searchFilter.RatingRange.IsEnabled)
                {
                    term += "&";
                }

                term += $"bpm[0]={searchFilter.BpmRange.Low}&bpm[1]={searchFilter.BpmRange.Hi}";
            }

            term += $"&randomAmount={randomAmount}";
            term += $"&maxAmount={maxAmount}";

            var response = await GetResponse(term);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<AllJoinedTable>>(result);
            }

            return null;

        }

        public async Task<AllJoinedTable> GetById(int id)
        {
            var response = await GetResponse($@"api/songs/getbyid/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<AllJoinedTable>(result);
            }

            return null;
        }

        private Task<HttpResponseMessage> GetResponse(string url)
        {
            return _client.GetAsync(BaseAddress + url);
        }

        public IEnumerable<string> GetEntries(SearchType searchType, char firstChar)
        {
            return GetEntries(searchType, firstChar.ToString());
        }

        public IEnumerable<string> GetEntries(SearchType searchType, string searchTerm, short maxAmount = -1)
        {
            //return _horsifySongService.GetAllFromTableAsStrings(searchType, searchTerm, maxAmount);            
            var response = GetResponse($@"api/songs/GetStringEntries?searchType={searchType}&search={searchTerm}&maxAmount={maxAmount}").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<string>>(result);
            }

            return null;
        }

        public async Task<IEnumerable<AllJoinedTable>> GetSongsFromPlaylistAsync(Playlist playlist)
        {
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(playlist), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseAddress}api/playlists/getsongs/", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<AllJoinedTable>>(result);
            }

            return null;
        }

        public async Task<bool> UpdatePlayedSongAsync(int id, int? rating)
        {
            //Set last played and new rating
            var term = $"api/songs/UpdateSong?id={id}";
            if (rating.HasValue)
                term += $"&rating={rating.Value}";

            var response = await GetResponse(term);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsByteArrayAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Filter>> GetFilters()
        {
            var response = GetResponse($@"api/filters/").Result;            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Filter>>(result);
            }

            return null;
        }

        public async Task<IEnumerable<Playlist>> GetPlaylists()
        {
            var response = GetResponse($@"api/playlists/").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Playlist>>(result);
            }

            return null;
        }
        
        public async Task<bool> InsertFilterAsync(Filter filter)
        {
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseAddress}/api/filters/", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public void UpdateFilter(long id, Filter filterToUpdate)
        {
            var response = GetResponse($@"api/filters/{id}?filter={filterToUpdate}").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
            }
        }

        public async Task InsertOrUpdatePlaylistsAsync(Playlist[] playlists)
        {
            var id = playlists?[0].Id;
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(playlists[0]), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseAddress}/api/playlists/{id}", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {                
            }

        }
    }
}
