using DrawEngine;
using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj;
using SureMeasure.ShapeObj.Interface;
using System;
using System.Linq;
using System.Text;
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
        public static event EventHandler<double> DragSize;
        public static double DragSizeKoeff = 1.5;
        public static event EventHandler<double> RegularSize;
        public static double RegularAnchorSize = 10;

        public static void CallDragSize(double value)
        {
            DragSize?.Invoke(null, value);
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
                activeObject.Dropped += CadAnchor1_Dropped;
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
                else if (Object is ConstraitLabel constraitLabel)
                {
                    this.AnchorLayout.Children.Add(constraitLabel);
                }
                else if (Object is AbsoluteLayout absoluteLayout)
                {
                    this.GroupLayout.Children.Add(absoluteLayout);
                }
            });

            return Object;
        }

        private void CadAnchor1_Dropped(object sender, object e)
        {
            if (sender != e)
            {
                if (sender is VisualAnchor cadAnchor2 && e is VisualAnchor cadAnchor1)
                {
                    ICommand ConnectPoint = new Command(async () =>
                    {
                        this.Contour.Add(new ConstraintLenth(cadAnchor1.cadPoint, cadAnchor2.cadPoint, -1));
                        await cadAnchor1.Update("Point");
                        await cadAnchor2.Update("Point");
                    });
                    ICommand MergePoint = new Command(() =>
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
                if (sender is VisualLine visualLine)
                {
                    this.ObjectLayout.Children.Remove(visualLine);
                }
                if (sender is VisualAnchor visualAnchor)
                {
                    this.AnchorLayout.Children.Remove(visualAnchor);
                }
                if (sender is ConstraitLabel label)
                {
                    this.AnchorLayout.Children.Remove(label);
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


        public async Task FitChild()
        {
            if (ObjectLayout.Children.Count > 0)
            {
                try
                {
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
                        else
                        {
                            minX = Math.Min(minX, visualElement.TranslationX + visualElement.Bounds.Left);
                            maxX = Math.Max(maxX, visualElement.TranslationX + visualElement.Bounds.Right);
                            minY = Math.Min(minY, visualElement.TranslationY + visualElement.Bounds.Top);
                            maxY = Math.Max(maxY, visualElement.TranslationY + visualElement.Bounds.Bottom);
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

        private async Task<object> DrawObject(object Object)
        {
            if (Object is CadPoint cadPoint)
            {
                await this.Add(new VisualAnchor(cadPoint)
                {
                    Scale = 1 / this.GroupLayout.Scale,
                });
                return cadPoint;
            }
            else if (Object is ConstraintLenth lenthConstrait)
            {
                await this.Add(new LenthLabel(lenthConstrait));
                await this.Add(new VisualLine(lenthConstrait)
                {
                    StrokeThickness = 5 * 1 / this.GroupLayout.Scale,
                });
                return lenthConstrait;
            }
            else if (Object is ConstraintAngle angleConstrait)
            {
                await this.Add(new AngleLabel(angleConstrait));
                return angleConstrait;
            }

            return null;
        }
        private void TranslateToPoint(Point point)
        {
            this.GroupLayout.TranslationX = -((this.GroupLayout.Width * (1 - this.GroupLayout.Scale)) / 2) -
                (point.X * this.GroupLayout.Scale - this.MainLayout.Width / 2);
            this.GroupLayout.TranslationY = -((this.GroupLayout.Height * (1 - this.GroupLayout.Scale)) / 2) -
                (point.Y * this.GroupLayout.Scale - this.MainLayout.Height / 2);
        }

        private Point startPoint = new Point(0, 0);
        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
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