using System;
using System.Globalization;
using System.Windows.Data;

namespace Horsesoft.Horsify.Resource.Windows.Converters
{
    class RatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var currentRating = System.Convert.ToByte(value);
                var param = System.Convert.ToInt32(parameter);                

                if (currentRating >= param)
                {
                    return true;
                }
            }

            return false;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = System.Convert.ToByte(parameter);
            //var rating = (Rating)param;

            return param;
        }
    }
}
