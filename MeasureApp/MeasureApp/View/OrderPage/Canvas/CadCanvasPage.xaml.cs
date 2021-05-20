using Plugin.Segmented.Control;
using SureMeasure.CadObjects;
using SureMeasure.Orders;
using SureMeasure.ShapeObj.Canvas;
using SureMeasure.Tools;
using SureMeasure.View.OrderPage.Canvas;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static SureMeasure.Orders.Contour;

namespace SureMeasure.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CadCanvasPage : ContentPage
    {
        public static CadVariable MeasureVariable;

        public Command AddContour => new Command(() =>
        {
            Contour tempContor = new Contour($"Contour {this.order.Contours.Count + 1}");
            this.order.Contours.Add(tempContor);
            ContourPicker.Dispatcher.BeginInvokeOnMainThread(() =>
            {
                ContourPicker.ItemsSource = null;
                ContourPicker.ItemsSource = this.order.Contours;
                ContourPicker.SelectedItem = tempContor;
            });
        });


        private Order order => (Order)this.BindingContext;
        private Contour contour => (Contour)ContourPicker.SelectedItem;

        public CadCanvasPage()
        {
            InitializeComponent();
            AddBtn.Clicked += AddBtn_Clicked;
            FitBtn.Clicked += FitBtn_Clicked;
            GetBtn.Clicked += GetBtn_Clicked;

            this.BindingContextChanged += CadCanvasPage_BindingContextChanged;
            ContourPicker.SelectedIndexChanged += ContourPicker_SelectedIndexChanged;
            AppShell.LenthUpdated += AppShell_LenthUpdated;

            DrawMethodSelecter.Children.Add(new SegmentedControlOption() { Text = "Step By Step", Item = DrawMethod.StepByStep });
            DrawMethodSelecter.Children.Add(new SegmentedControlOption() { Text = "From Point", Item = DrawMethod.FromPoint });

            if (this.MainCanvas.BindingContext != null)
            {
                Debug.WriteLine(this.MainCanvas.BindingContext.ToString());
            }
            this.BindingContext = AppShell.SelectOrder;

            AppShell.UpdatedOrder += AppShell_UpdatedOrder;
            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.MainCanvas.VisualClear();
            this.MainCanvas.DrawContour(this.contour);
            this.BindingContext = AppShell.SelectOrder;

        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            if (AppShell.SelectOrder.IsAlive == true)
            {
                await AppShell.OrdersDB.SaveItemAsync(AppShell.SelectOrder);
            }
        }

        private void GetBtn_Clicked(object sender, EventArgs e)
        {
            if (AppShell.BLEDevice != null)
            {
                AppShell.BLEDevice.OnDevice();
            }
        }


        private void AppShell_UpdatedOrder(object sender, Order e) => this.BindingContext = e;



        private void PoolDimLabel_Removed(object sender, EventArgs e)
        {
            SizePool.Children.Remove((ContentView)sender);
        }

        private void AppShell_LenthUpdated(object sender, Tuple<double, double> e)
        {
            if (MeasureVariable != null)
            {
                MeasureVariable.Value = MeasureVariable.IsLenth == true ? e.Item1 : e.Item2;
                MeasureVariable = null;
            }
            else
            {
                if (contour.BuildLine(e) == false) AddToSizePool(e);
            }
        }

        private void AddToSizePool(Tuple<double, double> tuple)
        {
            PoolDimLabel poolDimLabel = new PoolDimLabel($"{tuple.Item1}&{tuple.Item2}", this.Height);
            poolDimLabel.Removed += PoolDimLabel_Removed;
            SizePool.Children.Add(poolDimLabel);
        }


        private void CadCanvasPage_BindingContextChanged(object sender, EventArgs e)
        {
            if (this.BindingContext is Order order)
            {
                if (order.Contours.Count < 1)
                {
                    order.Contours.Add(new Contour($"Contour {this.order.Contours.Count + 1}"));
                }
                ContourPicker.ItemsSource = order.Contours;
                ContourPicker.SelectedItem = order.Contours[0];

            }
        }

        private void ContourPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ContourPicker.SelectedItem is Contour contour)
            {
                if (contour.Paths.Count < 1)
                {
                    contour.Paths.Add(new ContourPath(contour.Paths.Count.ToString()));
                }
                MainCanvas.BindingContext = contour;
            }
        }

        private void FitBtn_Clicked(object sender, EventArgs e)
        {
            MainCanvas.FitChild();
        }

        private void ClearBtn_Clicked(object sender, EventArgs e)
        {
            MainCanvas.Clear();
        }

        private async void AddBtn_Clicked(object sender, EventArgs e)
        {
            Random random = new Random();

            string result = await DisplayPromptAsync("Добавить линию", "Мне нужны твоя длинна и угол", "Add", "Cancel", "0000&00", -1, Keyboard.Numeric, $"{random.Next(100, 200)}&{random.Next(45, 270)}");
            if (string.IsNullOrEmpty(result) == false)
            {
                contour.BuildLine(Converters.ConvertDimMessage(result));
            }
        }

        

        private void ContourAddBtn_Clicked(object sender, EventArgs e)
        {
            Contour tempContor = new Contour($"Contour {this.order.Contours.Count + 1}");
            this.order.Contours.Add(tempContor);
            ContourPicker.ItemsSource = null;
            ContourPicker.ItemsSource = this.order.Contours;
            ContourPicker.SelectedItem = tempContor;
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (this.contour != null && sender is Xamarin.Forms.Switch sw)
            {
                this.contour.SelectedDrawMethod = sw.IsToggled == true ? DrawMethod.FromPoint : DrawMethod.StepByStep;
            }
        }

        private async void ShareBtn_Clicked(object sender, EventArgs e)
        {
            AppShell.ShareOrder(this.order);
        }

        private void DrawMethodSelecter_OnSegmentSelected(object sender, Plugin.Segmented.Event.SegmentSelectEventArgs e)
        {
            if (DrawMethodSelecter.Children[DrawMethodSelecter.SelectedSegment] is SegmentedControlOption controlOption)
                this.contour.SelectedDrawMethod = (DrawMethod)controlOption.Item;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (this.order.Contours.Count > 1)
            {
                Contour temp = (Contour)ContourPicker.SelectedItem;
                this.order.Contours.Remove(temp);

                ContourPicker.ItemsSource = null;
                ContourPicker.ItemsSource = this.order.Contours;
                ContourPicker.SelectedItem = this.order.Contours[0];
            }
            else if (this.order.Contours.Count == 1)
            {
                this.order.Contours[0].Clear();
            }
            this.MainCanvas.DrawContour(this.contour);
        }

        private void LaserCutBtn_Clicked(object sender, EventArgs e)
        {
            UdpClient udpClient = new UdpClient(11000);
            try
            {
                udpClient.Connect(new IPAddress(new byte[] { 192, 168, 33, 10 }), 11000);

                // Sends a message to the host to which you have connected.
                //Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");

                Byte[] sendBytes = this.contour.GetBytes();

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
    }
}