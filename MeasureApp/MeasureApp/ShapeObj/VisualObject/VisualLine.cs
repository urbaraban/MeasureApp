using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.Interface;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public class VisualLine : Path, CanvasObject
    {
        public ConstraintLenth AnchorsConstrait;
        public event EventHandler<bool> Removed;

        public new Xamarin.Forms.Rectangle Bounds => new Xamarin.Forms.Rectangle(
            Math.Min(AnchorsConstrait.Point1.OX, AnchorsConstrait.Point2.OX), 
            Math.Min(AnchorsConstrait.Point1.OY, AnchorsConstrait.Point2.OY), 
            Math.Abs(AnchorsConstrait.Point2.OX - AnchorsConstrait.Point1.OX), 
            Math.Abs(AnchorsConstrait.Point2.OY - AnchorsConstrait.Point1.OY));

        /// <summary>
        /// Visualize line between two anchor
        /// </summary>
        /// <param name="anchorAnchorLenth"></param>
        public VisualLine(ConstraintLenth anchorAnchorLenth, bool TempFlag)
        {
            this.HorizontalOptions = LayoutOptions.Start;
            this.VerticalOptions = LayoutOptions.Start;
            this.Stroke = anchorAnchorLenth.IsSelect == true ? Brush.Orange : Brush.Blue;
            this.StrokeThickness = 5;
            this.Fill = Brush.Blue;

            this.AnchorsConstrait = anchorAnchorLenth;
            this.AnchorsConstrait.IsSupport = TempFlag;
            this.AnchorsConstrait.Changed += Anchors_Changed;
            this.AnchorsConstrait.Removed += AnchorsConstrait_Removed;
            this.AnchorsConstrait.Supported += AnchorsConstrait_Supported;
            this.AnchorsConstrait.Selected += AnchorsConstrait_Selected;
            this.StrokeLineCap = PenLineCap.Round;
            CadCanvas.RegularSize += CadCanvas_RegularSize;
            Update();
        }

        private void AnchorsConstrait_Selected(object sender, bool e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                this.Stroke = e == true ? Brush.Orange : Brush.Blue;
            });
        }

        private void AnchorsConstrait_Supported(object sender, bool e)
        {
            if (e == true)
            {
                this.Stroke = Brush.LightGray;
                this.StrokeDashArray = new DoubleCollection() { 1, 2 };
            }
            else
            {
                this.Stroke = Brush.Blue;
                this.StrokeDashArray = new DoubleCollection() { 1 };
            }
        }

        private void AnchorsConstrait_Removed(object sender, bool e)
        {
            Removed?.Invoke(this, e);
        }

        /// <summary>
        /// Update geometry and layout line
        /// </summary>
        public void Update()
        {

            double Offset = 10;

            double MinX = Math.Min(this.AnchorsConstrait.Point1.OX, this.AnchorsConstrait.Point2.OX);
            double MinY = Math.Min(this.AnchorsConstrait.Point1.OY, this.AnchorsConstrait.Point2.OY);
            double MaxX = Math.Max(this.AnchorsConstrait.Point2.OX, this.AnchorsConstrait.Point1.OX) + CadCanvas.RegularAnchorSize;
            double MaxY = Math.Max(this.AnchorsConstrait.Point2.OY, this.AnchorsConstrait.Point1.OY) + CadCanvas.RegularAnchorSize;

            Xamarin.Forms.Device.InvokeOnMainThreadAsync(() => {
                this.Layout(new Xamarin.Forms.Rectangle(0, 0, MaxX, MaxY));
            });

            /*
            this.TranslationX = MinX - Offset;
            this.TranslationY = MinY - Offset;
            */

            this.Data = new LineGeometry()
            {
                StartPoint = new Point(this.AnchorsConstrait.Point1.OX, this.AnchorsConstrait.Point1.OY),
                EndPoint = new Point(this.AnchorsConstrait.Point2.OX, this.AnchorsConstrait.Point2.OY)
            };
 
        }

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.StrokeThickness = 5 * 1 / e;
        }

        private void Anchors_Changed(object sender, EventArgs e)
        {
            Update();
        }




    }
}

