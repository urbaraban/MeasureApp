using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.Views.Canvas;
using SureMeasure.Views.OrderPage;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LineView : ContentView, IActiveObject
    {
        private ConstraintLenth _lenthConstrait => (ConstraintLenth)this.BindingContext;

        public virtual SheetMenu SheetMenu { get => this._sheetMenu; set => this._sheetMenu = value; }
        private SheetMenu _sheetMenu;

        public LineView()
        {
            InitializeComponent();

            this.SheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>()
            {
                new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"),
                new SheetMenuItem(GetMeasure, "{MEASURE}"),
                new SheetMenuItem(SupportLine, "{SUPPORT_LINE}"),
                new SheetMenuItem(Verical, "{VERTICAL}"),
                new SheetMenuItem(Horizontal, "{HORIZONTAL}"),
                new SheetMenuItem(Free_Orientation, "{FREE_ORIENTATION}"),
                new SheetMenuItem(Fix, "{FIX}"),
                new SheetMenuItem(Remove, "{REMOVE}"),
                new SheetMenuItem(Split, "{SPLIT}")
            });
        }


        bool IActiveObject.ContainsPoint(Point InnerPoint)
        {
            RotateTransform rotateTransform = new RotateTransform
            {
                CenterX = TranslationX,
                CenterY = TranslationY,
                Angle = -this.Rotation
            };
            Point transformPoint = rotateTransform.Value.Transform(InnerPoint);
            return (transformPoint.X > TranslationX
            && transformPoint.X < TranslationX + Width
            && transformPoint.Y > TranslationY
            && transformPoint.Y < TranslationY + Height);
        }

        public void TapAction()
        {
            this._lenthConstrait.IsSelect = !this._lenthConstrait.IsSelect;
        }

        #region Command
        private ICommand CallValueDialog => new Command(async () =>
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(_lenthConstrait.Variable.Name, _lenthConstrait.Value.ToString());
            if (callresult != null)
            {
                this._lenthConstrait.Value = double.Parse(callresult);
            }
        });
        private ICommand GetMeasure => new Command(() =>
        {
            CadCanvasPage.MeasureVariable = this._lenthConstrait.Variable;
            if (AppShell.BLEDevice != null)
            {
                AppShell.BLEDevice.OnDevice();
            }
        });
        private ICommand SupportLine => new Command(() =>
        {
            this._lenthConstrait.IsSupport = !this._lenthConstrait.IsSupport;
        });
        private ICommand Verical => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.Vertical;
        });
        private ICommand Horizontal => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.Horizontal;
        });
        private ICommand Free_Orientation => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.OFF;
        });
        private ICommand Fix => new Command(() =>
        {
            this._lenthConstrait.Fix(true);
        });
        private ICommand Remove => new Command(() =>
        {
            this._lenthConstrait.TryRemove();
        });

        private ICommand Split => new Command(() =>
        {
            this._lenthConstrait.MakeSplit();
        });

        double IActiveObject.X 
        { 
            get => this._lenthConstrait.Point1.X;
            set
            {
                this._lenthConstrait.Point1.X = value;
            }
        }
        double IActiveObject.Y 
        {
            get => this._lenthConstrait.Point1.Y;
            set
            {
                this._lenthConstrait.Point1.Y = value;
            }
        }
        #endregion
    }

}