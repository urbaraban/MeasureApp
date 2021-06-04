using DrawEngine.CadObjects;
using SureMeasure.View.Canvas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DotView : ContentView
    {
        public CadPoint point => (CadPoint)this.BindingContext;

        protected SheetMenu SheetMenu
        {
            get => _sheetMenu;
            set
            {
                this._sheetMenu = value;
            }
        }
        protected SheetMenu _sheetMenu;

        public DotView()
        {
            InitializeComponent();

            this.SheetMenu = new SheetMenu(new List<SheetMenuItem>()
            {
                new SheetMenuItem(Fix, "{FIX}"),
                new SheetMenuItem(Remove, "{REMOVE}"),
                new SheetMenuItem(LastPoint, "{LASTPOINT}"),
                new SheetMenuItem(BasePoint, "{BASEPOINT}"),
                new SheetMenuItem(Split, "{SPLIT}")
            });
        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (CanvasView.RunningGestureObject == this 
                || CanvasView.RunningGestureObject == null)
            {
                if (e.StatusType == GestureStatus.Started)
                {
                    CanvasView.RunningGestureObject = this;
                }
                if (e.StatusType == GestureStatus.Running)
                {
                    this.point.X += e.TotalX;
                    this.point.Y += e.TotalY;
                }
                if (e.StatusType == GestureStatus.Completed)
                {
                    CanvasView.RunningGestureObject = null;
                }
            }
        }

        #region Command
        private ICommand Fix => new Command(() =>
        {
            point.IsFix = !point.IsFix;
        });
        private ICommand Remove => new Command(() =>
        {
            point.TryRemove();
        });
        private ICommand LastPoint => new Command(() =>
        {
            point.IsSelect = true;
        });

        private ICommand BasePoint => new Command(() =>
        {
            point.IsBase = true;
        });

        private ICommand Split => new Command(() =>
        {
            point.MakeSplit();
        });
        #endregion

        private void DropGesture_Drop(object sender, DropEventArgs e)
        {
            //this.Dropped?.Invoke(this, e.Data.Properties["Object"]);
        }

        private void DragGesture_DragStarting(object sender, DragStartingEventArgs e)
        {
            e.Data.Properties.Add("Object", this);
            //Draging?.Invoke(this, true);
        }


        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TapManager();
        }

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
                        point.IsSelect = !point.IsSelect;
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
    }


    public class CircleXPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + CanvasView.ZeroPoint.X - 12;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - CanvasView.ZeroPoint.X + 12;
        }
    }

    public class CircleYPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + CanvasView.ZeroPoint.Y - 12;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - CanvasView.ZeroPoint.Y + 12;
        }
    }

    public class ScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1 / (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }
    }
}