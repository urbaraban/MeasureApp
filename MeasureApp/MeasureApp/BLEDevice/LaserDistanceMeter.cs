using System;

namespace SureMeasure.BLEDevice
{
    public interface LaserDistanceMeter
    {
        public event EventHandler<Tuple<double, double>> LenthUpdated;

        public void OnDevice();
    }
}
