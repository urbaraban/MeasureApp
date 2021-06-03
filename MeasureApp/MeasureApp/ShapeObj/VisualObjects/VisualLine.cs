using DrawEngine.Constraints;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.View.Canvas;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace SureMeasure.ShapeObj
{
    public class VisualLine : Path, ICanvasObject
    {
        private readonly ConstraintLenth constraintLenth;

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
            this.AnchorX = 0;
            this.AnchorY = 0;
            this.HorizontalOptions = LayoutOptions.Start;
            this.VerticalOptions = LayoutOptions.Start;
            this.StrokeThickness = 5;

            this.constraintLenth = anchorAnchorLenth;
            this.constraintLenth.PropertyChanged += Anchors_Changed;
            this.constraintLenth.Removed += constraintLenth_Removed;
            this.constraintLenth.Selected += constraintLenth_Selected;
            this.constraintLenth.Variable.PropertyChanged += Variable_PropertyChanged;
            this.StrokeLineCap = PenLineCap.Round;
            CanvasView.RegularSize += CadCanvas_RegularSize;
            UpdateLayout();
            Update(string.Empty);
        }

        private void Variable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateLayout();
        }

        private async void UpdateLayout()
        {
            this.Data = new LineGeometry()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(this.constraintLenth.Value, 0)
            };

            await Xamarin.Forms.Device.InvokeOnMainThreadAsync(() => {
                this.Layout(new Xamarin.Forms.Rectangle(0, 0, this.constraintLenth.Value, this.StrokeThickness));
            });
        }

        private async void constraintLenth_Selected(object sender, bool e)  => await Update("Selected");


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
        public async Task Update(string Param)
        {
            if (constraintLenth.IsSupport) this.Stroke = Brush.LightGray;
            else if (constraintLenth.IsSelect) this.Stroke = Brush.DarkOrange;
            else this.Stroke = Brush.Blue;

            if (constraintLenth.IsSupport == true) this.StrokeDashArray = new DoubleCollection() { 1, 2 };
            else new DoubleCollection() { 1 };
          

            this.TranslationX = this.constraintLenth.Point1.OX;
            this.TranslationY = this.constraintLenth.Point1.OY;

            this.Rotation = Math.Atan2(constraintLenth.Vector.Y, constraintLenth.Vector.X) * (180 / Math.PI);
        }

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.StrokeThickness = 5 / e;
        }

        private async void Anchors_Changed(object sender, EventArgs e)
        {
            await Update("Changed");
        }




    }
}

