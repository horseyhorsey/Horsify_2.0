using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Horsesoft.Horsify.Resource.Windows.Converters
{
    public class StringToBitmapConverter : IValueConverter
    {
        //BitmapImage EmptyBitmap = new BitmapImage(new Uri((string)Environment.CurrentDirectory + @"\Images\Ho.jpg"));
        BitmapImage EmptyBitmap = null;

        /// <summary>
        /// Converts string to Bitmap image
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return EmptyBitmap;

            try
            {
                if (File.Exists((string)value))
                {
                    var uri = new Uri((string)value);
                    var bmp = new BitmapImage(uri);
                    //bmp.DecodePixelHeight = 250;
                    //bmp.DecodePixelWidth = 250;
                    bmp.Freeze();
                    return bmp;
                }
            }
            catch { }
            return EmptyBitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
