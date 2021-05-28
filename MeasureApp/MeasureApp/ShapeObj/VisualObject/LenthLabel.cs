using DrawEngine;
using DrawEngine.Constraints;
using SureMeasure.ShapeObj.Canvas;
using SureMeasure.View.OrderPage;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SureMeasure.ShapeObj
{

    public class LenthLabel : ConstraitLabel
    {
        #region Command
        private ICommand CallValueDialog => new Command(async () =>
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(_lenthConstrait.Variable.Name, _lenthConstrait.Value.ToString());
            if (callresult != null)
            {
                this._lenthConstrait.Variable.Value = double.Parse(callresult);
            }
        });
        private ICommand GetMeasure => new Command(() =>
        {
            CadCanvasPage.MeasureVariable = this._lenthConstrait.Variable;
            AppShell.BLEDevice.OnDevice();
        });
        private ICommand SupportLine => new Command(() =>
        {
            this._lenthConstrait.IsSupport = !this._lenthConstrait.IsSupport;
        });
        private ICommand Verical => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.Vertical;
        });
        private ICommand Horizontal => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.Horizontal;
        });
        private ICommand Free_Orientation => new Command(() =>
        {
            this._lenthConstrait.Orientation = Orientaton.OFF;
        });
        private ICommand Fix => new Command(() =>
        {
            this._lenthConstrait.Fix(true);
        });
        private ICommand Remove => new Command(() =>
        {
            this._lenthConstrait.TryRemove();
        });

        private ICommand Split => new Command(() =>
        {
            this._lenthConstrait.MakeSplit();
        });
        #endregion

        public override event EventHandler<bool> Removed;

        public override bool IsSelect 
        { 
            get => _lenthConstrait.IsSelect; 
            set => _lenthConstrait.IsSelect = value; 
        }

        public string Name => _lenthConstrait.Point1.ID + _lenthConstrait.Point2.ID;

        private ConstraintLenth _lenthConstrait;

        public LenthLabel(ConstraintLenth lenthConstrait) : base(lenthConstrait)
        {
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.Center;

            this._lenthConstrait = lenthConstrait;
            this.Text = this._lenthConstrait.Value.ToString();
            this.ScaleY = -1;

            this.BackgroundColor = Color.Green;
            this._lenthConstrait.PropertyChanged += LenthAnchorAnchor_Changed;
            this._lenthConstrait.Removed += _lenthAnchor_Removed;

            CadCanvas.RegularSize += CadCanvas_RegularSize;

            this.BindingContext = _lenthConstrait;
            this.SetBinding(Label.TextProperty, new Binding()
            {
                Source = _lenthConstrait,
                Path = "Value",
                Mode = BindingMode.OneWay,
                Converter = new ToStringConverter()
            });

            this.SheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>()
            {
                new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"),
                new SheetMenuItem(GetMeasure, "{MEASURE}"),
                new SheetMenuItem(SupportLine, "{SUPPORT_LINE}"),
                new SheetMenuItem(Verical, "{VERTICAL}"),
                new SheetMenuItem(Horizontal, "{HORIZONTAL}"),
                new SheetMenuItem(Free_Orientation, "{FREE_ORIENTATION}"),
                new SheetMenuItem(Fix, "{FIX}"),
                new SheetMenuItem(Remove, "{REMOVE}"),
                 new SheetMenuItem(Split, "{SPLIT}")
            }) ;

            this.Update(string.Empty);
        }

        public override async Task Update(string Param)
        {
            this.TranslationX = (this._lenthConstrait.Point2.OX + this._lenthConstrait.Point1.OX) / 2;
            this.TranslationY = (this._lenthConstrait.Point2.OY + this._lenthConstrait.Point1.OY) / 2;

            this.Rotation = Sizing.AngleHorizont(this._lenthConstrait.Point1, this._lenthConstrait.Point2);

            double templenth = Math.Round(Sizing.PtPLenth(this._lenthConstrait.Point1, this._lenthConstrait.Point2), 2);
            double tempdelta = Math.Abs(templenth / _lenthConstrait.Value - 1);
            if (tempdelta == 0 || tempdelta > 0.01) { 
                await Xamarin.Forms.Device.InvokeOnMainThreadAsync(() =>
                {
                    this.Text = $"{(_lenthConstrait.Value > -1 ? _lenthConstrait.Value.ToString() : string.Empty)}" +
                        $"{ (templenth != _lenthConstrait.Value ? $"({templenth})" : string.Empty)}";
            
                });
            }
        }

       
        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.Scale = 1 / e;
        }

        private async void LenthAnchorAnchor_Changed(object sender, System.EventArgs e)
        {
           await this.Update("Changed");
        }

        private void _lenthAnchor_Removed(object sender, bool e)
        {
            Removed?.Invoke(this, true);
        }

        public override void TryRemove()
        {
            this._lenthConstrait.TryRemove();
        }

    }
}
