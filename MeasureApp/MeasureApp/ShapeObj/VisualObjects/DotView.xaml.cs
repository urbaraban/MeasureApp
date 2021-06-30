using DrawEngine.CadObjects;
using SureMeasure.ShapeObj.Interface;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.ShapeObj.VisualObjects
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DotView : ContentView, ITouchObject, IStatusObject, IMoveObject
    {
        public CadPoint point => (CadPoint)this.BindingContext;

        public double X
        {
            get => point.X;
            set
            {
                point.X = value;
                OnPropertyChanged("X");
            }
        }

        public double Y
        {
            get => point.Y;
            set
            {
                point.Y = value;
                OnPropertyChanged("Y");
            }
        }

        public bool IsSelect
        {
            get => this.point.IsSelect;
            set
            {
                this.point.IsSelect = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsSupport
        {
            get => this.point.IsSupport;
            set
            {
                this.point.IsSupport = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsFix
        {
            get => this.point.IsFix;
            set
            {
                this.point.IsFix = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        public bool IsBase
        {
            get => this.point.IsBase;
            set
            {
                this.point.IsBase = value;
                OnPropertyChanged("ObjectStatus");
            }
        }

        bool ITouchObject.ContainsPoint(Point InnerPoint) => 
            (InnerPoint.X > TranslationX  - (Width / 2 * this.Scale)
            && InnerPoint.X < TranslationX + (Width  * this.Scale)
            && InnerPoint.Y > TranslationY - (Height / 2 * this.Scale)
            && InnerPoint.Y < TranslationY + (Height * this.Scale));

        public ObjectStatus ObjectStatus
        {
            get
            {
                if (this.point != null)
                {
                    if (this.IsSelect == true) return ObjectStatus.Select;
                    else if (this.IsBase == true) return ObjectStatus.Base;
                    else if (this.IsFix == true) return ObjectStatus.Fix;
                    else if (this.IsSupport == true) return ObjectStatus.Support;
                }
                return ObjectStatus.Regular;
            }
        }

        public SheetMenu SheetMenu
        {
            get => _sheetMenu;
            set
            {
                this._sheetMenu = value;
            }
        }
        protected SheetMenu _sheetMenu;

        public DotView()
        {
            InitializeComponent();


            this.SheetMenu = new SheetMenu(new List<SheetMenuItem>()
            {
                new SheetMenuItem(Fix, "{FIX}"),
                new SheetMenuItem(Remove, "{REMOVE}"),
                new SheetMenuItem(LastPoint, "{LASTPOINT}"),
                new SheetMenuItem(BasePoint, "{BASEPOINT}"),
                new SheetMenuItem(Split, "{SPLIT}")
            });
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.BindingContext is CadPoint cadPoint)
            {
                cadPoint.PropertyChanged += CadPoint_PropertyChanged;
            }
        }

        private void CadPoint_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ObjectStatus");
        }

        #region Command
        private ICommand Fix => new Command(() =>
        {
            this.IsFix = !this.IsFix;
        });
        private ICommand Remove => new Command(() =>
        {
            point.TryRemove();
        });
        private ICommand LastPoint => new Command(() =>
        {
            this.IsSelect = true;
        });

        private ICommand BasePoint => new Command(() =>
        {
            this.IsBase = true;
        });

        private ICommand Split => new Command(() =>
        {
            point.MakeSplit();
        });
        #endregion


        public void TapAction()
        {
            this.IsSelect = !this.IsSelect;
        }

    }


}