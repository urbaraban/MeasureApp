﻿using MeasureApp.ShapeObj.Constraints;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public class CadAnchor : CadObject
    {
        public event EventHandler<CadAnchor> ChangedAnchror;

        public List<CadConstraint> Constraints = new List<CadConstraint>();

        public EllipseGeometry _ellipse;

        public CadPoint cadPoint;

        public override double X { get => this.cadPoint.X; set => this.cadPoint.X = value; }
        public override double Y { get => this.cadPoint.Y; set => this.cadPoint.Y = value; }

        private double offcet => this._ellipse.RadiusX + this.StrokeThickness / 2;

        public CadAnchor(CadPoint point) : base(true)
        {
            this.cadPoint = point;
            this.cadPoint.PropertyChanged += CadPoint_PropertyChanged;
            this._ellipse = new EllipseGeometry()
            {
               Center = new Point(CadCanvas.RegularAnchorSize + this.StrokeThickness / 2, CadCanvas.RegularAnchorSize + this.StrokeThickness / 2),
               RadiusX = CadCanvas.RegularAnchorSize,
               RadiusY = CadCanvas.RegularAnchorSize
            };

            this.TranslationX = point.X - this.offcet;
            this.TranslationY = point.Y - this.offcet;

            this.Data = this._ellipse;

            CadCanvas.DragSize += CadCanvas_DragSize;
            CadCanvas.RegularSize += CadCanvas_RegularSize;

        }

        public void ChangeAnchor(CadAnchor cadAnchor)
        {
            ChangedAnchror?.Invoke(this, cadAnchor);
        }

        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.ScaleTo(1/e, 250, Easing.Linear);
        }

        private void CadCanvas_DragSize(object sender, double e)
        {
            this.ScaleTo(1/e * 1.5, 250, Easing.Linear);
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
