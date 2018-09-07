using Prism.Mvvm;

namespace Horsesoft.Horsify.SearchModule.Model
{
    public class RecentSearch : BindableBase
    {
        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }

        private int _resultCount;
        public int ResultCount
        {
            get { return _resultCount; }
            set { SetProperty(ref _resultCount, value); }
        }
    }
}
