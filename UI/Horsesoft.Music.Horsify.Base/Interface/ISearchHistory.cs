using Horsesoft.Music.Data.Model.Horsify;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface ISearchHistoryProvider
    {
        IList<ISearchHistory> RecentSearches { get; set; }
    }
}
