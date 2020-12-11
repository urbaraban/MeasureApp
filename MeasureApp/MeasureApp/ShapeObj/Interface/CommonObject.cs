using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureApp.ShapeObj.Interface
{
    interface CommonObject
    {
        event EventHandler Removed;

        /// <summary>
        /// Check for deletion. And delete;
        /// </summary>
        void TryRemove();
    }
}
