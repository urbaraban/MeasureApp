using App1;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.LabelObject
{
    public class AngleLabel : ConstraitLabel
    {
        private ICommand CallValueDialog => new Command(async () => 
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(_angleConstrait.Variable.Name, _angleConstrait.Angle.ToString());
            this._angleConstrait.Variable.Value = double.Parse(callresult);
        });
        private ICommand InvertAngle => new Command(async () =>
        {
            this.Variable.Value = Math.Abs((this.Variable.Value - 360) % 360);
        });
        private ICommand FreeAngle => new Command(async () =>
        {
            this._angleConstrait.Variable.Value = -1;
        });

        private List<SheetMenuItem> commands = new List<SheetMenuItem>();


        private ConstraintAngle _angleConstrait;
        public AngleLabel(ConstraintAngle AngleConstrait) : base(AngleConstrait.Variable)
        {
            this._angleConstrait = AngleConstrait;
            Point point = Sizing.GetPositionLineFromAngle(this._angleConstrait.anchorAnchor1.Point1, this._angleConstrait.anchorAnchor1.Point2, 10, this._angleConstrait.Angle / 2d);
            this.TranslationX = point.X;
            this.TranslationY = point.Y;
            this.Text = this._angleConstrait.Angle.ToString();
            this.ScaleY = -1;
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.End;
            this.BackgroundColor = Color.Yellow;
            this._angleConstrait.anchorAnchor1.Point1.PropertyChanged += Anchor_PropertyChanged;
            this._angleConstrait.anchorAnchor1.Point2.PropertyChanged += Anchor_PropertyChanged;
            this._angleConstrait.anchorAnchor2.Point2.PropertyChanged += Anchor_PropertyChanged;
            CadCanvas.RegularSize += CadCanvas_RegularSize;

            this.commands.Add(new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"));
            this.commands.Add(new SheetMenuItem(InvertAngle, "{INVERT_ANGLE}"));
            this.commands.Add(new SheetMenuItem(FreeAngle, "{FREE_ANGLE}"));

            this.SheetMenu = new SheetMenu(this.commands);
        }



        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.Scale = 1 /e;
        }

        private void Anchor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Point point = Sizing.GetPositionLineFromAngle(this._angleConstrait.anchorAnchor1.Point1, this._angleConstrait.anchorAnchor1.Point2, 10, this._angleConstrait.Angle / 2d);
            this.TranslationX = point.X;
            this.TranslationY = point.Y;
        }
    }
}
