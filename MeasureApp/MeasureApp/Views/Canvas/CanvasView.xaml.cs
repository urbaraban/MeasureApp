using DrawEngine;
using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.ShapeObj.VisualObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.Views.Canvas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CanvasView : ContentView, IActiveObject
    {
        public static Point ZeroPoint = new Point(5000, 5000);
        public static List<object> RunningGestureObject = new List<object>();

        private Contour Contour
        {
            get => _contour;
            set
            {
                if (_contour != null) _contour.CollectionChanged -= _contour_CollectionChanged;
                _contour = value;
                Refresh();
                _contour.CollectionChanged += _contour_CollectionChanged;
            }
        }

        private Contour _contour;


        public double CommonScale
        {
            get => (this.GroupLayout.Scale != double.PositiveInfinity ? this.GroupLayout.Scale : 1) / (draggingstatus ? 1.5 : 1);
            set
            {
                this.GroupLayout.Scale = value;
                OnPropertyChanged("CommonScale");
            }
        }
        private bool draggingstatus = false;

        public CanvasView()
        {
            InitializeComponent();
            this.BindingContextChanged += CanvasView_BindingContextChanged;

            this.SheetMenu = new SheetMenu(new List<SheetMenuItem>()
            {

            });
        }

        bool IActiveObject.ContainsPoint(Point InnerPoint) => true;

        /// <summary>
        /// Add object on Canvas. 
        /// </summary>
        /// <param name="Object"></param>
        private async Task<object> Add(object Object)
        {
            if (Object is ICanvasObject canvasObject)
            {
                canvasObject.Removed += CanvasObject_Removed;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Object is LineView || Object is AngleView)
                {
                    this.ObjectLayout.Children.Insert(0, (Xamarin.Forms.View)Object);
                }
                else if (Object is DotView cadAnchor)
                {
                    this.ObjectLayout.Children.Insert(this.ObjectLayout.Children.Count, cadAnchor);
                }
                else if (Object is AbsoluteLayout absoluteLayout)
                {
                    this.GroupLayout.Children.Add(absoluteLayout);
                }
            });

            return Object;
        }

        private async Task<object> DrawObject(object Object)
        {
            if (Object is CadPoint cadPoint)
            {
                /*await this.Add(new VisualAnchor(cadPoint)
                {
                    Scale = 1 / this.GroupLayout.Scale,
                });*/
                await this.Add(new DotView()
                { BindingContext = cadPoint });
                return cadPoint;
            }
            else if (Object is ConstraintLenth lenthConstrait)
            {
                await this.Add(new LineView() { BindingContext = lenthConstrait });
                return lenthConstrait;
            }
            else if (Object is ConstraintAngle angleConstrait)
            {
                await this.Add(new AngleView() { BindingContext = angleConstrait });
                return angleConstrait;
            }

            return null;
        }

        private void CadAnchor1_Dropped(object sender, object e)
        {
            if (sender != e)
            {
                if (sender is DotView cadAnchor2 && e is DotView cadAnchor1)
                {
                    ICommand ConnectPoint = new Command(async () =>
                    {
                        this.Contour.Add(new ConstraintLenth(cadAnchor1.point, cadAnchor2.point, -1));
                    });
                    ICommand MergePoint = new Command(() =>
                    {
                        CadPoint point = cadAnchor1.point;
                        cadAnchor1.BindingContext = cadAnchor2.BindingContext;
                        cadAnchor1.point.TryRemove();
                    });

                    SheetMenu sheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>() {
                        new SheetMenuItem(ConnectPoint, "{CONNECT_POINT}"),
                        new SheetMenuItem(MergePoint, "{MERGE_POINT}")
                    });

                    sheetMenu.ShowMenu(this);
                }
            }
        }


        private async void _contour_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ICadObject cadObject in e.NewItems)
                {
                    await DrawObject(cadObject);
                }
            }
            if (e.OldItems != null)
            {
                foreach (ICadObject cadObject1 in e.OldItems)
                {
                    Remove(cadObject1);
                }
            }
        }


        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="cadObject">select object</param>
        public void Remove(object sender)
        {
            if (sender is ICanvasObject canvasObject)
            {
                canvasObject.Removed -= CanvasObject_Removed;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (sender is ContentView visualLine)
                {
                    this.ObjectLayout.Children.Remove(visualLine);
                }
                if (sender is AbsoluteLayout layout)
                {
                    this.GroupLayout.Children.Remove(layout);
                }
            });
        }

        private void CanvasObject_Removed(object sender, bool e)
        {
            Remove(sender);
        }


        private void CanvasView_BindingContextChanged(object sender, EventArgs e)
        {
            this.Contour = (Contour)this.BindingContext;
        }

        public async Task Refresh()
        {
            await this.VisualClear();
            await DrawContour(this.Contour);
        }

        public async Task DrawContour(Contour contour)
        {
            if (contour.Count > 0)
            {
                foreach (ICadObject cadObject in contour)
                {
                    await DrawObject(cadObject);
                }
            }
        }

        public async Task VisualClear()
        {
            // this.AnchorLayout.Children.Clear();
            this.ObjectLayout.Children.Clear();
        }

        public async Task FitChild()
        {
            if (ObjectLayout.Children.Count > 0)
            {
                try
                {
                    double minX = double.MaxValue, maxX = double.MinValue, minY = double.MaxValue, maxY = double.MinValue;

                    foreach (VisualElement visualElement in this.ObjectLayout.Children)
                    {
                        if (visualElement is DotView dotView)
                        {
                            minX = Math.Min(minX, dotView.point.X + CanvasView.ZeroPoint.X);
                            maxX = Math.Max(maxX, dotView.point.X + CanvasView.ZeroPoint.X);
                            minY = Math.Min(minY, dotView.point.Y + CanvasView.ZeroPoint.Y);
                            maxY = Math.Max(maxY, dotView.point.Y + CanvasView.ZeroPoint.Y);
                        }
                    }
                    double scale = Math.Min(this.MainLayout.Width / (maxX - minX), this.MainLayout.Height / (maxY - minY));
                    this.CommonScale = scale * 0.6;

                    TranslateToPoint(new Point((minX + maxX) / 2, (minY + maxY) / 2));
                }
                catch
                {
                    Console.WriteLine("Fit Error");
                }
            }
        }

        private void TranslateToPoint(Point point)
        {
            this.GroupLayout.TranslationX = -((this.GroupLayout.Width * (1 - this.GroupLayout.Scale)) / 2) -
                (point.X * this.GroupLayout.Scale - this.MainLayout.Width / 2);
            this.GroupLayout.TranslationY = -((this.GroupLayout.Height * (1 - this.GroupLayout.Scale)) / 2) -
                (point.Y * this.GroupLayout.Scale - this.MainLayout.Height / 2);
        }

        public ICommand DraggingStartObject => new Command(() =>
        {
            draggingstatus = true;
            OnPropertyChanged("CommonScale");
        });

        public ICommand DraggingComplitObject => new Command(() =>
        {
            draggingstatus = false;
            OnPropertyChanged("CommonScale");
        });

        public ICommand DropComplit => new Command((object sender) =>
        {
            if (sender is Tuple<object, object> tuple)
            {
                if (tuple.Item1 is CadPoint point1 && tuple.Item2 is CadPoint point2)
                {
                    ICommand ConnectPoint = new Command(() =>
                    {
                        this.Contour.Add(new ConstraintLenth(point1, point2, -1));
                    });
                    ICommand MergePoint = new Command(() =>
                    {
                        point1.ChangePoint(point2);
                    });

                    SheetMenu sheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>() {
                        new SheetMenuItem(ConnectPoint, "{CONNECT_POINT}"),
                        new SheetMenuItem(MergePoint, "{MERGE_POINT}")
                    });

                    sheetMenu.ShowMenu(this);
                }
            }
        });

        public SheetMenu SheetMenu
        {
            get => _sheetMenu;
            set
            {
                this._sheetMenu = value;
            }
        }
        protected SheetMenu _sheetMenu;

        double IActiveObject.X
        {
            get => this.GroupLayout.TranslationX;
            set => this.GroupLayout.TranslationX = value;
        }
        double IActiveObject.Y
        {
            get => this.GroupLayout.TranslationY;
            set => this.GroupLayout.TranslationY = value;
        }

        private void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {

            }
            if (e.Status == GestureStatus.Running)
            {

                //start center position
                double FromCenterPosX = ((this.MainLayout.Width / 2 - this.GroupLayout.TranslationX) -
                    (this.GroupLayout.Width * (1 - this.GroupLayout.Scale) / 2)) / this.GroupLayout.Scale;
                double FromCenterPosY = ((this.MainLayout.Height / 2 - this.GroupLayout.TranslationY) -
                    (this.GroupLayout.Height * (1 - this.GroupLayout.Scale) / 2)) / this.GroupLayout.Scale;


                //setup new scale
                // Apply scale factor.
                this.CommonScale = this.GroupLayout.Scale * e.Scale;
                //find new center position from start position
                TranslateToPoint(new Point(FromCenterPosX, FromCenterPosY));
            }
            if (e.Status == GestureStatus.Completed)
            {

            }
        }

        private bool wasmove = false;
        private Point startPoint = new Point();
        private IActiveObject SelectActiveObject
        {
            get => selectAO == null ? this : selectAO;
            set
            {
                selectAO = value;
            }
        }
        private IActiveObject selectAO;
    
        private void TouchEffect_TouchAction(object sender, TouchTracking.TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchTracking.TouchActionType.Pressed:
                    startPoint = new Point(args.Location.X, args.Location.Y);
                    SelectActiveObject = GetActivObjectFromPoint(startPoint);
                    break;
                case TouchTracking.TouchActionType.Moved:
                    wasmove = true;
                    SelectActiveObject.X += (args.Location.X - startPoint.X) / this.GroupLayout.Scale;
                    SelectActiveObject.Y += (args.Location.Y - startPoint.Y) / this.GroupLayout.Scale;
                    startPoint = new Point(args.Location.X, args.Location.Y);
                    break;
                case TouchTracking.TouchActionType.Cancelled:
                case TouchTracking.TouchActionType.Released:
                    if (wasmove == true)
                    {
                        SelectActiveObject = null;
                    }
                    else
                    {
                        TapManager();
                    }
                    wasmove = false;
                    break;
            }
        }

        private int taps = 0;
        private bool runtimer = false;

        private  void TapManager()
        {
            taps += 1;
            if (this.runtimer == false)
            {
                this.runtimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
                {
                    if (taps < 2)
                    {
                        SelectActiveObject.TapAction();
                    }
                    else
                    {
                        SelectActiveObject.SheetMenu.ShowMenu(SelectActiveObject);
                    }

                    taps = 0;
                    return false; // return true to repeat counting, false to stop timer
                });
                this.runtimer = false;
            }
        }

        private IActiveObject GetActivObjectFromPoint(Point innerPoint)
        {
            Point ObjectLayoutPoint = ConvertMainPoint(innerPoint);
            for (int i = this.ObjectLayout.Children.Count - 1; i > -1; i -= 1)
            {
                if (this.ObjectLayout.Children[i] is IActiveObject activeObject)
                {
                    if (activeObject.ContainsPoint(ObjectLayoutPoint) == true)
                    {
                        return activeObject;
                    }
                }
            }
            return this;
        }

        private Point ConvertMainPoint(Point innerPoint)
        {
            double FromCenterPosX = ((innerPoint.X - this.GroupLayout.TranslationX) -
            (this.GroupLayout.Width * (1 - this.GroupLayout.Scale) / 2)) / this.GroupLayout.Scale;
            double FromCenterPosY = ((innerPoint.Y - this.GroupLayout.TranslationY) -
                (this.GroupLayout.Height * (1 - this.GroupLayout.Scale) / 2)) / this.GroupLayout.Scale;

            return new Point(FromCenterPosX, FromCenterPosY);
        }

        public void TapAction()
        {
            
        }
    }

}