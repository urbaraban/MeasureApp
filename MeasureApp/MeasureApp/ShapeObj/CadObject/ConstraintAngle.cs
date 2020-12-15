using MeasureApp.ShapeObj.Interface;
using MeasureApp.Tools;
using System;

namespace MeasureApp.ShapeObj.Constraints
{
    public class ConstraintAngle : CadConstraint, CadObject
    {
        public ConstraintLenth anchorAnchor1;
        public ConstraintLenth anchorAnchor2;
        public double Angle
        {
            get => this.Variable.Value > -1 ? this.Variable.Value : Sizing.AngleThreePoint(this.anchorAnchor1.Point1, this.anchorAnchor1.Point2, this.anchorAnchor2.Point2);
            set
            {
                this.Variable.Value = value;
            }
        }
        public double RadAngle => this.Angle * (Math.PI / 180);

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

            this.Variable = new CadVariable(Angle);
            this.Variable.PropertyChanged += Angle_PropertyChanged;

            this.anchorAnchor2.Point1.PropertyChanged += CadanchorMiddle_PropertyChanged;
            this.anchorAnchor2.Point2.PropertyChanged += CadanchorEnd_PropertyChanged;

            this.anchorAnchor1.Point1.Constraints.Add(this);
            this.anchorAnchor1.Point2.Constraints.Add(this);
            this.anchorAnchor2.Point2.Constraints.Add(this);
        }


        public void Remove()
        {
            this.Variable.PropertyChanged -= Angle_PropertyChanged;

            this.anchorAnchor2.Point1.PropertyChanged -= CadanchorMiddle_PropertyChanged;
            this.anchorAnchor2.Point2.PropertyChanged -= CadanchorEnd_PropertyChanged;

            this.anchorAnchor1.Point1.Constraints.Remove(this);
            this.anchorAnchor1.Point2.Constraints.Remove(this);
            this.anchorAnchor2.Point2.Constraints.Remove(this);
        }

        private void Angle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
            {
                MakeMagic(this.anchorAnchor1.Point1, this.anchorAnchor1.Point2, this.anchorAnchor2.Point2, this.anchorAnchor2.Lenth, this.Variable.Value);
            }
        }

        private void CadanchorMiddle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
                {
                    MakeMagic(this.anchorAnchor1.Point1, this.anchorAnchor1.Point2, this.anchorAnchor2.Point2, this.anchorAnchor2.Lenth, this.Variable.Value);
                }
            }
        }

        private void CadanchorEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
                {
                    MakeMagic(this.anchorAnchor2.Point2, this.anchorAnchor1.Point2, this.anchorAnchor1.Point1, this.anchorAnchor1.Lenth, 360 - this.Variable.Value);
                }
            }       
        }

        private void MakeMagic(CadPoint FirstPoint, CadPoint MiddlePoint, CadPoint LastPoint, double lenth, double angle)
        {
            if (this.Variable.Value > -1 && CheckConstraitOrAnchor(this, LastPoint) == false)
            {
                CadConstraint.AddRunConstrait(this, LastPoint);
                LastPoint.Update(Sizing.GetPositionLineFromAngle(FirstPoint, MiddlePoint, lenth, angle));
                foreach (CadConstraint cadConstraint in LastPoint.Constraints)
                {
                    if (cadConstraint is ConstraintLenth lenthAnchor)
                    {
                        if ((lenthAnchor.Point1 == MiddlePoint || lenthAnchor.Point2 == MiddlePoint) && (lenthAnchor.Point1 == LastPoint || lenthAnchor.Point2 == LastPoint))
                        {
                            CadConstraint.AddRunConstrait(lenthAnchor, LastPoint);
                        }
                    }
                }
                CadConstraint.RemoveRunConstrait(this, LastPoint);
            }
        }
    }
}
