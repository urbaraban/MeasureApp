
using System;
using System.Collections.Generic;
using System.Linq;

namespace SureMeasure.ShapeObj
{


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

