using SureCadSystem.CadObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SureCadSystem.Constraints
{
    public interface CadConstraint : INotifyPropertyChanged
    {
        #region static
        public static List<CadConstraint> RuntimeConstraits { get; set; } = new List<CadConstraint>();

        public static void RemoveConstraint()
        {
            for (int i = 0; i < RuntimeConstraits.Count; i += 1)
            {
                if (RuntimeConstraits[i].IsBase == false)
                {
                    RuntimeConstraits.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        #endregion
        public event EventHandler<bool> Selected;
        public event EventHandler<bool> Fixed;
        public event EventHandler<bool> Supported;

        #region non static

        public CadVariable Variable { get; set; }

        public double Value { get; set; }

        public string ID { get; }

        public bool IsSelect { get; set; }

        public bool IsFix { get; set; }

        public bool IsSupport { get; set; }
        #endregion

        public bool ContaintsPoint(CadPoint Point);

        public bool IsBase { get; }
    }
}
