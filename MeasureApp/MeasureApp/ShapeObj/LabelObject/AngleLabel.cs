using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.LabelObject
{
    public class AngleLabel : ConstraitLabel
    {
        private AngleBetweenThreeAnchor _angleConstrait;
        public AngleLabel(AngleBetweenThreeAnchor AngleConstrait) : base(AngleConstrait.Variable)
        {
            this._angleConstrait = AngleConstrait;
            this.Text = this._angleConstrait.Angle.ToString();
            this.ScaleY = -1;
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.End;
            this.BackgroundColor = Color.Yellow;
            this._angleConstrait.anchorAnchor1.Anchor1.PropertyChanged += Anchor_PropertyChanged;
            this._angleConstrait.anchorAnchor1.Anchor2.PropertyChanged += Anchor_PropertyChanged;
            this._angleConstrait.anchorAnchor2.Anchor2.PropertyChanged += Anchor_PropertyChanged;

            this.sheetMenu = new SheetMenu(new List<string> { "Call value", "Mirror", "Free" });

            this.sheetMenu.SheetMenuClosed += SheetMenu_SheetMenuClosed;
        }

        private void SheetMenu_SheetMenuClosed(object sender, string e)
        {
            switch (e)
            {
                case "Call value":
                    this.ChangeValueDialog();
                    break;
                case "Invert":
                    this.Variable.Value = Math.Abs((this.Variable.Value - 360) % 360);
                    break;
                case "Free":
                    this._angleConstrait.Angle.Value = -1;
                    break;
            }
        }

        private void Anchor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CadPoint cadPoint = Sizing.GetPositionLineFromAngle(this._angleConstrait.anchorAnchor1.Anchor1.cadPoint, this._angleConstrait.anchorAnchor1.Anchor2.cadPoint, 10, this._angleConstrait.Angle.Value / 2d);
            this.TranslationX = cadPoint.X;
            this.TranslationY = cadPoint.Y;
        }
    }
}
