using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Data.Model.Import;
using System.Collections.ObjectModel;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IDjHorsifyOption
    {
        bool IsEnabled { get; set; }
        int Amount { get; set; }
        ObservableCollection<IFilter> SelectedFilters { get; set; }
        RangeFilterOption<byte> BpmRange { get; set; }
        RangeFilterOption<byte> RatingRange { get; set; }
        OpenKeyNotation SelectedKeys { get; set; }
    }
}
