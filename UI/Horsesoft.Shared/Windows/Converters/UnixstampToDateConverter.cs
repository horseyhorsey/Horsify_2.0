using System;
using System.Globalization;
using System.Windows.Data;

namespace Horsesoft.Horsify.Resource.Windows.Converters
{
    public class UnixstampToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            int unixTime = (int)value;
            return new DateTime(1970, 1, 1, 0, 0, 0, 0)
                .AddSeconds(unixTime)
                .ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
