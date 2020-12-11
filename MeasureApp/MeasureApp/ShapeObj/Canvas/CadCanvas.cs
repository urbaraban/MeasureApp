﻿using App1;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.Interface;
using MeasureApp.ShapeObj.LabelObject;
using MeasureApp.Tools;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj
{
    public class CadCanvas : ContentView
    {



        public static new double Width = 100000;
        public static new double Height = 100000;
        public static Point ZeroPoint = new Point(5000, 5000);

        public static event EventHandler<double> DragSize;
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

        private DrawMethod _method = DrawMethod.StepByStep;

        public DrawMethod Method
        {
            get => this._method;
            set
            {
                this._method = value;
                OnPropertyChanged("Method");
            }
        }

        
        public event EventHandler<CadVariable> CallValueDialog;

        private AbsoluteLayout MainLayout;
        private AbsoluteLayout GroupLayout;
        private AbsoluteLayout ObjectLayout;
        private AbsoluteLayout AnchorLayout;

        private PanGestureRecognizer panGesture = new PanGestureRecognizer();

        private double startScale = 1;
        private double startProp = 1;
        
       

        private Point startPoint = new Point(0, 0);

        public LenthConstrait LastLenthConstrait { get; internal set; }
        public CadAnchor StartAnchor { get; internal set; }

        public CadAnchor LastAnchor { get; internal set; }

        public CadCanvas()
        {
            this.IsClippedToBounds = false;
            AppShell.LenthUpdated += AppShell_LenthUpdated;

            this.WidthRequest = 100;
            this.HeightRequest = 100;

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
            
        }

        private void AppShell_LenthUpdated(object sender, Tuple<double, double> e)
        {
            BuildLine(e.Item1, e.Item2);
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
                    if (visualElement is CadLine cadLine)
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

        public void Clear()
        {
            this.AnchorLayout.Children.Clear();
            this.ObjectLayout.Children.Clear();
            this.StartAnchor = null;
            this.LastLenthConstrait = null;
            this.LastAnchor = null;
            this.GroupLayout.Scale = 1;
            this.GroupLayout.TranslationX = -ZeroPoint.X + 100;
            this.GroupLayout.TranslationY = -ZeroPoint.Y + 100;
        }

        public void Remove(CadObject cadObject)
        {
            this.ObjectLayout.Children.Remove(cadObject);
            this.AnchorLayout.Children.Remove(cadObject);
        }

        /// <summary>
        /// Add object on Canvas. 
        /// </summary>
        /// <param name="Object"></param>
        public void Add(object Object)
        {
            if (Object is CommonObject canvasObject)
            {
                canvasObject.Removed += CanvasObject_Removed;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Object is CadLine)
                {
                    this.ObjectLayout.Children.Add((CadLine)Object);
                }
                if (Object is CadAnchor)
                {
                    this.AnchorLayout.Children.Add((CadAnchor)Object);
                }
                if (Object is ConstraitLabel)
                {
                    this.AnchorLayout.Children.Add((ConstraitLabel)Object);
                }
                if (Object is AbsoluteLayout)
                {
                    this.GroupLayout.Children.Add((AbsoluteLayout)Object);
                }
            });
        }

        private void CanvasObject_Removed(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (sender is CadLine)
                {
                    this.ObjectLayout.Children.Remove((CadLine)sender);
                }
                if (sender is CadAnchor)
                {
                    this.AnchorLayout.Children.Remove((CadAnchor)sender);
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
        /// Find position and make line on canvas with anchor
        /// </summary>
        /// <param name="Lenth"></param>
        /// <param name="Angle"></param>
       

        private void CadObject_Selected(object sender, bool e)
        {
            if (e == true)
            {
                if (sender is CadAnchor cadAnchor)
                {
                    if (this.StartAnchor != null)
                    {
                        this.StartAnchor.IsSelect = false;
                    }
                    this.StartAnchor = cadAnchor;
                }
                if (sender is LenthConstrait lenthConstrait)
                {
                    if (this.LastLenthConstrait != null)
                    {
                        this.LastLenthConstrait.IsSelect = false;
                    }
                    this.LastLenthConstrait = lenthConstrait;
                }
            }
        }

        private async void CadAnchor1_Droped(object sender, object e)
        {
            if (sender != e)
            {
                if (sender is CadAnchor cadAnchor2 && e is CadAnchor cadAnchor1)
                {
                    ICommand ConnectPoint = new Command(async () =>
                    {
                        MakeLine(new LenthConstrait(cadAnchor1, cadAnchor2, -1), -1);
                    });
                    ICommand MergePoint = new Command(async () =>
                    {
                        cadAnchor1.ChangeAnchor(cadAnchor2);
                    });

                    SheetMenu sheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>() {
                        new SheetMenuItem(ConnectPoint, "{CONNECT_POINT}"),
                        new SheetMenuItem(MergePoint, "{MERGE_POINT}")
                    });

                    string result = await AppShell.Instance.SheetMenuDialog(sheetMenu);
                    switch (result)
                    {
                        case "Connect":
                            MakeLine(new LenthConstrait(cadAnchor1, cadAnchor2, -1), -1);
                            break;
                        case "Merge":
                            cadAnchor1.ChangeAnchor(cadAnchor2);
                            break;
                    }


                }
            }
            }



        private CadAnchor MakeAnchor(double X, double Y, bool Visible = true)
        {
            CadAnchor cadAnchor = new CadAnchor(new CadPoint(X, Y));
            cadAnchor.Scale = 1 / this.GroupLayout.Scale;
            cadAnchor.Droped += CadAnchor1_Droped;
            cadAnchor.Selected += CadObject_Selected;
            cadAnchor.Removed += CadAnchor_Removed;
            cadAnchor.IsVisible = Visible;
            this.Add(cadAnchor);
            return cadAnchor;
        }

        private void CadAnchor_Removed(object sender, EventArgs e)
        {
            Remove((CadObject)sender);
        }

        public void BuildLine(double Lenth, double Angle)
        {
            CadLine cadLine = null;

            //Если у нас нет линии привязки
            if (this.LastLenthConstrait == null)
            {
                cadLine = MakeLine(new LenthConstrait(MakeAnchor(ZeroPoint.X, ZeroPoint.Y), MakeAnchor(ZeroPoint.X, ZeroPoint.Y + Lenth), Lenth), Angle);
            }
            else
            {
                if (this.LastLenthConstrait is LenthConstrait)
                {
                    Point point = Sizing.GetPositionLineFromAngle(this.LastLenthConstrait.Anchor1.cadPoint, this.LastLenthConstrait.Anchor2.cadPoint, Lenth, Angle < 0 ? 90 : Angle);
                    CadAnchor cadAnchor2 = MakeAnchor(point.X, point.Y);
                    cadLine = MakeLine(new LenthConstrait(this.LastAnchor, cadAnchor2, Lenth), Angle);

                    this.AddConstraiLabel(new AngleLabel(new AngleConstrait(this.LastLenthConstrait, cadLine.AnchorsConstrait, Angle)));
                }
            }

            if (cadLine != null)
            {
                if (this.Method == DrawMethod.StepByStep || this.StartAnchor == null || this.LastLenthConstrait == null)
                {
                    cadLine.AnchorsConstrait.IsSelect = true;
                    this.LastAnchor = cadLine.AnchorsConstrait.Anchor2;

                    if (this.Method == DrawMethod.StepByStep)
                    {
                        cadLine.AnchorsConstrait.Anchor2.IsSelect = true;
                    }
                    else if (this.StartAnchor == null)
                    {
                        cadLine.AnchorsConstrait.Anchor1.IsSelect = true;
                    }
                }
            }
        }

        private CadLine MakeLine(LenthConstrait lenthAnchorAnchor, double angle)
        {
            CadLine cadLine = new CadLine(lenthAnchorAnchor, false);
            cadLine.StrokeThickness = 5 *  1 / this.GroupLayout.Scale;
            cadLine.AnchorsConstrait.Selected += CadObject_Selected;

            LineLabel lineLabel = new LineLabel(cadLine.AnchorsConstrait);
            this.Add(cadLine);
            this.AddConstraiLabel(lineLabel);

            return cadLine;
        }

        private void AddConstraiLabel(ConstraitLabel constraitLabel)
        {
            constraitLabel.Scale = 1 / this.GroupLayout.Scale;
            this.Add(constraitLabel);
        }


        private void LineLabel_CallValueDialog(object sender, CadVariable e)
        {
            CallValueDialog?.Invoke(sender, e);
        }

    }

    public enum DrawMethod
    {
        StepByStep,
        FromPoint
    }
}
