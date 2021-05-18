using SureMeasure.CadObjects.Interface;
using SureMeasure.ShapeObj;
using SureMeasure.ShapeObj.Constraints;
using SureMeasure.Tools;
using System;
using System.Numerics;

namespace SureMeasure.CadObjects.Constraints
{
    public class ConstraintAngle : CadConstraint, CadObject
    {
        public ConstraintLenth anchorAnchor1;
        public ConstraintLenth anchorAnchor2;

        public event EventHandler<bool> Removed;
        public event EventHandler<bool> LastObject;

        public CadPoint Point1;
        public CadPoint Point2;
        public CadPoint Point3;

        public override double Value
        {
            get => this.Variable.Value > -1 ? this.Variable.Value : Sizing.AngleThreePoint(this.Point1, this.Point2, this.Point3);
            set
            {
                this.Variable.Value = value;
                OnPropertyChanged("Angle");
            }
        }
        public double RadAngle => this.Value * (Math.PI / 180);

        /// <summary>
        /// Constrait angle
        /// </summary>
        /// <param name="CadAnchor1">Start anchor</param>
        /// <param name="CadAnchor2">Middle anchor</param>
        /// <param name="CadAnchor3">End anchor</param>
        /// <param name="Angle">Angle in degrees</param>
        public ConstraintAngle(ConstraintLenth AnchorAnchor1, ConstraintLenth AnchorAnchor2, double Angle)
        {
            this.anchorAnchor1 = AnchorAnchor1;
            this.anchorAnchor2 = AnchorAnchor2;

            if (anchorAnchor1.Point1 != anchorAnchor2.Point1 && anchorAnchor1.Point1 != anchorAnchor2.Point2)
            {
                this.Point1 = anchorAnchor1.Point1;
                this.Point2 = anchorAnchor1.Point2;
                this.Point3 = anchorAnchor2.GetNotThisPoint(this.Point2);
            }
            else
            {
                this.Point1 = anchorAnchor1.Point2;
                this.Point2 = anchorAnchor1.Point1;
                this.Point3 = anchorAnchor2.GetNotThisPoint(this.Point2);
            }

            this.Variable = new CadVariable(Angle, false);
            this.Variable.PropertyChanged += Angle_PropertyChanged;

            this.Point1.PropertyChanged += CadanchorStart_PropertyChanged;
            this.Point3.PropertyChanged += CadanchorEnd_PropertyChanged;

            this.Point1.Removed += Point1_Removed;
            this.Point2.Removed += Point1_Removed;
            this.Point3.Removed += Point1_Removed;
        }

        private void Point1_Removed(object sender, bool e)
        {
            this.Removed?.Invoke(this, true);
        }

        public void Remove()
        {
            this.Variable.PropertyChanged -= Angle_PropertyChanged;

            this.Point1.PropertyChanged -= CadanchorStart_PropertyChanged;
            this.Point3.PropertyChanged -= CadanchorEnd_PropertyChanged;
        }

        private void Angle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
            {
                MakeMagic(this.Point1, this.Point2, this.Point3, this.Variable.Value);
            }
        }

        private void CadanchorStart_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
                {
                    MakeMagic(this.Point1, this.Point2, this.Point3, this.Variable.Value);
                }
            }
        }

        private void CadanchorEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
                {
                    MakeMagic(this.Point3, this.Point2, this.Point1, 360 - this.Variable.Value);
                }
            }       
        }

        private void MakeMagic(CadPoint FirstPoint, CadPoint MiddlePoint, CadPoint LastPoint, double angle)
        {
            if (CadConstraint.RuntimeConstraits.Contains(this) == false)
            {
                if (LastPoint.IsFix == false)
                {
                    Console.WriteLine($"Angle {this.ID}");
                    CadConstraint.RuntimeConstraits.Add(this);
                    Vector2 vector1 = Vector2.Normalize(new Vector2((float)(FirstPoint.X - MiddlePoint.X), (float)(FirstPoint.Y - MiddlePoint.Y)));
                    Vector2 vector2 = RotateRadians(vector1, angle);
                    if (anchorAnchor1.GetNotThisPoint(LastPoint) is CadPoint cadPoint1)
                    {
                        CadConstraint.RuntimeConstraits.Remove(anchorAnchor1);
                        anchorAnchor1.PreMagic(MiddlePoint, LastPoint, vector2);
                    }
                    else if (anchorAnchor2.GetNotThisPoint(LastPoint) is CadPoint cadPoint2)
                    {
                        CadConstraint.RuntimeConstraits.Remove(anchorAnchor2);                        
                        anchorAnchor2.PreMagic(MiddlePoint, LastPoint, vector2);
                    }
                    CadConstraint.RuntimeConstraits.Remove(this);
                }
                else
                {
                    this.MakeMagic(LastPoint, MiddlePoint, FirstPoint, 360 - angle);
                }
            }

            OnPropertyChanged("Point");
        }

        private Vector2 RotateRadians(Vector2 vector1, double angle)
        {
            double radians = angle * Math.PI / 180;

            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector2((float)(ca * vector1.X - sa * vector1.Y), (float)(sa * vector1.X + ca * vector1.Y));
        }

        /*private Vector2 RotateRadians(this Vector2 v, double Degrees)
        {
            
        }*/

        public void TryRemove()
        {
            this.Point1.Removed -= Point1_Removed;
            this.Point2.Removed -= Point1_Removed;
            this.Point3.Removed -= Point1_Removed;
            Point1.PropertyChanged -= CadanchorStart_PropertyChanged;
            Point3.PropertyChanged -= CadanchorEnd_PropertyChanged;
            Removed?.Invoke(this, true);
        }
    }
}
