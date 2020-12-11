using MeasureApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureApp.ShapeObj.Constraints
{
    public class AngleConstrait : CadConstraint
    {
        public LenthConstrait anchorAnchor1;
        public LenthConstrait anchorAnchor2;
        public double Angle
        {
            get => this.Variable.Value > -1 ? this.Variable.Value : Sizing.AngleThreePoint(this.anchorAnchor1.Anchor1.cadPoint, this.anchorAnchor1.Anchor2.cadPoint, this.anchorAnchor2.Anchor2.cadPoint);
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
        public AngleConstrait(LenthConstrait AnchorAnchor1, LenthConstrait AnchorAnchor2, double Angle)
        {
            this.anchorAnchor1 = AnchorAnchor1;
            this.anchorAnchor2 = AnchorAnchor2;

            this.Variable = new CadVariable(Angle);
            this.Variable.PropertyChanged += Angle_PropertyChanged;

            this.anchorAnchor2.Anchor1.PropertyChanged += CadanchorMiddle_PropertyChanged;
            this.anchorAnchor2.Anchor2.PropertyChanged += CadanchorEnd_PropertyChanged;

            this.anchorAnchor1.Anchor1.Constraints.Add(this);
            this.anchorAnchor1.Anchor2.Constraints.Add(this);
            this.anchorAnchor2.Anchor2.Constraints.Add(this);
        }


        public void Remove()
        {
            this.Variable.PropertyChanged -= Angle_PropertyChanged;

            this.anchorAnchor2.Anchor1.PropertyChanged -= CadanchorMiddle_PropertyChanged;
            this.anchorAnchor2.Anchor2.PropertyChanged -= CadanchorEnd_PropertyChanged;

            this.anchorAnchor1.Anchor1.Constraints.Remove(this);
            this.anchorAnchor1.Anchor2.Constraints.Remove(this);
            this.anchorAnchor2.Anchor2.Constraints.Remove(this);
        }

        private void Angle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
            {
                MakeMagic(this.anchorAnchor1.Anchor1, this.anchorAnchor1.Anchor2, this.anchorAnchor2.Anchor2, this.anchorAnchor2.Lenth, this.Variable.Value);
            }
        }

        private void CadanchorMiddle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
                {
                    MakeMagic(this.anchorAnchor1.Anchor1, this.anchorAnchor1.Anchor2, this.anchorAnchor2.Anchor2, this.anchorAnchor2.Lenth, this.Variable.Value);
                }
            }
        }

        private void CadanchorEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (this.anchorAnchor2.Orientation == Orientaton.OFF || this.anchorAnchor1.Orientation != Orientaton.OFF)
                {
                    MakeMagic(this.anchorAnchor2.Anchor2, this.anchorAnchor1.Anchor2, this.anchorAnchor1.Anchor1, this.anchorAnchor1.Lenth, 360 - this.Variable.Value);
                }
            }       
        }

        private void MakeMagic(CadAnchor FirstAnchor, CadAnchor MiddleAnchor, CadAnchor LastAnchor, double lenth, double angle)
        {
            if (this.Variable.Value > -1 && CheckConstraitOrAnchor(this, LastAnchor) == false)
            {
                CadConstraint.AddRunConstrait(this, LastAnchor);
                LastAnchor.cadPoint.Update(Sizing.GetPositionLineFromAngle(FirstAnchor.cadPoint, MiddleAnchor.cadPoint, lenth, angle));
                foreach (CadConstraint cadConstraint in LastAnchor.Constraints)
                {
                    if (cadConstraint is LenthConstrait lenthAnchor)
                    {
                        if ((lenthAnchor.Anchor1 == MiddleAnchor || lenthAnchor.Anchor2 == MiddleAnchor) && (lenthAnchor.Anchor1 == LastAnchor || lenthAnchor.Anchor2 == LastAnchor))
                        {
                            CadConstraint.AddRunConstrait(lenthAnchor, LastAnchor);
                        }
                    }
                }
                CadConstraint.RemoveRunConstrait(this, LastAnchor);
            }
        }
    }
}
