using InTheHand.Bluetooth;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SureMeasure.BLEDevice
{
    public class SureMeasureDevice : IDistanceMeter
    {
        public event EventHandler<Tuple<double, double>> LenthUpdated;

        public bool IsConnected { get; private set; } = false;

        public bool IsOn
        {
            get => ison;
            set
            {
                ison = value;
                TurnDevice();
                OnPropertyChanged("IsOn");
            }
        }
        private bool ison = false;

        public BluetoothDevice Device
        {
            get => this._device;
            set
            {
                _device = value;
                if (Device != null)
                {
                    this._device.GattServerDisconnected += Device_GattServerDisconnected;
                }
            }
        }
        private BluetoothDevice _device;

        public GattCharacteristic GattCharacteristic
        {
            get => this._gattCharacteristic;
            set
            {
                this._gattCharacteristic = value;
                this._gattCharacteristic.CharacteristicValueChanged += _gattCharacteristic_CharacteristicValueChanged;
                this._gattCharacteristic.StartNotificationsAsync();
            }
        }
        private GattCharacteristic _gattCharacteristic;


        public  SureMeasureDevice(BluetoothDevice bluetoothDevice)
        {
            LoadDevice(bluetoothDevice);
        }

        private async void LoadDevice(BluetoothDevice device)
        {
            await device.Gatt.ConnectAsync();

            if (device.Gatt.IsConnected == true)
            {
                var servs = await device.Gatt.GetPrimaryServicesAsync();

                //Service
                foreach (var serv in servs)
                {
                    if (serv.Uuid.ToString() == "FFB0")
                    {
                        var characteristics = await serv.GetCharacteristicsAsync();

                        //Characteristic
                        foreach (var characteristic in characteristics)
                        {
                            //Subcribe on service
                            if (characteristic.Uuid.ToString() == "FFB4")
                            {
                                if (characteristic.Properties.HasFlag(GattCharacteristicProperties.Notify))
                                {
                                    this.GattCharacteristic = characteristic;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void _gattCharacteristic_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
        {
            if (e.Value != null)
            {
                string value = Encoding.ASCII.GetString(e.Value);

                Regex regex = new Regex(@"[^\d]");

                double lenth = double.Parse(regex.Replace(value, ""));
                LenthUpdated?.Invoke(null, new Tuple<double, double>(lenth, -1));
            }
        }

        public async Task<bool> TurnDevice()
        {
            return false;
        }

        private async void Device_GattServerDisconnected(object sender, EventArgs e)
        {
            var device = sender as BluetoothDevice;
            await device.Gatt.ConnectAsync();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
