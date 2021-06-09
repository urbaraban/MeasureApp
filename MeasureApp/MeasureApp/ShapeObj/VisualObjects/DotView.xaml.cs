using DrawEngine.CadObjects;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.View.Canvas;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DotView : ContentView, IActiveObject
    {
        public CadPoint point => (CadPoint)this.BindingContext;

        public SheetMenu SheetMenu
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
            if (e.StatusType == GestureStatus.Started)
            {
                CanvasView.RunningGestureObject.Add(this);
                Console.WriteLine($"Start PanGesture Dot {sender} TouchID {e.GestureId}");
            }
            if (e.StatusType == GestureStatus.Running)
            {
                Console.WriteLine($"PanGesture Dot DeltaX {e.TotalX} DeltaY {e.TotalY}");
                this.point.X += e.TotalX;
                this.point.Y += e.TotalY;
            }
            if (e.StatusType == GestureStatus.Completed)
            {
                CanvasView.RunningGestureObject.Remove(this);
                Console.WriteLine($"Completed PanGesture Dot {sender} TouchID {e.GestureId}");
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
            object Obj = this;
            while((Obj is CanvasView) == false)
            {
                if (Obj is Element element)
                {
                    Obj = element.Parent;
                }
            }

            if (Obj is CanvasView canvasView)
            {
                Tuple<object, object> tuple = new Tuple<object, object>(this.point, e.Data.Properties["Object"]);
                canvasView.DropComplit.Execute(tuple);
            }
        }

        private void DragGesture_DragStarting(object sender, DragStartingEventArgs e)
        {
            e.Data.Properties.Add("Object", this.point);
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


}