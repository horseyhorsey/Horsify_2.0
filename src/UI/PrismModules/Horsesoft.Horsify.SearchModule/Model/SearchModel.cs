using Horsesoft.Music.Data.Model.Horsify;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Horsesoft.Horsify.SearchModule.Model
{
    /// <summary>
    /// SearchModel to search with
    /// </summary>
    public class SearchModel : BindableBase
    {
        private string _searchText = string.Empty;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        private SearchType _selectedSearchType = SearchType.Title;
        public SearchType SelectedSearchType
        {
            get { return _selectedSearchType; }
            set { SetProperty(ref _selectedSearchType, value); }
        }

        private ObservableCollection<string> _results = new ObservableCollection<string>();
        public ObservableCollection<string> Results
        {
            get { return _results; }
            set { SetProperty(ref _results, value); }
        }

        private AllJoinedTables _alljoinedTables = new AllJoinedTables();
        public AllJoinedTables AllJoinedTables
        {
            get { return _alljoinedTables; }
            set { SetProperty(ref _alljoinedTables, value); }
        }
    }
}
