using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj.Canvas
{
    public class BackCanvasRectangle : Path
    {
        private double _width = 0;
        private double _height = 0;
        private double _step = 0;

        private static Rect smRect = new Rect(0, 0, 10, 10);
        public static RectangleGeometry smRectGmtr = new RectangleGeometry()
        {
            Rect = smRect
        };

        public BackCanvasRectangle(double Width, double Height, double Step)
        {
            this.BackgroundColor = Color.White;
            this._width = Width;
            this._height = Height;

            this.WidthRequest = this._width;
            this.HeightRequest = this._height;

            Update(Step);
        }

        public void Update(double Step)
        {
            this._step = Step;
            BackCanvasRectangle.smRect = new Rect(0, 0, this._step, this._step);

            for (double i = 0; i < this._width; i += this._step)
            {

            }

            /*for (double i = 0; i < this._height; i += this._step)
            {
                this.Children.Add(new BackLine(new Point(0, i), new Point(this._width, i), (i / this._step) % 10  == 0 ? 2 : 1));
            }*/

            this.Layout(new Xamarin.Forms.Rectangle(0, 0, this._width, this._height ));
        }
    }

    public class BackLine : Path
    {
        private static LineGeometry VerticalLine = new LineGeometry() { StartPoint = new Point(0, 0), EndPoint = new Point(0, CadCanvas.Height) };
        public BackLine(double Thinkess)
        {
            this.Data = BackLine.VerticalLine;
            this.StrokeThickness = Thinkess;
            this.Stroke = Brush.LightBlue;
        }
    }
}
