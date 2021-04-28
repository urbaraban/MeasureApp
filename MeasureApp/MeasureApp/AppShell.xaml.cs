using InTheHand.Bluetooth;
using SureMeasure.Data;
using SureMeasure.Orders;
using SureMeasure.ShapeObj;
using SureMeasure.BLEDevice;
using SureMeasure.View.OrderPage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SureMeasure
{
    public partial class AppShell : Shell
    {
        private static Order selectorder;

        public static Order SelectOrder
        {
            get => selectorder;
            set
            {
                selectorder = value;
                UpdatedOrder?.Invoke(null, selectorder);
            }
        }

        private static OrdersDataBase ordersDataBase;

        public static OrdersDataBase OrdersDB
        {
            get
            {
                if (ordersDataBase == null)
                {
                    ordersDataBase = new OrdersDataBase();
                }
                return ordersDataBase;
            }
        }

        public static event EventHandler<Tuple<double, double>> LenthUpdated;
        public static event EventHandler<Order> UpdatedOrder;

        public static AppShell Instance { get; set; }

        public static LaserDistanceMeter BLEDevice
        {
            get => AppShell._bledevice;
            set
            {
                if (_bledevice != null)
                {
                    _bledevice.LenthUpdated -= _bledevice_LenthUpdated;
                }
                _bledevice = value;
                if (_bledevice != null)
                {
                    AppShell._bledevice.LenthUpdated += _bledevice_LenthUpdated; ;
                }
            }
        }
        private static void _bledevice_LenthUpdated(object sender, Tuple<double, double> e) => LenthUpdated?.Invoke(null, e);

        private static LaserDistanceMeter _bledevice;

        private List<BluetoothDevice> Devices = new List<BluetoothDevice>();

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AdressListPage), typeof(AdressListPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            AppShell.Instance = this;
            AppShell.SelectOrder = new Order();

            AdressListPage.SelectedOrderItem += AdressListPage_UpdatedOrder;
        }

        private void AdressListPage_UpdatedOrder(object sender, Order e)
        {
            AppShell.SelectOrder = e;
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
           // await Shell.Current.GoToAsync("//LoginPage");
        }

        //Call Dialog
        public async Task<string> SheetMenuDialog(SheetMenu sheetMenu)
        {
            string Result = await DisplayActionSheet("Чего делаем?", "Cancel", null, sheetMenu.ToArray());
            return Result;
        }

        public async Task<string> DisplayPromtDialog(string Title, string Value)
        {
            string result = await DisplayPromptAsync("Изменить значение", Title, "Add", "Cancel", "0000", -1, Keyboard.Numeric, Value);
            return result;
        }

        public async Task<bool> AlertDialog(string Title,string Message)
        {
            return await DisplayAlert(Title, Message, "Yes", "Cancel");
        }

        public async Task LoadBleScan()
        {
            RequestDeviceOptions options = new RequestDeviceOptions();
            options.AcceptAllDevices = true;
            BluetoothDevice device = await Bluetooth.RequestDeviceAsync(options);

            if (device != null)
            {
                switch (device.Name)
                {
                    case "Laser Distance Meter":
                        BLEDevice = new lomvumM40(device);
                        break;
                }

            }
        }

        private void Bluetooth_AdvertisementReceived(object sender, BluetoothAdvertisingEvent e)
        {
            Devices.Add(e.Device);
            Debug.WriteLine($"Name:{e.Name} Rssi:{e.Rssi}");
        }

        private static string ByteArrayToString(byte[] data)
        {
            if (data == null)
                return "<NULL>";

            StringBuilder sb = new StringBuilder();

            foreach (byte b in data)
            {
                sb.Append(b.ToString("X"));
            }

            return sb.ToString();
        }

        private async void DeviceItem_Clicked(object sender, EventArgs e)
        {
            LoadBleScan();
        }
    }
}
