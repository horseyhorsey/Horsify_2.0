using System;
using System.Globalization;
using System.Windows.Data;

namespace Horsesoft.Horsify.Resource.Windows.Converters
{
    public class XmlStringToStdStringConverter : IValueConverter
    {
        /// <summary>
        /// Removes the XML symbols from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //Format XML bits to string representation.
                string outputString = value.ToString().Replace("&apos;", "'");
                return outputString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
