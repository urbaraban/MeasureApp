using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace App1
{
    public partial class AppShell : Xamarin.Forms.Shell
    {

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MeasureApp.View.DrawPage.AdressListPage), typeof(MeasureApp.View.DrawPage.AdressListPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
           // await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
