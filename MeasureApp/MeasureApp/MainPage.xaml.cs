using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using Xamarin.Forms;

namespace MeasureApp
{
    public partial class MainPage : ContentPage
    {
        private IBluetoothLE ble = CrossBluetoothLE.Current;
        private IAdapter adapter = CrossBluetoothLE.Current.Adapter;

        public MainPage()
        {
            Device.SetFlags(new[] { "Shapes_Experimental", "Brush_Experimental" });
            InitializeComponent();
            AddBtn.Clicked += AddBtn_Clicked;
            ClearBtn.Clicked += ClearBtn_Clicked;
        }
 
        private void ClearBtn_Clicked(object sender, EventArgs e)
        {
            /*if (this.MainCanvas.SelectedLine != null)
            {
                MainCanvas.Remove(this.MainCanvas.SelectedLine);
            }
            else*/
            MainCanvas.Clear();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Добавить линию", "Мне нужны твоя длинна и угол", "Add", "Cancel", "0000&00", -1, Keyboard.Numeric, "300&90");

            if (string.IsNullOrEmpty(result) == false)
            {
                string[] strings = result.Split('&');

                double LineLenth = double.Parse(strings[0]);
                double LineAngle = double.Parse(strings[1]);

                MainCanvas.AddLine(LineLenth, LineAngle);
            }

        }
    }
}
