using MeasureApp.ShapeObj.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public abstract class CadObject : Path, INotifyPropertyChanged
    {
        public List<CadConstraint> ConstrainList = new List<CadConstraint>();

        public event EventHandler<bool> SelectStatusChange;
        public event EventHandler<Point> DeltaTranslate;

        private bool _isselect = false;

        public bool IsSelect
        {
            get => this._isselect;
            set
            {
                this._isselect = value;
                //SelectStatusChange?.Invoke(this, this._isselect);
            }
        }

        private bool _isfix = false;

        public bool IsFix
        {
            get => this._isfix;
            set
            {
                this._isfix = value;
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
        }

        private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            this.X += e.TotalX;
            this.Y += e.TotalY;
            this.DeltaTranslate?.Invoke(this, new Point(e.TotalX, e.TotalY));
        }
    }
}
