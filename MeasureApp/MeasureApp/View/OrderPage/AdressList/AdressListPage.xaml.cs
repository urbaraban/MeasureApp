using SureMeasure.Data;
using SureMeasure.Orders;
using SureOrder.Data;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdressListPage : ContentPage
    {
        public ICommand AddItem => new Command(async () =>
        {
            
        });

        public AdressListPage()
        {
            InitializeComponent();
            listView.SelectionChanged += ListView_SelectionChanged;
            AppShell.UpdatedOrder += AppShell_UpdatedOrder;
        }

        private void AppShell_UpdatedOrder(object sender, Order e)
        {
            foreach(OrderDataItem orderDataItem in listView.ItemsSource)
            {
                if (orderDataItem.ID == e.ID)
                {
                    listView.Dispatcher.BeginInvokeOnMainThread(() => {
                        listView.SelectedItem = orderDataItem;
                    });
                }
            }
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection[0] is OrderDataItem dataItem)
            {
                if (AppShell.SelectOrder.ID != dataItem.ID)
                {
                    if (xmlrw.Read(dataItem) is Order order)
                    {
                        AppShell.SelectOrder = order;
                    }
                    else
                    {
                        AppShell.SelectOrder = new Order(dataItem);
                    }
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateList();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            AppShell.SelectOrder = new Order();
            await AppShell.OrdersDB.SaveItemAsync(AppShell.SelectOrder);
            UpdateList();
        }

        private void UpdateList()
        {
            listView.Dispatcher.BeginInvokeOnMainThread(async () =>
            {
                listView.ItemsSource = await AppShell.OrdersDB.GetItemsAsync();

                if (AppShell.SelectOrder != null)
                {
                    foreach (OrderDataItem orderDataItem in listView.ItemsSource)
                    {
                        if (orderDataItem.ID == AppShell.SelectOrder.ID)
                        {
                            listView.SelectedItem = orderDataItem;
                            listView.ScrollTo(listView.SelectedItem);
                        }
                    }
                }
            });
        }

        private async void RemoveBtn_Clicked(object sender, EventArgs e)
        {
            if (listView.SelectedItem is OrderDataItem dataItem)
            {
                xmlrw.Remove(dataItem.XmlUrl);
                await AppShell.OrdersDB.DeleteItemAsync(dataItem);
            }
            UpdateList();
        }

        private async void ClearBtn_Clicked(object sender, EventArgs e)
        {
            foreach(OrderDataItem dataItem in await AppShell.OrdersDB.GetItemsAsync())
            {
                await AppShell.OrdersDB.DeleteItemAsync(dataItem);
            }
            UpdateList();
        }

        private async void CallButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.BindingContext == null)
                    await AppShell.Instance.AlertDialog("{Alert}", "{Phone number is empty}");
                else
                    PhoneDialer.Open((string)button.BindingContext);
            }
            
        }

        private async void WayButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                //var location = new Location(latitude, longitude);
                var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                await Map.OpenAsync(new Location(55.045258, 82.867106), options);
            }
        }

        private async void SwipeItem_Invoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem)
            {
                if (swipeItem.BindingContext is OrderDataItem dataItem)
                {
                    xmlrw.Remove(dataItem.XmlUrl);
                    await AppShell.OrdersDB.DeleteItemAsync(dataItem);
                }
            }
            UpdateList();
        }
    }
}