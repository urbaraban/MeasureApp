using MeasureApp.ShapeObj.LabelObject;
using System;

namespace MeasureApp.ShapeObj.Interface
{
    public interface MainObject
    {
        event EventHandler<bool> Selected;
        event EventHandler<bool> Fixed;
        event EventHandler<bool> Supported;

        string Name { get; }

        /// <summary>
        /// Selection object stat
        /// </summary>
        bool IsSelect { get; set; }

        /// <summary>
        /// Fixion object stat
        /// </summary>
        bool IsFix { get; set; }

        /// <summary>
        /// Support object stat
        /// </summary>
        bool IsSupport { get; set; }

    }
}
