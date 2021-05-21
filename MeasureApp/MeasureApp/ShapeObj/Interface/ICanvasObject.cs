using System;
using System.Threading.Tasks;

namespace SureMeasure.ShapeObj.Interface
{
    public interface ICanvasObject
    {
        event EventHandler<bool> Removed;

        /// <summary>
        /// Update visual layout;
        /// </summary>
        Task Update(string Param);


    }
}
