using System;

namespace SureMeasure.ShapeObj.Interface
{
    public interface IActiveObject
    {
        event EventHandler<bool> Draging;
        event EventHandler<object> Dropped;

        SheetMenu SheetMenu { get; set; }

    }
}
