using System;

namespace SureMeasure.BLEDevice
{
    public interface DistanceMeter
    {
        public event EventHandler<Tuple<double, double>> LenthUpdated;

        public void OnDevice();
    }
}
