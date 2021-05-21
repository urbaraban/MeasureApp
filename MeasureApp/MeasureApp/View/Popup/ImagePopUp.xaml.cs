using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.View.OrderPage.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagePopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        public ImagePopUp(string SelectImage, ObservableCollection<string> Paths)
        {
            InitializeComponent();
            this.Carusel.ItemsSource = Paths;
            Carusel.CurrentItem = SelectImage;
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
            Navigation.RemovePage(this);
        }
    }
}