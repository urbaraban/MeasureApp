using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using SureMeasure.CadObjects;
using SureMeasure.Orders;
using SureMeasure.ShapeObj.Canvas;
using SureMeasure.ShapeObj.Constraints;
using SureMeasure.View.OrderPage.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SureMeasure.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CadCanvasPage : ContentPage
    {
        public static CadVariable MeasureVariable;

        public ICommand AddContour => new Command(async () =>
        {
            this.order.Contours.Add(new Contour("Test"));
        });

        private Order order => (Order)this.BindingContext;
        private Contour contour => (Contour)ContourPicker.SelectedItem;

        public CadCanvasPage()
        {
            InitializeComponent();
            AddBtn.Clicked += AddBtn_Clicked;
            ClearBtn.Clicked += ClearBtn_Clicked;
            FitBtn.Clicked += FitBtn_Clicked;
            GetBtn.Clicked += GetBtn_Clicked;

            this.MainCanvas.Droped += MainCanvas_Droped;
            this.BindingContextChanged += CadCanvasPage_BindingContextChanged;
            ContourPicker.SelectedIndexChanged += ContourPicker_SelectedIndexChanged;
            AppShell.LenthUpdated += AppShell_LenthUpdated;

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
            AppShell.BLEDevice.OnDevice();
        }

        private void MainCanvas_Droped(object sender, DropEventArgs e)
        {
            if (sender is CadCanvas)
            {
                if (e.Data.Properties["Message"] != null)
                {
                   contour.BuildLine(ConvertDimMessage(e.Data.Properties["Message"].ToString()), true);
                }
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
                    order.Contours.Add(new Contour("Test"));
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
                PathPicker.ItemsSource = contour.Paths;
                PathPicker.SelectedItem = contour.Paths[0];
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

            string result = await DisplayPromptAsync("Добавить линию", "Мне нужны твоя длинна и угол", "Add", "Cancel", "0000&00", -1, Keyboard.Numeric, $"{random.Next(250, 1000)}&{random.Next(45, 270)}");
            if (string.IsNullOrEmpty(result) == false)
            {
                contour.BuildLine(ConvertDimMessage(result));
            }
        }

        private Tuple<double, double> ConvertDimMessage(string message)
        {
            double LineLenth = -1;
            double LineAngle = -1;

            if (string.IsNullOrEmpty(message) == false)
            {
                string[] strings = message.Split('&');
                LineLenth = double.Parse(strings[0]);
                LineAngle = -1;
                if (strings.Length > 1)
                {
                    LineAngle = double.Parse(strings[1]);
                }
            }
            return new Tuple<double, double>(LineLenth, LineAngle);
        }

        private void ContourAddBtn_Clicked(object sender, EventArgs e)
        {
            Contour tempContor = new Contour("Test");
            this.order.Contours.Add(tempContor);
            ContourPicker.ItemsSource = null;
            ContourPicker.ItemsSource = this.order.Contours;
            ContourPicker.SelectedItem = tempContor;
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (this.contour != null && sender is Xamarin.Forms.Switch sw)
            {
                this.contour.DrawMethod = sw.IsToggled == true ? DrawMethod.FromPoint : DrawMethod.StepByStep;
            }
        }

        private async void ShareBtn_Clicked(object sender, EventArgs e)
        {
            AppShell.ShareOrder(this.order);
        }
    }
}