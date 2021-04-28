using SureMeasure.CadObjects;
using SureMeasure.ShapeObj;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Xamarin.Forms;

namespace SureMeasure.Tools
{
    public static class Sizing
    {
        public static double PtPLenth(CadPoint cadPoint1, CadPoint cadPoint2)
        {
           return Math.Round(Math.Sqrt(Math.Pow(cadPoint2.X - cadPoint1.X, 2) + Math.Pow(cadPoint2.Y - cadPoint1.Y, 2)), 2);
        }

        public static CadPoint GetPositionLineFromAngle(CadPoint Point1, CadPoint Point2, double newLenth, double angle)
        {
            double Lenth = PtPLenth(Point1, Point2);

            double pos = newLenth / Lenth;
            Point pointH = new Point(Point2.X + (Point1.X - Point2.X) * pos, Point2.Y + (Point1.Y - Point2.Y) * pos);
            double angleInRadians = angle * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new CadPoint()
            {
                X =
                    (double)
                    (cosTheta * (pointH.X - Point2.X) -
                    sinTheta * (pointH.Y - Point2.Y) + Point2.X),
                Y =
                    (double)
                    (sinTheta * (pointH.X - Point2.X) +
                    cosTheta * (pointH.Y - Point2.Y) + Point2.Y)
            };
        }

        /// <summary>
        /// Get point on line from lenth.
        /// </summary>
        /// <param name="Point1">Start point</param>
        /// <param name="Point2">Last point</param>
        /// <param name="Lenth"></param>
        /// <returns></returns>
        public static CadPoint GetPostionOnLine(CadPoint Point1, CadPoint Point2, double Lenth)
        {
            double t = Lenth / PtPLenth(Point1, Point2);

            return new CadPoint()
            { 
                X = Point1.X + (Point2.X - Point1.X) * t,
                Y = Point1.Y + (Point2.Y - Point1.Y) * t
            };
        }

        public static double AngleHorizont(CadPoint cadPoint1, CadPoint cadPoint2)
        {
            double degrees;

            // Avoid divide by zero run values.
            if (cadPoint2.X - cadPoint1.X == 0)
            {
                if (cadPoint2.Y > cadPoint1.X)
                    degrees = 90;
                else
                    degrees = 270;
            }
            else
            {
                // Calculate angle from offset.
                double riseoverrun = (double)(cadPoint2.Y - cadPoint1.Y) / (double)(cadPoint2.X - cadPoint1.X);
                double radians = Math.Atan(riseoverrun);
                degrees = radians * ((double)180 / Math.PI);

                // Handle quadrant specific transformations.       
                if ((cadPoint2.X - cadPoint1.X) < 0 || (cadPoint2.Y - cadPoint1.Y) < 0)
                    degrees += 180;
                if ((cadPoint2.X - cadPoint1.X) > 0 && (cadPoint2.Y - cadPoint1.Y) < 0)
                    degrees -= 180;
                if (degrees < 0)
                    degrees += 360;
            }
            return degrees;
        }

        /// <summary>
        /// Find Angle from three point. Use contrClockwise model
        /// </summary>
        /// <param name="p0">First point</param>
        /// <param name="c">Center point</param>
        /// <param name="p1">EndPoint</param>
        /// <returns></returns>
        public static double AngleThreePoint(CadPoint p0, CadPoint c, CadPoint p1)
        {
            var p0c = Math.Sqrt(Math.Pow(c.X - p0.X, 2) +
                                Math.Pow(c.Y - p0.Y, 2)); // p0->c (b)   
            var p1c = Math.Sqrt(Math.Pow(c.X - p1.X, 2) +
                                Math.Pow(c.Y - p1.Y, 2)); // p1->c (a)
            var p0p1 = Math.Sqrt(Math.Pow(p1.X - p0.X, 2) +
                                 Math.Pow(p1.Y - p0.Y, 2)); // p0->p1 (c)
            return (Math.Acos((p1c * p1c + p0c * p0c - p0p1 * p0p1) / (2 * p1c * p0c)) * 180 / Math.PI);
        }
    }
}
