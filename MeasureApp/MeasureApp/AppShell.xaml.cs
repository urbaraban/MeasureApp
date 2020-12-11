using InTheHand.Bluetooth;
using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.LabelObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public static event EventHandler UpdatedDevice;
        public static event EventHandler<Tuple<double, double>> LenthUpdated;

        public static AppShell Instance;
        public static BluetoothLEScan scan;

        public static CadVariable MeasireVariable = null;

        private static BluetoothDevice _device;

        public static BluetoothDevice Device
        {
            get => AppShell._device;
            set
            {
                _device = value;
                if (Device != null)
                {
                    AppShell._device.GattServerDisconnected += Device_GattServerDisconnected;
                }
            }
        }

        private static GattCharacteristic _gattCharacteristic;

        public static GattCharacteristic GattCharacteristic
        {
            get => AppShell._gattCharacteristic;
            set
            {
                AppShell._gattCharacteristic = value;
                AppShell._gattCharacteristic.CharacteristicValueChanged += _gattCharacteristic_CharacteristicValueChanged;
                AppShell.GattCharacteristic.StartNotificationsAsync();
            }
        }

        private static void _gattCharacteristic_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
        {
            if (e.Value != null)
            {
                string value = Encoding.ASCII.GetString(e.Value);

                Regex regex = new Regex(@"[^\d]");

                double lenth = double.Parse(regex.Replace(value, ""));

                if (MeasireVariable != null)
                {
                    MeasireVariable.Value = lenth;
                    MeasireVariable = null;
                }
                else
                {
                    LenthUpdated?.Invoke(null, new Tuple<double, double>(lenth, -1));
                }
            }
        }

        private static async void Device_GattServerDisconnected(object sender, EventArgs e)
        {
            var device = sender as BluetoothDevice;
            await device.Gatt.ConnectAsync();
        }


        private List<BluetoothDevice> Devices = new List<BluetoothDevice>();

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MeasureApp.View.OrderPage.AdressListPage), typeof(MeasureApp.View.OrderPage.AdressListPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            AppShell.Instance = this;


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

        public async Task<string> DisplayPromtDialog(string Name, string Value)
        {
            string result = await DisplayPromptAsync("Изменить значение", Name, "Add", "Cancel", "0000", -1, Keyboard.Numeric, Value);
            return result;
        }

        private async Task LoadBleScan()
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

        private void Bluetooth_AdvertisementReceived(object sender, BluetoothAdvertisingEvent e)
        {
            Devices.Add(e.Device);
            Debug.WriteLine($"Name:{e.Name} Rssi:{e.Rssi}");
        }


        private async void LoadDevice(BluetoothDevice device)
        {
            await device.Gatt.ConnectAsync();

            if (device.Gatt.IsConnected == true)
            {
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

        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
          LoadBleScan();
        }
    }
}
