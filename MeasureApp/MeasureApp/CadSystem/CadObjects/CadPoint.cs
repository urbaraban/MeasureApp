using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SureCadSystem.CadObjects
{
    public class CadPoint : INotifyPropertyChanged, ICadObject
    {
        public static CadPoint ZeroPoint = new CadPoint(5000, 5000);

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
                if (Math.Abs(value - this._x) > 0.001 && this._isfix == false)
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
            get => _x + CadPoint.ZeroPoint.X;
            set 
            {
                if (this._isfix == false) 
                {
                    _x = value - CadPoint.ZeroPoint.X;
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
                if (Math.Abs(value - this._x) > 0.1 && this._isfix == false)
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
            get => _y + CadPoint.ZeroPoint.Y;
            set
            {
                if (this._y != value && this._isfix == false)
                {
                    _y = value - CadPoint.ZeroPoint.Y;
                    OnPropertyChanged("Point");
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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

        public CadPoint(double X, double Y, bool Adapt = false)
        {
            this._x = X - (Adapt == true ? CadPoint.ZeroPoint.X : 0);
            this._y = Y - (Adapt == true ? CadPoint.ZeroPoint.Y : 0);
        }

        public CadPoint(double X, double Y, string ID, bool Adapt = false)
        {
            this._x = X - (Adapt == true ? CadPoint.ZeroPoint.X : 0);
            this._y = Y - (Adapt == true ? CadPoint.ZeroPoint.Y : 0);
            this._id = ID;
        }

        public void ChangePoint(CadPoint cadPoint)
        {
            ChangedPoint?.Invoke(this, cadPoint);
            TryRemove();
        }
        public void Update(CadPoint cadPoint) => update(cadPoint.X, cadPoint.Y);

        public bool Update(double X, double Y) => update(X, Y);


        private bool update(double X, double Y)
        {
            if (this.IsFix == false)
            {
                if (Math.Abs(X / this._x - 1) > 0.01 || Math.Abs(Y / this._y - 1) > 0.01)
                {
                    this._x = X;
                    this._y = Y;
                    //Debug.WriteLine($"Update {this.ID}");
                    OnPropertyChanged("Point");
                    return true;
                }
            }
            return false;
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
