using SureMeasure.Orders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContourInfoPage : ContentPage
    {
        public Order GetOrder => (Order)this.BindingContext;

        public ContourInfoPage()
        {
            InitializeComponent();
            AppShell.UpdatedOrder += AppShell_UpdatedOrder;
            this.BindingContextChanged += ContourInfoPage_BindingContextChanged;
            this.BindingContext = AppShell.SelectOrder;
        }

        private void ContourInfoPage_BindingContextChanged(object sender, EventArgs e)
        {
            ContourPicker.ItemsSource = GetOrder.Contours;
        }

        private void AppShell_UpdatedOrder(object sender, Orders.Order e) => this.BindingContext = e;

        private async void PhotoButton_Clicked(object sender, EventArgs e)
        {
           string path = await AppShell.TakePhotoAsync();
            if (string.IsNullOrEmpty(path) == false)
            {
                ImageGallery.Dispatcher.BeginInvokeOnMainThread(() => { 
                AppShell.SelectOrder.AddPhoto(path);
                });

            }
        }

        protected override void OnAppearing()
        {
            this.GetOrder.UpdateBinding();
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            if (AppShell.SelectOrder.IsAlive == true)
            {
                await AppShell.OrdersDB.SaveItemAsync(AppShell.SelectOrder);
            }
        }

        private async void SelectPhotoButton_Clicked(object sender, EventArgs e)
        {
            
            string path = await AppShell.SelectPhoto();
            if (string.IsNullOrEmpty(path) == false)
            {
                ImageGallery.Dispatcher.BeginInvokeOnMainThread(() => {
                    AppShell.SelectOrder.AddPhoto(path);
                });
            }
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void ShareBtn_Clicked(object sender, EventArgs e)
        {
            AppShell.ShareOrder(this.GetOrder);
        }
    }
}