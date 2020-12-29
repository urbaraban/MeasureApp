using MeasureApp.ShapeObj;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace MeasureApp.Orders
{
    public class ContourPath : INotifyPropertyChanged, OrderObjectInt
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

        public string ID { get; set; }

        public ContourPath(string ID)
        {
            this.ID = ID;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return ID;
        }
    }
}
