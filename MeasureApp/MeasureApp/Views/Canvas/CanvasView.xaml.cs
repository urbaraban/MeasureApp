using DrawEngine;
using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.ShapeObj.VisualObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TouchTracking;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.Views.Canvas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CanvasView : ContentView, IMoveObject, ITouchObject
    {
        public static Point ZeroPoint = new Point(5000, 5000);
        public static List<object> RunningGestureObject = new List<object>();

        bool ITouchObject.ContainsPoint(Point InnerPoint) => true;

        private IStatusObject DragObjects
        {
            get => dragobject;
            set
            {
                dragobject = value;
                OnPropertyChanged("CommonScale");
            }
        }
        private IStatusObject dragobject;

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
                SetAnchorToPoint(new TouchTrackingPoint((float)this.MainLayout.Width / 2, (float)this.MainLayout.Height / 2));
                this.GroupLayout.Scale = value;
                OnPropertyChanged("CommonScale");
            }
        }
        private bool draggingstatus => DragObjects != null;

        public CanvasView()
        {
            InitializeComponent();
            this.BindingContextChanged += CanvasView_BindingContextChanged;

            this.SheetMenu = new SheetMenu(new List<SheetMenuItem>()
            {

            });
        }


        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
            Console.WriteLine($"Canvas property: {propertyName}");
        }



        /// <summary>
        /// Add object on Canvas. 
        /// </summary>
        /// <param name="Object"></param>
        private async Task<object> Add(object Object)
        {
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
            if (Object is CadAnchor cadPoint)
            {
                /*await this.Add(new VisualAnchor(cadPoint)
                {
                    Scale = 1 / this.GroupLayout.Scale,
                });*/
                await this.Add(new DotView()
                { BindingContext = cadPoint });
                return cadPoint;
            }
            else if (Object is LenthConstraint lenthConstrait)
            {
                await this.Add(new LineView() { BindingContext = lenthConstrait });
                return lenthConstrait;
            }
            else if (Object is AngleConstraint angleConstrait)
            {
                await this.Add(new AngleView() { BindingContext = angleConstrait });
                return angleConstrait;
            }

            return null;
        }

        private void DragDropManager(ITouchObject activeObject1, IStatusObject activeObject2)
        {
            if (activeObject1 != activeObject2)
            {
                if (activeObject1 is DotView cadAnchor2 && activeObject2 is DotView cadAnchor1)
                {
                    ICommand ConnectPoint = new Command(async () =>
                    {
                        this.Contour.Add(new LenthConstraint(cadAnchor1.Anchor, cadAnchor2.Anchor, -1, false));
                    });
                    ICommand MergePoint = new Command(() =>
                    {
                        cadAnchor1.Anchor.ChangePoint(cadAnchor2.Anchor);
                        cadAnchor1.BindingContext = cadAnchor2.BindingContext;
                    });

                    SheetMenu sheetMenu = new SheetMenu(new List<SheetMenuItem>() {
                        new SheetMenuItem(ConnectPoint, "{CONNECT_POINT}"),
                        new SheetMenuItem(MergePoint, "{MERGE_POINT}")
                    });

                    sheetMenu.ShowMenu(this, "{DRAGOPERATION}");
                }
                else if (activeObject1 is CanvasView canvasView && activeObject2 is DotView dotView)
                {
                    Point point = ConvertMainPoint(TouchPoint);
                    CadAnchor cadPoint = new CadAnchor(point.X - CanvasView.ZeroPoint.X, point.Y - CanvasView.ZeroPoint.Y);
                    this.Contour.Add(new LenthConstraint(dotView.Anchor, cadPoint, -1));
                    this.Contour.Add(cadPoint);
                }
                else if (activeObject1 is LineView lenth1 && activeObject2 is LineView lenth2)
                {
                    ICommand MakeAngle = new Command(async () =>
                    {
                        this.Contour.Add(new AngleConstraint(lenth1.Lenth, lenth2.Lenth));
                    });
                    ICommand Equalse = new Command(() =>
                    {
                        lenth2.Lenth.Variable = lenth1.Lenth.Variable;
                    });

                    SheetMenu sheetMenu = new SheetMenu(new List<SheetMenuItem>() {
                        new SheetMenuItem(MakeAngle, "{MAKE_ANGLE}"),
                        new SheetMenuItem(Equalse, "{EQUALSE}")
                    });

                    sheetMenu.ShowMenu(this, "{DRAGOPERATION}");
                }
            }
        }

        public void Zoom(double v)
        {
            //Point point = ConvertMainPoint(new TouchTrackingPoint((float)this.MainLayout.Width / 2, (float)this.MainLayout.Height / 2));
            this.CommonScale += this.CommonScale * v;
            //TranslateToPoint(point);
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
            View view = FindViewByCadObject(sender);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                this.ObjectLayout.Children.Remove(view);
            });
        }

        private View FindViewByCadObject(object cadObject)
        {
            foreach (View view in this.ObjectLayout.Children)
            {
                if (view.BindingContext == cadObject) return view;
            }
            return null;
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
                            minX = Math.Min(minX, dotView.X + CanvasView.ZeroPoint.X);
                            maxX = Math.Max(maxX, dotView.X + CanvasView.ZeroPoint.X);
                            minY = Math.Min(minY, dotView.Y + CanvasView.ZeroPoint.Y);
                            maxY = Math.Max(maxY, dotView.Y + CanvasView.ZeroPoint.Y);
                        }
                    }


                    double scale = Math.Min(this.MainLayout.Width / (maxX - minX), this.MainLayout.Height / (maxY - minY));
                    this.CommonScale = scale * 0.6;
                    TranslateToPoint(new Point((minX + maxX) / 2, (minY + maxY) / 2));
                    //TranslateToPoint(new TouchTrackingPoint((float)this.MainLayout.Width / 2, (float)this.MainLayout.Height / 2));
                }
                catch
                {
                    Console.WriteLine("Fit Error");
                }
            }

        }


        public ICommand DraggingStartObject => new Command(() =>
        {
            OnPropertyChanged("CommonScale");
        });

        public ICommand DraggingComplitObject => new Command(() =>
        {
            OnPropertyChanged("CommonScale");
        });

        public ICommand DropComplit => new Command((object sender) =>
        {
            if (sender is Tuple<object, object> tuple)
            {
                if (tuple.Item1 is CadAnchor point1 && tuple.Item2 is CadAnchor point2)
                {
                    ICommand ConnectPoint = new Command(() =>
                    {
                        this.Contour.Add(new LenthConstraint(point1, point2, -1));
                    });
                    ICommand MergePoint = new Command(() =>
                    {
                        point1.ChangePoint(point2);
                    });

                    SheetMenu sheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>() {
                        new SheetMenuItem(ConnectPoint, "{CONNECT_POINT}"),
                        new SheetMenuItem(MergePoint, "{MERGE_POINT}")
                    });

                    sheetMenu.ShowMenu(this, "{DRAGDROPOPERATION}");
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

        double IMoveObject.X
        {
            get => this.GroupLayout.TranslationX;
            set => this.GroupLayout.TranslationX = value;
        }
        double IMoveObject.Y
        {
            get => this.GroupLayout.TranslationY;
            set => this.GroupLayout.TranslationY = value;
        }

        private TouchTrackingPoint TouchPoint;
        private IMoveObject SelectMoveObject
        {
            get => selectMO == null ? this : selectMO;
            set
            {
                selectMO = value;
            }
        }
        private IMoveObject selectMO;
        private Dictionary<long, TouchTrackingPoint> touchDictionary = new Dictionary<long, TouchTrackingPoint>();
        private double pinchLenth;
        private double startScale;
        private bool wasmove = false;

        private async void TouchEffect_TouchAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:

                    if (touchDictionary.ContainsKey(args.Id) == false)
                    {
                        touchDictionary.Add(args.Id, args.Location);
                    }

                    if (touchDictionary.Count == 1)
                    {
                        TouchPoint = args.Location;
                        //Get drag object from press
                        Device.StartTimer(TimeSpan.FromMilliseconds(1500), () =>
                        {
                            if (this.SelectMoveObject == this && touchDictionary.Count == 1)
                            {
                                this.DragObjects = (IStatusObject)GetObjectFromPoint(TouchPoint, typeof(IStatusObject));
                            }
                            return false; // runs again, or false to stop
                        });
                    }
                    else if (touchDictionary.Count > 1)
                    {
                        TouchTrackingPoint point2 = touchDictionary[(args.Id + 1) % touchDictionary.Count];
                        pinchLenth = PtPLenth(args.Location, point2);
                        TouchPoint = new TouchTrackingPoint((args.Location.X + point2.X) / 2, (args.Location.Y + point2.Y) / 2);
                        startScale = this.GroupLayout.Scale;
                    }

                    break;
                case TouchActionType.Moved:
                    touchDictionary[args.Id] = args.Location;

                    if (touchDictionary.Count == 1)
                    {
                        if (this.DragObjects == null && SelectMoveObject == this)
                        {
                            SelectMoveObject = (IMoveObject)GetObjectFromPoint(TouchPoint, typeof(IMoveObject));
                        }
                        if (this.DragObjects == null)
                        {
                            wasmove = true;
                            SelectMoveObject.X += (args.Location.X - TouchPoint.X) / (SelectMoveObject == this ? 1 : this.GroupLayout.Scale);
                            SelectMoveObject.Y += (args.Location.Y - TouchPoint.Y) / (SelectMoveObject == this ? 1 : this.GroupLayout.Scale);
                            TouchPoint = args.Location;
                        }
                    }
                    else if (touchDictionary.Count >= 2)
                    {
                        TouchTrackingPoint point2 = touchDictionary[(args.Id + 1) % touchDictionary.Count];
                        TouchTrackingPoint centerpoint = new TouchTrackingPoint((args.Location.X + point2.X) / 2, (args.Location.Y + point2.Y) / 2);

                        this.GroupLayout.TranslationX += (centerpoint.X - TouchPoint.X) / this.GroupLayout.Scale;
                        this.GroupLayout.TranslationY += (centerpoint.Y - TouchPoint.Y) / this.GroupLayout.Scale;
                        TouchPoint = centerpoint;

                        Point point = ConvertMainPoint(TouchPoint);

                        this.CommonScale = startScale * (PtPLenth(args.Location, touchDictionary[(args.Id + 1) % touchDictionary.Count]) / pinchLenth);
                        //TranslateToPoint(centerpoint);
                    }
                    break;
                case TouchActionType.Cancelled:
                case TouchActionType.Released:
                    if (touchDictionary.Count == 1)
                    {
                        if (DragObjects == null)
                        {
                            if (this.Contour.SelectedDrawMethod == DrawMethod.Manual && wasmove == false)
                            {
                                if ((ITouchObject)GetObjectFromPoint(args.Location, typeof(ITouchObject)) is DotView dotView)
                                {
                                    await this.Contour.BuildLine(dotView.Anchor, false);
                                }
                                else
                                {
                                    Point point = ConvertMainPoint(args.Location);
                                    await this.Contour.BuildLine(new CadAnchor(point.X - CanvasView.ZeroPoint.X, point.Y - CanvasView.ZeroPoint.Y), false);
                                }
                            }

                            else if (SelectMoveObject == this)
                            {
                                TapManager((ITouchObject)GetObjectFromPoint(args.Location, typeof(ITouchObject)));
                            }
                            else
                            {
                                SelectMoveObject = null;
                            }
                        }
                        else
                        {
                            TouchPoint = args.Location;
                            ITouchObject statusObject = (ITouchObject)GetObjectFromPoint(TouchPoint, typeof(ITouchObject), DragObjects);
                            if (statusObject != null)
                            {
                                DragDropManager(statusObject, dragobject);
                            }
                            else if (this.Contour.SelectedDrawMethod == DrawMethod.Manual)
                            {
                                DragDropManager(this, dragobject);
                            }
                            DragObjects = null;
                        }
                    }

                    if (touchDictionary.ContainsKey(args.Id))
                    {
                        touchDictionary.Remove(args.Id);
                    }
                    wasmove = false;
                    break;
            }


            double PtPLenth(TouchTrackingPoint cadPoint1, TouchTrackingPoint cadPoint2)
            {
                return Math.Sqrt(Math.Pow(cadPoint2.X - cadPoint1.X, 2) + Math.Pow(cadPoint2.Y - cadPoint1.Y, 2));
            }
        }


        private void TranslateToPoint(Point point)
        {
            this.GroupLayout.TranslationX = -((this.GroupLayout.Width * (1 - this.GroupLayout.Scale)) * this.GroupLayout.AnchorX) -
                (point.X * this.GroupLayout.Scale - this.MainLayout.Width / 2);
            this.GroupLayout.TranslationY = -((this.GroupLayout.Height * (1 - this.GroupLayout.Scale)) * this.GroupLayout.AnchorY) -
                (point.Y * this.GroupLayout.Scale - this.MainLayout.Height / 2);
            //SetAnchorToPoint(new TouchTrackingPoint((float)this.MainLayout.Width/2, (float)this.MainLayout.Height/2));
        }

        private void TranslateToPoint(TouchTrackingPoint trackingPoint) => TranslateToPoint(ConvertMainPoint(trackingPoint));

        private void SetAnchorToPoint(TouchTrackingPoint point)
        {
            this.GroupLayout.AnchorX = (-this.GroupLayout.TranslationX + point.X ) / (this.GroupLayout.Width);
            this.GroupLayout.AnchorY = (-this.GroupLayout.TranslationY + point.Y ) / (this.GroupLayout.Height);
        }

        private int taps = 0;
        private bool runtimer = false;

        private  void TapManager(ITouchObject touchObject)
        {
            if (touchObject != null)
            {
                taps += 1;
                if (this.runtimer == false)
                {
                    this.runtimer = true;
                    Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
                    {
                        if (taps < 2)
                        {
                            touchObject.TapAction();
                        }
                        else
                        {
                            touchObject.SheetMenu.ShowMenu(touchObject, touchObject.ToString());
                        }

                        taps = 0;
                        return false; // return true to repeat counting, false to stop timer
                });
                    this.runtimer = false;
                }
            }
        }


        private object GetObjectFromPoint(TouchTrackingPoint innerPoint, Type Filter, object ignoreObject = null)
        {
            Point ObjectLayoutPoint = ConvertMainPoint(innerPoint);

            for (int i = this.ObjectLayout.Children.Count - 1; i > -1; i -= 1)
            {
                if (this.ObjectLayout.Children[i] != ignoreObject 
                    && this.ObjectLayout.Children[i].GetType().GetInterface(Filter.Name) != null)
                {
                    if (this.ObjectLayout.Children[i] is ITouchObject touchObject)
                    {
                        if (touchObject.ContainsPoint(ObjectLayoutPoint) == true)
                        {
                            return touchObject;
                        }
                    }
                }
            }
            return null;
        }

        private Point ConvertMainPoint(TouchTrackingPoint innerPoint)
        {
            double FromCenterPosX = ((innerPoint.X - this.GroupLayout.TranslationX) -
            (this.GroupLayout.Width * (1 - this.GroupLayout.Scale) * this.GroupLayout.AnchorX)) / this.GroupLayout.Scale;
            double FromCenterPosY = ((innerPoint.Y - this.GroupLayout.TranslationY) -
                (this.GroupLayout.Height * (1 - this.GroupLayout.Scale) * this.GroupLayout.AnchorY)) / this.GroupLayout.Scale;

            return new Point((float)FromCenterPosX, (float)FromCenterPosY);
        }

        public void TapAction()
        {
            
        }
    }

}