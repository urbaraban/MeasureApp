using SureMeasure.Data;
using SureMeasure.Orders;
using SureOrder.Data;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdressListPage : ContentPage
    {


        public AdressListPage()
        {
            InitializeComponent();
            listView.SelectionChanged += ListView_SelectionChanged;
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection[0] is OrderDataItem dataItem)
            {
                if (AppShell.SelectOrder.ID != dataItem.ID)
                {
                    if (await xmlrw.Read(dataItem) is Order order)
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void ClearBtn_Clicked(object sender, EventArgs e)
        {
            foreach(OrderDataItem dataItem in await AppShell.OrdersDB.GetItemsAsync)
            {
                await AppShell.OrdersDB.DeleteItemAsync(dataItem);
            }
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
        }
    }
}