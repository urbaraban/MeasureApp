using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using DrawEngine.Tools;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.Views.OrderPage;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AngleView : ContentView, ITouchObject, IMoveObject
    {
        public double WidthBlock
        {
            get => widthblock;
            set
            {
                widthblock = value;
                OnPropertyChanged("WidthBlock");
            }
        }
        private double widthblock = 80;

        private AngleConstraint constraintAngle => (AngleConstraint)this.BindingContext;

        private List<SheetMenuItem> commands = new List<SheetMenuItem>();

        public virtual SheetMenu SheetMenu { get => this._sheetMenu; set => this._sheetMenu = value; }
        private SheetMenu _sheetMenu;

        public AngleView()
        {
            InitializeComponent();

            this.commands.Add(new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"));
            this.commands.Add(new SheetMenuItem(SendMeasure, "{MEASURE}"));
            this.commands.Add(new SheetMenuItem(InvertAngle, "{INVERT_ANGLE}"));
            this.commands.Add(new SheetMenuItem(FreeAngle, "{FREE_ANGLE}"));
            this.commands.Add(new SheetMenuItem(Remove, "{REMOVE}"));

            this.SheetMenu = new SheetMenu(this.commands);
        }


        bool ITouchObject.ContainsPoint(Point InnerPoint)
        {
            RotateTransform rotateTransform = new RotateTransform
            {
                CenterX = TranslationX + this.Width * this.AnchorX,
                CenterY = TranslationY + this.Height * this.AnchorY,
                Angle = -this.Rotation
            };
            Point transformPoint = rotateTransform.Value.Transform(InnerPoint);
            return (
                transformPoint.X > TranslationX + (WidthBlock - ToychLabel.Height * 3)  * this.Scale
                && transformPoint.X < TranslationX + WidthBlock * this.Scale
                && transformPoint.Y > TranslationY
                && transformPoint.Y < TranslationY + ToychLabel.Width * this.Scale);
        }

        public void TapAction() { }

        private ICommand CallValueDialog => new Command(async () =>
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(constraintAngle.Variable.Name, Math.Round(constraintAngle.Value * 180 / Math.PI, 2).ToString());
            if (callresult != null)
            {
                this.constraintAngle.Variable.Value = double.Parse(callresult) * Math.PI / 180;
            }
        });
        private ICommand SendMeasure => new Command(() =>
        {
            CadCanvasPage.MeasureVariable = this.constraintAngle.Variable;
            AppShell.BLEDevice.OnDevice();
        });
        private ICommand InvertAngle => new Command(() =>
        {
            this.constraintAngle.Value = Math.Abs((this.constraintAngle.Value - Math.PI * 2) % (Math.PI * 2));
        });
        private ICommand FreeAngle => new Command(() =>
        {
            this.constraintAngle.IsFix = !this.constraintAngle.IsFix;
        });
        private ICommand Remove => new Command(() =>
        {
            this.constraintAngle.TryRemove();
        });

        double IMoveObject.X
        {
            get => LineTools.GetPointOnVector(constraintAngle.Intersection, constraintAngle.Mediana, WidthBlock).X;
            set 
            {
                CadPoint cadPoint = LineTools.GetPointOnVector(constraintAngle.Intersection, constraintAngle.Mediana, WidthBlock);
                MoveWidthBlock(value, cadPoint.Y);
            }
        }
        double IMoveObject.Y
        {
            get => LineTools.GetPointOnVector(constraintAngle.Intersection, constraintAngle.Mediana, WidthBlock).Y;
            set
            {
                CadPoint cadPoint = LineTools.GetPointOnVector(constraintAngle.Intersection, constraintAngle.Mediana, WidthBlock);
                MoveWidthBlock(cadPoint.X, value);
            }
        }

        private void MoveWidthBlock(double X, double Y)
        {
            CadPoint PointOnVector = LineTools.GetPointOnVector(constraintAngle.Intersection, constraintAngle.Mediana, WidthBlock);
            CadPoint PPoint = LineTools.GetPerpendecularOnLine(constraintAngle.Intersection, PointOnVector, new CadPoint(X, Y));
            double lenth = PointTools.Lenth(constraintAngle.Intersection, PPoint);
            WidthBlock = lenth < 60 ? 60 : lenth;
        }
    }
}