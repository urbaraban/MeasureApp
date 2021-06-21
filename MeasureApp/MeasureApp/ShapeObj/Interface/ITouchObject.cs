using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SureMeasure.ShapeObj.Interface
{
    interface ITouchObject
    {
        SheetMenu SheetMenu { get; set; }

        void TapAction();

        bool ContainsPoint(Point InnerPoint);
    }
}
