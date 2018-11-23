using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Horsesoft.Music.Data.Model.Horsify
{
    public class HorsifySongApi : IHorsifySongApi
    {
        #region Properties / Fields
        private HttpClient _client;
        public string BaseAddress { get; set; } 
        #endregion

        #region Constructors

        /// <summary>
        /// Horsify API Helper
        /// </summary>
        /// <param name="address"></param>
        public HorsifySongApi(string address)
        {
            BaseAddress = address;
            _client = new HttpClient();
        }
        #endregion

        #region Public Methods

        public async Task<bool> DeleteFilterAsync(Filter filter)
        {            
            var response = await _client.DeleteAsync($"{BaseAddress}api/Filters/{filter.Name}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();                
                return Convert.ToBoolean(result);     
            }

            return false;
        }

        public async Task<bool> DeleteFilterSearchAsync(int? id)
        {
            var response = await _client.DeleteAsync($"{BaseAddress}/api/filterssearch/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeletePlaylistAsync(int id)
        {
            var response = await _client.DeleteAsync($"{BaseAddress}api/Playlists/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
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

        public async Task<IEnumerable<FiltersSearch>> GetSavedSearchFiltersAsync()
        {
            var response = GetResponse($@"api/filterssearch/").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<FiltersSearch>>(result);
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

        public async Task InsertOrUpdatePlaylistsAsync(Playlist[] playlists)
        {
            var id = playlists?[0].Id;
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(playlists[0]), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseAddress}/api/playlists/{id}", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
            }

        }

        public async Task<bool> InsertSavedSearchFiltersAsync(FiltersSearch searchFilter)
        {
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(searchFilter), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseAddress}/api/filterssearch/", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<AllJoinedTable>> SearchAsync(string term, SearchType searchTypes)
        {
            term = $"api/songs/search?term={term.Replace("&", "&amp;")}";
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
            //Apply filters if any available
            string term = $"api/songs/SearchFilters?";
            term += $"randomAmount={randomAmount}";
            term += $"&maxAmount={maxAmount}";
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(searchFilter);
            //var filter = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchFilter>(json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseAddress}{term}", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<AllJoinedTable>>(result);
            }

            return null;

        }

        public async Task UpdateFilterAsync(long id, Filter filterToUpdate)
        {
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(filterToUpdate), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseAddress}/api/filters/{id}", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                    
            }
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

        public async Task<bool> UpdateSavedSearchFiltersAsync(FiltersSearch filter)
        {
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseAddress}/api/filterssearch/{filter.Id}", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        } 
        #endregion

        #region Private Methods
        private Task<HttpResponseMessage> GetResponse(string url)
        {
            return _client.GetAsync(BaseAddress + url);
        } 
        #endregion
    }
}
