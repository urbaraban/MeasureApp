using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SureMeasure.Views.Canvas
{

    public class CadObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class OnButtonScale : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool on = (bool)value;
            return on == true ? 5 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
