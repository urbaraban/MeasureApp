﻿using MeasureApp.ShapeObj.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.ShapeObj.Constraints
{
    public abstract class CadConstraint : INotifyPropertyChanged 
    {

        #region static
        public static List<CadPoint> FixedPoint = new List<CadPoint>();
        public static List<CadConstraint> RuntimeConstraits = new List<CadConstraint>();

        public static void AddRunConstrait(CadConstraint cadConstraint, CadPoint cadPoint)
        {
            RuntimeConstraits.Add(cadConstraint);
            FixedPoint.Add(cadPoint);
        }

        public static void RemoveRunConstrait(CadConstraint cadConstraint, CadPoint cadPoint)
        {
            if (RuntimeConstraits.Contains(cadConstraint) == true)
            {
                if (cadConstraint == RuntimeConstraits[0])
                {
                    RuntimeConstraits.Clear();
                    FixedPoint.Clear();
                }
                else
                {
                    int ContsrIndex = RuntimeConstraits.IndexOf(cadConstraint);
                    if (ContsrIndex > -1)
                    {
                        RuntimeConstraits.RemoveRange(ContsrIndex, RuntimeConstraits.Count - ContsrIndex);
                    }

                    int AnchorIndex = FixedPoint.IndexOf(cadPoint);
                    if (AnchorIndex > -1)
                    {
                        FixedPoint.RemoveRange(AnchorIndex, FixedPoint.Count - AnchorIndex);
                    }
                }
            }
        }

        public static bool CheckConstraitOrAnchor(CadConstraint cadConstraint, CadPoint cadAnchor)
        {
            return RuntimeConstraits.Contains(cadConstraint) || FixedPoint.Contains(cadAnchor);
        }
        #endregion
        public event EventHandler<bool> Selected;
        public virtual event EventHandler Removed;
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
                OnPropertyChanged("Value");
            }
        }

        public virtual string ID { get; set; }

        public bool IsSelect
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

        public bool IsSupport
        {
            get => this._issupprot;
            set
            {
                this._issupprot = value;
                Supported?.Invoke(this, this._issupprot);
            }
        }
        private bool _issupprot = false;

        public virtual void TryRemove()
        {
            Removed?.Invoke(this, null);
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}