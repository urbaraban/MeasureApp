using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureApp.CadObjects.Interface
{
    public interface CadObject
    {
        event EventHandler<bool> Removed;
        event EventHandler<bool> Selected;
        event EventHandler<bool> Supported;
        event EventHandler<bool> LastObject;

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

        void MakeLast();
    }
}
