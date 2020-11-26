using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureApp.ShapeObj.LabelObject
{
    public class SheetMenu
    {
        public List<string> Buttons;
        public SheetMenu(List<string> Buttons)
        {
            this.Buttons = Buttons;
        }

        public event EventHandler<string> SheetMenuClosed;
        public event EventHandler<string> ReturnedValue;
        public void SendAction(string Action)
        {
            SheetMenuClosed?.Invoke(this, Action);
        }

        public void ReturnValue(string Value)
        {
            ReturnedValue?.Invoke(this, Value);
        }
    }
}
