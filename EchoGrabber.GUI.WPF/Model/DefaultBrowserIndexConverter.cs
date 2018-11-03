using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EchoGrabber.GUI.WPF.Model
{
    public class DefaultBrowserIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brs = value as ICollectionView;
            foreach (var item in brs)
            {
                var bi = item as BrowserInfo;
                if (bi.IsDefault)
                {
                    return brs.CurrentPosition;
                }
            }
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
