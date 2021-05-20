using System;

namespace SureMeasure.ShapeObj.Interface
{
    public interface ICanvasObject
    {
        event EventHandler<bool> Removed;

        /// <summary>
        /// Update visual layout;
        /// </summary>
        void Update(string Param);


    }
}
