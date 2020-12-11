using MeasureApp.ShapeObj.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.ShapeObj.Constraints
{
    public abstract class CadConstraint : INotifyPropertyChanged, MainObject
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


        public event EventHandler Removed;
        public event EventHandler<bool> Selected;
        public event EventHandler<bool> Fixed;
        public event EventHandler<bool> Supported;

        public virtual bool IsSupport {
            get => this._issupport;
            set
            {
                this._issupport = value;
                Supported?.Invoke(this, this._issupport);
            }
        }
        private bool _issupport = false;


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

        public virtual string Name { get; set; }

        public virtual bool IsSelect 
        { 
            get => this._isselect; 
            set 
            {
                this._isselect = value;
                Selected?.Invoke(this, this._isselect);
            } 
        }
        private bool _isselect = false;
        public virtual bool IsFix { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void TryRemove()
        {
            Removed?.Invoke(this, null);
        }
        #endregion
    }
}
