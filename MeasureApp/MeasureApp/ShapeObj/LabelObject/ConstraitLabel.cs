using System;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.LabelObject
{
    public abstract class  ConstraitLabel : Label
    {
        public event EventHandler<SheetMenu> ShowObjectMenu;
        public event EventHandler<CadVariable> CallValueDialog;

        public SheetMenu sheetMenu;

        public CadVariable Variable;

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

        public void ChangeValueDialog()
        {
            CallValueDialog?.Invoke(this, this.Variable);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ShowObjectMenu?.Invoke(this, sheetMenu);
        }
        
    }
}
