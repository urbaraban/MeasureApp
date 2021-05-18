using SureMeasure.ShapeObj.Canvas;
using SureMeasure.ShapeObj.Constraints;
using SureMeasure.ShapeObj.Interface;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace SureMeasure.ShapeObj
{
    public class VisualLine : Path, CanvasObject
    {
        private ConstraintLenth constraintLenth;
        public event EventHandler<bool> Removed;

        private LineGeometry lineGeometry => (LineGeometry)this.Data;

        public new Xamarin.Forms.Rectangle Bounds => new Xamarin.Forms.Rectangle(
            Math.Min(constraintLenth.Point1.OX, constraintLenth.Point2.OX), 
            Math.Min(constraintLenth.Point1.OY, constraintLenth.Point2.OY), 
            Math.Abs(constraintLenth.Point2.OX - constraintLenth.Point1.OX), 
            Math.Abs(constraintLenth.Point2.OY - constraintLenth.Point1.OY));

        /// <summary>
        /// Visualize line between two anchor
        /// </summary>
        /// <param name="anchorAnchorLenth"></param>
        public VisualLine(ConstraintLenth anchorAnchorLenth)
        {
            this.Data = new LineGeometry();
            this.HorizontalOptions = LayoutOptions.Start;
            this.VerticalOptions = LayoutOptions.Start;
            this.StrokeThickness = 5;

            this.constraintLenth = anchorAnchorLenth;
            this.constraintLenth.Changed += Anchors_Changed;
            this.constraintLenth.Removed += constraintLenth_Removed;
            this.constraintLenth.Supported += constraintLenth_Supported;
            this.constraintLenth.Selected += constraintLenth_Selected;
            this.StrokeLineCap = PenLineCap.Round;
            CadCanvas.RegularSize += CadCanvas_RegularSize;
            Update(string.Empty);
        }

        private void constraintLenth_Selected(object sender, bool e) => Update("Selected");


        private void constraintLenth_Supported(object sender, bool e)
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

        private void constraintLenth_Removed(object sender, bool e)
        {
            Removed?.Invoke(this, e);
        }

        /// <summary>
        /// Update geometry and layout line
        /// </summary>
        public void Update(string Param)
        {
            if (constraintLenth.IsSupport) this.Stroke = Brush.LightGray;
            else if (constraintLenth.IsSelect) this.Stroke = Brush.DarkOrange;
            else this.Stroke = Brush.Blue;

            if (constraintLenth.IsSupport == true) this.StrokeDashArray = new DoubleCollection() { 1, 2 };
            else new DoubleCollection() { 1 };

            double MinX = Math.Min(this.constraintLenth.Point1.OX, this.constraintLenth.Point2.OX);
            double MinY = Math.Min(this.constraintLenth.Point1.OY, this.constraintLenth.Point2.OY);
            double MaxX = Math.Max(this.constraintLenth.Point2.OX, this.constraintLenth.Point1.OX) + CadCanvas.RegularAnchorSize;
            double MaxY = Math.Max(this.constraintLenth.Point2.OY, this.constraintLenth.Point1.OY) + CadCanvas.RegularAnchorSize;

            this.Data = new LineGeometry()
            {
                StartPoint = new Point(this.constraintLenth.Point1.OX, this.constraintLenth.Point1.OY),
                EndPoint = new Point(this.constraintLenth.Point2.OX, this.constraintLenth.Point2.OY)
            };

            this.lineGeometry.Dispatcher.BeginInvokeOnMainThread(() => {
                this.Layout(new Xamarin.Forms.Rectangle(0, 0, MaxX, MaxY));
            });
        }

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.StrokeThickness = 5 / e;
        }

        private void Anchors_Changed(object sender, EventArgs e)
        {
            Update("Changed");
        }




    }
}

