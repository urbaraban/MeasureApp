
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureApp.ShapeObj
{
        public enum Orientaton : int
        {
            OFF = -1,
            Vertical = 0,
            Horizontal = 1
        }

    public class EnumTools
    {

        public static List<string> ToNameArray<T>()
        {
            return Enum.GetNames(typeof(T)).ToList();
        }

        public static List<T> ToListOfValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}

