using App1;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeasureApp
{
    public partial class App : Application
    {
        public static IAdapter Adapter;
        public App()
        {
            Device.SetFlags(new[] { "Shapes_Experimental", "Brush_Experimental", "DragAndDrop_Experimental" });
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
