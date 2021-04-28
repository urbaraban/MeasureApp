using System;

namespace SureMeasure.ShapeObj.Interface
{
    public interface CanvasObject
    {
        event EventHandler<bool> Removed;

        /// <summary>
        /// Update visual layout;
        /// </summary>
        void Update();


    }
}
