using MeasureApp.ShapeObj.LabelObject;
using System;

namespace MeasureApp.ShapeObj.Interface
{
    public interface ActiveObject
    {
        event EventHandler<object> Droped;

        SheetMenu SheetMenu { get; set; }

    }
}
