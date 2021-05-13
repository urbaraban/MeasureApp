using SureMeasure.CadObjects;
using SureMeasure.CadObjects.Interface;
using SureMeasure.Tools;
using System;

namespace SureMeasure.ShapeObj.Constraints
{
    public class ConstraintLenth : CadConstraint, CadObject
    {
        public event EventHandler Changed;
        public event EventHandler<bool> Removed;
        public override event EventHandler<bool> Selected;
        public override event EventHandler<bool> Supported;

        public Orientaton Orientation = Orientaton.OFF; // -1 — Off, 0 — Vetical, 1 — Horizontal

        /// <summary>
        /// Set temp attribute on this constaint
        /// </summary>

        public CadPoint Point1 { get; set; }
        public CadPoint Point2 { get; set; }

        public double Lenth 
        {
            get => this.Variable.Value < 0 ? Sizing.PtPLenth(this.Point1, this.Point2) : this.Variable.Value;
            set
            {
                this.Variable.Value = value;
                OnPropertyChanged("Lenth");
            }
        }

        public override bool IsSelect
        {
            get => this._isselect;
            set
            {
                this._isselect = value;
                Selected?.Invoke(this, this._isselect);
            }
        }
        private bool _isselect = false;

        public override bool IsSupport
        {
            get => this._issupport;
            set
            {
                this._issupport = value;
                Supported?.Invoke(this, this._issupport);
            }
        }
        private bool _issupport = false;

        public override string ID => Point1.ID + Point2.ID;

        public ConstraintLenth(CadPoint point1, CadPoint point2, double Lenth, bool isSupport = false)
        {
            this.Point1 = point1;
            this.Point2 = point2;

            this.Variable = new CadVariable(Lenth, true);
            this.IsSupport = isSupport;

            SubAnchor(this.Point1);
            SubAnchor(this.Point2);

            this.Variable.PropertyChanged += Variable_PropertyChanged;
            this.Point1.ChangedPoint += Point_ChangedPoint;
            this.Point2.ChangedPoint += Point_ChangedPoint;

            this.Point1.Removed += Point_Removed;
            this.Point2.Removed += Point_Removed;
        }

        private void Point_Removed(object sender, bool e)
        {
            if (this.Point1 == sender || this.Point2 == sender)
            {
                Removed?.Invoke(this, true);
            }
        }

        private void Point_ChangedPoint(object sender, CadPoint e)
        {
            if (this.Point1 == sender) 
            {
                UnSubAnchor(this.Point1);
                this.Point1 = e;
                SubAnchor(this.Point1);
            }
            if (this.Point2 == sender)
            {
                UnSubAnchor(this.Point2);
                this.Point2 = e;
                SubAnchor(this.Point2);
            }
            Changed?.Invoke(this, null);
        }

        public void Fix(bool state)
        {
            this.Point1.IsFix = state;
            this.Point2.IsFix = state;
        }

        public void TryRemove()
        {
            UnSubAnchor(this.Point1);
            this.Point1.TryRemove();
            UnSubAnchor(this.Point2);
            this.Point2.TryRemove();
            this.Variable.PropertyChanged -= Variable_PropertyChanged;
            Removed?.Invoke(this, true);
        }

        public void UnSubAnchor(CadPoint cadPoint)
        {
            cadPoint.PropertyChanged -= Point_PropertyChanged;
        }

        public void SubAnchor(CadPoint cadPoint)
        {
            cadPoint.PropertyChanged += Point_PropertyChanged;
        }

        public CadPoint GetNotThisPoint(CadPoint cadPoint)
        {
            if (this.Point1 == cadPoint) return this.Point2;
            else if (this.Point2 == cadPoint) return this.Point1;
            return null;
        }

        public override string ToString()
        {
            return $"{this.ID}:{Lenth.ToString()}";
        }

        private void Variable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MakeMagic(this.Point1, this.Point2);
        }

        private void Point_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MakeMagic((CadPoint)sender, sender != this.Point2 ? this.Point2 : this.Point1);
        }

        public void MakeMagic(CadPoint cadPoint1, CadPoint cadPoint2)
        {
            //Debug.WriteLine($"MakeMagicLine {cadPoint1.ID}{cadPoint2.ID}");

            if (this.Running == false)
            {
                this.Running = true;
                if (cadPoint2.IsFix == false)
                {
                    if (this.Variable.Value > -1 &&
                        this.Variable.Value != Sizing.PtPLenth(cadPoint1, cadPoint2) &&
                        CadConstraint.RuntimeConstraits.Contains(this) == false &&
                        CadConstraint.FixedPoint.Contains(cadPoint1) == false)
                    {
                        CadConstraint.AddRunConstrait(this, cadPoint1);
                        if (this.Orientation == Orientaton.OFF)
                        {
                            cadPoint2.Update(Sizing.GetPostionOnLine(cadPoint1, cadPoint2, this.Lenth), true);
                        }
                        else
                        {
                            int vectorX = cadPoint1.X < cadPoint2.X ? 1 : -1;
                            int vectorY = cadPoint1.Y < cadPoint2.Y ? 1 : -1;
                            cadPoint2.Update(this.Orientation == Orientaton.Vertical ? cadPoint1.X : cadPoint1.X + this.Lenth * vectorX,
                                this.Orientation == Orientaton.Vertical ? cadPoint1.Y + this.Lenth * vectorY : cadPoint1.Y, true);
                        }
                        CadConstraint.RemoveRunConstrait(this, cadPoint1);
                    }
                }
                else if (cadPoint1.IsFix == false)
                {
                    this.Running = false;
                    MakeMagic(cadPoint2, cadPoint1);

                }
                this.Running = false;
            }

            this.Changed?.Invoke(this, null);
        }

        public ConstraintLenth GetInvertClone()
        {
            return new ConstraintLenth(this.Point2, this.Point1, this.Lenth, this.IsSupport);
        }
    }
}
