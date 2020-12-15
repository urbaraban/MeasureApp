using MeasureApp.View.OrderPage.OrderClass;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdressListPage : ContentPage
    {
        public AdressListPage()
        {
            InitializeComponent();
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string orderName = (e.CurrentSelection.FirstOrDefault() as Order).Name;
            // This works because route names are unique in this application.
           // await Shell.Current.GoToAsync($"catdetails?name={orderName}");
            // The full route is shown below.
            // await Shell.Current.GoToAsync($"//animals/domestic/cats/catdetails?name={catName}");
        }
    }
}