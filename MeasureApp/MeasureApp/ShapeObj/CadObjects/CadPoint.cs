using SureMeasure.CadObjects.Interface;
using SureMeasure.ShapeObj;
using SureMeasure.ShapeObj.Canvas;
using SureMeasure.ShapeObj.Constraints;
using SureMeasure.ShapeObj.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace SureMeasure.CadObjects
{
    public class CadPoint : INotifyPropertyChanged, CadObject
    {
        public event EventHandler<bool> Selected;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<bool> Fixed;
        public event EventHandler<bool> Supported;
        public event EventHandler<bool> Removed;

        public event EventHandler<CadPoint> ChangedPoint;

        public event EventHandler<bool> BaseObject;

        public bool IsSelect
        {
            get => this._isselect;
            set
            {
                this._isselect = value;
                Selected?.Invoke(this, this._isselect);
                OnPropertyChanged("IsSelect");
            }
        }
        private bool _isselect = false;

        private double _x;
        public double X
        {
            get => this._x;
            set
            {
                if (this._x != (value) && this._isfix == false)
                {
                    this._x = value;
                    OnPropertyChanged("Point");
                }
            }
        }

        /// <summary>
        /// Cordinate Y with zero Offcet
        /// </summary>
        public double OX 
        {
            get => _x + CadCanvas.ZeroPoint.X;
            set 
            {
                if (this._isfix == false) 
                {
                    _x = value - CadCanvas.ZeroPoint.X;
                    OnPropertyChanged("Point");
                }
            }
        }
        
        private double _y;

        public double Y
        {
            get => this._y;
            set
            {
                if (this._y != value && this._isfix == false)
                {
                    this._y = value;
                    OnPropertyChanged("Point");
                }

            }
        }

        /// <summary>
        /// Cordinate Y with zero Offcet
        /// </summary>
        public double OY
        {
            get => _y + CadCanvas.ZeroPoint.Y;
            set
            {
                if (this._y != value && this._isfix == false)
                {
                    _y = value - CadCanvas.ZeroPoint.Y;
                    OnPropertyChanged("Point");
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Point Point
        {
            get => new Point(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public bool IsFix
        {
            get => _isfix;
            set
            {
                this._isfix = value;
                Fixed?.Invoke(this, this._isfix);
            }
        }
        private bool _isfix = false;
        public bool IsSupport 
        { 
            get => this._issupprot;
            set
            {
                this._issupprot = value;
                Supported?.Invoke(this, this._issupprot);
                OnPropertyChanged("IsSelect");
            }
        }
        private bool _issupprot = false;

        public bool IsBase 
        { 
            get => this._isbase;
            set
            {
                this._isbase = value;
                BaseObject?.Invoke(this, value);
                OnPropertyChanged("IsBase");
            }
        }
        private bool _isbase = false;

        public string ID
        {
            get => this._id;
            set
            {
                this._id = value;
                OnPropertyChanged("ID");
            }
        }
        private string _id;

        //public List<CadConstraint> Constraints = new List<CadConstraint>();

        public CadPoint()
        {

        }

        public CadPoint(double X, double Y, string ID, bool Adapt = false)
        {
            this._x = X - (Adapt == true ? CadCanvas.ZeroPoint.X : 0);
            this._y = Y - (Adapt == true ? CadCanvas.ZeroPoint.Y : 0);
            this._id = ID;
        }

        public void ChangePoint(CadPoint cadPoint)
        {
            ChangedPoint?.Invoke(this, cadPoint);
            TryRemove();
        }
        public void Update(CadPoint cadPoint, bool Constraint = false)
        {
            if (this.IsFix == false)
            {
                this._x = cadPoint.X;
                this._y = cadPoint.Y;
            }
            OnPropertyChanged("Point");
        }

        public void Update(Point Point, bool Constraint = false)
        {
            if (this.IsFix == false)
            {
                this._x = Point.X;
                this._y = Point.Y;
            }
            OnPropertyChanged("Point");
        }

        public void Add(Point cadPoint)
        {
            this._x += cadPoint.X;
            this._y += cadPoint.Y;
        }

        public void Update(double X, double Y, bool Constraint = false)
        {
            this._x = X;
            this._y = Y;
            OnPropertyChanged("Point");
        }

        public override string ToString()
        {
            return $"{this.ID}:{X.ToString()}:{Y.ToString()}";
        }

        public void TryRemove()
        {
            Removed?.Invoke(this, true);
        }
    }
}
