using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SureMeasure.BLEDevice
{
    public interface IDistanceMeter : INotifyPropertyChanged
    {
        public bool IsConnected { get; }

        public bool IsOn { get; set; }

        public event EventHandler<Tuple<double, double>> LenthUpdated;
    }
}
