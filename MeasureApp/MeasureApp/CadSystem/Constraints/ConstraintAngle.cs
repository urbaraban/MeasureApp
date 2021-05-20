using SureCadSystem.CadObjects;
using SureCadSystem.Tools;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SureCadSystem.Constraints
{
    public class ConstraintAngle : CadConstraint, ICadObject
    {
        public ConstraintLenth anchorAnchor1;
        public ConstraintLenth anchorAnchor2;

        public event EventHandler<bool> Fixed;
        public event EventHandler<bool> Removed;
        /// <summary>
        /// Defines the Selected.
        /// </summary>
        public event EventHandler<bool> Selected;
        /// <summary>
        /// Defines the Supported.
        /// </summary>
        public event EventHandler<bool> Supported;

        public CadPoint Point1;
        public CadPoint Point2;
        public CadPoint Point3;

        public string ID => Point1.ID + Point2.ID + Point3.ID;

        public double Value
        {
            get => this.Variable.Value > -1 ? this.Variable.Value : Sizing.AngleThreePoint(this.Point1, this.Point2, this.Point3);
            set
            {
                this.Variable.Value = value;
                OnPropertyChanged("Angle");
            }
        }

        public CadVariable Variable
        {
            get => this._variable;
            set
            {
                this._variable = value;
                OnPropertyChanged("Variable");
            }
        }
        private CadVariable _variable;

        public bool IsSelect
        {
            get => this._isselect;
            set
            {
                this._isselect = value;
                Selected?.Invoke(this, this._isselect);
            }
        }
        private bool _isselect = false;

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

        public virtual bool IsSupport
        {
            get => this._issupprot;
            set
            {
                this._issupprot = value;
                Supported?.Invoke(this, this._issupprot);
            }
        }
        private bool _issupprot = false;

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

            this.anchorAnchor1.PropertyChanged += AnchorAnchor1_PropertyChanged;
            this.anchorAnchor2.PropertyChanged += AnchorAnchor1_PropertyChanged;

            this.Point1.Removed += Point1_Removed;
            this.Point2.Removed += Point1_Removed;
            this.Point3.Removed += Point1_Removed;
        }

        private void AnchorAnchor1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MakeMagic(this.Point1, this.Point2, this.Point3, this.Variable.Value);
            CadConstraint.RemoveConstraint();
        }

        private void Point1_Removed(object sender, bool e)
        {
            this.Removed?.Invoke(this, true);
        }


        private void Angle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
            {
                MakeMagic(this.Point1, this.Point2, this.Point3, this.Variable.Value);
            }
            CadConstraint.RemoveConstraint();
        }

        private void MakeMagic(CadPoint FirstPoint, CadPoint MiddlePoint, CadPoint LastPoint, double angle)
        {
            if (CadConstraint.RuntimeConstraits.Contains(this) == false)
            {
                if (LastPoint.IsFix == false)
                {
                    Console.WriteLine($"Angle {this.ID} EndPoint {LastPoint}");
                    CadConstraint.RuntimeConstraits.Add(this);
                    Vector2 vector1 = Vector2.Normalize(new Vector2((float)(FirstPoint.X - MiddlePoint.X), (float)(FirstPoint.Y - MiddlePoint.Y)));
                    Vector2 vector2 = RotateRadians(vector1, angle);

                    if (GetLenth(LastPoint) is ConstraintLenth lenth)
                    {
                        CadConstraint.RuntimeConstraits.Remove(lenth);
                        lenth.PreMagic(MiddlePoint, LastPoint, vector2);
                    }

                    CadConstraint.RuntimeConstraits.Remove(this);
                }
                else
                {
                    this.MakeMagic(LastPoint, MiddlePoint, FirstPoint, 360 - angle);
                }
            }

            OnPropertyChanged("Point");

            ConstraintLenth GetLenth(CadPoint cadPoint)
            {
                if (anchorAnchor1.GetNotThisPoint(LastPoint) is CadPoint cadPoint1)
                {
                    return anchorAnchor1;
                }
                else if (anchorAnchor2.GetNotThisPoint(LastPoint) is CadPoint cadPoint2)
                {
                    return anchorAnchor2;
                }
                return null;
            }
        }

        private Vector2 RotateRadians(Vector2 vector1, double angle)
        {
            double radians = angle * Math.PI / 180;

            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector2((float)(ca * vector1.X - sa * vector1.Y), (float)(sa * vector1.X + ca * vector1.Y));
        }

        private double AngleBetweenVectors(Vector2 v1, Vector2 v2)
        {
            return Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
        }

        /*private Vector2 RotateRadians(this Vector2 v, double Degrees)
        {
            
        }*/

        public void TryRemove()
        {
            this.Point1.Removed -= Point1_Removed;
            this.Point2.Removed -= Point1_Removed;
            this.Point3.Removed -= Point1_Removed;
            this.anchorAnchor1.PropertyChanged -= AnchorAnchor1_PropertyChanged;
            this.anchorAnchor2.PropertyChanged -= AnchorAnchor1_PropertyChanged;
            Removed?.Invoke(this, true);
        }

        public bool ContaintsPoint(CadPoint Point) => this.Point1 == Point || this.Point2 == Point || this.Point3 == Point;

        public bool IsBase => this.Point1.IsFix == true || this.Point2.IsFix == true || this.Point3.IsFix == true;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
