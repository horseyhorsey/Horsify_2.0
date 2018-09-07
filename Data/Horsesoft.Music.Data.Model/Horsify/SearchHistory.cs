namespace Horsesoft.Music.Data.Model.Horsify
{
    public class SearchHistory : ISearchHistory
    {
        public ISearchFilter Filter { get; set; }
        public int? ResultCount { get; set; }
    }

    public interface ISearchHistory
    {
        ISearchFilter Filter { get; set; }
        int? ResultCount { get; set; }
    }
}
