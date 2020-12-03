using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp.View.BLEDevice.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceList : ContentPage
    {
        IAdapter adapter;
        ObservableCollection<IDevice> devices;

        public DeviceList()
        {
            InitializeComponent();

            this.adapter = CrossBluetoothLE.Current.Adapter;
            this.devices = new ObservableCollection<IDevice>();

            listView.ItemsSource = devices;

            adapter.DeviceDiscovered += (object sender, DeviceEventArgs e) => devices.Add(e.Device);

            adapter.ScanTimeoutElapsed += (sender, e) =>
            {
                adapter.StopScanningForDevicesAsync(); // not sure why it doesn't stop already, if the timeout elapses... or is this a fake timeout we made?
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => DisplayAlert("Timeout", "Bluetooth scan timeout elapsed", "OK"));
            };

            ScanAllButton.Clicked += (sender, e) => StartScanning();

            ScanHrmButton.Clicked += (sender, e) => StartScanning();
        }


        public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (((ListView)sender).SelectedItem == null)
                return;

            Debug.WriteLine(" xxxxxxxxxxxx OnItemSelected " + e.SelectedItem.ToString());

            StopScanning();

            var device = e.SelectedItem as IDevice;

            //var servicePage = new ServiceList(adapter, device);

            // load services on the next page
            //await Navigation.PushAsync(servicePage);

            ((ListView)sender).SelectedItem = null; // clear selection
        }

        void StartScanning()
        {
            Guid[] guids = new Guid[1];

            StartScanning(guids);
        }

        void StartScanning(Guid[] forService)
        {
            if (adapter.IsScanning)
            {
                adapter.StopScanningForDevicesAsync();

                Debug.WriteLine("adapter.StopScanningForDevices()");
            }
            else
            {
                devices.Clear();
                adapter.StartScanningForDevicesAsync();

                Debug.WriteLine("adapter.StartScanningForDevices(" + forService + ")");
            }
        }

        void StopScanning()
        {
            // stop scanning
            new Task(() =>
            {
                if (adapter.IsScanning)
                {
                    Debug.WriteLine("Still scanning, stopping the scan");
                    adapter.StopScanningForDevicesAsync();
                }
            }).Start();
        }
    }
}