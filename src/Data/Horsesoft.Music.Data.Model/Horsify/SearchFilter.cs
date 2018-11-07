using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Horsesoft.Music.Data.Model.Horsify
{
    public interface IFilter
    {
        int Id { get; set; }
        string FileName { get; set; }
        List<string> Filters { get; set; }
        SearchType SearchType { get; set; }
        SearchAndOrOption SearchAndOrOption { get; set; }
    }

    /// <summary>
    /// The filter is a saved file that holds a list of string filters and a search type. The filters could be something like [1982, 1983, 1988] SearchType: Year
    /// </summary>
    [DataContract]
    public class HorsifyFilter : IFilter
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public List<string> Filters { get; set; }
        [DataMember]
        public SearchType SearchType { get; set; }
        [DataMember]
        public SearchAndOrOption SearchAndOrOption { get; set; }

        public Filter Filter { get; set; }

        public HorsifyFilter()
        {
        }

        public HorsifyFilter(Filter filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// Gets the filters and search type from a string. <para/>
        /// "Artist:Noisia;Dlr" , "Label:Renegade Hardware"
        /// </summary>
        /// <param name="filterString">The filter string.</param>
        /// <returns></returns>
        public static IFilter GetFilterFromString(string filterString, Filter filter)
        {
            var splitParam = filterString.Split(':');
            var searchTypeString = splitParam[0];
            var searchTerms = splitParam[1].Split(';');
            var searchType = Enum.Parse(typeof(SearchType), searchTypeString);

            return new HorsifyFilter(filter)
            {
                SearchType = (SearchType)searchType,
                Filters = searchTerms.ToList()
            };
        }
    }

    public interface ISearchFilter
    {
        RangeFilterOption<byte> BpmRange { get; set; }
        IEnumerable<HorsifyFilter> Filters { get; set; }
        RangeFilterOption<byte> RatingRange { get; set; }
        string MusicKeys { get; set; }
    }

    [DataContract]
    public class SearchFilter : ISearchFilter
    {
        [DataMember]
        public IEnumerable<HorsifyFilter> Filters { get; set; }
        [DataMember]
        public RangeFilterOption<byte> BpmRange { get; set; }
        [DataMember]
        public RangeFilterOption<byte> RatingRange { get; set; }
        [DataMember]
        public string MusicKeys { get; set; }

        public override bool Equals(object obj)
        {
            var sf = (obj as SearchFilter);
            if (sf != null)
            {
                if (sf.Filters?.First().Filters?.First() == this.Filters?.First().Filters?.First())
                    return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Constructors
        public SearchFilter()
        {

        }

        public SearchFilter(string quickFilter)
        {
            Filters = new List<HorsifyFilter>()
            {
                new HorsifyFilter()
                {
                    SearchType = SearchType.All,
                    Filters = new List<string>{ quickFilter}
                }
            };
        }

        public SearchFilter(string[] filters, SearchType searchType = SearchType.All)
        {
            if (filters?.Length > 0)
            {
                Filters = new List<HorsifyFilter>()
                {
                    new HorsifyFilter()
                    {
                        SearchType = searchType,
                        Filters = filters.ToList()
                    }
                };
            }
        }
        #endregion
    }

    public interface IFilterOption
    {
        bool IsEnabled { get; set; }
    }

    [DataContract]
    public class FilterOption : IFilterOption
    {
        [DataMember]
        public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// Has Low, High (Range) and IsEnabled. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="FilterOption" />
    [DataContract]
    public class RangeFilterOption<T> : FilterOption
    {
        [DataMember]
        public T Low { get; set; }
        [DataMember]
        public T Hi { get; set; }

        public RangeFilterOption(T low, T high)
        {
            Low = low;
            Hi = high;
        }
    }
}
