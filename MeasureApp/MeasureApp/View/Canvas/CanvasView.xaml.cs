using DrawEngine;
using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.ShapeObj.VisualObjects;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.View.Canvas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CanvasView : ContentView
    {
        public static Point ZeroPoint = new Point(5000, 5000);
        public static object RunningGestureObject;

        public static event EventHandler<double> DragSize;
        public static double DragSizeKoeff = 1.5;
        public static event EventHandler<double> RegularSize;
        public static double RegularAnchorSize = 10;

        public static void CallDragSize(double value)
        {
            DragSize?.Invoke(null, value / 1.1);
        }

        public static void CallRegularSize(double value)
        {
            RegularSize?.Invoke(null, value);
        }

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

        public double CanvasScale => this.GroupLayout.Scale != double.PositiveInfinity ? this.GroupLayout.Scale : 1;

        public CanvasView()
        {
            InitializeComponent();
            this.BindingContextChanged += CanvasView_BindingContextChanged;
        }

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
            if (Object is IActiveObject activeObject)
            {
                activeObject.Draging += ActiveObject_Draging;
                activeObject.Dropped += CadAnchor1_Dropped;
            }

            if (Object is ConstraitLabel constraitLabel)
            {
                constraitLabel.Scale = 1 / this.GroupLayout.Scale;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Object is LineView cadLine)
                {
                    this.ObjectLayout.Children.Insert(0, cadLine);
                }
                else if (Object is DotView cadAnchor)
                {
                    this.ObjectLayout.Children.Insert(this.ObjectLayout.Children.Count, cadAnchor);
                }
                else if (Object is ConstraitLabel constraitLabel)
                {
                    this.ObjectLayout.Children.Add(constraitLabel);
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
                await this.Add(new AngleLabel(angleConstrait));
                return angleConstrait;
            }

            return null;
        }

        private void ActiveObject_Draging(object sender, bool e)
        {
            if (e == true) CallDragSize(this.CanvasScale);
            else CallRegularSize(this.CanvasScale);
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
                /*if (sender is VisualAnchor visualAnchor)
                {
                    this.AnchorLayout.Children.Remove(visualAnchor);
                }
                if (sender is ConstraitLabel label)
                {
                    this.AnchorLayout.Children.Remove(label);
                }*/
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
                    this.GroupLayout.Scale = scale * 0.6;

                    TranslateToPoint(new Point((minX + maxX) / 2, (minY + maxY) / 2));

                    
                    CallRegularSize(this.CanvasScale);
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

        private Point startPoint = new Point();

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
            {
                startPoint.X = this.GroupLayout.TranslationX;
                startPoint.Y = this.GroupLayout.TranslationY;
            }
            if (e.StatusType == GestureStatus.Running)
            {
                if (CanvasView.RunningGestureObject == null)
                {
                    this.GroupLayout.TranslationX = startPoint.X + e.TotalX;
                    this.GroupLayout.TranslationY = startPoint.Y + e.TotalY;
                }
            }
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
                    (this.GroupLayout.Height * (1 - this.GroupLayout.Scale) / 2)) / this.GroupLayout.Scale; ;


                //setup new scale
                // Apply scale factor.
                this.GroupLayout.Scale *= e.Scale;

                //find new center position from start position
                TranslateToPoint(new Point(FromCenterPosX, FromCenterPosY));
            }
            if (e.Status == GestureStatus.Completed)
            {
                CallRegularSize(this.CanvasScale);
            }
        }
    }

}