using SureMeasure.ShapeObj.Canvas;
using SureMeasure.ShapeObj.Interface;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace SureMeasure.ShapeObj
{
    public abstract class VisualObject : Path, INotifyPropertyChanged, CanvasObject, ActiveObject
    {
        public virtual event EventHandler<bool> Selected;
        public virtual event EventHandler<object> Dropped;
        public virtual event EventHandler<bool> Removed;

        public virtual string ID { get; set; }

        public virtual bool IsSelect
        {
            get => this._isselect;
            set
            {
                this._isselect = value;
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { 
                this.Stroke = this._isselect == true ? Brush.Orange : Brush.Blue;
                });
                Selected?.Invoke(this, this._isselect);
            }
        }
        private bool _isselect = false;

        public virtual double X
        {
            get => this.TranslationX;
            set
            {
                if (this.TranslationX != value)
                {
                    this.TranslationX = value;
                    //OnPropertyChanged("X");
                }
            }
        }

        public virtual double Y
        {
            get => this.TranslationY;
            set
            {
                if (this.TranslationY != value)
                {
                    this.TranslationY = value;
                    //OnPropertyChanged("Y");
                }
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
        private SheetMenu _sheetMenu;

        #region Gestured
        private PanGestureRecognizer panGesture = new PanGestureRecognizer();
        private TapGestureRecognizer tapGesture = new TapGestureRecognizer();
        private DropGestureRecognizer dropGesture = new DropGestureRecognizer();
        private DragGestureRecognizer dragGesture = new DragGestureRecognizer();
        #endregion

        private int taps = 0;
        private bool runtimer = false;

        public VisualObject(bool Move)
        {
            this.HorizontalOptions = LayoutOptions.StartAndExpand;
            this.VerticalOptions = LayoutOptions.StartAndExpand;
            this.Stroke = Brush.Blue;
            this.StrokeThickness = 5;
            this.Fill = Brush.White;
            if (Move == true)
            {
                this.panGesture.PanUpdated += PanGesture_PanUpdated;
                this.GestureRecognizers.Add(this.panGesture);
            }
            this.tapGesture.Tapped += TapGesture_Tapped;
            this.GestureRecognizers.Add(this.tapGesture);

            this.dragGesture.CanDrag = true;
            this.dragGesture.DragStarting += DragGesture_DragStarting;
            this.dragGesture.DropCompleted += DragGesture_DropCompleted;
            this.GestureRecognizers.Add(this.dragGesture);

            this.dropGesture.AllowDrop = true;
            this.dropGesture.Drop += DropGesture_Drop;
            this.GestureRecognizers.Add(this.dropGesture);

            CadCanvas.SellectAll += CadCanvas_SellectAll;
        }

        private void CadCanvas_SellectAll(object sender, bool e)
        {
            this.IsSelect = e;
        }

        private void DragGesture_DropCompleted(object sender, DropCompletedEventArgs e)
        {
            CadCanvas.CallRegularSize();
        }


        private void DragGesture_DragStarting(object sender, DragStartingEventArgs e)
        {
            e.Data.Properties.Add("Object", this);
            CadCanvas.CallDragSize();
        }

        private void DropGesture_Drop(object sender, DropEventArgs e)
        {
            this.Dropped?.Invoke(this, e.Data.Properties["Object"]);
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            TapManager();
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            this.X += e.TotalX;
            this.Y += e.TotalY;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public virtual void Update()
        {
            
        }

        public virtual void TryRemove()
        {
            Removed?.Invoke(this, true);
        }

        private void TapManager()
        {
            taps += 1;
            if (this.runtimer == false)
            {
                this.runtimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
                {
                    if (taps < 2)
                    {
                        this.IsSelect = !this.IsSelect;
                    }
                    else
                    {
                        this.SheetMenu.ShowMenu(this);
                    }

                    taps = 0;
                    return false; // return true to repeat counting, false to stop timer
                });
                this.runtimer = false;
            }
        }
    }
}
