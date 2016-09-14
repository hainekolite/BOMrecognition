using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BomRainB.Views.Converters
{
    public class SideBarWidthConverter : IValueConverter
    {
        private const int SIDE_BAR_MIN_WIDTH = 48;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? double.NaN : SIDE_BAR_MIN_WIDTH;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
