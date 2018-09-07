using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Horsesoft.Music.Horsify.RepositoryTests
{
    public class SqliteTests : IClassFixture<SqliteFixture>
    {
        SqliteFixture _fixture;

        public SqliteTests(SqliteFixture sqliteFixture)
        {
            _fixture = sqliteFixture;
        }

        [Theory(Skip = "Integration")]
        [InlineData(SearchType.All, "198%")]
        [InlineData(SearchType.All, "%SKC%")]
        [InlineData(SearchType.Artist, "%Jackson%")]
        [InlineData(SearchType.Artist | SearchType.Album, "%SKC%|%Noisia%")]
        [InlineData(SearchType.Bpm, "17%")]
        [InlineData(SearchType.Genre, "%Neurofunk%")]
        [InlineData(SearchType.FileLocation, "%Noisia%")]
        [InlineData(SearchType.Title, "%Noisia%")]
        [InlineData(SearchType.Year, "198%")]
        [InlineData(SearchType.FileLocation, "%SKC%|%Noisia%")]
        public void Repo_SearchLikeWithSearchType(SearchType searchType, string searchTerms = null)
        {
            var repo = _fixture._HorsifyDataSqliteRepo;
            var searchString = repo.SearchLike(searchType, searchTerms);
            var songs = repo.ExecuteSearchLike(searchString, -1);

            Assert.True(songs.GetEnumerator().MoveNext());
        }

        [Theory(Skip = "Integration")]
        //[InlineData(SearchType.All, "ub40|noisia")]
        [InlineData(SearchType.All, "%can't%")]
        //[InlineData(SearchType.All, "%STIFF LITTLE%")]
        //[InlineData(SearchType.Year, "1992")]
        public void Repo_SearchLikeWithSearchType_2(SearchType searchType, string searchTerms = null)
        {
            var repo = _fixture._HorsifyDataSqliteRepo;
            var searchString = repo.SearchLike(searchType, searchTerms);
            var songs = repo.ExecuteSearchLike(searchString, -1);

            Assert.True(songs.GetEnumerator().MoveNext());
        }

        [Theory(Skip = "Integration")]
        [InlineData(new byte[] { 105, 255 })]
        [InlineData(new byte[] { 0, 105 })]
        [InlineData(new byte[] { 0, 105 }, new byte[] { 150, 175 })]
        [InlineData(null, null, "Artist:Noisia;Dlr", "Label:Renegade Hardware")]
        [InlineData(null, null, "Label:Renegade Hardware")]
        public void Repo_GetSongsFromSavedFilters(byte[] rating, byte[] bpmRange = null, params string[] filters)
        {
            var repo = _fixture._HorsifyDataSqliteRepo;
            ISearchFilter searchFilter = new SearchFilter();

            //Create a filter from incoming string
            if (filters != null)
            {
                try
                {
                    var tempFilters = new List<Data.Model.Horsify.HorsifyFilter>();
                    foreach (var filterString in filters)
                    {
                        tempFilters.Add(GetFilterFromString(filterString));
                    }

                    searchFilter.Filters = tempFilters;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (bpmRange != null)
                searchFilter.BpmRange = new RangeFilterOption<byte>(bpmRange[0], bpmRange[1]);

            if (rating != null)
                searchFilter.RatingRange = new RangeFilterOption<byte>(rating[0], rating[1]);

            var songs = repo.GetSongsFromSavedFilters(searchFilter)?.ToList();

            Assert.True(songs.Count > 0);
        }

        [Theory(Skip = "Integration")]
        //[InlineData(new byte[] { 105, 255 } , null, "Label:Renegade Hardware")]
        //[InlineData(null, null, "Genre:%Neurofunk%")]
        //[InlineData(null, null, "Genre:%Pop%")]
        //[InlineData(new byte[] { 255, 255 }, null, "Label:%Renegade Hardware%;%Subtitles%", "Artist:Plan B")]
        [InlineData(new byte[] { 196, 255 }, null, "Label:Renegade Hardware")]
        //[InlineData(null, null, "Year:1982;1984")]
        //[InlineData(null, null, "Artist:Plan B")]
        public void Repo_GetSongsFromSavedFilters_Faster(byte[] rating, byte[] bpmRange = null, params string[] filters)
        {
            //Setup
            var repo = _fixture._HorsifyDataSqliteRepo;
            SearchFilter searchFilter = new SearchFilter();
            CreateTestFilters(filters, searchFilter);
            InitTestRanges(rating, bpmRange, searchFilter);

            //SearchString
            var sqlString = repo.SearchLike(searchFilter);

            //Execute string
            var songs = repo.ExecuteSearchLike(sqlString, 10);

            Assert.NotNull(songs);
            Assert.True(songs.GetEnumerator().MoveNext());
        }

        [Fact(Skip = "Integration")]
        public void Repo_GetNullSongTags()
        {
            var repo = _fixture._HorsifyDataSqliteRepo;

            var allUntaggedSongs = repo.GetUntaggedFiles();

            var untaggedCount = allUntaggedSongs.Count();
        }

        /// <summary>
        /// TODO: Needs to be quicker
        /// </summary>
        /// <param name="searchType">Type of the search.</param>
        /// <param name="searchTerms">The search terms.</param>
        /// <param name="amount">The amount.</param>
        [Theory(Skip = "Integration")]
        [InlineData(SearchType.All, "%Renegade Hardware%|%Subtitle%", 0)]
        //[InlineData(SearchType.Artist, "%Jackson%", 20)]
        //[InlineData(SearchType.Genre, "%Neurofunk%", 20)]
        public void Repo_GetRandomSongs(SearchType searchType, string searchTerms = null, short amount = 10)
        {
            var repo = _fixture._HorsifyDataSqliteRepo;
            var sqlSearchStr = repo.SearchLike(searchType, searchTerms);
            var songs = repo.ExecuteSearchLike(sqlSearchStr, amount);
            Assert.True(songs.GetEnumerator().MoveNext());
        }

        [Fact(Skip = "Integration")]
        public void GetMostPlayed()
        {
            var repo = _fixture._HorsifyDataSqliteRepo;
            var results = repo.GetMostPlayed();
        }

        [Fact(Skip = "Integration")]
        public void GetRecentlyPlayed()
        {
            var repo = _fixture._HorsifyDataSqliteRepo;
            var results = repo.GetRecentlyPlayed();
        }

        [Fact(Skip = "Integration")]
        public void GetRecentlyAdded()
        {
            var repo = _fixture._HorsifyDataSqliteRepo;
            var results = repo.GetRecentlyAdded();
        }

        [Fact(Skip = "Integration")]
        public void GetFilterRowsFromDatabase()
        {
            var filters = _fixture._HorsifyDataSqliteRepo.FilterRepository.Get();
            var filter = GetFilterFromString(filters.ElementAt(0).SearchTerms);
        }

        [Fact(Skip = "Integration")]
        public void AddFilterToDatabase()
        {
            var filter = new Data.Model.Filter();
            filter.Name = "Mehe";
            filter.SearchTerms = "Album:Thriller";

            _fixture._HorsifyDataSqliteRepo.FilterRepository.Insert(filter);
            ((IUnitOfWork)_fixture._HorsifyDataSqliteRepo).Save();
        }

        #region Private Methods
        private void CreateTestFilters(string[] filters, ISearchFilter searchFilter)
        {
            //Create a filter from incoming string
            if (filters != null)
            {
                try
                {
                    var tempFilters = new List<Data.Model.Horsify.HorsifyFilter>();
                    foreach (var filterString in filters)
                    {
                        tempFilters.Add(GetFilterFromString(filterString));
                    }

                    searchFilter.Filters = tempFilters;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the filters and search type from a string. <para/>
        /// "Artist:Noisia;Dlr" , "Label:Renegade Hardware"
        /// </summary>
        /// <param name="filterString">The filter string.</param>
        /// <returns></returns>
        private Data.Model.Horsify.HorsifyFilter GetFilterFromString(string filterString)
        {
            var splitParam = filterString.Split(':');
            var searchTypeString = splitParam[0];
            var searchTerms = splitParam[1].Split(';');

            var searchType = Enum.Parse(typeof(SearchType), searchTypeString);

            return new Data.Model.Horsify.HorsifyFilter
            {
                SearchType = (SearchType)searchType,
                Filters = searchTerms.ToList()
            };
        }

        private static void InitTestRanges(byte[] rating, byte[] bpmRange, ISearchFilter searchFilter)
        {
            if (bpmRange != null)
                searchFilter.BpmRange = new RangeFilterOption<byte>(bpmRange[0], bpmRange[1]) { IsEnabled = true};

            if (rating != null)
                searchFilter.RatingRange = new RangeFilterOption<byte>(rating[0], rating[1]) { IsEnabled = true } ;
        } 
        #endregion

    }
}
