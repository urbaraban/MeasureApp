using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.LabelObject;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public static AppShell Instance;

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MeasureApp.View.DrawPage.AdressListPage), typeof(MeasureApp.View.DrawPage.AdressListPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            AppShell.Instance = this;
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
           // await Shell.Current.GoToAsync("//LoginPage");
        }

        //Call Dialog
        public async Task<string> SheetMenuDialog(SheetMenu sheetMenu)
        {
            string Result = await DisplayActionSheet("Чего делаем?", "Cancel", null, sheetMenu.Buttons.ToArray());
            return Result;
        }

        public async Task<string> DisplayPromtDialog(string Name, string Value)
        {
            string result = await DisplayPromptAsync("Изменить значение", Name, "Add", "Cancel", "0000", -1, Keyboard.Numeric, Value);
            return result;
        }
    }
}
