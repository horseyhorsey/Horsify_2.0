using Horsesoft.Music.Data.Model.Horsify;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.Repositories
{
    public interface IHorsifyDataTableRepo
    {
        /// <summary>
        /// Gets the all the distinct entries for a certian table
        /// </summary>
        /// <param name="searchType">Type of the search.</param>
        /// <returns></returns>
        IEnumerable<string> GetEntries(SearchType searchType, char firstChar);

        IEnumerable<string> GetEntries(SearchType searchType, string searchTerm, short maxAmount = -1);
    }
}
