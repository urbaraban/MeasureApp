using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureApp.ShapeObj.Interface
{
    public interface CadObject
    {
        public event EventHandler Removed;
        event EventHandler<bool> Selected;
        event EventHandler<bool> Supported;

        public void TryRemove();

        string ID { get; set; }

        /// <summary>
        /// Selection object stat
        /// </summary>
        bool IsSelect { get; set; }

        /// <summary>
        /// Support object stat
        /// </summary>
        bool IsSupport { get; set; }
    }
}
