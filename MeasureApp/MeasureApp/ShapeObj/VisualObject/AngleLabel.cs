using SureMeasure.CadObjects;
using SureMeasure.CadObjects.Constraints;
using SureMeasure.ShapeObj.Canvas;
using SureMeasure.Tools;
using SureMeasure.View.OrderPage;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace SureMeasure.ShapeObj
{
    public class AngleLabel : ConstraitLabel
    {
        private ICommand CallValueDialog => new Command(async () => 
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(_angleConstrait.Variable.Name, _angleConstrait.Value.ToString());
            this._angleConstrait.Variable.Value = double.Parse(callresult);
        });
        private ICommand Measure => new Command(async () =>
        {
            CadCanvasPage.MeasureVariable = this._angleConstrait.Variable;
            AppShell.BLEDevice.OnDevice();
        });
        private ICommand InvertAngle => new Command(async () =>
        {
            this._angleConstrait.Value = Math.Abs((this._angleConstrait.Value - 360) % 360);
        });
        private ICommand FreeAngle => new Command(async () =>
        {
            this._angleConstrait.Variable.Value = -1;
        });

        private List<SheetMenuItem> commands = new List<SheetMenuItem>();


        private ConstraintAngle _angleConstrait;
        public AngleLabel(ConstraintAngle AngleConstrait) : base(AngleConstrait)
        {
            this._angleConstrait = AngleConstrait;
            CadPoint point = Sizing.GetPositionLineFromAngle(this._angleConstrait.Point1, this._angleConstrait.Point2, 10, this._angleConstrait.Value / 2d);
            this.TranslationX = point.OX;
            this.TranslationY = point.OY;
            this.Text = Math.Round(this._angleConstrait.Value, 1).ToString();
            this.BackgroundColor = Color.Yellow;
            this.ScaleY = -1;
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.End;
            this.BackgroundColor = Color.Yellow;
            this._angleConstrait.PropertyChanged += AngleConstrait_PropertyChanged;
            CadCanvas.RegularSize += CadCanvas_RegularSize;

            this.commands.Add(new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"));
            this.commands.Add(new SheetMenuItem(Measure, "{MEASURE}"));
            this.commands.Add(new SheetMenuItem(InvertAngle, "{INVERT_ANGLE}"));
            this.commands.Add(new SheetMenuItem(FreeAngle, "{FREE_ANGLE}"));

            this.SheetMenu = new SheetMenu(this.commands);
        }

        private void AngleConstrait_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CadPoint point = Sizing.GetPositionLineFromAngle(this._angleConstrait.Point1, this._angleConstrait.Point2, 20 * this.Scale, this._angleConstrait.Value / 2d - 360);
            this.TranslationX = point.OX;
            this.TranslationY = point.OY;
        }

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.Scale = 1 /e;
        }
    }
}
