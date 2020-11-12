using MeasureApp.Tools;
using System.Linq;

namespace MeasureApp.ShapeObj.Constraints
{
    public class AngleBetweenThreeAnchor : CadConstraint
    {
        private AnchorAnchorLenth anchorAnchor1;
        private AnchorAnchorLenth anchorAnchor2;

        
        public double Angle;

        /// <summary>
        /// Constrait angle
        /// </summary>
        /// <param name="CadAnchor1">Start anchor</param>
        /// <param name="CadAnchor2">Middle anchor</param>
        /// <param name="CadAnchor3">End anchor</param>
        /// <param name="Angle"></param>
        public AngleBetweenThreeAnchor(AnchorAnchorLenth AnchorAnchor1, AnchorAnchorLenth AnchorAnchor2, double Angle)
        {
            this.anchorAnchor1 = AnchorAnchor1;
            this.anchorAnchor2 = AnchorAnchor2;

            this.Angle = Angle;

            this.anchorAnchor2.Anchor1.PropertyChanged += CadanchorMiddle_PropertyChanged;
            this.anchorAnchor2.Anchor2.PropertyChanged += CadanchorEnd_PropertyChanged;
        }

        private void CadanchorMiddle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (CheckConstraitOrAnchor(this, this.anchorAnchor2.Anchor2) == false)
                {
                    CadConstraint.AddRunConstrait(this, (CadAnchor)sender);
                    this.anchorAnchor2.Anchor2.cadPoint.Update(Sizing.GetPositionLineFromAngle(this.anchorAnchor1.Anchor1.cadPoint, this.anchorAnchor1.Anchor2.cadPoint, anchorAnchor2.Lenth, this.Angle));
                    CadConstraint.RemoveRunConstrait(this, (CadAnchor)sender);
                }
            }
        }

        private void CadanchorEnd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (CheckConstraitOrAnchor(this, this.anchorAnchor1.Anchor1) == false)
                {
                    CadConstraint.AddRunConstrait(this, (CadAnchor)sender);
                    this.anchorAnchor1.Anchor1.cadPoint.Update(Sizing.GetPositionLineFromAngle(this.anchorAnchor2.Anchor2.cadPoint, this.anchorAnchor2.Anchor1.cadPoint, anchorAnchor1.Lenth, 360 - this.Angle));
                    CadConstraint.RemoveRunConstrait(this, (CadAnchor)sender);
                }
            }
        }
    }
}
