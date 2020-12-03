using MeasureApp.ShapeObj.Canvas;
using MeasureApp.ShapeObj.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public abstract class CadObject : Path, INotifyPropertyChanged
    {
        public event EventHandler<bool> Selected;
        public event EventHandler<Point> DeltaTranslate;
        public event EventHandler<object> Droped;

        private bool _isselect = false;

        public bool IsSelect
        {
            get => this._isselect;
            set
            {
                this._isselect = value;
                this.Stroke = this._isselect == true ? Brush.Orange : Brush.Blue;
                Selected?.Invoke(this, this._isselect);
            }
        }

        private bool _isfix = false;

        public bool IsFix
        {
            get => this._isfix;
            set
            {
                this._isfix = value;
                this.Stroke = this._isfix == true ? Brush.Gray : Brush.Blue;
                OnPropertyChanged("IsFix");
            }
        }

        public virtual double X
        {
            get => this.TranslationX;
            set
            {
                if (this.IsFix == false)
                {
                    if (this.TranslationX != value)
                    {
                        this.TranslationX = value;
                        OnPropertyChanged("X");
                    }
                }
            }
        }

        public virtual double Y
        {
            get => this.TranslationY;
            set
            {
                if (this.IsFix == false)
                {
                    if (this.TranslationY != value)
                    {
                        this.TranslationY = value;
                        OnPropertyChanged("Y");
                    }
                }
            }
        }

        private PanGestureRecognizer panGesture = new PanGestureRecognizer();

        private TapGestureRecognizer tapGesture = new TapGestureRecognizer();

        private DropGestureRecognizer dropGesture = new DropGestureRecognizer();

        private DragGestureRecognizer dragGesture = new DragGestureRecognizer();

        public CadObject(bool Move)
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
        }

        private double regulaScale = 1;

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
            this.Droped?.Invoke(this, e.Data.Properties["Object"]);
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            this.IsSelect = !this.IsSelect;
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            this.X += e.TotalX;
            this.Y += e.TotalY;
            this.DeltaTranslate?.Invoke(this, new Point(e.TotalX, e.TotalY));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
