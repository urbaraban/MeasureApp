using DrawEngine.CadObjects;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.Views.Canvas;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace SureMeasure.ShapeObj.VisualObjects
{

   public class CentrePointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + CanvasView.ZeroPoint.X - 15;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - (CanvasView.ZeroPoint.X - 15);
        }
    }



    public class FrameAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Numerics.Vector2 vector2 = (System.Numerics.Vector2)value;
            double angle = (Math.Atan2(vector2.Y, vector2.X) +  Math.PI * 2) % (Math.PI * 2);
            return angle > (Math.PI / 2) && angle < Math.PI * 1.5 ? 180 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value;
        }
    }

    public class PerpendecularAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Numerics.Vector2 vector2 = (System.Numerics.Vector2)value;
            double angle = (Math.Atan2(vector2.Y, vector2.X) + Math.PI * 2 + Math.PI / 2) % (Math.PI * 2) ;
            return angle < (Math.PI * 2) && angle > Math.PI ? 0 : 180;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value;
        }
    }

    public class AngleValueToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Color.LightBlue : Color.LimeGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value;
        }
    }

    public class ScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1 / (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }


    public class ValueToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new LineGeometry(new Point(0, 0), new Point((double)value, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObjectStatus objectStatus)
            {
                if (objectStatus == ObjectStatus.Select) return Brush.Orange;
                else if (objectStatus == ObjectStatus.Base) return Brush.Red;
                else if(objectStatus == ObjectStatus.Fix) return Brush.Gray;
                else if(objectStatus == ObjectStatus.Support) return Brush.LightGray;
                else if(objectStatus == ObjectStatus.Regular) return Brush.Blue;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class SupportDashConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bl && bl == true)
            {
                return new DoubleCollection() { 2, 1 };
            }
            return new DoubleCollection() { 1 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    public class YPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + CanvasView.ZeroPoint.Y - 7;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - CanvasView.ZeroPoint.Y;
        }
    }

    public class XPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + CanvasView.ZeroPoint.X;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - CanvasView.ZeroPoint.X;
        }
    }

    public class LineThinkessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 7 * (1 / (double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }
    public class LineHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 30 / (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }

    public class PointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CadAnchor cadPoint)
            {
                return new Point(cadPoint.Point.X + CanvasView.ZeroPoint.X, cadPoint.Point.Y + CanvasView.ZeroPoint.Y);
            }
            return new Point();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }

    public class RoundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{Math.Round((double)value, 1)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }

    public class RoundAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{Math.Round((double)value * 180 / Math.PI, 1)}°";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }

    public class AngleRotateVectorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Numerics.Vector2 vector2 = (System.Numerics.Vector2)value;
            double angle = Math.Atan2(vector2.Y, vector2.X) * (180 / Math.PI);
            return angle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }

    public class AllTrueMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (!(value is bool b) || targetTypes.Any(t => !t.IsAssignableFrom(typeof(bool))))
            {
                // Return null to indicate conversion back is not possible
                return null;
            }

            if (b)
            {
                return targetTypes.Select(t => (object)true).ToArray();
            }
            else
            {
                // Can't convert back from false because of ambiguity
                return null;
            }
        }
    }

    public class AngleLabelPosition : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class VectorToAngle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Numerics.Vector2 vector2 = (System.Numerics.Vector2)value;
            return Math.Atan2(vector2.Y, vector2.X) * (180 / Math.PI);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
