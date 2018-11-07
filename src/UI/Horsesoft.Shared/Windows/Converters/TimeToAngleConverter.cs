using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Horsesoft.Horsify.Resource.Windows.Converters
{
    public class TimeToAngleConverter : IValueConverter
    {
        public double MinValue { get; set; } = 20;
        public double MaxValue { get; set; } = 42;

        public TimeSpan SongDuration { get; set; }
        public TimeSpan CurrentSongTime { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var zeroTime = TimeSpan.Zero;
            CurrentSongTime = (TimeSpan)value;

            return ConvertRange(zeroTime, TimeSpan.FromMinutes(5), MinValue, MaxValue, CurrentSongTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //var zeroTime = TimeSpan.Zero;
            //CurrentSongTime = (TimeSpan)value;

            return 22;

            //var currValue = (double)value;
            //return CurrentSongTime + TimeSpan.FromSeconds(20);
            //return ConvertRange(zeroTime, TimeSpan.FromMinutes(5), MinValue, MaxValue, currentTime);
        }

        /// <summary>
        /// Gets the current value from the mixValue maxValue range.        
        /// </summary>
        /// <param name="startTime">Start of the song</param>
        /// <param name="duration"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ConvertRange(
                    TimeSpan startTime,
                    TimeSpan duration,
                    double minValue,
                    double maxValue,
                    TimeSpan value)
        {
            var originalRange = duration.TotalMilliseconds - startTime.TotalMilliseconds;
            var newRange = maxValue - minValue;
            var ratio = newRange / originalRange;            
            var newValue = value.TotalMilliseconds * ratio;
            var finalValue = newValue + minValue;
            return finalValue;
        }

        /// <summary>
        /// Gets the current value from the mixValue maxValue range.        
        /// </summary>
        /// <param name="startTime">Start of the song</param>
        /// <param name="duration"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TimeSpan ConvertRangeBack(
                    double minAngle,
                    double maxAngle,
                    TimeSpan minValue,
                    TimeSpan maxValue,
                    double value)
        {
            var originalRange = maxAngle - minAngle;
            var newRange = maxValue.TotalMilliseconds - minValue.TotalMilliseconds;
            var ratio = newRange / originalRange;
            var newValue = value * ratio;
            var finalValue = newValue + minValue.TotalMilliseconds;
            return TimeSpan.FromSeconds(finalValue);
        }
    }
}
