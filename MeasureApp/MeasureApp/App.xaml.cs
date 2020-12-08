using App1;
using Xamarin.Forms;

namespace MeasureApp
{
    public partial class App : Application
    {
        public App()
        {
            Device.SetFlags(new[] { "Shapes_Experimental", "Brush_Experimental", "DragAndDrop_Experimental", "RadioButton_Experimental" });
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
