using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.ShapeObj.Constraints
{
    public abstract class CadConstraint : INotifyPropertyChanged
    {
        #region static
        public static List<CadAnchor> FixedAnchor = new List<CadAnchor>();
        public static List<CadConstraint> RuntimeConstraits = new List<CadConstraint>();

        public static void AddRunConstrait(CadConstraint cadConstraint, CadAnchor cadAnchor)
        {
            RuntimeConstraits.Add(cadConstraint);
            FixedAnchor.Add(cadAnchor);
        }

        public static void RemoveRunConstrait(CadConstraint cadConstraint, CadAnchor cadAnchor)
        {
            if (RuntimeConstraits.Contains(cadConstraint) == true)
            {
                if (cadConstraint == RuntimeConstraits[0])
                {
                    RuntimeConstraits.Clear();
                    FixedAnchor.Clear();
                }
                else
                {
                    int ContsrIndex = RuntimeConstraits.IndexOf(cadConstraint);
                    if (ContsrIndex > -1)
                    {
                        RuntimeConstraits.RemoveRange(ContsrIndex, RuntimeConstraits.Count - ContsrIndex);
                    }

                    int AnchorIndex = FixedAnchor.IndexOf(cadAnchor);
                    if (AnchorIndex > -1)
                    {
                        FixedAnchor.RemoveRange(AnchorIndex, FixedAnchor.Count - AnchorIndex);
                    }
                }
            }
        }

        public static bool CheckConstraitOrAnchor(CadConstraint cadConstraint, CadAnchor cadAnchor)
        {
            return RuntimeConstraits.Contains(cadConstraint) || FixedAnchor.Contains(cadAnchor);
        }
        #endregion

        #region non static
        private CadVariable _variable;

        public CadVariable Variable
        {
            get => this._variable;
            set
            {
                this._variable = value;
                OnPropertyChanged("Value");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event EventHandler Removed;

        public void Remove()
        {
            Removed?.Invoke(this, null);
        }
        #endregion
    }
}
