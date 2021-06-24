using System;
using Xamarin.Forms;

namespace SureMeasure.ShapeObj.Interface
{
    public interface IStatusObject
    {
        bool IsSelect { get; set; }

        bool IsFix { get; set; }

        bool IsBase { get; set; }

        bool IsSupport { get; set; }

        public ObjectStatus ObjectStatus { get; }
    }

    public enum ObjectStatus
    {
        Regular,
        Base,
        Select,
        Fix,
        Support
    }
}
