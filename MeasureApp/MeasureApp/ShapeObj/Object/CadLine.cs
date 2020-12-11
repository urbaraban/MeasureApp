using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.Interface;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public class CadLine : Path, CommonObject
    {
        public LenthConstrait AnchorsConstrait;
        public event EventHandler Removed;

        public new Xamarin.Forms.Rectangle Bounds => new Xamarin.Forms.Rectangle(
            Math.Min(AnchorsConstrait.Anchor1.X, AnchorsConstrait.Anchor2.X), 
            Math.Min(AnchorsConstrait.Anchor1.Y, AnchorsConstrait.Anchor2.Y), 
            Math.Abs(AnchorsConstrait.Anchor2.X - AnchorsConstrait.Anchor1.X), 
            Math.Abs(AnchorsConstrait.Anchor2.Y - AnchorsConstrait.Anchor1.Y));

        /// <summary>
        /// Visualize line between two anchor
        /// </summary>
        /// <param name="anchorAnchorLenth"></param>
        public CadLine(LenthConstrait anchorAnchorLenth, bool TempFlag)
        {
            this.HorizontalOptions = LayoutOptions.StartAndExpand;
            this.VerticalOptions = LayoutOptions.StartAndExpand;
            this.Stroke = Brush.Blue;
            this.StrokeThickness = 5;
            this.Fill = Brush.White;

            this.AnchorsConstrait = anchorAnchorLenth;
            this.AnchorsConstrait.IsSupport = TempFlag;
            this.AnchorsConstrait.Changed += Anchors_Changed;
            this.AnchorsConstrait.Removed += AnchorsConstrait_Removed;
            this.AnchorsConstrait.Supported += AnchorsConstrait_Supported;
            this.StrokeLineCap = PenLineCap.Round;
            CadCanvas.RegularSize += CadCanvas_RegularSize;
            AnchorsConstrait.PropertyChanged += AnchorsConstrait_PropertyChanged;
            Update();
        }

        private void AnchorsConstrait_Supported(object sender, bool e)
        {
            if (e == true)
            {
                this.Stroke = Brush.LightGray;
                this.StrokeDashArray = new DoubleCollection() { 1, 1 };
            }
            else
            {
                this.Stroke = Brush.Blue;
                this.StrokeDashArray = new DoubleCollection() { 1 };
            }
        }

        private void AnchorsConstrait_Removed(object sender, EventArgs e)
        {
            TryRemove();
        }

        public void Update()
        {
            double Offset = 10;

            double MinX = Math.Min(this.AnchorsConstrait.Anchor1.cadPoint.X, this.AnchorsConstrait.Anchor2.cadPoint.X);
            double MinY = Math.Min(this.AnchorsConstrait.Anchor1.cadPoint.Y, this.AnchorsConstrait.Anchor2.cadPoint.Y);
            double MaxX = Math.Max(this.AnchorsConstrait.Anchor2.cadPoint.X, this.AnchorsConstrait.Anchor1.cadPoint.X);
            double MaxY = Math.Max(this.AnchorsConstrait.Anchor2.cadPoint.Y, this.AnchorsConstrait.Anchor1.cadPoint.Y);

            this.TranslationX = MinX - Offset;
            this.TranslationY = MinY - Offset;

            this.Data = new LineGeometry()
            {
                StartPoint = new Point(this.AnchorsConstrait.Anchor1.cadPoint.Point.X - this.TranslationX, this.AnchorsConstrait.Anchor1.cadPoint.Point.Y - this.TranslationY),
                EndPoint = new Point(this.AnchorsConstrait.Anchor2.cadPoint.Point.X - this.TranslationX, this.AnchorsConstrait.Anchor2.cadPoint.Point.Y - this.TranslationY)
            };
            Xamarin.Forms.Device.InvokeOnMainThreadAsync(() => {
                this.Layout(new Xamarin.Forms.Rectangle(0, 0, MaxX - this.TranslationX + Offset, MaxY - this.TranslationY + Offset));
            });
        }

        public void TryRemove()
        {
            Removed?.Invoke(this, null);
        }

        private void AnchorsConstrait_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsTemp")
            Xamarin.Forms.Device.InvokeOnMainThreadAsync(() =>
            {
                this.StrokeDashArray = this.AnchorsConstrait.IsSupport ? new DoubleCollection() { 1, 1 } : new DoubleCollection() { 1 };
                this.Stroke = this.AnchorsConstrait.IsSupport ? Brush.LightGray : Brush.Blue;
            });
            
        }

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.StrokeThickness = 5 * 1 / e;
        }

        private void Anchors_Changed(object sender, EventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Update geometry and layout line
        /// </summary>


    }
}

