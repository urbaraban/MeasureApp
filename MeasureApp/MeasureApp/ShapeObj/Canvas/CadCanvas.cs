using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MeasureApp.ShapeObj.Canvas
{
    public class CadCanvas : ContentView
    {
        public static double Width = 1000;
        public static double Height = 1000;

        public event EventHandler<double> ScaleChanged;

        private AbsoluteLayout ObjectLayout;

        private PanGestureRecognizer panGesture = new PanGestureRecognizer();

        private double currentScale = 1;
        private double startScale = 1;
        private double xOffset = 0;
        private double yOffset = 0;
       

        private Point startPoint = new Point(0, 0);

        public AnchorAnchorLenth SelectedLine { get; internal set; }

        public CadCanvas()
        {
            this.ObjectLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand
            };

            this.ObjectLayout.BackgroundColor = Color.White;
            this.ObjectLayout.WidthRequest = CadCanvas.Width;
            this.ObjectLayout.HeightRequest = CadCanvas.Height;
            this.ObjectLayout.ScaleY = -1;

            this.ObjectLayout.Layout(new Rectangle(-100, -100, CadCanvas.Width + 100, CadCanvas.Height + 100));

            this.ObjectLayout.ChildAdded += Canvas_ChildAdded;

            PinchGestureRecognizer pinchGestureRecognizer = new PinchGestureRecognizer();
            pinchGestureRecognizer.PinchUpdated += PinchGestureRecognizer_PinchUpdated;

            this.GestureRecognizers.Add(pinchGestureRecognizer);

            panGesture.PanUpdated += PanGesture_PanUpdated;
            this.GestureRecognizers.Add(panGesture);

            this.Content = this.ObjectLayout;
        }

        public void FitChild()
        {
            if (this.ObjectLayout.Children.Count > 0)
            {
                double minX = this.ObjectLayout.Children[0].Bounds.X,
                    maxX = this.ObjectLayout.Children[0].Bounds.X + this.ObjectLayout.Children[0].Bounds.Width,
                    minY = this.ObjectLayout.Children[0].Bounds.Y,
                    maxY = this.ObjectLayout.Children[0].Bounds.Y + this.ObjectLayout.Children[0].Bounds.Height;

                foreach (VisualElement visualElement in this.ObjectLayout.Children)
                {
                    minX = Math.Min(minX, visualElement.X);
                    maxX = Math.Max(maxX, visualElement.X + this.ObjectLayout.Children[0].Bounds.Width);
                    minY = Math.Min(minY, visualElement.Y);
                    maxY = Math.Max(maxY, visualElement.Y + this.ObjectLayout.Children[0].Bounds.Height);
                }

                this.ObjectLayout.Scale = Math.Min(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density / (maxX - minX),
                     DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density / (maxY - minY));

                this.ObjectLayout.TranslationX -= ((maxX - minX) / 2 - DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density / 2) * this.Scale;
                this.ObjectLayout.TranslationY -= ((maxY - minY) / 2 - DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density / 2) * this.Scale;
            }
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (e.StatusType == GestureStatus.Started)
            {
                startPoint.X = this.ObjectLayout.TranslationX;
                startPoint.Y = this.ObjectLayout.TranslationY;
            }
            if (e.StatusType == GestureStatus.Running)
            {
                this.ObjectLayout.TranslationX = startPoint.X + e.TotalX;
                this.ObjectLayout.TranslationY = startPoint.Y + e.TotalY;
            }
        }

        private void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                // Store the current scale factor applied to the wrapped user interface element,
                // and zero the components for the center point of the translate transform.
                double startScale = this.ObjectLayout.Scale;
               // this.MainLayout.AnchorX = 0;
                //this.MainLayout.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.
                currentScale += (e.Scale - 1) * startScale/2;
               // currentScale = Math.Max(0.5, currentScale);

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the X pixel coordinate.
                double renderedX = this.ObjectLayout.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (this.ObjectLayout.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the Y pixel coordinate.
                double renderedY = this.ObjectLayout.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (this.ObjectLayout.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.
                double targetX = xOffset - (originX * this.ObjectLayout.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * this.ObjectLayout.Height) * (currentScale - startScale);

                // Apply translation based on the change in origin.
                this.ObjectLayout.TranslationX = targetX.Clamp(-this.ObjectLayout.Width * (currentScale - 1), 0);
                this.ObjectLayout.TranslationY = targetY.Clamp(-this.ObjectLayout.Height * (currentScale - 1), 0);

                // Apply scale factor.
                this.ObjectLayout.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                // Store the translation delta's of the wrapped user interface element.
                xOffset = this.ObjectLayout.TranslationX;
                yOffset = this.ObjectLayout.TranslationY;
            }
        }

        private void Canvas_ChildAdded(object sender, ElementEventArgs e)
        {

        }

        public void Clear()
        {
            this.ObjectLayout.Children.Clear();
            this.SelectedLine = null;
        }

        public void Remove(CadObject cadObject)
        {
            this.ObjectLayout.Children.Remove(cadObject);
            this.SelectedLine = null;
        }
        /*
        private double ch(double value)
        {
            return this.zeroOffcet.X + value;
        }

        private double cv(double value)
        {
            return this.ObjectLayout.HeightRequest - this.zeroOffcet.Y - value;
        }*/

        public void AddLine(double lineLenth, double lineAngle)
        {
            if (SelectedLine == null)
            {

                CadAnchor cadAnchor1 = new CadAnchor(new CadPoint(30, 30), 7);
                CadAnchor cadAnchor2 = new CadAnchor(new CadPoint(30, 30 + lineLenth), 7);
                
                CadLine cadLine = new CadLine(new AnchorAnchorLenth(cadAnchor1, cadAnchor2, lineLenth));
                //new PointLineMerge(cadLine, cadAnchor1, 1);
                //new PointLineMerge(cadLine, cadAnchor2, 1);

                this.ObjectLayout.Children.Add(cadAnchor1);
                this.ObjectLayout.Children.Add(cadAnchor2);
                this.ObjectLayout.Children.Add(cadLine);

                cadLine.Update();

                SelectedLine = cadLine.Anchors;
            }
            else
            {
                if (SelectedLine is AnchorAnchorLenth anchorAnchor)
                {
                    CadAnchor cadAnchor2 = new CadAnchor(Sizing.GetPositionLineFromAngle(anchorAnchor.Anchor1.cadPoint, anchorAnchor.Anchor2.cadPoint, lineLenth, lineAngle));

                    CadLine cadLine = new CadLine(new AnchorAnchorLenth(anchorAnchor.Anchor2, cadAnchor2, lineLenth));
                    //new PointLineMerge(cadLine, anchorAnchor.Anchor2, 1);
                    //new PointLineMerge(cadLine, cadAnchor2, 1);

                    new AngleBetweenThreeAnchor(anchorAnchor, cadLine.Anchors, lineAngle);

                    this.ObjectLayout.Children.Add(cadAnchor2);
                    this.ObjectLayout.Children.Add(cadLine);

                    SelectedLine = cadLine.Anchors;
                }
            }
                // 

                

            
           // this.Children.Add(cadLine);
  

           // Point point2 = new Point(100, 100);

           /* 
                this.Children.Add(new CadLine(new Point(0, 0), point2, lineAngle));
                this.SelectedObject = (CadLine)this.Children.Last();
            }
  
                if (SelectedObject is CadLine cadLine)
                {
                    this.Children.Add(cadLine.GetPositionLineFromAngle((float)lineLenth, (float)lineAngle));
                    this.SelectedObject = (CadLine)this.Children.Last();
                }
            }*/

            
        }
    }
}
