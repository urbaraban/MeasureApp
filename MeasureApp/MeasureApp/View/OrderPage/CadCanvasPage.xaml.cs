using MeasureApp.ShapeObj;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CadCanvasPage : ContentPage
    {
        public CadCanvasPage()
        {
            InitializeComponent();
            AddBtn.Clicked += AddBtn_Clicked;
            ClearBtn.Clicked += ClearBtn_Clicked;
            FitBtn.Clicked += FitBtn_Clicked;
     

            Binding binding = new Binding()
            {
                Source = this.MainCanvas,
                Path = "Method",
                Mode = BindingMode.OneWay,
                Converter = new ToStringConverter()
            };
            DrawMethodLabel.Text = this.MainCanvas.Method.ToString();
            DrawMethodLabel.BindingContext = this.MainCanvas.Method;
            DrawMethodLabel.SetBinding(Label.TextProperty, binding);
        }


        private void FitBtn_Clicked(object sender, EventArgs e)
        {
            MainCanvas.FitChild();
        }

        private void ClearBtn_Clicked(object sender, EventArgs e)
        {
            MainCanvas.Clear();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            Random random = new Random();

            string result = await DisplayPromptAsync("Добавить линию", "Мне нужны твоя длинна и угол", "Add", "Cancel", "0000&00", -1, Keyboard.Numeric, $"{random.Next(250, 1000)}&{random.Next(45, 270)}");

            if (string.IsNullOrEmpty(result) == false)
            {
                string[] strings = result.Split('&');

                double LineLenth = double.Parse(strings[0]);
                double LineAngle = -1;
                if (strings.Length > 1)
                {
                    LineAngle = double.Parse(strings[1]);
                }

                MainCanvas.BuildLine(LineLenth, LineAngle);
            }

        }

        private void RadioStep_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            this.MainCanvas.Method = DrawMethod.StepByStep;
        }

        private void RadioPoint_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            this.MainCanvas.Method = DrawMethod.FromPoint;
        }
    }
}