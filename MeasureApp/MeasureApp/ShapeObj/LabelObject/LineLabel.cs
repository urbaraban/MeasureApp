using App1;
using MeasureApp.ShapeObj.Canvas;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using MeasureApp.View.DrawPage;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.LabelObject
{
    public class LineLabel : ConstraitLabel
    {
        private LenthConstrait _lenthAnchor;
        private CadLine _cadLine;

        public LineLabel(CadLine cadLine) : base(cadLine.AnchorsConstrait.Variable)
        {
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.Center;

            this._cadLine = cadLine;
            this._lenthAnchor = cadLine.AnchorsConstrait;
            this.Text = this._lenthAnchor.Lenth.ToString();
            this.ScaleY = -1;

            this.BackgroundColor = Color.Green;
            this._lenthAnchor.Changed += LenthAnchorAnchor_Changed;

            CadCanvas.RegularSize += CadCanvas_RegularSize;

            this.BindingContext = cadLine.AnchorsConstrait;
            this.SetBinding(Label.TextProperty, new Binding()
            {
                Source = cadLine.AnchorsConstrait,
                Path = "Lenth",
                Mode = BindingMode.OneWay,
                Converter = new ToStringConverter()
            });

            this.Selected += LineLabel_Selected;

            this.sheetMenu = new SheetMenu(new List<string> { 
                "Call value", 
                "Temp line",
                "Vertical",
                "Horizontal",
                "Free"
            });

            this.ShowObjectMenu += LineLabel_ShowObjectMenu;

            this.Update();
        }

        private async void LineLabel_ShowObjectMenu(object sender, SheetMenu e)
        {
            string result = await AppShell.Instance.SheetMenuDialog(e);

            switch (result)
            {
                case "Call value":
                    string callresult = await AppShell.Instance.DisplayPromtDialog(_lenthAnchor.Variable.Name, _lenthAnchor.Variable.Value.ToString());
                    this._lenthAnchor.Variable.Value = double.Parse(callresult);
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

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.Scale = 1 / e;
        }

        private void LineLabel_Selected(object sender, bool e)
        {
            this._cadLine.IsSelect = !this._cadLine.IsSelect;
        }

        private void SheetMenu_ReturnedValue(object sender, string e)
        {
            this.Variable.Value = double.Parse(e);
        }

        private async void SheetMenu_SheetMenuClosed(object sender, string e)
        {

        }

        private void LenthAnchorAnchor_Changed(object sender, System.EventArgs e)
        {
            this.Update();
        }

        public void Update()
        {
            this.TranslationX = (this._lenthAnchor.Anchor2.X + this._lenthAnchor.Anchor1.X) / 2;
            this.TranslationY = (this._lenthAnchor.Anchor2.Y + this._lenthAnchor.Anchor1.Y) / 2;

            this.Rotation = Sizing.AngleHorizont(this._lenthAnchor.Anchor1.cadPoint, this._lenthAnchor.Anchor2.cadPoint);

            this.Text = Math.Round(Sizing.PtPLenth(this._lenthAnchor.Anchor1.cadPoint, this._lenthAnchor.Anchor2.cadPoint), 2).ToString();
        }
    }
}
