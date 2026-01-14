using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Cinestar_app.Helpers;

namespace Cinestar_app.Converters
{
    public class QrCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string url && !string.IsNullOrEmpty(url))
                return QrCodeHelper.GenerateQrCode(url);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
