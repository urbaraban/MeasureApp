using MeasureApp.CadObjects;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace MeasureApp.Orders
{
    public class ContourPath : INotifyPropertyChanged, OrderObjectInt
    {
        public bool IsSubstract { get; set; } = false;

        /// <summary>
        /// Contour sqare
        /// </summary>
        public double Area
        {
            get 
            {
                double area = 0;
                if (IsClosed == true)
                {
                    for (int i = 1; i <= this.Points.Length; i += 1)
                    {
                        area += (Points[i - 1].X + Points[i % (Points.Length - 1)].X) * (Points[i % (Points.Length - 1)].Y + Points[i - 1].Y);
                    }
                }
                return area / 2;
            }
        }

        public double Perimeter
        {
            get
            {
                double lenth = 0;
                if (Points.Length > 1)
                {
                    for (int i = 1; i < Points.Length; i += 1)
                    {
                        lenth += Sizing.PtPLenth(Points[i - 1], Points[i]);
                    }
                    if (this.IsClosed == true)
                    {
                        lenth += Sizing.PtPLenth(Points[0], Points[Points.Length - 1]);
                    }
                }
                return lenth;
            }
        }

        /// <summary>
        /// Gabarit Sqare
        /// </summary>
        public double GabaritSqare
        {
            get => 0;
        }

        public bool IsClosed
        {
            get => this._isclosed;
            set
            {
                this._isclosed = value;
                OnPropertyChanged("IsClosed");
            }
        }
        private bool _isclosed;

        public List<ConstraintLenth> Lenths = new List<ConstraintLenth>();

        public CadPoint[] Points
        {
            get
            {
                if (this.Lenths.Count > 0)
                {
                    CadPoint[] points = new CadPoint[this.Lenths.Count];
                    
                    for (int i = 0; i < Lenths.Count; i += 1)
                    {
                        points[i] = this.Lenths[0].Point1;
                    }
                    return points;
                }
                return null;
            }
        }

        public void SortLenth()
        {
            for (int i = 0; i < Lenths.Count; i += 1)
            {

            }
        }

        public string Color = "#0000FF";

        public string ID { get; set; }

        public ContourPath(string ID)
        {
            this.ID = ID;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString() => this.ID;

        internal bool CheckInsertLenth(ConstraintLenth constraintLenth)
        {
            foreach (ConstraintLenth constraint in this.Lenths)
            {
                if (constraintLenth.Point1.ID == constraint.Point1.ID ||
                    constraintLenth.Point2.ID == constraint.Point1.ID ||
                    constraintLenth.Point1.ID == constraint.Point2.ID ||
                    constraintLenth.Point2.ID == constraint.Point2.ID)
                    return true;
            }
            return false;
        }
    }
}
