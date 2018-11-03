using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Horsesoft.Horsify.ServicesModule
{
    public interface IHorsifySongApi
    {
        string BaseAddress { get; set; }
        /// <summary>
        /// Call the songs api extra search, like most played etc <see cref="ExtraSearchType"/>
        /// </summary>
        /// <param name="extraSearchType"></param>
        Task<IEnumerable<AllJoinedTable>> ExtraSearch(ExtraSearchType extraSearchType);

        /// <summary>
        /// Search for songs with types. Use * for wildcard likes
        /// </summary>
        /// <param name="term"></param>
        /// <param name="searchTypes"></param>
        /// <returns></returns>
        Task<IEnumerable<AllJoinedTable>> SearchAsync(string term, SearchType searchTypes);

        Task<IEnumerable<AllJoinedTable>> SearchLikeFiltersAsync(SearchFilter searchFilter, short randomAmount = 0, short maxAmount = -1);
    }

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
            string term = $"/api/songs/searchFilter?";

            if (searchFilter.RatingRange.IsEnabled)
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

        private Task<HttpResponseMessage> GetResponse(string url)
        {
            return _client.GetAsync(BaseAddress + url);
        }
    }
}
