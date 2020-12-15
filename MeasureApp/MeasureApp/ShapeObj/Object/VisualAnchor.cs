﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public class VisualAnchor : VisualObject
    {
        private ICommand Fix => new Command(() =>
        {
            this.IsFix = !this.IsFix;
        });
        private ICommand Remove => new Command(async () =>
        {
            this.TryRemove();
        });

        public override event EventHandler Removed;

        public EllipseGeometry _ellipse;

        public CadPoint cadPoint;

        public override string ID => cadPoint.ID;

        public bool IsFix { get => cadPoint.IsFix; set => cadPoint.IsFix = value; }

        public override bool IsSelect 
        {
            get => cadPoint.IsSelect;
            set
            {
                cadPoint.IsSelect = value;
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                    this.Stroke = (cadPoint.IsSelect == true ? Brush.Orange : Brush.Blue);
                });
            }
        }

        public override double X { get => this.cadPoint.OX; set => this.cadPoint.OX = value; }
        public override double Y { get => this.cadPoint.OY; set => this.cadPoint.OY = value; }

        public VisualAnchor(CadPoint point) : base(true)
        {
            this.cadPoint = point;
            this.cadPoint.PropertyChanged += CadPoint_PropertyChanged;
            this.cadPoint.ChangedPoint += CadPoint_ChangedPoint;
            this.cadPoint.Removed += CadPoint_Removed;
            this._ellipse = new EllipseGeometry()
            {
                Center = new Point(CadCanvas.RegularAnchorSize + CadCanvas.RegularAnchorSize / 2, CadCanvas.RegularAnchorSize + CadCanvas.RegularAnchorSize / 2),
                RadiusX = CadCanvas.RegularAnchorSize,
                RadiusY = CadCanvas.RegularAnchorSize
            };

            this.TranslationX = this.X - CadCanvas.RegularAnchorSize - this.StrokeThickness;
            this.TranslationY = this.Y - CadCanvas.RegularAnchorSize - this.StrokeThickness;

            this.Data = this._ellipse;

            CadCanvas.DragSize += CadCanvas_DragSize;
            CadCanvas.RegularSize += CadCanvas_RegularSize;

            this.Stroke = cadPoint.IsSelect == true ? Brush.Orange : Brush.Blue;

            this.SheetMenu = new LabelObject.SheetMenu(new List<LabelObject.SheetMenuItem>()
            {
                new LabelObject.SheetMenuItem(Fix, "{FIX}"),
                new LabelObject.SheetMenuItem(Remove, "{REMOVE}")
            });
        }

        private void CadPoint_Removed(object sender, EventArgs e)
        {
            Removed?.Invoke(this, null);
        }

        private void CadPoint_ChangedPoint(object sender, CadPoint e)
        {
            this.cadPoint.PropertyChanged -= CadPoint_PropertyChanged;
            this.cadPoint.ChangedPoint -= CadPoint_ChangedPoint;
            this.cadPoint = e;
            this.cadPoint.PropertyChanged += CadPoint_PropertyChanged;
            this.cadPoint.ChangedPoint += CadPoint_ChangedPoint;
            Update();
        }

        public void ChangedPoint(CadPoint cadPoint)
        {
            this.cadPoint.ChangePoint(cadPoint);
            Removed?.Invoke(this, null);
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
                Update();
            }
        }

        public override void Update()
        {
            this.TranslationX = this.X - CadCanvas.RegularAnchorSize - this.StrokeThickness;
            this.TranslationY = this.Y - CadCanvas.RegularAnchorSize - this.StrokeThickness;
            this.OnPropertyChanged("Point");
        }

        public override void TryRemove()
        {
            this.cadPoint.TryRemove();
        }
    }
}