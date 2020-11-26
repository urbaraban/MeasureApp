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

        private LineGeometry Line;

        public LenthAnchorAnchor AnchorsConstrait;


        /// <summary>
        /// Visualize line between two anchor
        /// </summary>
        /// <param name="anchorAnchorLenth"></param>
        public CadLine(LenthAnchorAnchor anchorAnchorLenth, bool TempFlag) : base(false)
        {
            this.TempLine = TempFlag;
            this.AnchorsConstrait = anchorAnchorLenth;
            this.AnchorsConstrait.Changed += Anchors_Changed;
            this.StrokeLineCap = PenLineCap.Round;

            Update();
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
            double MinX = Math.Min(this.AnchorsConstrait.Anchor1.cadPoint.X, this.AnchorsConstrait.Anchor2.cadPoint.X) - Offset;
            double MinY = Math.Min(this.AnchorsConstrait.Anchor1.cadPoint.Y, this.AnchorsConstrait.Anchor2.cadPoint.Y) - Offset;
            double MaxX = Math.Abs(this.AnchorsConstrait.Anchor2.cadPoint.X - this.AnchorsConstrait.Anchor1.cadPoint.X) + Offset;
            double MaxY = Math.Abs(this.AnchorsConstrait.Anchor2.cadPoint.Y - this.AnchorsConstrait.Anchor1.cadPoint.Y) + Offset;

            this.Data = new LineGeometry()
            {
                StartPoint = new Point(this.AnchorsConstrait.Anchor1.cadPoint.Point.X - MinX, this.AnchorsConstrait.Anchor1.cadPoint.Point.Y - MinY),
                EndPoint = new Point(this.AnchorsConstrait.Anchor2.cadPoint.Point.X - MinX, this.AnchorsConstrait.Anchor2.cadPoint.Point.Y - MinY)
            };

            this.Layout(new Xamarin.Forms.Rectangle(MinX, MinY, MaxX, MaxY));


        }

        public double Lenth => Sizing.PtPLenth(this.AnchorsConstrait.Anchor1.cadPoint, this.AnchorsConstrait.Anchor2.cadPoint);

    }
}

