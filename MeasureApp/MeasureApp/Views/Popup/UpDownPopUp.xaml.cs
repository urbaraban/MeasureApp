using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpDownPopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        public event EventHandler<int> ReturnFormDialog;

        public int Count 
        {
            get => count;
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        } 
        private int count = 0;

        public UpDownPopUp(int count = 0)
        {
            InitializeComponent();
            this.Count = count;
        }

        public ICommand Plus => new Command(() => {
            Count += 1;
        });
        public ICommand Minus => new Command(() => {
            Count -= Count <= 1 ? 0 : 1;
        });

        public ICommand OK => new Command(() => {
            ReturnFormDialog?.Invoke(this, Count);
            Close();
        });

        public ICommand Cancel => new Command(() => {
            ReturnFormDialog?.Invoke(this, -1);
            Close();
        });

        private async void Close()
        {
            await Navigation.PopPopupAsync();
            Navigation.RemovePage(this);
        }
    }
}