using MeasureApp.ShapeObj.Interface;
using MeasureApp.Tools;
using System;

namespace MeasureApp.ShapeObj.Constraints
{
    public class LenthConstrait : CadConstraint, MainObject
    {
        public event EventHandler Changed;
        public event EventHandler<bool> Selected;

        public Orientaton Orientation = Orientaton.OFF; // -1 — Off, 0 — Vetical, 1 — Horizontal

        /// <summary>
        /// Set temp attribute on this constaint
        /// </summary>

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

        public override string Name => Anchor1.Name + Anchor2.Name;

        public LenthConstrait(CadAnchor point1, CadAnchor point2, double Lenth)
        {
            this.Anchor1 = point1;
            this.Anchor2 = point2;

            this.Variable = new CadVariable(Lenth);

            SubAnchor(this.Anchor1);
            SubAnchor(this.Anchor2);

            this.Variable.PropertyChanged += Variable_PropertyChanged;

        }

        private void Anchor_ChangedAnchror(object sender, CadAnchor e)
        {
            if (this.Anchor1 == sender) 
            {
                UnSubAnchor(this.Anchor1);
                this.Anchor1.TryRemove();
                this.Anchor1 = e;
                SubAnchor(this.Anchor1);
            }
            if (this.Anchor2 == sender)
            {
                UnSubAnchor(this.Anchor2);
                this.Anchor2.TryRemove();
                this.Anchor2 = e;
                SubAnchor(this.Anchor2);
            }
        }

        public void Fix(bool state)
        {
            this.Anchor1.IsFix = state;
            this.Anchor2.IsFix = state;
        }

        public void Remove()
        {
            UnSubAnchor(this.Anchor1);
            UnSubAnchor(this.Anchor2);

            this.Variable.PropertyChanged -= Variable_PropertyChanged;
        }

        public void UnSubAnchor(CadAnchor cadAnchor)
        {
            cadAnchor.PropertyChanged -= Point_PropertyChanged;
            cadAnchor.Constraints.Remove(this);
            cadAnchor.ChangedAnchror -= Anchor_ChangedAnchror;
            cadAnchor.TryRemove();
        }

        public void SubAnchor(CadAnchor cadAnchor)
        {
            cadAnchor.PropertyChanged += Point_PropertyChanged;
            cadAnchor.Constraints.Add(this);
            cadAnchor.ChangedAnchror += Anchor_ChangedAnchror;
        }

        private void Variable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MakeMagic(this.Anchor1, this.Anchor2);
        }

        private void Point_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MakeMagic((CadAnchor)sender, sender != this.Anchor2 ? this.Anchor2 : this.Anchor1);
        }

        private void MakeMagic(CadAnchor cadAnchor1, CadAnchor cadAnchor2)
        {
            if (this.Anchor2.IsFix == false)
            {
                if (this.Variable.Value > -1 &&
                    this.Variable.Value != Sizing.PtPLenth(cadAnchor1.cadPoint, cadAnchor2.cadPoint) &&
                    CadConstraint.RuntimeConstraits.Contains(this) == false &&
                    CadConstraint.FixedAnchor.Contains(this.Anchor2) == false)
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
            else
            {
                MakeMagic(cadAnchor2, cadAnchor1);

            }

            this.Changed?.Invoke(this, null);
        }
    }
}
