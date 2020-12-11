using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.Constraints;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.View.OrderPage
{
    class Contour : INotifyPropertyChanged
    {
        /// <summary>
        /// Contour sqare
        /// </summary>
        public double Sqare
        {
            get => this._sqare;
        }
        private double _sqare;

        /// <summary>
        /// Gabarit Sqare
        /// </summary>
        public double GabaritSqare
        {
            get => this._gabaritsqare;
        }
        private double _gabaritsqare;

        /// <summary>
        /// Perimetr controur
        /// </summary>
        public double Perimetr => CalcPerimetr();

        private double CalcPerimetr()
        {
            double lenth = 0;

            foreach (LenthConstrait lenthConstrait in Lenths)
            {
                if (lenthConstrait.IsSupport == false)
                {
                    lenth += lenthConstrait.Lenth;
                }
            }
            return lenth;
        }

        public List<CadAnchor> Anchors;

        public List<LenthConstrait> Lenths;

        public List<AngleConstrait> Angles;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
