using Xamarin.Forms;

namespace SureMeasure.ShapeObj.Interface
{
    interface ITouchObject
    {
        string ToString();

        SheetMenu SheetMenu { get; set; }

        void TapAction();

        bool ContainsPoint(Point InnerPoint);
    }
}
