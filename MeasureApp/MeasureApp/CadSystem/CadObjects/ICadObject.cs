using System;

namespace SureCadSystem.CadObjects
{
    public interface ICadObject
    {
        event EventHandler<bool> Removed;
        event EventHandler<bool> Selected;
        event EventHandler<bool> Supported;

        public void TryRemove();

        string ID { get; }

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
