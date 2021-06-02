using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace SureMeasure.View.Canvas
{

    public class CadObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CadPoint cadPoint)
            {
                return new VisualAnchor(cadPoint)
                {
                    Scale = 1 / 1,
                };
            }
            if (value is ConstraintLenth lenthConstrait)
            {
                return new VisualLine(lenthConstrait)
                {
                    StrokeThickness = 5 * 1 / 1,
                };
            }
            if (value is ConstraintAngle angleConstrait)
            {
                return new AngleLabel(angleConstrait);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
