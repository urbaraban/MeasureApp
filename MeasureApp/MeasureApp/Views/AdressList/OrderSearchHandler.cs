using SureMeasure.Data;
using SureMeasure.Orders;
using SureOrder.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SureMeasure.OrderPage.Controls
{
    public class OrderSearchHandler : SearchHandler
    {
        public Type SelectedItemNavigationTarget { get; set; }

        protected override async void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                IList<OrderDataItem> Orders = await AppShell.OrdersDB.GetItemsAsync;
                ItemsSource = Orders.Where(Order => 
                Order.Name.ToLower().Contains(newValue.ToLower()) || 
                Order.Adress.ToLower().Contains(newValue.ToLower()));
            }
        }

        protected override void OnItemSelected(object item)
        {
            base.OnItemSelected(item);

            AppShell.SelectOrder = new Order((OrderDataItem)item);
            // Note: strings will be URL encoded for navigation (e.g. "Blue Monkey" becomes "Blue%20Monkey"). Therefore, decode at the receiver.
            // This works because route names are unique in this application.
            //await Shell.Current.GoToAsync($"{GetNavigationTarget()}?name={((Order)item).Name}");
        }
    }
}
