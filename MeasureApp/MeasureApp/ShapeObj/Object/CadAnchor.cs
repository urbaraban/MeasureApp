using MeasureApp.ShapeObj.Constraints;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public class CadAnchor : CadObject
    {
        private ICommand Fix => new Command(() =>
        {
            this.IsFix = !this.IsFix;
        });

        public event EventHandler<CadAnchor> ChangedAnchror;
        public override event EventHandler Removed;

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


            this.SheetMenu = new LabelObject.SheetMenu(new List<LabelObject.SheetMenuItem>()
            {
                new LabelObject.SheetMenuItem(Fix, "{FIX}")
            });

            this.SheetMenu.ReturnValue += SheetMenu_ReturnValue;
        }

        private void SheetMenu_ReturnValue(object sender, string e)
        {
            RunSheetMenuCommand(e);
        }

        public void RunSheetMenuCommand(string CommandName)
        {
            switch (CommandName)
            {
                case "Call value":
                    
                    break;
                case "Invert":
                    
                    break;
                case "Free":
                    
                    break;
            }
        }

            /// <summary>
            /// Make event change this anchor on inner in Lenth constraint
            /// </summary>
            /// <param name="cadAnchor"></param>
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

        public override void TryRemove()
        {
            if (this.Constraints.Count < 2)
            {
                Removed?.Invoke(this, null);
            }
        }
    }
}
