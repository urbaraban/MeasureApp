using MeasureApp.Tools;
using System;
using System.Linq;

namespace MeasureApp.ShapeObj.Constraints
{
    public class AnchorAnchorLenth : CadConstraint
    {

        public event EventHandler Changed;

        public CadAnchor Anchor1 { get; }
        public CadAnchor Anchor2 { get; }
        public double Lenth { get; set; }

        public AnchorAnchorLenth(CadAnchor point1, CadAnchor point2, double Lenth)
        {
            this.Anchor1 = point1;
            this.Anchor2 = point2;
            this.Lenth = Lenth;

            this.Anchor1.PropertyChanged += Point1_PropertyChanged;
            this.Anchor2.PropertyChanged += Point2_PropertyChanged;

            this.Anchor1.ConstrainList.Add(this);
            this.Anchor2.ConstrainList.Add(this);
        }

        private void Point2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (this.Lenth != -1 && this.Lenth != Sizing.PtPLenth(this.Anchor1.cadPoint, this.Anchor2.cadPoint) &&
                CadConstraint.RuntimeConstraits.Contains(this) == false && CadConstraint.FixedAnchor.Contains(this.Anchor1) == false)
                {
                    CadConstraint.AddRunConstrait(this, (CadAnchor)sender);
                    this.Anchor1.cadPoint.Update(Sizing.GetPostionOnLine(this.Anchor2.cadPoint, this.Anchor1.cadPoint, this.Lenth), true);
                    CadConstraint.RemoveRunConstrait(this, (CadAnchor)sender);

                    this.Changed?.Invoke(this, null);
                }
            }
        }

        private void Point1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (this.Lenth != -1 && this.Lenth != Sizing.PtPLenth(this.Anchor1.cadPoint, this.Anchor2.cadPoint) &&
                CadConstraint.RuntimeConstraits.Contains(this) == false && CadConstraint.FixedAnchor.Contains(this.Anchor2) == false)
                {
                    CadConstraint.AddRunConstrait(this, (CadAnchor)sender);
                    this.Anchor2.cadPoint.Update(Sizing.GetPostionOnLine(this.Anchor1.cadPoint, this.Anchor2.cadPoint, this.Lenth), true);
                    CadConstraint.RemoveRunConstrait(this, (CadAnchor)sender);

                    this.Changed?.Invoke(this, null);
                }
            }
        }
    }
}
