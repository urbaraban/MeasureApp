using MeasureApp.Tools;
using System;
using System.Linq;

namespace MeasureApp.ShapeObj.Constraints
{
    public class AngleBetweenThreeAnchor : CadConstraint
    {
        public LenthAnchorAnchor anchorAnchor1;
        public LenthAnchorAnchor anchorAnchor2;
        public CadVariable Angle
        {
            get => this.Variable;
            set
            {
                this.Variable = value;
            }
        }
        public double RadAngle => this.Angle.Value * (Math.PI / 180);

        /// <summary>
        /// Constrait angle
        /// </summary>
        /// <param name="CadAnchor1">Start anchor</param>
        /// <param name="CadAnchor2">Middle anchor</param>
        /// <param name="CadAnchor3">End anchor</param>
        /// <param name="Angle">Angle in degrees</param>
        public AngleBetweenThreeAnchor(LenthAnchorAnchor AnchorAnchor1, LenthAnchorAnchor AnchorAnchor2, double Angle)
        {
            this.anchorAnchor1 = AnchorAnchor1;
            this.anchorAnchor2 = AnchorAnchor2;

            this.Variable = new CadVariable(Angle);
            this.Variable.PropertyChanged += Angle_PropertyChanged;

            this.anchorAnchor2.Anchor1.PropertyChanged += CadanchorMiddle_PropertyChanged;
            this.anchorAnchor2.Anchor2.PropertyChanged += CadanchorEnd_PropertyChanged;
        }

        private void Angle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MakeMagic(this.anchorAnchor1.Anchor1, this.anchorAnchor1.Anchor2, this.anchorAnchor2.Anchor2, this.anchorAnchor2.Lenth, this.Angle.Value);
        }

        private void CadanchorMiddle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                MakeMagic(this.anchorAnchor1.Anchor1, this.anchorAnchor1.Anchor2, this.anchorAnchor2.Anchor2, this.anchorAnchor2.Lenth, this.Angle.Value);
            }
        }

        private void CadanchorEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                MakeMagic(this.anchorAnchor2.Anchor2, this.anchorAnchor2.Anchor1, this.anchorAnchor1.Anchor1, this.anchorAnchor1.Lenth, 360 - this.Angle.Value);
            }
        }

        private void MakeMagic(CadAnchor FirstAnchor, CadAnchor MiddleAnchor, CadAnchor LastAnchor, double lenth, double angle)
        {
            if (this.Angle.Value > -1 && CheckConstraitOrAnchor(this, LastAnchor) == false)
            {
                CadConstraint.AddRunConstrait(this, FirstAnchor);
                LastAnchor.cadPoint.Update(Sizing.GetPositionLineFromAngle(FirstAnchor.cadPoint, MiddleAnchor.cadPoint, lenth, angle));
                CadConstraint.RemoveRunConstrait(this, FirstAnchor);
            }
        }

        public void UpdateAngle(double Angle)
        {
            this.Angle.Value = Angle;
            MakeMagic(this.anchorAnchor1.Anchor1, this.anchorAnchor1.Anchor2, this.anchorAnchor2.Anchor2, this.anchorAnchor2.Lenth, this.Angle.Value);
        }
    }
}
