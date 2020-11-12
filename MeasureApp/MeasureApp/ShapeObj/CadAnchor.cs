using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public class CadAnchor : CadObject
    {

        public EllipseGeometry _ellipse;

        public CadPoint cadPoint;

        public override double X { get => this.cadPoint.X; set => this.cadPoint.X = value; }
        public override double Y { get => this.cadPoint.Y; set => this.cadPoint.Y = value; }

        private double offcet => this._ellipse.RadiusX + this.StrokeThickness / 2;

        public CadAnchor(CadPoint point, double radius = 7) : base(true)
        {
            new EllipseGeometry();
            this.cadPoint = point;
            this.cadPoint.PropertyChanged += CadPoint_PropertyChanged;
            this._ellipse = new EllipseGeometry()
            {
               Center = new Point(radius + this.StrokeThickness / 2, radius + this.StrokeThickness / 2),
               RadiusX = radius,
               RadiusY = radius
            };

            this.TranslationX = point.X - this.offcet;
            this.TranslationY = point.Y - this.offcet;

            this.Data = this._ellipse;
        }

        private void CadPoint_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                this.TranslationX = this.cadPoint.X - this.offcet;
                this.TranslationY = this.cadPoint.Y - this.offcet;
                this.OnPropertyChanged("Point");
            }
        }
    }
}
