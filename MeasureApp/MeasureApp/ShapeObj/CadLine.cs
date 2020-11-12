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
        public LineGeometry Line;

        public AnchorAnchorLenth Anchors;


        /// <summary>
        /// Visualize line between two anchor
        /// </summary>
        /// <param name="anchorAnchorLenth"></param>
        public CadLine(AnchorAnchorLenth anchorAnchorLenth) : base(false)
        {
            this.Anchors = anchorAnchorLenth;

            this.Anchors.Changed += Anchors_Changed;
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
            this.Layout(new Xamarin.Forms.Rectangle(
                Math.Min(this.Anchors.Anchor1.cadPoint.X, this.Anchors.Anchor2.cadPoint.X), //change on postion on line
                Math.Min(this.Anchors.Anchor1.cadPoint.Y, this.Anchors.Anchor2.cadPoint.Y),
                Math.Abs(this.Anchors.Anchor2.cadPoint.X - this.Anchors.Anchor1.cadPoint.X),
                Math.Abs(this.Anchors.Anchor2.cadPoint.Y - this.Anchors.Anchor1.cadPoint.Y)));

            this.Data = new LineGeometry()
            {
                StartPoint = new Point(this.Anchors.Anchor1.cadPoint.Point.X - this.Bounds.X , this.Anchors.Anchor1.cadPoint.Point.Y - this.Bounds.Y),
                EndPoint = new Point(this.Anchors.Anchor2.cadPoint.Point.X - this.Bounds.X, this.Anchors.Anchor2.cadPoint.Point.Y - this.Bounds.Y)
            };
        }


        public double Lenth => Sizing.PtPLenth(this.Anchors.Anchor1.cadPoint, this.Anchors.Anchor2.cadPoint);

    }
}

