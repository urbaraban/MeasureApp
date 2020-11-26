using MeasureApp.ShapeObj.EnumLibrary;
using MeasureApp.Tools;
using System;

namespace MeasureApp.ShapeObj.Constraints
{
    public class LenthAnchorAnchor : CadConstraint
    {
        private CadVariable _lenth
        {
            get => this.Variable;
            set
            {
                this.Variable = value;
            }
        }

        public event EventHandler Changed;

        public Orientaton Orientation = Orientaton.OFF; // -1 — Off, 0 — Vetical, 1 — Horizontal

        public CadAnchor Anchor1 { get; }
        public CadAnchor Anchor2 { get; }
        public double Lenth 
        {
            get => this._lenth.Value < 0 ? Sizing.PtPLenth(this.Anchor1.cadPoint, this.Anchor2.cadPoint) : this._lenth.Value;
            set => this._lenth.Value = value;
        }

        public LenthAnchorAnchor(CadAnchor point1, CadAnchor point2, double Lenth)
        {
            this.Anchor1 = point1;
            this.Anchor2 = point2;
            this._lenth = new CadVariable(Lenth);

            this.Anchor1.PropertyChanged += Point1_PropertyChanged;
            this.Anchor2.PropertyChanged += Point2_PropertyChanged;

            this.Anchor1.ConstrainList.Add(this);
            this.Anchor2.ConstrainList.Add(this);

            this.Variable.PropertyChanged += Variable_PropertyChanged;
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
                if (this._lenth.Value > -1 && this._lenth.Value != Sizing.PtPLenth(cadAnchor1.cadPoint, cadAnchor2.cadPoint) &&
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

                    this.Changed?.Invoke(this, null);
                }
            }
            else MakeMagic(cadAnchor2, cadAnchor1);
        }


    }
}
