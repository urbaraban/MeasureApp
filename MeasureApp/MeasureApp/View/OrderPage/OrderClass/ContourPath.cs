using MeasureApp.ShapeObj;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace MeasureApp.View.OrderPage.OrderClass
{
    public class ContourPath : INotifyPropertyChanged
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

        public bool IsClosed
        {
            get => this._isclosed;
            set
            {
                this._isclosed = value;
                OnPropertyChanged("IsClosed");
            }
        }
        private bool _isclosed;

        public List<CadPoint> Points = new List<CadPoint>();

        public string Color = "#0000FF";



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
