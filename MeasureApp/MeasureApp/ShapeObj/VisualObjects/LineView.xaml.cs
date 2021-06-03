using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.View.OrderPage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LineView : ContentView
    {
        private ConstraintLenth _lenthConstrait => (ConstraintLenth)this.BindingContext;

        public virtual SheetMenu SheetMenu { get => this._sheetMenu; set => this._sheetMenu = value; }
        private SheetMenu _sheetMenu;

        public LineView()
        {
            InitializeComponent();



            this.SheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>()
            {
                new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"),
                new SheetMenuItem(GetMeasure, "{MEASURE}"),
                new SheetMenuItem(SupportLine, "{SUPPORT_LINE}"),
                new SheetMenuItem(Verical, "{VERTICAL}"),
                new SheetMenuItem(Horizontal, "{HORIZONTAL}"),
                new SheetMenuItem(Free_Orientation, "{FREE_ORIENTATION}"),
                new SheetMenuItem(Fix, "{FIX}"),
                new SheetMenuItem(Remove, "{REMOVE}"),
                 new SheetMenuItem(Split, "{SPLIT}")
            });
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
                        _lenthConstrait.IsSelect = !_lenthConstrait.IsSelect;
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

        #region Command
        private ICommand CallValueDialog => new Command(async () =>
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(_lenthConstrait.Variable.Name, _lenthConstrait.Value.ToString());
            if (callresult != null)
            {
                this._lenthConstrait.Value = double.Parse(callresult);
            }
        });
        private ICommand GetMeasure => new Command(() =>
        {
            CadCanvasPage.MeasureVariable = this._lenthConstrait.Variable;
            AppShell.BLEDevice.OnDevice();
        });
        private ICommand SupportLine => new Command(() =>
        {
            this._lenthConstrait.IsSupport = !this._lenthConstrait.IsSupport;
        });
        private ICommand Verical => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.Vertical;
        });
        private ICommand Horizontal => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.Horizontal;
        });
        private ICommand Free_Orientation => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.OFF;
        });
        private ICommand Fix => new Command(() =>
        {
            this._lenthConstrait.Fix(true);
        });
        private ICommand Remove => new Command(() =>
        {
            this._lenthConstrait.TryRemove();
        });

        private ICommand Split => new Command(() =>
        {
            this._lenthConstrait.MakeSplit();
        });
        #endregion
    }

    public class ValueToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new LineGeometry(new Point(0, 0), new Point((double)value, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObjectStatus objectStatus)
            {
                if (objectStatus == ObjectStatus.Select) return Brush.Orange;
                if (objectStatus == ObjectStatus.Fix) return Brush.Gray;
                if (objectStatus == ObjectStatus.Regular) return Brush.Blue;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class SupportDashConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bl && bl == true)
            {
                return new DoubleCollection() { 2, 1 };
            }
            return new DoubleCollection() { 1 }; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
          return 0;
        }
    }
}