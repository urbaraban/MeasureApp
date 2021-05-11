﻿using SureMeasure.CadObjects;
using SureMeasure.CadObjects.Constraints;
using SureMeasure.Orders;
using SureMeasure.ShapeObj.Constraints;
using SureMeasure.ShapeObj.Interface;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace SureMeasure.ShapeObj.Canvas
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
        private DynamicBackground BackgroundLayout;
        #endregion

        public event EventHandler<DropEventArgs> Droped;

        private PanGestureRecognizer panGesture = new PanGestureRecognizer();
        private Point startPoint = new Point(0, 0);

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


            this.BackgroundLayout = new DynamicBackground(this.GroupLayout, this.MainLayout)
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                WidthRequest = CadCanvas.Width,
                HeightRequest = CadCanvas.Height,
            };

            //Gestured
            //Tap
            /*TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            this.GestureRecognizers.Add(tapGestureRecognizer);*/
            //Pinch
            PinchGestureRecognizer pinchGestureRecognizer = new PinchGestureRecognizer();
            pinchGestureRecognizer.PinchUpdated += PinchGestureRecognizer_PinchUpdated;
            this.GestureRecognizers.Add(pinchGestureRecognizer);
            //Pan
            panGesture.PanUpdated += PanGesture_PanUpdated;
            this.GestureRecognizers.Add(panGesture);

            this.Content = this.MainLayout;
            this.MainLayout.Children.Add(GroupLayout);
            this.Add(this.BackgroundLayout);
            this.Add(this.ObjectLayout);
            this.Add(this.AnchorLayout);
            this.GroupLayout.TranslationX = -ZeroPoint.X + 100;
            this.GroupLayout.TranslationY = -ZeroPoint.Y + 100;
            this.BindingContextChanged += CadCanvas_BindingContextChanged;

            //Drop
            DropGestureRecognizer dropGestureRecognizer = new DropGestureRecognizer();
            dropGestureRecognizer.Drop += DropGestureRecognizer_Drop;
            this.GestureRecognizers.Add(dropGestureRecognizer);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            
        }

        private void DropGestureRecognizer_Drop(object sender, DropEventArgs e)
        {
            Droped?.Invoke(this, e);
        }

        private void CadCanvas_BindingContextChanged(object sender, EventArgs e)
        {
            if (this.BindingContext is Contour contour)
            {
                if (this.Contour != null)
                {
                    this.Contour.ObjectAdded -= Contour_ObjectAdded;
                }
                this.Clear();
                this.Contour = contour;
                DrawContour(this.Contour);
                FitChild();
                this.Contour.ObjectAdded += Contour_ObjectAdded;
            }
            else
            {
                this.BindingContext = null;
            }
        }

        private void Contour_ObjectAdded(object sender, object e)
        {
            DrawObject(e);
        }

        /// <summary>
        /// Select visual form for inner Contour object
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public object DrawObject(object Object)
        {
            if (Object is CadPoint cadPoint)
            {
                this.Add(new VisualAnchor(cadPoint)
                {
                    Scale = 1 / this.GroupLayout.Scale,
                });
                return cadPoint;
            }
            if (Object is ConstraintLenth lenthConstrait)
            {
                this.Add(new LenthLabel(lenthConstrait));
                this.Add(new VisualLine(lenthConstrait)
                {
                    StrokeThickness = 5 * 1 / this.GroupLayout.Scale,
                });
                return lenthConstrait;
            }
            if (Object is ConstraintAngle angleConstrait)
            {
                this.Add(new AngleLabel(angleConstrait));
                return angleConstrait;
            }

            return null;
        }

        public async void DrawContour(Contour contour)
        {
            if (contour.Paths != null)
            {
                foreach (CadPoint point in contour.Points)
                {
                    DrawObject(point);
                }

                foreach (ConstraintLenth constraintLenth in contour.Lenths)
                {
                    DrawObject(constraintLenth);
                }

                foreach (ConstraintAngle constraintAngle in contour.Angles)
                {
                    DrawObject(constraintAngle);
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

                Debug.WriteLine($"Fit{this.GroupLayout.TranslationX - this.MainLayout.Width / 2 / this.GroupLayout.Scale}:{this.GroupLayout.TranslationY - this.MainLayout.Width / 2 / this.GroupLayout.Scale}");

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

            if (this.Contour != null)
            {
                this.Contour.Clear();
            }
        }

        public void VisualClear()
        {
            this.AnchorLayout.Children.Clear();
            this.ObjectLayout.Children.Clear();
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
            if (Object is ActiveObject activeObject)
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

        private void CanvasObject_Removed(object sender, bool e)
        {
            Remove(sender);
        }

        private void CadAnchor1_Dropped(object sender, object e)
        {
            if (sender != e)
            {
                if (sender is VisualAnchor cadAnchor2 && e is VisualAnchor cadAnchor1)
                {
                    ICommand ConnectPoint = new Command(async () =>
                    {
                        this.Contour.Add(new ConstraintLenth(cadAnchor1.cadPoint, cadAnchor2.cadPoint, -1), true);
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

            //Moved(null, null);
            //Console.WriteLine($"{this.GroupLayout.TranslationX} {this.GroupLayout.TranslationY}");
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
                CadCanvas.canvasscale = this.GroupLayout.Scale;
                CallRegularSize();
            }
        }

        private void TranslateToPoint(Point point)
        {
            this.GroupLayout.TranslationX = -((this.GroupLayout.Width * (1 - this.GroupLayout.Scale)) / 2) - 
                (point.X * this.GroupLayout.Scale - this.MainLayout.Width / 2 );
            this.GroupLayout.TranslationY = -((this.GroupLayout.Height * (1 - this.GroupLayout.Scale)) / 2) -
                (point.Y * this.GroupLayout.Scale - this.MainLayout.Height / 2);
        }
    }


}
