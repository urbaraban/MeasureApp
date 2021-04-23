using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using MeasureApp.Orders;
using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.Canvas;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using MeasureApp.View.OrderPage.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp.View.OrderPage
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
        private ContourPath contourPath => (ContourPath)PathPicker.SelectedItem;

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
            this.BindingContext = new Order();

            AppShell.UpdatedOrder += AppShell_UpdatedOrder;
            
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
                    BuildLine(ConvertDimMessage(e.Data.Properties["Message"].ToString()), true);
                }
            }
        }

        private void AppShell_UpdatedOrder(object sender, Order e) => this.BindingContext = e;

        /// <summary>
        /// Make line with anchor on canvas.
        /// </summary>
        /// <param name="tuple">item1 - lenth, item2 - angle</param>
        /// <param name="Forced">Make line from label</param>
        public void BuildLine(Tuple<double, double> tuple, bool Forced = false)
        {
            //Если у нас нет линии привязки
            if ((this.contour.BaseLenthConstrait == null) || (Forced == true))
            {
                CadPoint cadPoint1 = (CadPoint)this.contour.Add(new CadPoint(0, 0, this.contour.GetNewPointName()), 0);
                cadPoint1.IsFix = !Forced;
                CadPoint cadPoint2 = (CadPoint)this.contour.Add(new CadPoint(0, 0 + tuple.Item1, this.contour.GetNewPointName()), 0);
                cadPoint2.IsFix = !Forced;
                this.contour.Add(new ConstraintLenth(cadPoint2, cadPoint1, tuple.Item1), 0);
            }
            else if (this.contour.BasePoint != null)
            {
                ConstraintLenth lastLenthConstr = this.contour.BaseLenthConstrait;

                CadPoint point1 = this.contour.BaseLenthConstrait.GetNotThisPoint(this.contour.BasePoint);
                if (point1 == null) return;

                CadPoint point = Sizing.GetPositionLineFromAngle(point1, this.contour.BasePoint, tuple.Item1, tuple.Item2 < 0 ? 90 : tuple.Item2);
                CadPoint cadPoint2 = new CadPoint(point.X, point.Y, this.contour.GetNewPointName());

                ConstraintLenth lenthConstrait = 
                    (ConstraintLenth)this.contour.Add(
                        new ConstraintLenth(this.contour.BasePoint, cadPoint2, tuple.Item1, 
                        this.contour.Method == DrawMethod.FromPoint && this.contour.BasePoint != this.contour.LastPoint), 0);

                ConstraintAngle constraintAngle = 
                    (ConstraintAngle)this.contour.Add(
                        new ConstraintAngle(lastLenthConstr, lenthConstrait, tuple.Item2), 0);


                CadPoint last = this.contour.LastPoint;
                CadPoint point2 = (CadPoint)this.contour.Add(cadPoint2, 0);

                if (this.contour.Method == DrawMethod.FromPoint && this.contour.BasePoint != this.contour.LastPoint)
                {
                    lenthConstrait.IsSupport = true;
                    this.contour.Add(new ConstraintLenth(last, point2, -1, false), 0);
                    // this.Contour.Add(new ConstraintAngle(this.Contour.LastLenthConstrait, lenthConstrait, Angle), 0);
                }
            }
            else
            {
                PoolDimLabel poolDimLabel = new PoolDimLabel($"{tuple.Item1}&{tuple.Item2}", this.Height);
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
            if (MeasureVariable != null)
            {
                MeasureVariable.Value = MeasureVariable.IsLenth == true ? e.Item1 : e.Item2;
                MeasureVariable = null;
            }
            else
            {
                BuildLine(e);
            }
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

            BuildLine(ConvertDimMessage(result));
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
                this.contour.Method = sw.IsToggled == true ? DrawMethod.FromPoint : DrawMethod.StepByStep;
            }
            
        }

        private async void ShareBtn_Clicked(object sender, EventArgs e)
        {
            List<string> paths = new List<string>();

            foreach (Contour contour in this.order.Contours) 
            {
                DxfFile dxfFile = new DxfFile();
                dxfFile.Header.SetDefaults();
                dxfFile.Header.AlternateDimensioningScaleFactor = 1;
                dxfFile.Header.DrawingUnits = DxfDrawingUnits.Metric;
                dxfFile.Header.UnitFormat = DxfUnitFormat.Decimal;
                dxfFile.Header.DefaultDrawingUnits = DxfUnits.Millimeters;
                dxfFile.Header.AlternateDimensioningUnits = DxfUnitFormat.Decimal;
                dxfFile.ViewPorts.Clear();
                foreach (ConstraintLenth constraintLenth in contour.Lenths)
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
            foreach(string str in paths)
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