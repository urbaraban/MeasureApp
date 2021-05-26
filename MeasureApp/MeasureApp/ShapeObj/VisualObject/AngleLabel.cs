using DrawEngine;
using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj.Canvas;
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
            string callresult = await AppShell.Instance.DisplayPromtDialog(angleConstrait.Variable.Name, angleConstrait.Value.ToString());
            if (callresult != null)
            {
                this.angleConstrait.Variable.Value = double.Parse(callresult);
            }
        });
        private ICommand SendMeasure => new Command(() =>
        {
            CadCanvasPage.MeasureVariable = this.angleConstrait.Variable;
            AppShell.BLEDevice.OnDevice();
        });
        private ICommand InvertAngle => new Command(() =>
        {
            this.angleConstrait.Value = Math.Abs((this.angleConstrait.Value - 360) % 360);
        });
        private ICommand FreeAngle => new Command(() =>
        {
            this.angleConstrait.Variable.Value = -1;
        });
        private ICommand Remove => new Command(() =>
        {
            this.angleConstrait.TryRemove();
        });

        private List<SheetMenuItem> commands = new List<SheetMenuItem>();


        private readonly ConstraintAngle angleConstrait;
        public AngleLabel(ConstraintAngle AngleConstrait) : base(AngleConstrait)
        {
            this.angleConstrait = AngleConstrait;
            CadPoint point = Sizing.GetPositionLineFromAngle(this.angleConstrait.Point1, this.angleConstrait.Point2, 10, this.angleConstrait.Value / 2d);
            this.TranslationX = point.OX;
            this.TranslationY = point.OY;
            this.Text = Math.Round(this.angleConstrait.Value, 1).ToString();
            this.BackgroundColor = Color.Yellow;
            this.ScaleY = -1;
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.End;
            this.BackgroundColor = Color.Yellow;
            this.angleConstrait.PropertyChanged += AngleConstrait_PropertyChanged;
            this.angleConstrait.Removed += AngleConstrait_Removed;
            CadCanvas.RegularSize += CadCanvas_RegularSize;

            this.commands.Add(new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"));
            this.commands.Add(new SheetMenuItem(SendMeasure, "{MEASURE}"));
            this.commands.Add(new SheetMenuItem(InvertAngle, "{INVERT_ANGLE}"));
            this.commands.Add(new SheetMenuItem(FreeAngle, "{FREE_ANGLE}"));
            this.commands.Add(new SheetMenuItem(Remove, "{REMOVE}"));

            this.SheetMenu = new SheetMenu(this.commands);
        }

        private void AngleConstrait_Removed(object sender, bool e)
        {
            this.TryRemove();
        }

        private void AngleConstrait_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CadPoint point = Sizing.GetPositionLineFromAngle(this.angleConstrait.Point1, this.angleConstrait.Point2, 30 * this.Scale, this.angleConstrait.Value / 2d);
            this.TranslationX = point.OX;
            this.TranslationY = point.OY;
        }

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.Scale = 1 /e;
        }
    }
}
