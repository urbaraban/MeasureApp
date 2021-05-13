using System;

namespace SureMeasure.Tools
{
    public static class Converters
    {
        public static Tuple<double, double> ConvertDimMessage(string message)
        {
            double LineLenth = -1;
            double LineAngle = -1;

            if (string.IsNullOrEmpty(message) == false)
            {
                string[] strings = message.Split('&');
                LineLenth = double.Parse(strings[0]);
                LineAngle = -1;
                if (strings.Length > 1)
                {
                    LineAngle = double.Parse(strings[1]);
                }
            }
            return new Tuple<double, double>(LineLenth, LineAngle);
        }
    }
}
