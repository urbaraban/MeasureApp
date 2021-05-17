using SureMeasure.CadObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SureMeasure.ShapeObj.Constraints
{
    public abstract class CadConstraint : INotifyPropertyChanged 
    {
        #region static
        public static List<CadConstraint> RuntimeConstraits = new List<CadConstraint>();
        public static List<CadPoint> BreakPoints = new List<CadPoint>();


        #endregion
        public virtual event EventHandler<bool> Selected;
        public virtual event EventHandler<bool> Fixed;
        public virtual event EventHandler<bool> Supported;

        #region non static
        private CadVariable _variable;

        public CadVariable Variable
        {
            get => this._variable;
            set
            {
                this._variable = value;
                OnPropertyChanged("Variable");
            }
        }

        public virtual double Value
        {
            get => this.Variable.Value;
            set
            {
                this.Variable.Value = value;
                OnPropertyChanged("Value");
            }
        }

        public virtual string ID { get; set; }

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

        public bool IsFix
        {
            get => _isfix;
            set
            {
                this._isfix = value;
                Fixed?.Invoke(this, this._isfix);
            }
        }
        private bool _isfix = false;

        public virtual bool IsSupport
        {
            get => this._issupprot;
            set
            {
                this._issupprot = value;
                Supported?.Invoke(this, this._issupprot);
            }
        }
        private bool _issupprot = false;
        #endregion

        public virtual bool Execute { get; } = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
