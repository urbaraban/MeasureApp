using InTheHand.Bluetooth;
using SureMeasure.ShapeObj;
using SureMeasure.BLEDevice;
using SureMeasure.Views.OrderPage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.IO;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using SureMeasure.Orders;
using SureMeasure.Data;
using DrawEngine;
using DrawEngine.Constraints;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SureMeasure
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void NotifyStaticPropertyChanged([CallerMemberName] string name = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }

        public static Order SelectOrder
        {
            get => selectorder;
            set
            {
                selectorder = value;
                NotifyStaticPropertyChanged("SelectOrder");
            }
        }
        private static Order selectorder;

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

        public static AppShell Instance { get; set; }

        public static DistanceMeter BLEDevice
        {
            get => AppShell._bledevice;
            set
            {
                if (_bledevice != null)
                {
                    _bledevice.LenthUpdated -= Bledevice_LenthUpdated;
                }
                _bledevice = value;
                if (_bledevice != null)
                {
                    AppShell._bledevice.LenthUpdated += Bledevice_LenthUpdated; ;
                }
            }
        }
        private static void Bledevice_LenthUpdated(object sender, Tuple<double, double> e) => LenthUpdated?.Invoke(null, e);

        private static DistanceMeter _bledevice;

        private readonly List<BluetoothDevice> Devices = new List<BluetoothDevice>();

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AdressListPage), typeof(AdressListPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            AppShell.Instance = this;
            AppShell.SelectOrder = new Order();
        }


        //Call Dialog
        public async Task<string> SheetMenuDialog(SheetMenu sheetMenu, string Head)
        {
            string Result = await DisplayActionSheet(Head, "Cancel", null, sheetMenu.ToArray());
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
            RequestDeviceOptions options = new RequestDeviceOptions
            {
                AcceptAllDevices = true
            };
            BluetoothDevice device = await Bluetooth.RequestDeviceAsync(options);

            if (device != null)
            {
                switch (device.Name)
                {
                    case "Laser Distance Meter":
                        BLEDevice = new lomvumM40(device);
                        break;
                    case "Sure Measure Device":
                        BLEDevice = new SureMeasureDevice(device);
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
            await LoadBleScan();
        }

        public async static Task<string> TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = $"SureMeasure.{DateTime.Now:dd.MM.yyyy_hh.mm.ss}.png"
                });

                // для примера сохраняем файл в локальном хранилище
                var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);

                return photo.FullPath;
                // загружаем в ImageView
               // img.Source = ImageSource.FromFile(photo.FullPath);
            }
            catch (Exception ex)
            {
                await AppShell.Instance.AlertDialog("Сообщение об ошибке", ex.Message);
            }
            return string.Empty;
        }

        public async static Task<string> SelectPhoto()
        {
            try
            {
                // выбираем фото
                var photo = await MediaPicker.PickPhotoAsync();
                return photo.FullPath;
            }
            catch (Exception ex)
            {
                await AppShell.Instance.AlertDialog("Сообщение об ошибке", ex.Message);
            }

            return string.Empty;
        }

        public async static void ShareOrder(Order order)
        {
            List<string> paths = new List<string>();

            foreach (Contour contour in order.Contours)
            {
                DxfFile dxfFile = new DxfFile();
                dxfFile.Header.SetDefaults();
                dxfFile.Header.AlternateDimensioningScaleFactor = 1;
                dxfFile.Header.DrawingUnits = DxfDrawingUnits.Metric;
                dxfFile.Header.UnitFormat = DxfUnitFormat.Decimal;
                dxfFile.Header.DefaultDrawingUnits = DxfUnits.Millimeters;
                dxfFile.Header.AlternateDimensioningUnits = DxfUnitFormat.Decimal;
                dxfFile.ViewPorts.Clear();
                foreach (ConstraintLenth constraintLenth in contour.GetLenths(false))
                {
                    if (constraintLenth.IsSupport == false)
                    {
                        dxfFile.Entities.Add(new DxfLine(
                            new DxfPoint(constraintLenth.Point1.X, constraintLenth.Point1.Y, 0),
                            new DxfPoint(constraintLenth.Point2.X, constraintLenth.Point2.Y, 0)));
                    }
                }
                string path = Path.ChangeExtension(Path.Combine(FileSystem.CacheDirectory, Path.GetTempFileName()), ".dxf");
                dxfFile.Save(path);
                paths.Add(path);
            }

            List<ShareFile> shareFiles = new List<ShareFile>();
            foreach (string str in paths)
            {
                shareFiles.Add(new ShareFile(str));
            }

            await Share.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "Отправить чертеж",
                Files = shareFiles
            });

        }


    }
}
