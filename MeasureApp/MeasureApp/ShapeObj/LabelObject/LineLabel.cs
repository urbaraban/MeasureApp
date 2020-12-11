using App1;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.Interface;
using MeasureApp.Tools;
using MeasureApp.View.OrderPage;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace MeasureApp.ShapeObj.LabelObject
{
    public enum LineMenuItems
    {
        Edit_value,
        Measure,
        Temped,
        Vertical,
        Horizontal,
        Free,
        Fix_this,
        Remove
    }

    public class LineLabel : ConstraitLabel
    {
        private ICommand CallValueDialog => new Command(async () =>
        {
            string callresult = await AppShell.Instance.DisplayPromtDialog(_lenthConstrait.Variable.Name, _lenthConstrait.Variable.Value.ToString());
            this._lenthConstrait.Variable.Value = double.Parse(callresult);
        });
        private ICommand Measure => new Command(async () =>
        {
            AppShell.MeasireVariable = this.Variable;
        });
        private ICommand SupportLine => new Command(async () =>
        {
            this._lenthConstrait.IsSupport = !this._lenthConstrait.IsSupport;
        });
        private ICommand Verical => new Command(async () =>
        {
            this._lenthConstrait.Orientation = Orientaton.Vertical;
        });
        private ICommand Horizontal => new Command(async () =>
        {
            this._lenthConstrait.Orientation = Orientaton.Horizontal;
        });
        private ICommand Free_Orientation => new Command(async () =>
        {
            this._lenthConstrait.Orientation = Orientaton.OFF;
        });
        private ICommand Fix => new Command(async () =>
        {
            this._lenthConstrait.Fix(true);
        });
        private ICommand Remove => new Command(async () =>
        {
            this._lenthConstrait.TryRemove();
        });


        public string Name => _lenthConstrait.Anchor1.Name + _lenthConstrait.Anchor2.Name;

        private LenthConstrait _lenthConstrait;

        public LineLabel(LenthConstrait lenthConstrait) : base(lenthConstrait.Variable)
        {
            this.HorizontalTextAlignment = TextAlignment.Center;
            this.VerticalTextAlignment = TextAlignment.Center;

            this._lenthConstrait = lenthConstrait;
            this.Text = this._lenthConstrait.Lenth.ToString();
            this.ScaleY = -1;

            this.BackgroundColor = Color.Green;
            this._lenthConstrait.Changed += LenthAnchorAnchor_Changed;
            this._lenthConstrait.Removed += _lenthAnchor_Removed;

            CadCanvas.RegularSize += CadCanvas_RegularSize;

            this.BindingContext = _lenthConstrait;
            this.SetBinding(Label.TextProperty, new Binding()
            {
                Source = _lenthConstrait,
                Path = "Lenth",
                Mode = BindingMode.OneWay,
                Converter = new ToStringConverter()
            });

            this.Selected += LineLabel_Selected;

            this.SheetMenu = new SheetMenu(new System.Collections.Generic.List<SheetMenuItem>()
            {
                new SheetMenuItem(CallValueDialog, "{CALL_VALUE_DIALOG}"),
                new SheetMenuItem(Measure, "{MEASURE}"),
                new SheetMenuItem(SupportLine, "{SUPPORT_LINE}"),
                new SheetMenuItem(Verical, "{VERTICAL}"),
                new SheetMenuItem(Horizontal, "{HORIZONTAL}"),
                new SheetMenuItem(Free_Orientation, "{FREE_ORIENTATION}"),
                new SheetMenuItem(Fix, "{FIX}"),
                new SheetMenuItem(Remove, "{REMOVE}"),
            }) ;

            this.Update();
        }


        public void Update()
        {
            this.TranslationX = (this._lenthConstrait.Anchor2.X + this._lenthConstrait.Anchor1.X) / 2;
            this.TranslationY = (this._lenthConstrait.Anchor2.Y + this._lenthConstrait.Anchor1.Y) / 2;

            this.Rotation = Sizing.AngleHorizont(this._lenthConstrait.Anchor1.cadPoint, this._lenthConstrait.Anchor2.cadPoint);

            Xamarin.Forms.Device.InvokeOnMainThreadAsync(() => {
                this.Text = Math.Round(Sizing.PtPLenth(this._lenthConstrait.Anchor1.cadPoint, this._lenthConstrait.Anchor2.cadPoint), 2).ToString();
            });
        }

       
        private void CadCanvas_RegularSize(object sender, double e)
        {
            this.Scale = 1 / e;
        }

        private void LineLabel_Selected(object sender, bool e)
        {
            //this._cadLine.IsSelect = !this._cadLine.IsSelect;
        }

        private void LenthAnchorAnchor_Changed(object sender, System.EventArgs e)
        {
            this.Update();
        }

        private void _lenthAnchor_Removed(object sender, EventArgs e)
        {
            this.TryRemove();
        }

    }
}
