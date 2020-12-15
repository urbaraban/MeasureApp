using MeasureApp.ShapeObj.LabelObject;
using System;

namespace MeasureApp.ShapeObj.Interface
{
    public interface CanvasObject
    {
        event EventHandler Removed;

        /// <summary>
        /// Update visual layout;
        /// </summary>
        void Update();
    }
}
