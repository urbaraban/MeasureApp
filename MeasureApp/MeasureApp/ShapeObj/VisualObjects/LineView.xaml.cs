using DrawEngine.Constraints;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.Views.OrderPage;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LineView : ContentView, IStatusObject, IMoveObject, ITouchObject
    {
        public bool IsSelect
        {
            get => this._lenthConstrait.IsSelect;
            set
            {
                this._lenthConstrait.IsSelect = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsFix
        {
            get => this._lenthConstrait.IsFix;
            set
            {
                this._lenthConstrait.IsFix = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsBase
        {
            get => this._lenthConstrait.IsBase;
            set
            {
                this._lenthConstrait.IsBase = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsSupport
        {
            get => this._lenthConstrait.IsSupport;
            set
            {
                this._lenthConstrait.IsSupport = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public ObjectStatus ObjectStatus
        {
            get
            {
                if (this._lenthConstrait != null)
                {
                    if (this.IsSelect == true) return ObjectStatus.Select;
                    else if (this.IsBase == true) return ObjectStatus.Base;
                    else if (this.IsFix == true) return ObjectStatus.Fix;
                    else if (this.IsSupport == true) return ObjectStatus.Support;
                }
                return ObjectStatus.Regular;
            }
        }

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
                new SheetMenuItem(Orientation, "{ORIENTATIONAL}"),
                new SheetMenuItem(Fix, "{FIX}"),
                new SheetMenuItem(Remove, "{REMOVE}"),
                new SheetMenuItem(Split, "{SPLIT}")
            });
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.BindingContext is ConstraintLenth constraintLenth)
            {
                constraintLenth.PropertyChanged += ConstraintLenth_PropertyChanged; ;
            }
        }

        private void ConstraintLenth_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ObjectStatus");
        }

        bool ITouchObject.ContainsPoint(Point InnerPoint)
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
            && transformPoint.Y > TranslationY - Height / 4
            && transformPoint.Y < TranslationY + Height);
        }

        public void TapAction()
        {
            this.IsSelect = !this.IsSelect;
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
            this.IsSupport = !this.IsSupport;
        });

        private ICommand Orientation => new Command(() =>
        {
            ICommand Verical = new Command(() =>
            {
                this._lenthConstrait.Orientation = Orientaton.Vertical;
            });
            ICommand Horizontal = new Command(() =>
            {
                this._lenthConstrait.Orientation = Orientaton.Horizontal;
            });
            ICommand Free_Orientation = new Command(() =>
            {
                this._lenthConstrait.Orientation = Orientaton.Free;
            });

            ICommand Fix_Orientation = new Command(() =>
            {
                this._lenthConstrait.Orientation = Orientaton.Fix;
            });

            SheetMenu menu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>()
            {
                new SheetMenuItem(Verical, "{VERTICAL}"),
                new SheetMenuItem(Horizontal, "{HORIZONTAL}"),
                new SheetMenuItem(Free_Orientation, "{FREE_ORIENTATION}"),
                new SheetMenuItem(Fix_Orientation, "{FIX_ORIENTATION}"),
            });
            menu.ShowMenu(this);
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
        #endregion
        double IMoveObject.X 
        { 
            get => this._lenthConstrait.Point1.X;
            set
            {
                this._lenthConstrait.Point1.X = value;
            }
        }
        double IMoveObject.Y 
        {
            get => this._lenthConstrait.Point1.Y;
            set
            {
                this._lenthConstrait.Point1.Y = value;
            }
        }

    }

}