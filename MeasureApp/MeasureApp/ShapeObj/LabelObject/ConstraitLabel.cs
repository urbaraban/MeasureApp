using System;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.LabelObject
{
    public abstract class  ConstraitLabel : Label
    {
        public event EventHandler<SheetMenu> ShowObjectMenu;
        public event EventHandler<CadVariable> CallValueDialog;
        public event EventHandler<bool> Selected;

        public SheetMenu sheetMenu;

        public CadVariable Variable;

        private int taps = 0;
        private bool runtimer = false;


        public ConstraitLabel(CadVariable Variable)
        {
            this.Variable = Variable;
            this.Variable.PropertyChanged += Value_PropertyChanged;

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            this.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void Value_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Text = this.Variable.ToString();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            taps += 1;
            if (this.runtimer == false)
            {
                this.runtimer = true;
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
                {
                    if (taps < 2)
                    {
                        Selected?.Invoke(this, true);
                    }
                    else
                    {
                        ShowObjectMenu?.Invoke(this, this.sheetMenu);
                    }

                    taps = 0;
                    return false; // return true to repeat counting, false to stop timer
                });
                this.runtimer = false;
            }
        }
        
    }
}
