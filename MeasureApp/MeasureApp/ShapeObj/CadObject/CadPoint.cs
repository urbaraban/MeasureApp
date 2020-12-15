using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj
{
    public class CadPoint : INotifyPropertyChanged, CadObject
    {
        public event EventHandler<bool> Selected;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<bool> Fixed;
        public event EventHandler<bool> Supported;
        public event EventHandler Removed;

        public event EventHandler<CadPoint> ChangedPoint;


        public  bool IsSelect
        {
            get => this._isselect;
            set
            {
                this._isselect = value;
                Selected?.Invoke(this, this._isselect);
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
            }
        }
        private bool _issupprot = false;

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

        public List<CadConstraint> Constraints = new List<CadConstraint>();

        public CadPoint()
        {

        }

        public CadPoint(double X, double Y, string ID)
        {
            this._x = X;
            this._y = Y;
            this._id = ID;
        }

        public void ChangePoint(CadPoint cadPoint)
        {
            ChangedPoint?.Invoke(this, cadPoint);
        }
        public void Update(CadPoint cadPoint, bool Constraint = false)
        {
            this._x = cadPoint.X;
            this._y = cadPoint.Y;
            OnPropertyChanged("Point");
        }

        public void Update(Point Point, bool Constraint = false)
        {
            this._x = Point.X;
            this._y = Point.Y;
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
            return $"{X.ToString()}:{Y.ToString()}";
        }

        public void TryRemove()
        {
            if (Constraints.Count < 2)
            {
                Removed?.Invoke(this, null);
            }
        }
    }
}
