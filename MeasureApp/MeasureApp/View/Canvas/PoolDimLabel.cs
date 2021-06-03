using SureMeasure.ShapeObj;
using SureMeasure.View.Canvas;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace SureMeasure.View.OrderPage.Canvas
{
    public class PoolDimLabel : ContentView
    {
        public event EventHandler Removed;

        private DragGestureRecognizer dragGesture = new DragGestureRecognizer();
        private TapGestureRecognizer tapGesture = new TapGestureRecognizer();
        private string message;

        public PoolDimLabel(string Message, double height)
        {
            this.message = Message;
            double koef = 0.04;
            Frame MainFrame = new Frame()
            {
                BackgroundColor = Color.LightSkyBlue,
                CornerRadius = (float)(height * koef / 2),
                HeightRequest = height * koef,
                Padding = new Thickness(height * koef / 10)
            };
            StackLayout MainStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };
            Label label = new Label()
            {
                Text = Message,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.White,
                BackgroundColor = Color.Transparent
            };
            Button button = new Button()
            {
                Text = "X",
                HeightRequest = height * koef / 2,
                WidthRequest = height * koef / 2,
                Padding = new Thickness(0),
                Command = Remove
            };

            MainStack.Children.Add(label);
            MainStack.Children.Add(button);

            MainFrame.Content = MainStack;
            this.Content = MainFrame;

            dragGesture.CanDrag = true;
            dragGesture.DragStarting += DragGesture_DragStarting;
            dragGesture.DropCompleted += DragGesture_DropCompleted;
            tapGesture.Tapped += TapGesture_Tapped;
            this.GestureRecognizers.Add(tapGesture);
            this.GestureRecognizers.Add(dragGesture);
        }

        private void DragGesture_DropCompleted(object sender, DropCompletedEventArgs e)
        {
            CanvasView.CallRegularSize(1);
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            Debug.WriteLine("Tapped");
        }

        private void DragGesture_DragStarting(object sender, DragStartingEventArgs e)
        {
            e.Data.Properties.Add("Message", this.message);
            CanvasView.CallDragSize(1);
        }

        private ICommand Remove => new Command(() =>
        {
            Removed?.Invoke(this, null);
        });
    }
}
