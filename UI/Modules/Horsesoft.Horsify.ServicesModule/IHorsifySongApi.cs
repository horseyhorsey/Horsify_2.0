using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.ServicesModule
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
                    response = await GetResponse(@"/api/songs/mostplayed");
                    break;
                case ExtraSearchType.RecentlyAdded:
                    response = await GetResponse(@"/api/songs/recentadded");
                    break;
                case ExtraSearchType.RecentlyPlayed:
                    response = await GetResponse(@"/api/songs/playedrecent");
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
            term = $"/api/songs/search?term={term}";
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
            //TODO BPM
            string term = $"/api/songs/searchFilter?";
            var filters = searchFilter.Filters;
            
            if (filters?.Count() > 0)
            {
                for (int i = 0; i < filters.Count(); i++)
                {
                    var filter = filters.ElementAt(0);
                    term += $"filters[{i}]={filter.Filters[0]}&";
                }                
            }

            if (searchFilter.RatingRange != null)
            {
                term += $"rating[0]={searchFilter.RatingRange.Low}&rating[1]={searchFilter.RatingRange.Hi}";
            }

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
            //return _horsifySongService.GetAllFromTableAsStrings(searchType, firstChar.ToString(), -1);
            return null;
        }

        public IEnumerable<string> GetEntries(SearchType searchType, string searchTerm, short maxAmount = -1)
        {
            //return _horsifySongService.GetAllFromTableAsStrings(searchType, searchTerm, maxAmount);
            return null;
        }

        public async Task<IEnumerable<AllJoinedTable>> GetSongsFromPlaylistAsync(Playlist playlist)
        {
            var response = await GetResponse($@"api/songs/playlist/{playlist.Id}");
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
            var term = $"api/songs/update?id={id}";
            if (rating.HasValue)
                term += $"rating={rating.Value}";

            var response = await GetResponse(term);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsByteArrayAsync();
                return true;
            }

            return false;
        }

        //TODO
        public void InsertFilter(Filter filter)
        {
            var response = GetResponse($@"api/filters/");
        }
    }
}
