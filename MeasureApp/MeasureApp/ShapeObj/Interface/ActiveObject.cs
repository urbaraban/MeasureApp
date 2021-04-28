using System;

namespace SureMeasure.ShapeObj.Interface
{
    public interface ActiveObject
    {
        event EventHandler<object> Dropped;

        SheetMenu SheetMenu { get; set; }

    }
}
