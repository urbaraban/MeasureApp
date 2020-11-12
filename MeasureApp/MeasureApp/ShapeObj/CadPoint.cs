using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj
{
    public class CadPoint : INotifyPropertyChanged
    {
        private double _x;
        public double X
        {
            get => this._x;
            set
            {
                if (this._x != value)
                {
                    this._x = value;
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
                if (this._y != value)
                {
                    this._y = value;
                    OnPropertyChanged("Point");
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public CadPoint()
        {

        }

        public CadPoint(double X, double Y)
        {
            this._x = X;
            this._y = Y;
        }

        public void Update(CadPoint cadPoint, bool Constraint = false)
        {
            this._x = cadPoint.X;
            this._y = cadPoint.Y;
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
    }
}
