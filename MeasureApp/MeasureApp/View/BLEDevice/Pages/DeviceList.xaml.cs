
using App1;
using InTheHand.Bluetooth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp.View.BLEDevice.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceList : ContentPage
    {
        private List<BluetoothDevice> Devices = new List<BluetoothDevice>();

        public DeviceList()
        {
            InitializeComponent();

            //////////////////////////////
            ///
            LoadBleScan();

            ////////////////////////////////

            ScanAllButton.Clicked += (sender, e) => LoadBleScan();

            ScanHrmButton.Clicked += (sender, e) => StartScanning();
        }


        private async void LoadBleScan()
        {
            //listView.ItemsSource = Devices;

            bool availability = false;

            while (!availability)
            {
                availability = await Bluetooth.GetAvailabilityAsync();
                await Task.Delay(500);
            }

            foreach (var d in await Bluetooth.GetPairedDevicesAsync())
            {
                Devices.Add(d);
                Debug.WriteLine($"{d.Id} {d.Name}");
            }

            Bluetooth.AdvertisementReceived += Bluetooth_AdvertisementReceived;
            AppShell.scan = await Bluetooth.RequestLEScanAsync();

            RequestDeviceOptions options = new RequestDeviceOptions();
            options.AcceptAllDevices = true;
            BluetoothDevice device = await Bluetooth.RequestDeviceAsync(options);

            if (device != null)
            {
                LoadDevice(device);
            }
        }

        private async void LoadDevice(BluetoothDevice device)
        {
            await device.Gatt.ConnectAsync();

            var servs = await device.Gatt.GetPrimaryServicesAsync();

            foreach (var serv in servs)
            {
                var rssi = await device.Gatt.ReadRssi();
                Debug.WriteLine($"{rssi} {serv.Uuid} Primary:{serv.IsPrimary}");

                Debug.Indent();

                if (serv.Uuid.ToString() == "FFB0")
                {
                    var characteristics = await serv.GetCharacteristicsAsync();


                    foreach (var characteristic in characteristics)
                    {
                        Debug.WriteLine($"{characteristic.Uuid} UserDescription:{characteristic.UserDescription} Properties:{characteristic.Properties}");

                        Debug.Indent();

                        if (characteristic.Uuid.ToString() == "FFB2")
                        {
                             if (characteristic.Properties.HasFlag(GattCharacteristicProperties.Notify))
                                {
                                //var notifyResult = await characteristic.WriteValueWithoutResponseAsync();
                                AppShell.GattCharacteristic = characteristic;

                               // await characteristic.StartNotificationsAsync();
                               // characteristic.WriteValueWithoutResponseAsync(Encoding.ASCII.GetBytes("1"));
                                }


                                foreach (var descriptors in await characteristic.GetDescriptorsAsync())
                                {
                                    Debug.WriteLine($"Descriptor:{descriptors.Uuid}");

                                    var val2 = await descriptors.ReadValueAsync();

                                    if (descriptors.Uuid == GattDescriptorUuids.ClientCharacteristicConfiguration)
                                    {
                                        Debug.WriteLine($"Notifying:{val2[0] > 0}");
                                    }
                                    else if (descriptors.Uuid == GattDescriptorUuids.CharacteristicUserDescription)
                                    {
                                        Debug.WriteLine($"UserDescription:{ByteArrayToString(val2)}");
                                    }
                                    else
                                    {
                                        Debug.WriteLine(ByteArrayToString(val2));
                                    }

                                }
                            
                        }

                        Debug.Unindent();
                    }
                }
                Debug.Unindent();
            }
        }

        private void Chars_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
        {
            
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


        private void StartScanning()
        {
           
        }

        private void StopScanning()
        {
            
        }
    }
}