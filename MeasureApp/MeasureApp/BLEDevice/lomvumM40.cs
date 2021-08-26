using InTheHand.Bluetooth;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SureMeasure.BLEDevice
{
    public class lomvumM40 : IDistanceMeter
    {
        public byte[] OnMsg { get; } = new byte[] { 0x64, 0x74, 0x0d, 0x0a, 0x00 };

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
                if (_device != null) this._device.GattServerDisconnected -= Device_GattServerDisconnected;
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
                if (this._gattCharacteristic != null)
                {
                    this._gattCharacteristic.CharacteristicValueChanged -= _gattCharacteristic_CharacteristicValueChanged;
                    this._gattCharacteristic.StopNotificationsAsync();
                }
                this._gattCharacteristic = value;
                this._gattCharacteristic.CharacteristicValueChanged += _gattCharacteristic_CharacteristicValueChanged;
                this._gattCharacteristic.StartNotificationsAsync();
            }
        }
        private GattCharacteristic _gattCharacteristic;

        public lomvumM40(BluetoothDevice device)
        {
            LoadDevice(device);
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
                            if (characteristic.Uuid.ToString() == "FFB2")
                            {
                                if (characteristic.Properties.HasFlag(GattCharacteristicProperties.Notify))
                                {
                                    this.GattCharacteristic = characteristic;
                                }
                            }

                            Debug.Unindent();
                        }
                    }
                }
            }
        }

        private void _gattCharacteristic_CharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
        {
            if (e.Value != null)
            {
                ison = Equality(this.GattCharacteristic.Value, OnMsg);
                OnPropertyChanged("IsOn");

                string value = Encoding.ASCII.GetString(e.Value);

                Regex regex = new Regex(@"[^\d]");

                double lenth = double.Parse(regex.Replace(value, ""));
                LenthUpdated?.Invoke(null, new Tuple<double, double>(lenth, -1));
            }
        }

        private object ByteArrayToString(byte[] val2)
        {
            throw new NotImplementedException();
        }

        private async void TurnDevice()
        {
            await this.GattCharacteristic.WriteValueWithResponseAsync(OnMsg);
            ison = Equality(this.GattCharacteristic.Value, OnMsg);
            OnPropertyChanged("IsOn");
        }

        private bool Equality(byte[] a1, byte[] b1)
        {
            int i;
            if (a1.Length == b1.Length)
            {
                i = 0;
                while (i < a1.Length && (a1[i] == b1[i])) //Earlier it was a1[i]!=b1[i]
                {
                    i++;
                }
                if (i == a1.Length)
                {
                    return true;
                }
            }

            return false;
        }

        private async void Device_GattServerDisconnected(object sender, EventArgs e)
        {
            IsConnected = false;
            var device = sender as BluetoothDevice;
            await device.Gatt.ConnectAsync();
            IsConnected = device.Gatt.IsConnected;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
