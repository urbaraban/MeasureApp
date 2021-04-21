using MeasureApp.Data;
using MeasureApp.Orders;
using System;
using System.Linq;
using System.Windows.Input;
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

           /* listView.ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };
                label.SetBinding(Label.TextProperty, "Name");

                var stackLayout = new StackLayout
                {
                    Margin = new Thickness(20, 0, 0, 0),
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Children = { label }
                };

                return new ViewCell { View = stackLayout };
            });*/
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            listView.ItemsSource = await AppShell.OrdersDB.GetItemsAsync();
        }

        async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is OrderDataItem dataItem && AppShell.SelectOrder.DataItem != dataItem)
            {
                if (AppShell.SelectOrder != null)
                {
                    await AppShell.OrdersDB.SaveItemAsync(AppShell.SelectOrder);
                }

                Order order = xmlrw.Read(dataItem);
                SelectedOrderItem(this, order == null ? new Order(dataItem) : order);
            }
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
    }
}