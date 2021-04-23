using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureApp.View.BLEDevice
{
    public interface LaserDistanceMeter
    {
        public event EventHandler<Tuple<double, double>> LenthUpdated;

        public void OnDevice();
    }
}
