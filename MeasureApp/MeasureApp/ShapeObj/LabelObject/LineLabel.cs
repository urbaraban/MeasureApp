using MeasureApp.ShapeObj.Constraints;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.LabelObject
{
    public class LineLabel : ConstraitLabel
    {


        private LenthAnchorAnchor _lenthAnchor;
        private CadLine _cadLine;
        public LineLabel(CadLine cadLine) : base(cadLine.AnchorsConstrait.Variable)
        {
            this._cadLine = cadLine;
            this._lenthAnchor = cadLine.AnchorsConstrait;
            this.Text = this._lenthAnchor.Lenth.ToString();
            this.ScaleY = -1;
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.End;
            this.BackgroundColor = Color.Green;
            this._lenthAnchor.Changed += LenthAnchorAnchor_Changed;

            this.sheetMenu = new SheetMenu(new List<string> { 
                "Call value", 
                "Temp line",
                "Vertical",
                "Horizontal",
                "Free"
            });

            this.sheetMenu.SheetMenuClosed += SheetMenu_SheetMenuClosed;
            this.sheetMenu.ReturnedValue += SheetMenu_ReturnedValue;
        }

        private void SheetMenu_ReturnedValue(object sender, string e)
        {
            this.Variable.Value = double.Parse(e);
        }

        private void SheetMenu_SheetMenuClosed(object sender, string e)
        {
            switch (e)
            {
                case "Call value":
                    this.ChangeValueDialog();
                    break;
                case "Temp line":
                    this._cadLine.TempLine = !this._cadLine.TempLine;
                break;
                case "Vertical":
                        this._lenthAnchor.Orientation = EnumLibrary.Orientaton.Vertical;
                    break;
                case "Horizontal":
                    this._lenthAnchor.Orientation = EnumLibrary.Orientaton.Horizontal;
                    break;
                case "Free":
                    this._lenthAnchor.Orientation = EnumLibrary.Orientaton.OFF;
                    break;
            }
        }

        private void LenthAnchorAnchor_Changed(object sender, System.EventArgs e)
        {
            this.TranslationX = (this._lenthAnchor.Anchor2.X + this._lenthAnchor.Anchor1.X) / 2;
            this.TranslationY = (this._lenthAnchor.Anchor2.Y + this._lenthAnchor.Anchor1.Y) / 2;
        }
    }
}
