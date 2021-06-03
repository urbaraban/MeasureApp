using DrawEngine.CadObjects;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.View.Canvas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace SureMeasure.ShapeObj
{
    public class VisualAnchor : VisualObject, ICanvasObject
    {
        private ICommand Fix => new Command(() =>
        {
            this.IsFix = !this.IsFix;
        });
        private ICommand Remove => new Command(() =>
        {
            this.cadPoint.TryRemove();
        });
        private ICommand LastPoint => new Command(() =>
        {
            this.cadPoint.IsSelect = true;
        });

        private ICommand BasePoint => new Command(() =>
        {
            this.cadPoint.IsBase = true;
        });

        private ICommand Split => new Command(() =>
        {
            this.cadPoint.MakeSplit();
        });

        public override event EventHandler<bool> Removed;

        public EllipseGeometry _ellipse;

        public CadPoint cadPoint;

        public override string ID => cadPoint.ID;

        public bool IsFix { get => cadPoint.IsFix; set => cadPoint.IsFix = value; }

        public override bool IsSelect 
        {
            get => cadPoint.IsSelect;
            set => cadPoint.IsSelect = value;
        }

        public bool IsBase => this.cadPoint.IsBase;

        public override double X { get => this.cadPoint.OX; set => this.cadPoint.OX = value; }
        public override double Y { get => this.cadPoint.OY; set => this.cadPoint.OY = value; }

        public VisualAnchor(CadPoint point) : base(true)
        {
            this.cadPoint = point;
            this.cadPoint.PropertyChanged += CadPoint_PropertyChanged;
            this.cadPoint.ChangedPoint += CadPoint_ChangedPoint;
            this.cadPoint.Removed += CadPoint_Removed;
            this.cadPoint.Selected += CadPoint_Selected;
            this._ellipse = new EllipseGeometry()
            {
                Center = new Point(CanvasView.RegularAnchorSize + CanvasView.RegularAnchorSize / 2, CanvasView.RegularAnchorSize + CanvasView.RegularAnchorSize / 2),
                RadiusX = CanvasView.RegularAnchorSize,
                RadiusY = CanvasView.RegularAnchorSize,
            };

            this.TranslationX = this.X - CanvasView.RegularAnchorSize - this.StrokeThickness;
            this.TranslationY = this.Y - CanvasView.RegularAnchorSize - this.StrokeThickness;

            this.Data = this._ellipse;

            CanvasView.DragSize += CadCanvas_DragSize;
            CanvasView.RegularSize += CadCanvas_RegularSize;

            this.Stroke = cadPoint.IsSelect == true ? Brush.Orange : Brush.Blue;

            this.SheetMenu = new SheetMenu(new List<SheetMenuItem>()
            {
                new SheetMenuItem(Fix, "{FIX}"),
                new SheetMenuItem(Remove, "{REMOVE}"),
                new SheetMenuItem(LastPoint, "{LASTPOINT}"),
                new SheetMenuItem(BasePoint, "{BASEPOINT}"),
                new SheetMenuItem(Split, "{SPLIT}")
            });
        }

        private async void CadPoint_Selected(object sender, bool e) => await Update("IsSelected");

        private void CadPoint_Removed(object sender, bool e)
        {
            Removed?.Invoke(this, true);
        }

        private async void CadPoint_ChangedPoint(object sender, CadPoint e)
        {
            this.cadPoint.PropertyChanged -= CadPoint_PropertyChanged;
            this.cadPoint.ChangedPoint -= CadPoint_ChangedPoint;
            this.cadPoint = e;
            this.cadPoint.PropertyChanged += CadPoint_PropertyChanged;
            this.cadPoint.ChangedPoint += CadPoint_ChangedPoint;
            await Update("Point");
        }

        public void ChangedPoint(CadPoint cadPoint)
        {
            this.cadPoint.ChangePoint(cadPoint);
            Removed?.Invoke(this, true);
        }

        private void CadCanvas_RegularSize(object sender, double e) => this.ScaleTo(1 / e, 250, Easing.Linear);


        private void CadCanvas_DragSize(object sender, double e) => this.ScaleTo((1 / e) * 1.5, 250, Easing.Linear);


        private void CadPoint_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => this.Update(e.PropertyName);

        public async Task Update(string Param)
        {
            this.TranslationX = this.X - CanvasView.RegularAnchorSize - this.StrokeThickness;
            this.TranslationY = this.Y - CanvasView.RegularAnchorSize - this.StrokeThickness;

            await Xamarin.Forms.Device.InvokeOnMainThreadAsync(() =>
                {
                    if (this.IsSelect == true)
                        this.Stroke = Brush.Orange;
                    else if (this.IsBase == true)
                        this.Stroke = Brush.DarkRed;
                    else if (this.IsFix == true)
                        this.Stroke = Brush.Gray;
                    else
                        this.Stroke = Brush.Blue;
                });
            
            this.OnPropertyChanged("Point");
        }

        public override void TryRemove()
        {
            this.cadPoint.TryRemove();
        }

        public override string ToString()
        {
            return $"{this.ID}:{this.X} {this.Y}";
        }
    }
}
