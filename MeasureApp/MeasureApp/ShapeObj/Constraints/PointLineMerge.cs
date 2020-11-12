using System.Linq;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.Constraints
{
    public class PointLineMerge : CadConstraint
    {
        private CadLine cadLine;
        private CadAnchor cadAnchor;
        private int index;

        public PointLineMerge(CadLine line, CadAnchor anchor, int index)
        {
            this.cadLine = line;
            this.cadAnchor = anchor;
            this.index = index;

            this.cadAnchor.PropertyChanged += CadAnchor_PropertyChanged;
            //this.cadLine.DeltaTranslate += CadLine_DeltaTranslate;
        }

        private void CadLine_DeltaTranslate(object sender, Point e)
        {
            this.cadAnchor.TranslationX += e.X;
            this.cadAnchor.TranslationY += e.Y;
            this.cadAnchor.cadPoint.Add(e);
        }

        private void CadAnchor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CadConstraint.RuntimeConstraits.Contains(this) == false)
            {
                CadConstraint.RuntimeConstraits.Add(this);
                this.cadLine.Update();
                if (CadConstraint.RuntimeConstraits.First() == this) CadConstraint.RuntimeConstraits.Clear();
            }
        }
    }
}
