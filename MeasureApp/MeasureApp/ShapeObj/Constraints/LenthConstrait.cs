using MeasureApp.ShapeObj.EnumLibrary;
using MeasureApp.Tools;
using System;

namespace MeasureApp.ShapeObj.Constraints
{
    public class LenthConstrait : CadConstraint
    {
        public event EventHandler Changed;

        public Orientaton Orientation = Orientaton.OFF; // -1 — Off, 0 — Vetical, 1 — Horizontal

        public CadAnchor Anchor1 { get; set; }
        public CadAnchor Anchor2 { get; set; }
        public double Lenth 
        {
            get => this.Variable.Value < 0 ? Sizing.PtPLenth(this.Anchor1.cadPoint, this.Anchor2.cadPoint) : this.Variable.Value;
            set
            {
                this.Variable.Value = value;
                OnPropertyChanged("Lenth");
            }
        }

        public LenthConstrait(CadAnchor point1, CadAnchor point2, double Lenth)
        {
            this.Anchor1 = point1;
            this.Anchor2 = point2;

            this.Variable = new CadVariable(Lenth);

            this.Anchor1.PropertyChanged += Point1_PropertyChanged;
            this.Anchor2.PropertyChanged += Point2_PropertyChanged;

            this.Anchor1.Constraints.Add(this);
            this.Anchor2.Constraints.Add(this);

            this.Variable.PropertyChanged += Variable_PropertyChanged;

            this.Anchor1.ChangedAnchror += Anchor1_ChangedAnchror;
            this.Anchor2.ChangedAnchror += Anchor1_ChangedAnchror;
        }

        private void Anchor1_ChangedAnchror(object sender, CadAnchor e)
        {
            if (this.Anchor1 == sender) this.Anchor1 = e;
            if (this.Anchor2 == sender) this.Anchor2 = e;
        }


        public void Remove()
        {
            this.Anchor1.PropertyChanged -= Point1_PropertyChanged;
            this.Anchor2.PropertyChanged -= Point2_PropertyChanged;

            this.Anchor1.Constraints.Remove(this);
            this.Anchor2.Constraints.Remove(this);

            this.Variable.PropertyChanged -= Variable_PropertyChanged;
        }

        private void Variable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MakeMagic(this.Anchor1, this.Anchor2);
        }

        private void Point2_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MakeMagic(this.Anchor2, this.Anchor1);
        }

        private void Point1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MakeMagic(this.Anchor1, this.Anchor2);
        }

        private void MakeMagic(CadAnchor cadAnchor1, CadAnchor cadAnchor2)
        {
            if (this.Anchor2.IsFix == false)
            {
                if (this.Variable.Value > -1 && this.Variable.Value != Sizing.PtPLenth(cadAnchor1.cadPoint, cadAnchor2.cadPoint) &&
                    CadConstraint.RuntimeConstraits.Contains(this) == false && CadConstraint.FixedAnchor.Contains(this.Anchor2) == false)
                {
                    CadConstraint.AddRunConstrait(this, cadAnchor1);
                    if (this.Orientation == Orientaton.OFF)
                    {
                        cadAnchor2.cadPoint.Update(Sizing.GetPostionOnLine(cadAnchor1.cadPoint, cadAnchor2.cadPoint, this.Lenth), true);
                    }
                    else
                    {
                        int vectorX = cadAnchor1.X < cadAnchor2.X ? 1 : -1;
                        int vectorY = cadAnchor1.Y < cadAnchor2.Y ? 1 : -1;
                        cadAnchor2.cadPoint.Update(this.Orientation == Orientaton.Vertical ? cadAnchor1.cadPoint.X : cadAnchor1.cadPoint.X + this.Lenth * vectorX,
                            this.Orientation == Orientaton.Vertical ? cadAnchor1.cadPoint.Y + this.Lenth * vectorY : cadAnchor1.cadPoint.Y, true);
                    }
                    CadConstraint.RemoveRunConstrait(this, cadAnchor1);
                }
            }
            else MakeMagic(cadAnchor2, cadAnchor1);

            this.Changed?.Invoke(this, null);
        }
    }
}
