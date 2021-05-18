using SureMeasure.ShapeObj.Constraints;
using SureMeasure.ShapeObj.Interface;
using System;
using Xamarin.Forms;

namespace SureMeasure.ShapeObj
{
    public abstract class  ConstraitLabel : Label, CanvasObject, ActiveObject
    {
        public virtual event EventHandler<object> Dropped;
        public virtual event EventHandler<bool> Removed;

        public CadConstraint Constraint;

        public virtual bool IsSelect
        {
            get => this.Constraint.IsSelect;
            set
            {
                this.Constraint.IsSelect = value;
            }
        }


        private int taps = 0;
        private bool runtimer = false;

        public  virtual SheetMenu SheetMenu { get => this._sheetMenu; set => this._sheetMenu = value; }
        private SheetMenu _sheetMenu;

        public ConstraitLabel(CadConstraint Constraint)
        {
            this.Constraint = Constraint;
            this.Constraint.PropertyChanged += Value_PropertyChanged;

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            this.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void Value_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Update(e.PropertyName);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TapManager();
        }

        public virtual void TryRemove()
        {
            this.Removed?.Invoke(this, true);
        }

        public virtual void Update(string Param)
        {
            Xamarin.Forms.Device.InvokeOnMainThreadAsync(() => {
                this.Text = Math.Round(this.Constraint.Value, 1).ToString();
            });
        }

        private void TapManager()
        {
            taps += 1;
            if (this.runtimer == false)
            {
                this.runtimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
                {
                    if (taps < 2)
                    {
                        this.IsSelect = !this.IsSelect;
                    }
                    else
                    {
                        this.SheetMenu.ShowMenu(this);
                    }

                    taps = 0;
                    return false; // return true to repeat counting, false to stop timer
                });
                this.runtimer = false;
            }
        }
    }
}
