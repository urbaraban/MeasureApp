using SureMeasure.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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

        private void PhotoButton_Clicked(object sender, EventArgs e)
        {

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
    }
}