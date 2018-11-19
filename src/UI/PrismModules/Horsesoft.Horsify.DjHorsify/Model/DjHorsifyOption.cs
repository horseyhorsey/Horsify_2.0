using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Data.Model.Import;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Horsesoft.Horsify.DjHorsify.Model
{
    public class DjHorsifyOption : BindableBase, IDjHorsifyOption
    {
        public DjHorsifyOption()
        {
            SelectedFilters = new ObservableCollection<IFilter>();
            BpmRange.Low = 110;
            BpmRange.Hi = 120;
            RatingRange.Low = 196;
            RatingRange.Hi = 255;
        }

        private bool _isEnabled = false;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private bool _harmonicEnabled;
        public bool HarmonicEnabled
        {
            get { return _harmonicEnabled; }
            set { SetProperty(ref _harmonicEnabled, value); }
        }

        private RangeFilterOption<byte> _bpmRange = new RangeFilterOption<byte>(0,0);
        public RangeFilterOption<byte> BpmRange
        {
            get { return _bpmRange; }
            set { SetProperty(ref _bpmRange, value); }
        }

        private RangeFilterOption<byte> _ratingRange = new RangeFilterOption<byte>(0,0);
        public RangeFilterOption<byte> RatingRange
        {
            get { return _ratingRange; }
            set { SetProperty(ref _ratingRange, value); }
        }

        private ObservableCollection<IFilter> _selectedFilters;
        public ObservableCollection<IFilter> SelectedFilters
        {
            get { return _selectedFilters; }
            set { SetProperty(ref _selectedFilters, value); }
        }

        private OpenKeyNotation _selectedKeys;
        public OpenKeyNotation SelectedKeys
        {
            get { return _selectedKeys; }
            set { SetProperty(ref _selectedKeys, value); }
        }
    }
}
