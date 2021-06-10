using System;
using Xamarin.Forms;

namespace SureMeasure.ShapeObj.Interface
{
    public interface IActiveObject
    {
        SheetMenu SheetMenu { get; set; }

        double X { get; set; }

        double Y { get; set; }

        bool ContainsPoint(Point InnerPoint);

        void TapAction();
    }
}
