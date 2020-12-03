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
    public class AngleLabel : ConstraitLabel
    {
        private AngleConstrait _angleConstrait;
        public AngleLabel(AngleConstrait AngleConstrait) : base(AngleConstrait.Variable)
        {
            this._angleConstrait = AngleConstrait;
            Point point = Sizing.GetPositionLineFromAngle(this._angleConstrait.anchorAnchor1.Anchor1.cadPoint, this._angleConstrait.anchorAnchor1.Anchor2.cadPoint, 10, this._angleConstrait.Angle.Value / 2d);
            this.TranslationX = point.X;
            this.TranslationY = point.Y;
            this.Text = this._angleConstrait.Angle.ToString();
            this.ScaleY = -1;
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.End;
            this.BackgroundColor = Color.Yellow;
            this._angleConstrait.anchorAnchor1.Anchor1.PropertyChanged += Anchor_PropertyChanged;
            this._angleConstrait.anchorAnchor1.Anchor2.PropertyChanged += Anchor_PropertyChanged;
            this._angleConstrait.anchorAnchor2.Anchor2.PropertyChanged += Anchor_PropertyChanged;
            CadCanvas.RegularSize += CadCanvas_RegularSize;
            this.sheetMenu = new SheetMenu(new List<string> { "Call value", "Invert", "Free" });

            this.ShowObjectMenu += AngleLabel_ShowObjectMenu;
        }

        private async void AngleLabel_ShowObjectMenu(object sender, SheetMenu e)
        {
            string result = await AppShell.Instance.SheetMenuDialog(e);

            switch (result)
            {
                case "Call value":
                    string callresult = await AppShell.Instance.DisplayPromtDialog(_angleConstrait.Angle.Name, _angleConstrait.Angle.Value.ToString());
                    this._angleConstrait.Angle.Value = double.Parse(callresult);
                    break;
                case "Invert":
                    this.Variable.Value = Math.Abs((this.Variable.Value - 360) % 360);
                    break;
                case "Free":
                    this._angleConstrait.Angle.Value = -1;
                    break;
            }
        }

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.Scale = 1 /e;
        }

        private void Anchor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Point point = Sizing.GetPositionLineFromAngle(this._angleConstrait.anchorAnchor1.Anchor1.cadPoint, this._angleConstrait.anchorAnchor1.Anchor2.cadPoint, 10, this._angleConstrait.Angle.Value / 2d);
            this.TranslationX = point.X;
            this.TranslationY = point.Y;
        }
    }
}
