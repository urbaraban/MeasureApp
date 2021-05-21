using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.View.OrderPage.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public MapPopup()
        {
            InitializeComponent();
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
            Navigation.RemovePage(this);
        }
    }
}