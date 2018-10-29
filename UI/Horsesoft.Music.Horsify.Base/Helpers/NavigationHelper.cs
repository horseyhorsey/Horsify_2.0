using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Data.Model.Horsify;
using Prism.Regions;
using System;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.Base.Helpers
{
    /// <summary>
    /// Helper to create NavigationParamerters for the RegionManager
    /// </summary>
    public class NavigationHelper
    {
        /// <summary>
        /// Creates a search filter for navigation.
        /// </summary>
        /// <param name="searchType">Type of the search.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <returns></returns>
        public static NavigationParameters CreateSearchFilterNavigation(SearchType searchType, string searchTerm)
        {
            var filter = new SearchFilter
            {
                Filters = new List<Data.Model.Horsify.HorsifyFilter>()
                                {
                                    new Data.Model.Horsify.HorsifyFilter{SearchType = searchType,
                                        Filters = new List<string>(){searchTerm},
                                        SearchAndOrOption = SearchAndOrOption.Or
                                }}
            };

            var navParams = new NavigationParameters();
            navParams.Add("search_filter", filter);
            return navParams;
        }

        public static NavigationParameters CreateSearchFilterNavigation(SearchFilter searchFilter)
        {
            var navParams = new NavigationParameters();
            navParams.Add("search_filter", searchFilter);
            return navParams;
        }


        public static NavigationParameters CreateSearchFilterNavigation(Data.Model.Horsify.HorsifyFilter filter)
        {
            var searchFilter = new SearchFilter
            {                
                Filters = new List<Data.Model.Horsify.HorsifyFilter>()
                {
                    filter
                }
            };

            return CreateSearchFilterNavigation(searchFilter);
        }

        public static NavigationParameters CreateSearchFilterNavigation(Data.Model.Horsify.IFilter filter)
        {
            var searchFilter = new SearchFilter
            {
                Filters = new List<HorsifyFilter>()
                {
                    new HorsifyFilter()
                    {
                        FileName = filter.FileName,
                        Filters = filter.Filters,
                        SearchAndOrOption = SearchAndOrOption.And,
                        SearchType = filter.SearchType,
                        Id= filter.Id
                    }
                }
            };

            return CreateSearchFilterNavigation(searchFilter);
        }

        public static NavigationParameters CreateSearchFilterNavigation(AllJoinedTable allJoinedTable, string searchType)
        {
            SearchType type = (SearchType)Enum.Parse(typeof(SearchType), searchType);
            string searchTerm = allJoinedTable.GetType().GetProperty(searchType).GetValue(allJoinedTable).ToString();

            var filter = new SearchFilter
            {                
                Filters = new List<Data.Model.Horsify.HorsifyFilter>()
                                {
                                    new Data.Model.Horsify.HorsifyFilter{SearchType = type, SearchAndOrOption = SearchAndOrOption.Or,
                                        Filters = new List<string>(){
                                        searchTerm
                                    }
                                }}                
            };

            var navParams = new NavigationParameters();
            navParams.Add("search_filter", filter);
            return navParams;
        }
    }
}
