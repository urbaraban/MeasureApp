using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.Views.Canvas;
using SureMeasure.Views.OrderPage;
using System;
using System.Collections.Generic;
using System.Globalization;

using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AngleView : ContentView, ITouchObject
    {
        private ConstraintAngle constraintAngle => (ConstraintAngle)this.BindingContext;

        private List<SheetMenuItem> commands = new List<SheetMenuItem>();

        public virtual SheetMenu SheetMenu { get => this._sheetMenu; set => this._sheetMenu = value; }
        private SheetMenu _sheetMenu;

        public AngleView()
        {
            InitializeComponent();

            this.commands.Add(new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"));
            this.commands.Add(new SheetMenuItem(SendMeasure, "{MEASURE}"));
            this.commands.Add(new SheetMenuItem(InvertAngle, "{INVERT_ANGLE}"));
            this.commands.Add(new SheetMenuItem(FreeAngle, "{FREE_ANGLE}"));
            this.commands.Add(new SheetMenuItem(Remove, "{REMOVE}"));

            this.SheetMenu = new SheetMenu(this.commands);
        }


        bool ITouchObject.ContainsPoint(Point InnerPoint) =>
    (InnerPoint.X > TranslationX
    && InnerPoint.X < TranslationX + Width
    && InnerPoint.Y > TranslationY
    && InnerPoint.Y < TranslationY + Height);

        public void TapAction() { }

        private ICommand CallValueDialog => new Command(async () =>
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(constraintAngle.Variable.Name, Math.Round(constraintAngle.Value * 180 / Math.PI, 2).ToString());
            if (callresult != null)
            {
                this.constraintAngle.Variable.Value = double.Parse(callresult) * Math.PI / 180;
            }
        });
        private ICommand SendMeasure => new Command(() =>
        {
            CadCanvasPage.MeasureVariable = this.constraintAngle.Variable;
            AppShell.BLEDevice.OnDevice();
        });
        private ICommand InvertAngle => new Command(() =>
        {
            this.constraintAngle.Value = Math.Abs((this.constraintAngle.Value - 360) % 360);
        });
        private ICommand FreeAngle => new Command(() =>
        {
            this.constraintAngle.Value = -1;
        });
        private ICommand Remove => new Command(() =>
        {
            this.constraintAngle.TryRemove();
        });
    }
}