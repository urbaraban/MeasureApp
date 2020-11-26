using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.LabelObject;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp.View.DrawPage
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
            MainCanvas.ShowObjectMenu += MainCanvas_ShowObjectMenu;
            MainCanvas.CallValueDialog += MainCanvas_CallValueDialog;
        }

        private async void MainCanvas_CallValueDialog(object sender, CadVariable e)
        {
            string result = await DisplayPromptAsync("Изменить значение", e.Name, "Add", "Cancel", "0000", -1, Keyboard.Numeric, e.Value.ToString());

            if (string.IsNullOrEmpty(result) == false)
            {
                e.Value = double.Parse(result);
            }
        }

        private async void MainCanvas_ShowObjectMenu(object sender, SheetMenu e)
        {
            e.SendAction(await DisplayActionSheet("Чего делаем?", "Cancel", null, e.Buttons.ToArray()));
        }

        private void FitBtn_Clicked(object sender, EventArgs e)
        {
            MainCanvas.FitChild();
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
            string result = await DisplayPromptAsync("Добавить линию", "Мне нужны твоя длинна и угол", "Add", "Cancel", "0000&00", -1, Keyboard.Numeric, "100&90");

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