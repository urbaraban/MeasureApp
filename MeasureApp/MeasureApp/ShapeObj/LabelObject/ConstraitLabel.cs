using App1;
using MeasureApp.ShapeObj.Interface;
using System;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.LabelObject
{
    public abstract class  ConstraitLabel : Label, CanvasObject, CommonObject
    {
        public event EventHandler Removed;
        public event EventHandler<bool> Selected;

        public CadVariable Variable;


        private int taps = 0;
        private bool runtimer = false;

        public  virtual SheetMenu SheetMenu { get => this._sheetMenu; set => this._sheetMenu = value; }
        private SheetMenu _sheetMenu;

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
            Update();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            TapManager();
        }


        public void TryRemove()
        {
            this.Removed?.Invoke(this, null);
        }

        public void Update()
        {
            Xamarin.Forms.Device.InvokeOnMainThreadAsync(() => {
                this.Text = this.Variable.ToString();
            });
        }

        public virtual void RunSheetMenuCommand(string NameCommand)
        {
            
        }

        public void TapManager()
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
