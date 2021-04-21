using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj.Canvas
{
    public class DynamicBackground : AbsoluteLayout
    {
        private AbsoluteLayout _parentlayout;
        private AbsoluteLayout _masklayout;

        public DynamicBackground(AbsoluteLayout Parent, AbsoluteLayout Mask)
        {
            this._parentlayout = Parent;
            this._masklayout = Mask;
        }

        private void CadCanvas_Moved(object sender, System.EventArgs e)
        {
            Update();
        }

        public void Update()
        {/*
            for (int i = 0; i < this.Children.Count; i += 1)
            {
                if (this.Children[i] is Line line)
                {
                    if ((line.X1 > (-this._parentlayout.TranslationX + this._masklayout.Width / this._parentlayout.Scale) &&
                        line.X2 > (-this._parentlayout.TranslationX + this._masklayout.Width / this._parentlayout.Scale)) ||
                        (line.X1 < (-this._parentlayout.TranslationX) &&
                        line.X2 < (-this._parentlayout.TranslationX)))
                    {
                        this.Children.Remove(line);
                        i -= 1;
                    }
                }
            }

            if (this.Children.Count < 1)
            {
                double x = 0, y = 0;
                while (x <= this._masklayout.Width / this._parentlayout.Scale)
                {
                    this.Children.Add(new Line()
                    {
                        X1 = ((int)(-this._parentlayout.TranslationX + x) / 20) * 20,
                        X2 = ((int)(-this._parentlayout.TranslationX + x) / 20) * 20,
                        Y1 = -this._parentlayout.TranslationY,
                        Y2 = -this._parentlayout.TranslationY + this._masklayout.Height / this._parentlayout.Scale,
                        Stroke = Brush.Black,
                        StrokeThickness = 1
                    });
                    x += 20;
                }

                while (y <= this._masklayout.Height / this._parentlayout.Scale)
                {
                    this.Children.Add(new Line()
                    {
                        Y1 = ((int)(-this._parentlayout.TranslationY + y) / 20) * 20,
                        Y2 = ((int)(-this._parentlayout.TranslationY + y) / 20) * 20,
                        X1 = -this._parentlayout.TranslationX,
                        X2 = -this._parentlayout.TranslationX + this._masklayout.Width / this._parentlayout.Scale,
                        Stroke = Brush.Black,
                        StrokeThickness = 1
                    });
                    y += 20;
                }
            }
            */
        }
    }
}
