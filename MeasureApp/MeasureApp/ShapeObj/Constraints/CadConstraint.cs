using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MeasureApp.ShapeObj.Constraints
{
    public abstract class CadConstraint : INotifyPropertyChanged
    {
        public static List<CadAnchor> FixedAnchor = new List<CadAnchor>();
        public static List<CadConstraint> RuntimeConstraits = new List<CadConstraint>();

        public static void AddRunConstrait(CadConstraint cadConstraint, CadAnchor cadAnchor)
        {
            RuntimeConstraits.Add(cadConstraint);
            FixedAnchor.Add(cadAnchor);
        }

        public static void RemoveRunConstrait(CadConstraint cadConstraint, CadAnchor cadAnchor)
        {
            if (RuntimeConstraits.Last() == cadConstraint)
            {
                RuntimeConstraits.Remove(cadConstraint);
                FixedAnchor.Remove(cadAnchor);
            }
        }

        public static bool CheckConstraitOrAnchor(CadConstraint cadConstraint, CadAnchor cadAnchor)
        {
            return RuntimeConstraits.Contains(cadConstraint) || FixedAnchor.Contains(cadAnchor);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
