using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base.Interface;
using System.Collections.Generic;

namespace Horsesoft.Horsify.ServicesModule
{
    public class SearchHistoryProvider : ISearchHistoryProvider
    {
        public IList<ISearchHistory> RecentSearches { get; set; }

        public SearchHistoryProvider()
        {
            RecentSearches = new List<ISearchHistory>();
        }
    }
}
