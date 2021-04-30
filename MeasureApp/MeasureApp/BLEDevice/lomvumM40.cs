using InTheHand.Bluetooth;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace SureMeasure.BLEDevice
{
    public class lomvumM40 : DistanceMeter
    {
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


        public event EventHandler<Tuple<double, double>> LenthUpdated;

        public lomvumM40(BluetoothDevice device)
        {
            LoadDevice(device);
        }

        private GattCharacteristic _gattchart;

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
                                /*
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
                                */
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

        public async void OnDevice()
        {
            byte[] msg = new byte[] { 0x64, 0x74, 0x0d, 0x0a, 0x00 };
            await this.GattCharacteristic.WriteValueWithResponseAsync(msg);
        }

        private async void Device_GattServerDisconnected(object sender, EventArgs e)
        {
            var device = sender as BluetoothDevice;
            await device.Gatt.ConnectAsync();
        }
    }
}
