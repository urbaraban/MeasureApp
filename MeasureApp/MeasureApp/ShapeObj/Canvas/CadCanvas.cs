using App1;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.Interface;
using MeasureApp.ShapeObj.LabelObject;
using MeasureApp.Tools;
using MeasureApp.View.OrderPage.OrderClass;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj
{
    public class CadCanvas : ContentView
    {
        #region static
        public static new double Width = 100000;
        public static new double Height = 100000;
        public static Point ZeroPoint = new Point(5000, 5000);

        public static event EventHandler<double> DragSize;
        public static event EventHandler<bool> SellectAll;
        public static double DragSizeKoeff = 1.5;
        public static event EventHandler<double> RegularSize;
        public static double RegularAnchorSize = 10;
        
        private static double canvasscale = 1;

        public static void CallDragSize()
        {
            DragSize?.Invoke(null, canvasscale);
        }

        public static void CallRegularSize()
        {
            RegularSize?.Invoke(null, canvasscale);
        }
        #endregion

        #region Layouts
        private AbsoluteLayout MainLayout;
        private AbsoluteLayout GroupLayout;
        private AbsoluteLayout ObjectLayout;
        private AbsoluteLayout AnchorLayout;
        #endregion

        /// <summary>
        /// Drawing method parametr
        /// </summary>


        private PanGestureRecognizer panGesture = new PanGestureRecognizer();
        private double startScale = 1;
        private double startProp = 1;
        private Point startPoint = new Point(0, 0);

        public DrawMethod Method
        {
            get => this.Contour.Method;
            set
            {
                this.Contour.Method = value;
                OnPropertyChanged("Method");
            }
        }

        private Contour Contour;

        public CadCanvas()
        {
            //setting
            this.IsClippedToBounds = false;
            this.WidthRequest = 100;
            this.HeightRequest = 100;

            //Make layer
            this.MainLayout = new AbsoluteLayout()
            {
                ScaleY = -1,
            };
            this.GroupLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                BackgroundColor = Color.White,
            };
            this.AnchorLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                WidthRequest = CadCanvas.Width,
                HeightRequest = CadCanvas.Height,
            };
            this.ObjectLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                WidthRequest = CadCanvas.Width,
                HeightRequest = CadCanvas.Height,
            };

            //Gestured
            PinchGestureRecognizer pinchGestureRecognizer = new PinchGestureRecognizer();
            pinchGestureRecognizer.PinchUpdated += PinchGestureRecognizer_PinchUpdated;
            this.GestureRecognizers.Add(pinchGestureRecognizer);
            panGesture.PanUpdated += PanGesture_PanUpdated;
            this.GestureRecognizers.Add(panGesture);

            this.Content = this.MainLayout;
            this.MainLayout.Children.Add(GroupLayout);
            this.Add(this.ObjectLayout);
            this.Add(this.AnchorLayout);
            this.GroupLayout.TranslationX = -ZeroPoint.X + 100;
            this.GroupLayout.TranslationY = -ZeroPoint.Y + 100;
            this.BindingContext = new Order();

            AppShell.LenthUpdated += AppShell_LenthUpdated;

            this.Contour = new Contour("Templayted");
            this.Contour.ObjectAdded += Contour_ObjectAdded;
            this.BindingContextChanged += CadCanvas_BindingContextChanged;
            this.Contour.ObjectAdded += Contour_ObjectAdded;
        }

        private void CadCanvas_BindingContextChanged(object sender, EventArgs e)
        {
            this.Contour.ObjectAdded -= Contour_ObjectAdded;
            this.Contour = (Contour)this.BindingContext;
            DrawContour(this.Contour);
            this.Contour.ObjectAdded += Contour_ObjectAdded;
        }

        private void Contour_ObjectAdded(object sender, object e)
        {
            DrawObject(e);
        }

        public object DrawObject(object Object)
        {
            if (Object is CadPoint cadPoint)
            {
                this.Add(MakeAnchor(cadPoint));
                return cadPoint;
            }
            if (Object is ConstraintLenth lenthConstrait)
            {
                this.Add(new LenthLabel(lenthConstrait));
                this.Add(MakeLine(lenthConstrait));
                return lenthConstrait;
            }
            if (Object is ConstraintAngle angleConstrait)
            {
                this.Add(new AngleLabel(angleConstrait));
                return angleConstrait;
            }

            return null;
            
            VisualAnchor MakeAnchor(CadPoint cadPoint)
            {
                VisualAnchor cadAnchor = new VisualAnchor(cadPoint);
                cadAnchor.Scale = 1 / this.GroupLayout.Scale;
                cadAnchor.Droped += CadAnchor1_Droped;
                return cadAnchor;
            }

            VisualLine MakeLine(ConstraintLenth lenthAnchorAnchor)
            {
                VisualLine cadLine = new VisualLine(lenthAnchorAnchor, false);
                cadLine.StrokeThickness = 5 * 1 / this.GroupLayout.Scale;
                cadLine.Stroke = Brush.Blue;
                return cadLine;
            }
        }

        public async void DrawContour(Contour contour)
        {
            if (contour.Paths != null)
            {
                foreach (ContourPath contourPath in contour.Paths)
                {
                    foreach (CadPoint point in contour.Points)
                    {
                        Task.Run(() => { DrawObject(point); });
                    }
                }
            }
        }


        /// <summary>
        /// Adapt screen for size object on Canvas
        /// </summary>
        public void FitChild()
        {
            if (this.ObjectLayout.Children.Count > 0)
            {
                double startWidth = this.GroupLayout.Width * this.GroupLayout.Scale;
                Point startCenterPoint = new Point()
                {
                    X = (this.MainLayout.Width / 2 - this.GroupLayout.TranslationX) / this.GroupLayout.Scale,
                    Y = (this.MainLayout.Height / 2 - this.GroupLayout.TranslationY) / this.GroupLayout.Scale
                };

                double minX = double.MaxValue, maxX = double.MinValue, minY = double.MaxValue, maxY = double.MinValue;

                foreach (VisualElement visualElement in this.ObjectLayout.Children)
                {
                    if (visualElement is VisualLine cadLine)
                    {
                        minX = Math.Min(minX, cadLine.Bounds.Left);
                        maxX = Math.Max(maxX, cadLine.Bounds.Right);
                        minY = Math.Min(minY, cadLine.Bounds.Top);
                        maxY = Math.Max(maxY, cadLine.Bounds.Bottom);
                    }
                }
                double scale = Math.Min(this.MainLayout.Width / (maxX - minX), this.MainLayout.Height / (maxY - minY));
                this.GroupLayout.Scale = scale * 0.8;

                this.GroupLayout.TranslationX = -(this.GroupLayout.Width / 2 - (this.GroupLayout.Width / 2 - minX) * this.GroupLayout.Scale) + Math.Abs(maxX - minX) * this.GroupLayout.Scale * 0.1;
                this.GroupLayout.TranslationY = -(this.GroupLayout.Height / 2 - (this.GroupLayout.Height / 2 - minY) * this.GroupLayout.Scale) + Math.Abs(maxY - minY) * this.GroupLayout.Scale * 0.1;

                Console.WriteLine($"{this.GroupLayout.TranslationX} {this.GroupLayout.TranslationY}");

                CadCanvas.canvasscale = this.GroupLayout.Scale;
                CallRegularSize();
            }
        }

        /// <summary>
        /// Clear canvas and return default setting
        /// </summary>
        public void Clear()
        {
            this.AnchorLayout.Children.Clear();
            this.ObjectLayout.Children.Clear();
            this.GroupLayout.Scale = 1;
            this.GroupLayout.TranslationX = -ZeroPoint.X + 100;
            this.GroupLayout.TranslationY = -ZeroPoint.Y + 100;

            this.Contour.Clear();
        }

        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="cadObject">select object</param>
        public void Remove(object sender)
        {
            if (sender is CanvasObject canvasObject)
            {
                canvasObject.Removed -= CanvasObject_Removed;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (sender is VisualLine visualLine)
                {
                    this.ObjectLayout.Children.Remove((VisualLine)sender);
                }
                if (sender is VisualAnchor visualAnchor)
                {
                    this.AnchorLayout.Children.Remove((VisualAnchor)sender);
                }
                if (sender is ConstraitLabel)
                {
                    this.AnchorLayout.Children.Remove((ConstraitLabel)sender);
                }
                if (sender is AbsoluteLayout)
                {
                    this.GroupLayout.Children.Remove((AbsoluteLayout)sender);
                }
            });
        }

        /// <summary>
        /// Add object on Canvas. 
        /// </summary>
        /// <param name="Object"></param>
        private object Add(object Object)
        {
            if (Object is CanvasObject canvasObject)
            {
                canvasObject.Removed += CanvasObject_Removed;
            }

            if (Object is ConstraitLabel constraitLabel)
            {
                constraitLabel.Scale = 1 / this.GroupLayout.Scale;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Object is VisualLine cadLine)
                {
                    this.ObjectLayout.Children.Add(cadLine);
                }
                else if (Object is VisualAnchor cadAnchor)
                {
                    this.AnchorLayout.Children.Add(cadAnchor);
                }
                else if(Object is ConstraitLabel constraitLabel)
                {
                    this.AnchorLayout.Children.Add(constraitLabel);
                }
                else if(Object is AbsoluteLayout absoluteLayout)
                {
                    this.GroupLayout.Children.Add(absoluteLayout);
                }
            });

            return Object;
        }




        /// <summary>
        /// Find position and make line on canvas with anchor
        /// </summary>
        /// <param name="Lenth"></param>
        /// <param name="Angle"></param>
        public void BuildLine(double Lenth, double Angle)
        {
            //Если у нас нет линии привязки
            if (this.Contour.LastLenthConstrait == null)
            {
                CadPoint cadPoint1 = (CadPoint)this.Contour.Add(new CadPoint(0, 0, "A"), 0);
                CadPoint cadPoint2 = (CadPoint)this.Contour.Add(new CadPoint(0, 0 + Lenth, "B"), 0);
                this.Contour.Add(new ConstraintLenth(cadPoint1, cadPoint2, Lenth), 0);
            }
            else
            {
                if (this.Contour.LastLenthConstrait is ConstraintLenth)
                {
                    Point point = Sizing.GetPositionLineFromAngle(this.Contour.LastLenthConstrait.Point1, this.Contour.LastLenthConstrait.Point2, Lenth, Angle < 0 ? 90 : Angle);
                    CadPoint point2 = (CadPoint)this.Contour.Add(new CadPoint(point.X, point.Y, "Z"), 0);
                    ConstraintLenth lenthConstrait = (ConstraintLenth)this.Contour.Add(new ConstraintLenth(this.Contour.StartPoint, point2, Lenth), 0);
                    if (this.Contour.Method == DrawMethod.FromPoint)
                    {
                        lenthConstrait.IsSupport = true;
                        this.Contour.Add(new ConstraintLenth(this.Contour.LastPoint, point2, -1, true), 0);
                        this.Contour.Add(new ConstraintAngle(this.Contour.LastLenthConstrait, lenthConstrait, Angle), 0);
                    }
                }
            }
        }

        private void CanvasObject_Removed(object sender, EventArgs e)
        {
            Remove(sender);
        }

        private void CadAnchor1_Droped(object sender, object e)
        {
            if (sender != e)
            {
                if (sender is VisualAnchor cadAnchor2 && e is VisualAnchor cadAnchor1)
                {
                    ICommand ConnectPoint = new Command(async () =>
                    {
                        this.Contour.Add(new ConstraintLenth(cadAnchor1.cadPoint, cadAnchor2.cadPoint, -1), 0);
                        cadAnchor1.Update();
                        cadAnchor2.Update();
                    });
                    ICommand MergePoint = new Command(async () =>
                    {
                        cadAnchor1.ChangedPoint(cadAnchor2.cadPoint);
                    });

                    SheetMenu sheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>() {
                        new SheetMenuItem(ConnectPoint, "{CONNECT_POINT}"),
                        new SheetMenuItem(MergePoint, "{MERGE_POINT}")
                    });

                    sheetMenu.ShowMenu(this);
                }
            }
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
            {
                startPoint.X = this.GroupLayout.TranslationX;
                startPoint.Y = this.GroupLayout.TranslationY;
            }
            if (e.StatusType == GestureStatus.Running)
            {
                this.GroupLayout.TranslationX = startPoint.X + e.TotalX;
                this.GroupLayout.TranslationY = startPoint.Y - e.TotalY;
            }

            //Console.WriteLine($"{this.GroupLayout.TranslationX} {this.GroupLayout.TranslationY}");
        }

        private void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                startProp = this.MainLayout.Width / (this.GroupLayout.Width * this.GroupLayout.Scale) * 30;
            }
            if (e.Status == GestureStatus.Running)
            {
                double StartWidth = this.GroupLayout.Width * this.GroupLayout.Scale;

                startScale = this.GroupLayout.Scale;
                // Apply scale factor.
                this.GroupLayout.Scale = startScale * (1 - ((1 - e.Scale) * startProp));

                this.GroupLayout.TranslationX += (this.GroupLayout.Width * this.GroupLayout.Scale - StartWidth) / 2;
                this.GroupLayout.TranslationY += (this.GroupLayout.Width * this.GroupLayout.Scale - StartWidth) / 2;
            }
            if (e.Status == GestureStatus.Completed)
            {
                CadCanvas.canvasscale = this.GroupLayout.Scale;
                CallRegularSize();
            }
        }

        private void AppShell_LenthUpdated(object sender, Tuple<double, double> e)
        {
            BuildLine(e.Item1, e.Item2);
        }

    }

    public enum DrawMethod
    {
        StepByStep,
        FromPoint
    }
}
