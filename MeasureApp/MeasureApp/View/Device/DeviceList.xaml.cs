using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BLE.Abstractions.EventArgs;

namespace MeasureApp.View.Device
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceList : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;
        ObservableCollection<IDevice> deviceList;
        IDevice device;

        public DeviceList()
        {
            InitializeComponent();

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();
            ListViewer.ItemsSource = deviceList;
        }

        /// <summary>
        /// Define the status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStatus_Clicked(object sender, EventArgs e)
        {
            var state = ble.State;

            DisplayAlert("Notice", state.ToString(), "OK !");
            if (state == BluetoothState.Off)
            {
                txtErrorBle.BackgroundColor = Color.Red;
                txtErrorBle.TextColor = Color.White;
                txtErrorBle.Text = "Your Bluetooth is off ! Turn it on !";
            }
        }

        /// <summary>
        /// Scan the list of Devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnScan_Clicked(object sender, EventArgs e)
        {
            try
            {
                deviceList.Clear();
                adapter.DeviceDiscovered += (s, a) =>
                {
                    deviceList.Add(a.Device);
                };

                //We have to test if the device is scanning 
                if (!ble.Adapter.IsScanning)
                {
                    await adapter.StartScanningForDevicesAsync();

                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Notice", ex.Message.ToString(), "Error !");
            }

        }

        /// <summary>
        /// Connect to a specific device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnConnect_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (device != null)
                {
                    await adapter.ConnectToDeviceAsync(device);

                }
                else
                {
                    DisplayAlert("Notice", "No Device selected !", "OK");
                }
            }
            catch (DeviceConnectionException ex)
            {
                //Could not connect to the device
                DisplayAlert("Notice", ex.Message.ToString(), "OK");
            }
        }



        private async void btnKnowConnect_Clicked(object sender, EventArgs e)
        {

            try
            {
                await adapter.ConnectToKnownDeviceAsync(new Guid("guid"));

            }
            catch (DeviceConnectionException ex)
            {
                //Could not connect to the device
                DisplayAlert("Notice", ex.Message.ToString(), "OK");
            }
        }

    }
}