using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.LabelObject;
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
        public event EventHandler<SheetMenu> ShowObjectMenu;
        public event EventHandler<CadVariable> CallValueDialog;

        private AbsoluteLayout MainLayout;
        private AbsoluteLayout ObjectLayout;
        private AbsoluteLayout AnchorLayout;

        private PanGestureRecognizer panGesture = new PanGestureRecognizer();

        private double currentScale = 1;
        private double startScale = 1;
        private double xOffset = 0;
        private double yOffset = 0;
       

        private Point startPoint = new Point(0, 0);

        public LenthAnchorAnchor SelectedLine { get; internal set; }

        public CadCanvas()
        {
            this.MainLayout = new AbsoluteLayout()
            {
                ScaleY = -1
            };

            this.AnchorLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                WidthRequest = CadCanvas.Width,
                HeightRequest = CadCanvas.Height,
            };
            //this.AnchorLayout.Layout(new Rectangle(-100, -100, CadCanvas.Width + 100, CadCanvas.Height + 100));
            this.AnchorLayout.ChildAdded += Canvas_ChildAdded;

            this.ObjectLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                BackgroundColor = Color.White,
                WidthRequest = CadCanvas.Width,
                HeightRequest = CadCanvas.Height,
            };
            this.ObjectLayout.Layout(new Rectangle(-100, -100, CadCanvas.Width + 100, CadCanvas.Height + 100));
            this.ObjectLayout.ChildAdded += Canvas_ChildAdded;



            PinchGestureRecognizer pinchGestureRecognizer = new PinchGestureRecognizer();
            pinchGestureRecognizer.PinchUpdated += PinchGestureRecognizer_PinchUpdated;

            this.GestureRecognizers.Add(pinchGestureRecognizer);

            panGesture.PanUpdated += PanGesture_PanUpdated;
            this.GestureRecognizers.Add(panGesture);
            this.MainLayout.Children.Add(this.ObjectLayout);
            this.MainLayout.Children.Add(this.AnchorLayout);

            this.Content = this.MainLayout;
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
                startPoint.X = this.MainLayout.TranslationX;
                startPoint.Y = this.MainLayout.TranslationY;
            }
            if (e.StatusType == GestureStatus.Running)
            {
                this.MainLayout.TranslationX = startPoint.X + e.TotalX;
                this.MainLayout.TranslationY = startPoint.Y + e.TotalY;
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
                double renderedX = this.MainLayout.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (this.MainLayout.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the Y pixel coordinate.
                double renderedY = this.MainLayout.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (this.MainLayout.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.
                double targetX = xOffset - (originX * this.MainLayout.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * this.MainLayout.Height) * (currentScale - startScale);

                // Apply translation based on the change in origin.
                this.MainLayout.TranslationX = targetX.Clamp(-this.MainLayout.Width * (currentScale - 1), 0);
                this.MainLayout.TranslationY = targetY.Clamp(-this.MainLayout.Height * (currentScale - 1), 0);

                // Apply scale factor.
                this.MainLayout.Scale = currentScale;
            }
            if (e.Status == GestureStatus.Completed)
            {
                // Store the translation delta's of the wrapped user interface element.
                xOffset = this.MainLayout.TranslationX;
                yOffset = this.MainLayout.TranslationY;
            }
        }

        private void Canvas_ChildAdded(object sender, ElementEventArgs e)
        {

        }

        public void Clear()
        {
            this.AnchorLayout.Children.Clear();
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
                cadAnchor1.Droped += CadAnchor1_Droped;
                CadAnchor cadAnchor2 = new CadAnchor(new CadPoint(30, 30 + lineLenth), 7);
                cadAnchor2.Droped += CadAnchor1_Droped;
                MakeLine(new LenthAnchorAnchor(cadAnchor1, cadAnchor2, lineLenth), lineAngle);
            }
            else
            {
                if (SelectedLine is LenthAnchorAnchor anchorAnchor)
                {
                    CadAnchor cadAnchor2 = new CadAnchor(Sizing.GetPositionLineFromAngle(anchorAnchor.Anchor1.cadPoint, anchorAnchor.Anchor2.cadPoint, lineLenth, lineAngle));
                    MakeLine(new LenthAnchorAnchor(anchorAnchor.Anchor2, cadAnchor2, lineLenth), lineAngle);

                }
            }
        }

        private void CadAnchor1_Droped(object sender, object e)
        {
            if (sender is CadAnchor cadAnchor2 && e is CadAnchor cadAnchor1)
            {
                MakeLine(new LenthAnchorAnchor(cadAnchor1, cadAnchor2, -1), -1);
            }
        }

        private void MakeLine(LenthAnchorAnchor lenthAnchorAnchor, double angle)
        {
            CadLine cadLine = new CadLine(lenthAnchorAnchor, false);
            LineLabel lineLabel = new LineLabel(cadLine);

            this.ObjectLayout.Children.Add(cadLine);

            this.AddConstraiLabel(lineLabel);
            this.AnchorLayout.Children.Add(lenthAnchorAnchor.Anchor2);

            if (SelectedLine is LenthAnchorAnchor anchorAnchor)
            {
              //  AngleLabel angleLabel = new AngleLabel(new AngleBetweenThreeAnchor(anchorAnchor, cadLine.AnchorsConstrait, angle));
              //  this.AddConstraiLabel(angleLabel);
            }
            else
            {
                this.AnchorLayout.Children.Add(lenthAnchorAnchor.Anchor1);
            }

            cadLine.Update();

            SelectedLine = cadLine.AnchorsConstrait;
        }

        private void AddConstraiLabel(ConstraitLabel constraitLabel)
        {
            constraitLabel.ShowObjectMenu += LineLabel_ShowObjectMenu;
            constraitLabel.CallValueDialog += LineLabel_CallValueDialog;
            this.AnchorLayout.Children.Add(constraitLabel);
        }

        private void LineLabel_CallValueDialog(object sender, CadVariable e)
        {
            CallValueDialog?.Invoke(sender, e);
        }

        private void LineLabel_ShowObjectMenu(object sender, SheetMenu e)
        {
            ShowObjectMenu?.Invoke(sender, e);
        }
    }
}
