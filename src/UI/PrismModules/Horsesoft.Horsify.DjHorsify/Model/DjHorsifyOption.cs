using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Data.Model.Import;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        #region Properties

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

        private RangeFilterOption<byte> _bpmRange = new RangeFilterOption<byte>(0, 0);
        public RangeFilterOption<byte> BpmRange
        {
            get { return _bpmRange; }
            set { SetProperty(ref _bpmRange, value); }
        }

        private RangeFilterOption<byte> _ratingRange = new RangeFilterOption<byte>(0, 0);
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
        #endregion

        /// <summary>
        /// Converts a string of keys separated with ',' to OpenKeyNotation selected flags
        /// </summary>
        /// <param name="musicKeys"></param>
        /// <returns></returns>
        public OpenKeyNotation ConvertKeys(string musicKeys)
        {
            OpenKeyNotation keys = OpenKeyNotation.None;
            var mKeys = musicKeys.Split(',');
            foreach (var key in mKeys)
            {                
                keys |= (OpenKeyNotation)Enum.Parse(typeof(OpenKeyNotation), key);
            }

            return keys;
        }

        /// <summary>
        /// Checks whether user has applied any filters to search on and if any options are enabled
        /// </summary>
        /// <returns></returns>
        public bool IsOptionsValid()
        {
            if (this.RatingRange.IsEnabled || this.BpmRange.IsEnabled || this.HarmonicEnabled && this.SelectedKeys != OpenKeyNotation.None)
                return true;

            if (this.SelectedFilters.Any(x => x.SearchAndOrOption == SearchAndOrOption.Or || x.SearchAndOrOption == SearchAndOrOption.And))
                return true;

            return false;
        }
    }
}
