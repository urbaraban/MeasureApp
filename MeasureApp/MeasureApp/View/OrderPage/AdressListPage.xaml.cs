using MeasureApp.Data;
using MeasureApp.Orders;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdressListPage : ContentPage
    {
        public static event EventHandler<Order> SelectedOrderItem;

        public ICommand AddItem => new Command(async () =>
        {
            
        });

        public AdressListPage()
        {
            InitializeComponent();

            listView.SelectionChanged += ListView_SelectionChanged;
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection[0] is OrderDataItem dataItem)
            {
                if (AppShell.SelectOrder.DataItem != dataItem && AppShell.SelectOrder.Contours.Count > 0)
                {
                    if (AppShell.SelectOrder.Contours[0].Lenths.Count > 0)
                    {
                        if (await AppShell.Instance.AlertDialog("Сохранить текущий?", string.Empty) == true)
                        {
                            await AppShell.OrdersDB.SaveItemAsync(AppShell.SelectOrder);
                        }
                    }
                }

                Order order = xmlrw.Read(dataItem);
                SelectedOrderItem(this, order == null ? new Order(dataItem) : order);

                listView.Dispatcher.BeginInvokeOnMainThread(async () =>
                {
                    listView.ItemsSource = await AppShell.OrdersDB.GetItemsAsync();
                });

            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            listView.ItemsSource = await AppShell.OrdersDB.GetItemsAsync();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            await AppShell.OrdersDB.SaveItemAsync(AppShell.SelectOrder);
        }

        private async void RemoveBtn_Clicked(object sender, EventArgs e)
        {
            if (listView.SelectedItem is OrderDataItem dataItem)
            {
                xmlrw.Remove(dataItem.XmlUrl);
                await AppShell.OrdersDB.DeleteItemAsync(dataItem);
                listView.ItemsSource = await AppShell.OrdersDB.GetItemsAsync();
            }
        }

        private async void ClearBtn_Clicked(object sender, EventArgs e)
        {
            foreach(OrderDataItem dataItem in await AppShell.OrdersDB.GetItemsAsync())
            {
                await AppShell.OrdersDB.DeleteItemAsync(dataItem);
            }
            listView.ItemsSource = await AppShell.OrdersDB.GetItemsAsync();
        }

        private void CallButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.BindingContext == null) 
                    PhoneDialer.Open("+79999999999");
                else
                PhoneDialer.Open((string)button.BindingContext);
            }
            
        }

        private async void WayButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                //var location = new Location(latitude, longitude);
                var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                await Map.OpenAsync(new Location(55.045258, 82.867106), options);
            }
        }
    }
}