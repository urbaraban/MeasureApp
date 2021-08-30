using DrawEngine.Constraints;
using Rg.Plugins.Popup.Extensions;
using SureMeasure.ShapeObj.Interface;
using SureMeasure.Views.OrderPage;
using SureMeasure.Views.Popup;
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
            get => this.Lenth.IsSelect;
            set
            {
                this.Lenth.IsSelect = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsFix
        {
            get => this.Lenth.IsFix;
            set
            {
                this.Lenth.IsFix = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsBase
        {
            get => this.Lenth.IsBase;
            set
            {
                this.Lenth.IsBase = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsSupport
        {
            get => this.Lenth.IsSupport;
            set
            {
                this.Lenth.IsSupport = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public ObjectStatus ObjectStatus
        {
            get
            {
                if (this.Lenth != null)
                {
                    if (this.IsSelect == true) return ObjectStatus.Select;
                    else if (this.IsBase == true) return ObjectStatus.Base;
                    else if (this.IsFix == true) return ObjectStatus.Fix;
                    else if (this.IsSupport == true) return ObjectStatus.Support;
                }
                return ObjectStatus.Regular;
            }
        }

        public LenthConstraint Lenth => (LenthConstraint)this.BindingContext;

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
                new SheetMenuItem(Split, "{SPLIT}"),
                new SheetMenuItem(Free, "{Free}")
            });
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.BindingContext is LenthConstraint constraintLenth)
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
                CenterX = TranslationX + this.Width * this.AnchorX,
                CenterY = TranslationY + this.Height * this.AnchorY,
                Angle = -this.Rotation
            };
            Point transformPoint = rotateTransform.Value.Transform(InnerPoint);
            return (transformPoint.X > TranslationX
            && transformPoint.X < TranslationX + Width * this.Scale
            && transformPoint.Y > TranslationY
            && transformPoint.Y < TranslationY + Height * this.Scale);
        }

        public void TapAction()
        {
            this.IsSelect = !this.IsSelect;
        }

        public override string ToString()
        {
            return $"LINE  {this.Lenth.ID}";
        }

        #region Command
        private ICommand CallValueDialog => new Command(async () =>
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(Lenth.Variable.Name, Lenth.Value.ToString());
            if (callresult != null)
            {
                this.Lenth.Value = double.Parse(callresult);
            }
        });

        private ICommand Free => new Command(async () =>
        {
            this.Lenth.IsFix = !this.Lenth.IsFix;
        });

        private ICommand GetMeasure => new Command(() =>
        {
            CadCanvasPage.MeasureVariable = this.Lenth.Variable;
            if (AppShell.BLEDevice != null)
            {
                AppShell.BLEDevice.IsOn = true;
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
                this.Lenth.Orientation = OrientationStat.Vertical;
            });
            ICommand Horizontal = new Command(() =>
            {
                this.Lenth.Orientation = OrientationStat.Horizontal;
            });
            ICommand Free_Orientation = new Command(() =>
            {
                this.Lenth.Orientation = OrientationStat.Free;
            });

            ICommand Fix_Orientation = new Command(() =>
            {
                this.Lenth.Orientation = OrientationStat.Fix;
            });

            ICommand Dynamic_Orientation = new Command(() =>
            {
                this.Lenth.Orientation = OrientationStat.Dynamic;
            });

            SheetMenu menu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>()
            {
                new SheetMenuItem(Verical, "{VERTICAL}"),
                new SheetMenuItem(Horizontal, "{HORIZONTAL}"),
                new SheetMenuItem(Free_Orientation, "{FREE_ORIENTATION}"),
                new SheetMenuItem(Fix_Orientation, "{FIX_ORIENTATION}"),
                new SheetMenuItem(Dynamic_Orientation, "{DYNAMIC_ORIENTATION}"),
            });
            menu.ShowMenu(this, Lenth.ToString());
        });


        private ICommand Fix => new Command(() =>
        {
            ICommand Position = new Command(() =>
            {
                this.Lenth.Anchor1.IsFix = !this.Lenth.Anchor1.IsFix;
                this.Lenth.Anchor2.IsFix = this.Lenth.Anchor1.IsFix;
            });
            ICommand Lenth = new Command(() =>
            {
                this.Lenth.IsFix = !this.Lenth.IsFix;
            });

            SheetMenu menu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>()
            {
                new SheetMenuItem(Position, "{POSITION}"),
                new SheetMenuItem(Lenth, "{LENTH}")
            });
            menu.ShowMenu(this, "{FIXEDTYPE}");
        });
        private ICommand Remove => new Command(() =>
        {
            this.Lenth.TryRemove();
        });

        private ICommand Split => new Command(async () =>
        {
            UpDownPopUp upDownPopUp = new UpDownPopUp(2);
            upDownPopUp.ReturnFormDialog += (sender, count) =>
            {
                if (count > 1) this.Lenth.MakeSplit(count);
            };
            await Navigation.PushPopupAsync(upDownPopUp);
        });
        #endregion
        double IMoveObject.X 
        { 
            get => this.Lenth.Anchor1.Point.X;
            set
            {
                this.Lenth.Anchor1.Point.X = value;
            }
        }
        double IMoveObject.Y 
        {
            get => this.Lenth.Anchor1.Point.Y;
            set
            {
                this.Lenth.Anchor1.Point.Y = value;
            }
        }

    }

}