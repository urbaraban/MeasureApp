using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using System;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public class CadLine : CadObject
    {
        private bool _temp = false;

        public bool TempLine {
            get => this._temp;
            set
            {
                this._temp = value;
                if (this._temp == true)
                {
                    this.StrokeDashArray = new DoubleCollection() { 3, 2 };
                }
                else
                {
                    this.StrokeDashArray = new DoubleCollection() { 1 };
                }
            }
            
        }

        public LenthConstrait AnchorsConstrait;

        public new Xamarin.Forms.Rectangle Bounds => new Xamarin.Forms.Rectangle(
            Math.Min(AnchorsConstrait.Anchor1.X, AnchorsConstrait.Anchor2.X), 
            Math.Min(AnchorsConstrait.Anchor1.Y, AnchorsConstrait.Anchor2.Y), 
            Math.Abs(AnchorsConstrait.Anchor2.X - AnchorsConstrait.Anchor1.X), 
            Math.Abs(AnchorsConstrait.Anchor2.Y - AnchorsConstrait.Anchor1.Y));

        /// <summary>
        /// Visualize line between two anchor
        /// </summary>
        /// <param name="anchorAnchorLenth"></param>
        public CadLine(LenthConstrait anchorAnchorLenth, bool TempFlag) : base(false)
        {
            this.TempLine = TempFlag;
            this.AnchorsConstrait = anchorAnchorLenth;
            this.AnchorsConstrait.Changed += Anchors_Changed;
            this.StrokeLineCap = PenLineCap.Round;
            CadCanvas.RegularSize += CadCanvas_RegularSize;
            Update();
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

            this.Layout(new Xamarin.Forms.Rectangle(0, 0, MaxX - this.TranslationX + Offset, MaxY - this.TranslationY + Offset));



        }

        public double Lenth => Sizing.PtPLenth(this.AnchorsConstrait.Anchor1.cadPoint, this.AnchorsConstrait.Anchor2.cadPoint);

    }
}

