using MeasureApp.ShapeObj.LabelObject;
using System;

namespace MeasureApp.ShapeObj.Interface
{
    public interface CanvasObject
    {
        event EventHandler<bool> Selected;

        SheetMenu SheetMenu { get; set; }

        /// <summary>
        /// Update visual layout;
        /// </summary>
        void Update();

        void RunSheetMenuCommand(string CommandName);

        void TapManager();
    }
}
