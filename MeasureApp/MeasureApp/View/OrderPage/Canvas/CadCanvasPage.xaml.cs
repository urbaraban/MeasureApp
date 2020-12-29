using MeasureApp.Orders;
using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using MeasureApp.View.OrderPage.Canvas;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp.View.OrderPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CadCanvasPage : ContentPage
    {
        public ICommand AddContour => new Command(async () =>
        {
            this.order.Contours.Add(new Contour("Test"));
        });

        private Order order => (Order)this.BindingContext;
        private Contour contour => (Contour)ContourPicker.SelectedItem;
        private ContourPath contourPath => (ContourPath)PathPicker.SelectedItem;

        public CadCanvasPage()
        {
            
            InitializeComponent();
            AddBtn.Clicked += AddBtn_Clicked;
            ClearBtn.Clicked += ClearBtn_Clicked;
            FitBtn.Clicked += FitBtn_Clicked;

            this.BindingContextChanged += CadCanvasPage_BindingContextChanged;
            ContourPicker.SelectedIndexChanged += ContourPicker_SelectedIndexChanged;
            AppShell.LenthUpdated += AppShell_LenthUpdated;
            
            if (this.MainCanvas.BindingContext != null)
                Debug.WriteLine(this.MainCanvas.BindingContext.ToString());
            this.BindingContext = new Order();

            AppShell.UpdatedOrder += AppShell_UpdatedOrder;
            
        }

        private void AppShell_UpdatedOrder(object sender, Order e)
        {
            this.BindingContext = e;
        }

        public void BuildLine(double Lenth, double Angle)
        {
            //Если у нас нет линии привязки
            if (this.contour.BaseLenthConstrait == null)
            {
                CadPoint cadPoint1 = (CadPoint)this.contour.Add(new CadPoint(0, 0, this.contour.GetNewPointName()), 0);
                cadPoint1.IsFix = true;
                CadPoint cadPoint2 = (CadPoint)this.contour.Add(new CadPoint(0, 0 + Lenth, this.contour.GetNewPointName()), 0);
                cadPoint2.IsFix = true;
                this.contour.Add(new ConstraintLenth(cadPoint1, cadPoint2, Lenth), 0);
            }
            else if (this.contour.BasePoint != null)
            {
                CadPoint point1 = this.contour.BaseLenthConstrait.GetNotThisPoint(this.contour.BasePoint);
                if (point1 == null) return;

                Point point = Sizing.GetPositionLineFromAngle(point1, this.contour.BasePoint, Lenth, Angle < 0 ? 90 : Angle);
                CadPoint cadPoint2 = new CadPoint(point.X, point.Y, this.contour.GetNewPointName());
                ConstraintLenth lenthConstrait = (ConstraintLenth)this.contour.Add(new ConstraintLenth(this.contour.BasePoint, cadPoint2, Lenth), 0);

                CadPoint last = this.contour.LastPoint;
                CadPoint point2 = (CadPoint)this.contour.Add(cadPoint2, 0);

                if (this.contour.Method == DrawMethod.FromPoint && this.contour.BasePoint != this.contour.LastPoint)
                {
                    lenthConstrait.IsSupport = true;
                    this.contour.Add(new ConstraintLenth(last, point2, -1, true), 0);
                    // this.Contour.Add(new ConstraintAngle(this.Contour.LastLenthConstrait, lenthConstrait, Angle), 0);
                }
            }
            else
            {
                PoolDimLabel poolDimLabel = new PoolDimLabel($"{Lenth}&{Angle}", this.Height);
                poolDimLabel.Removed += PoolDimLabel_Removed;
                SizePool.Children.Add(poolDimLabel);
            }

        }

        private void PoolDimLabel_Removed(object sender, EventArgs e)
        {
            SizePool.Children.Remove((ContentView)sender);
        }

        private void AppShell_LenthUpdated(object sender, Tuple<double, double> e)
        {
            BuildLine(e.Item1, e.Item2);
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
                string[] strings = result.Split('&');

                double LineLenth = double.Parse(strings[0]);
                double LineAngle = -1;
                if (strings.Length > 1)
                {
                    LineAngle = double.Parse(strings[1]);
                }

                BuildLine(LineLenth, LineAngle);
            }

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
                this.contour.Method = sw.IsToggled == true ? DrawMethod.FromPoint : DrawMethod.StepByStep;
            }
            
        }
    }
}