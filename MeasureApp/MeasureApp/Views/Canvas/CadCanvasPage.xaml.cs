using DrawEngine;
using DrawEngine.Constraints;
using Plugin.Segmented.Control;
using SureMeasure.Common;
using SureMeasure.Orders;
using SureMeasure.Tools;
using SureMeasure.Views.OrderPage.Canvas;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.Views.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CadCanvasPage : ContentPage
    {
        public static CadVariable MeasureVariable;

        private Order Order => (Order)this.BindingContext;
        private Contour Contour => AppShell.SelectOrder.SelectContour;

        public CadCanvasPage()
        {
            InitializeComponent();

            AppShell.LenthUpdated += AppShell_LenthUpdated;

            DrawMethodSelecter.Children.Add(new SegmentedControlOption() { Text = IconFont.StepByStep, Item = DrawMethod.StepByStep });
            DrawMethodSelecter.Children.Add(new SegmentedControlOption() { Text = IconFont.FromPoint, Item = DrawMethod.FromPoint });
            DrawMethodSelecter.Children.Add(new SegmentedControlOption() { Text = IconFont.Touch, Item = DrawMethod.Manual });

            if (this.MainCanvas.BindingContext != null)
            {
                Debug.WriteLine(this.MainCanvas.BindingContext.ToString());
            }          
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.BindingContext = AppShell.SelectOrder;
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            if (AppShell.SelectOrder.IsAlive == true)
            {
                await AppShell.OrdersDB.SaveItemAsync(this.Order);
            }
        }


        private void PoolDimLabel_Removed(object sender, EventArgs e)
        {
            //SizePool.Children.Remove((ContentView)sender);
        }

        private async void AppShell_LenthUpdated(object sender, Tuple<double, double> e)
        {
            if (MeasureVariable != null)
            {
                MeasureVariable.Value = MeasureVariable.IsLenth == true ? e.Item1 : e.Item2;
                MeasureVariable = null;
            }
            else
            {
                if (await Contour.BuildLine(e) == false) AddToSizePool(e);
            }
        }

        private void AddToSizePool(Tuple<double, double> tuple)
        {
            PoolDimLabel poolDimLabel = new PoolDimLabel($"{tuple.Item1}&{tuple.Item2}", this.Height);
            poolDimLabel.Removed += PoolDimLabel_Removed;
           // SizePool.Children.Add(poolDimLabel);
        }


        public ICommand AddLine => new Command(async ()=> {
            Random random = new Random();

            string result = await DisplayPromptAsync("Добавить линию", "Мне нужны твоя длинна и угол", "Add", "Cancel", "0000&00", -1, Keyboard.Numeric, $"{random.Next(100, 200)}&{random.Next(45, 270)}");
            if (string.IsNullOrEmpty(result) == false)
            {
                await Contour.BuildLine(Converters.ConvertDimMessage(result));
            }
        });


        private void ShareBtn_Clicked(object sender, EventArgs e)
        {
            AppShell.ShareOrder(this.Order);
        }


        private void LaserCutBtn_Clicked(object sender, EventArgs e)
        {
            UdpClient udpClient = new UdpClient(11000);
            try
            {
                udpClient.Connect(new IPAddress(new byte[] { 192, 168, 33, 10 }), 11000);

                // Sends a message to the host to which you have connected.
                //Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");

                Byte[] sendBytes = this.Contour.GetBytes();

                udpClient.Send(sendBytes, sendBytes.Length);

                // Sends a message to a different host using optional hostname and port parameters.
                UdpClient udpClientB = new UdpClient();
                udpClientB.Send(sendBytes, sendBytes.Length, "AlternateHostMachineName", 11000);

                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Console.WriteLine("This is the message you received " +
                                             returnData.ToString());
                Console.WriteLine("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());

                udpClient.Close();
                udpClientB.Close();
            }
            catch (Exception er)
            {
                Console.WriteLine(er.ToString());
            }
        }

        public ICommand FitPage => new Command( async ()=> {
            await MainCanvas.FitChild();
        });

        public ICommand ZoomIn => new Command(async () => {
            MainCanvas.Zoom(0.2);
        });

        public ICommand ZoomOut => new Command(async () => {
            MainCanvas.Zoom(-0.2);
        });

        public ICommand GetDevice => new Command(() => {
            if (AppShell.BLEDevice != null)
            {
                AppShell.BLEDevice.OnDevice();
            }
        });
    }
}