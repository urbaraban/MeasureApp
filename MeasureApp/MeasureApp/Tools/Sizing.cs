﻿using MeasureApp.ShapeObj;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MeasureApp.Tools
{
    public static class Sizing
    {
        public static double PtPLenth(CadPoint cadPoint1, CadPoint cadPoint2)
        {
           return Math.Sqrt(Math.Pow(cadPoint2.X - cadPoint1.X, 2) + Math.Pow(cadPoint2.Y - cadPoint1.Y, 2));
        }

        public static CadPoint GetPositionLineFromAngle(CadPoint Point1, CadPoint Point2, double newLenth, double angle)
        {
            double Lenth = PtPLenth(Point1, Point2);

            double pos = newLenth / Lenth;
            CadPoint pointH = new CadPoint(Point2.X + (Point1.X - Point2.X) * pos, Point2.Y + (Point1.Y - Point2.Y) * pos);
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
    }
}
