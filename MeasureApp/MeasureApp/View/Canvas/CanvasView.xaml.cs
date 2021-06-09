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

namespace SureMeasure.View.Canvas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CanvasView : ContentView
    {
        public static Point ZeroPoint = new Point(20, 20);
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
            this.MainScroll.Scrolled += MainScroll_Scrolled;
            this.BindingContextChanged += CanvasView_BindingContextChanged;
        }

        private void MainScroll_Scrolled(object sender, ScrolledEventArgs e)
        {

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

            if (Object is ConstraitLabel constraitLabel)
            {
                constraitLabel.Scale = 1 / this.GroupLayout.Scale;
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
                    double scale = Math.Min(this.MainScroll.Width / (maxX - minX), this.MainScroll.Height / (maxY - minY));
                    this.CommonScale = scale * 0.6;

                    TranslateToPoint(new Point((minX + maxX) / 2, (minY + maxY) / 2));
                    
                }
                catch
                {
                    Console.WriteLine("Fit Error");
                }
            }
            TranslateToPoint(ZeroPoint);
        }

        public void TranslateToPoint(Point point)
        {
            double X = point.X;
            double Y = point.Y;

            MainScroll.ScrollToAsync(X, this.MainScroll.Content.Height - Y - this.MainScroll.Height, false);
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

        private Point startPoint = new Point();

        /*private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Console.WriteLine($"PanGesture Canvas {sender} TouchID {e.GestureId}");
            if (e.StatusType == GestureStatus.Started)
            {
                CanvasView.RunningGestureObject.Add(this);
                startPoint.X = this.GroupLayout.TranslationX;
                startPoint.Y = this.GroupLayout.TranslationY;
            }
            else if (e.StatusType == GestureStatus.Running)
            {
                Console.WriteLine($"PanGesture Canvas DeltaX {e.TotalX} DeltaY {e.TotalY}");
                this.GroupLayout.TranslationX = startPoint.X + (e.TotalX * this.GroupLayout.Scale);
                this.GroupLayout.TranslationY = startPoint.Y + (e.TotalY * this.GroupLayout.Scale);
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                CanvasView.RunningGestureObject.Remove(this);
                startPoint.X = this.GroupLayout.TranslationX;
                startPoint.Y = this.GroupLayout.TranslationY;
            }
        }*/

        private void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {

            }
            if (e.Status == GestureStatus.Running)
            {

                //start center position
                double FromCenterPosX = ((this.MainScroll.Width / 2 - this.GroupLayout.TranslationX) -
                    (this.GroupLayout.Width * (1 - this.GroupLayout.Scale) / 2)) / this.GroupLayout.Scale;
                double FromCenterPosY = ((this.MainScroll.Height / 2 - this.GroupLayout.TranslationY) -
                    (this.GroupLayout.Height * (1 - this.GroupLayout.Scale) / 2)) / this.GroupLayout.Scale; ;


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


    }

}