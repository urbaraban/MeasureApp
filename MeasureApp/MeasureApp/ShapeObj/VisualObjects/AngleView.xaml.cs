using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.View.Canvas;
using SureMeasure.View.OrderPage;
using System;
using System.Collections.Generic;
using System.Globalization;

using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AngleView : ContentView
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


        private void TapGestureRecognizer_Tapped(object sender, EventArgs e) => TapManager();
        private int taps = 0;
        private bool runtimer = false;
        private void TapManager()
        {
            taps += 1;
            if (this.runtimer == false)
            {
                this.runtimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
                {
                    if (taps < 2)
                    {
                        constraintAngle.IsSelect = !constraintAngle.IsSelect;
                    }
                    else
                    {
                        this.SheetMenu.ShowMenu(this);
                    }

                    taps = 0;
                    return false; // return true to repeat counting, false to stop timer
                });
                this.runtimer = false;
            }
        }

        private ICommand CallValueDialog => new Command(async () =>
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(constraintAngle.Variable.Name, constraintAngle.Value.ToString());
            if (callresult != null)
            {
                this.constraintAngle.Variable.Value = double.Parse(callresult);
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

    public class PointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CadPoint cadPoint)
            {
                return new Point(cadPoint.X + CanvasView.ZeroPoint.X, cadPoint.Y + CanvasView.ZeroPoint.Y);
            }
            return new Point();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }

    public class RoundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round((double)value, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }

    public class LabelAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 2 + 90;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }
}