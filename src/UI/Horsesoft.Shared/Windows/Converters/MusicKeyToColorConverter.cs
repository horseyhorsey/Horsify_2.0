using Horsesoft.Music.Data.Model.Import;
using Horsesoft.Music.Data.Model.Tags;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Horsesoft.Horsify.Resource.Windows.Converters
{
    /// <summary>
    /// Converts a <see cref="OpenKeyNotation"/> to corresponding color.
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class MusicKeyToColorConverter : IValueConverter
    {
        public MusicKeyToColorConverter()
        {            
            //var s = new OpenKeyColors();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OpenKeyNotation key = OpenKeyNotation.None;
            Enum.TryParse((string)value, out key);

            if (OpenKeyColors.OpenKeyColorsDict.Any(x => x.Key == key))
            {
                return OpenKeyColors.OpenKeyColorsDict.First(x => x.Key == key).Value;
            }

            return "Red";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
