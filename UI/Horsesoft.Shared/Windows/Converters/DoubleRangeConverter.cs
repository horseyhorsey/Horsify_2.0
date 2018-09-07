using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Horsesoft.Horsify.Resource.Windows.Converters
{
    /// <summary>
    /// Converts from one range to another
    /// </summary>
    public class DoubleRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var doubleArray = ((string)value).Split('|');
                //Value needs to be current songtime, parameter is the song duration -- In seconds
                return ConvertRange(
                  0,
                  double.Parse(doubleArray[1]),
                  //Angle
                  22, 42,
                  double.Parse(doubleArray[0])
                  );
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
                    double minAngle,
                    double maxAngle,
                    double minValue,
                    double maxValue,
                    double value)
        {
            var originalRange = maxAngle - minAngle;
            var newRange = maxValue - minValue;
            var ratio = newRange / originalRange;
            var newValue = value * ratio;
            var finalValue = newValue + minValue;
            return finalValue;
        }
    }
}
