namespace SureMeasure.ShapeObj.Constraints
{
    using SureMeasure.CadObjects;
    using SureMeasure.CadObjects.Interface;
    using SureMeasure.Tools;
    using System;
    using System.Diagnostics;
    using System.Numerics;

    /// <summary>
    /// Defines the <see cref="ConstraintLenth" />.
    /// </summary>
    public class ConstraintLenth : CadConstraint, CadObject
    {
        /// <summary>
        /// Defines the Changed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Defines the Removed.
        /// </summary>
        public event EventHandler<bool> Removed;

        /// <summary>
        /// Defines the Selected.
        /// </summary>
        public override event EventHandler<bool> Selected;

        /// <summary>
        /// Defines the Supported.
        /// </summary>
        public override event EventHandler<bool> Supported;

        /// <summary>
        /// Defines the Orientation.
        /// </summary>
        public Orientaton Orientation = Orientaton.OFF;// -1 — Off, 0 — Vetical, 1 — Horizontal

        /// <summary>
        /// Gets or sets the Point1
        /// Set temp attribute on this constaint.
        /// </summary>
        public CadPoint Point1 { get; set; }

        /// <summary>
        /// Gets or sets the Point2.
        /// </summary>
        public CadPoint Point2 { get; set; }
        /// <summary>
        /// Get line Vector
        /// </summary>
        public Vector2 Vector  => Vector2.Normalize(new Vector2((float)(Point2.X - Point1.X), (float)(Point2.Y - Point1.Y)));

        /// <summary>
        /// Gets or sets the Lenth.
        /// </summary>
        public override double Value
        {
            get => this.Variable.Value < 0 ? Sizing.PtPLenth(this.Point1, this.Point2) : this.Variable.Value;
            set
            {
                this.Variable.Value = value;
                OnPropertyChanged("Value");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsSelect.
        /// </summary>
        public override bool IsSelect
        {
            get => this._isselect;
            set
            {
                this._isselect = value;
                Selected?.Invoke(this, this._isselect);
            }
        }

        /// <summary>
        /// Defines the _isselect.
        /// </summary>
        private bool _isselect = false;

        /// <summary>
        /// Gets or sets a value indicating whether IsSupport.
        /// </summary>
        public override bool IsSupport
        {
            get => this._issupport;
            set
            {
                this._issupport = value;
                Supported?.Invoke(this, this._issupport);
            }
        }

        /// <summary>
        /// Defines the _issupport.
        /// </summary>
        private bool _issupport = false;

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public override string ID => Point1.ID + Point2.ID;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintLenth"/> class.
        /// </summary>
        /// <param name="point1">The point1<see cref="CadPoint"/>.</param>
        /// <param name="point2">The point2<see cref="CadPoint"/>.</param>
        /// <param name="Lenth">The Lenth<see cref="double"/>.</param>
        /// <param name="isSupport">The isSupport<see cref="bool"/>.</param>
        public ConstraintLenth(CadPoint point1, CadPoint point2, double Lenth, bool isSupport = false)
        {
            this.Point1 = point1;
            this.Point2 = point2;

            this.Variable = new CadVariable(Lenth, true);
            this.IsSupport = isSupport;

            SubAnchor(this.Point1);
            SubAnchor(this.Point2);

            this.Variable.PropertyChanged += Variable_PropertyChanged;
            this.Point1.ChangedPoint += Point_ChangedPoint;
            this.Point2.ChangedPoint += Point_ChangedPoint;

            this.Point1.Removed += Point_Removed;
            this.Point2.Removed += Point_Removed;
        }

        /// <summary>
        /// The Point_Removed.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="bool"/>.</param>
        private void Point_Removed(object sender, bool e)
        {
            if (this.Point1 == sender || this.Point2 == sender)
            {
                Removed?.Invoke(this, true);
            }
        }

        /// <summary>
        /// The Point_ChangedPoint.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="CadPoint"/>.</param>
        private void Point_ChangedPoint(object sender, CadPoint e)
        {
            if (this.Point1 == sender)
            {
                UnSubAnchor(this.Point1);
                this.Point1 = e;
                SubAnchor(this.Point1);
            }
            if (this.Point2 == sender)
            {
                UnSubAnchor(this.Point2);
                this.Point2 = e;
                SubAnchor(this.Point2);
            }
            Changed?.Invoke(this, null);
        }

        /// <summary>
        /// The Fix.
        /// </summary>
        /// <param name="state">The state<see cref="bool"/>.</param>
        public void Fix(bool state)
        {
            this.Point1.IsFix = state;
            this.Point2.IsFix = state;
        }

        /// <summary>
        /// The TryRemove.
        /// </summary>
        public void TryRemove()
        {
            UnSubAnchor(this.Point1);
            this.Point1.TryRemove();
            UnSubAnchor(this.Point2);
            this.Point2.TryRemove();
            this.Variable.PropertyChanged -= Variable_PropertyChanged;
            Removed?.Invoke(this, true);
        }

        /// <summary>
        /// The UnSubAnchor.
        /// </summary>
        /// <param name="cadPoint">The cadPoint<see cref="CadPoint"/>.</param>
        public void UnSubAnchor(CadPoint cadPoint)
        {
            cadPoint.PropertyChanged -= Point_PropertyChanged;
        }

        /// <summary>
        /// The SubAnchor.
        /// </summary>
        /// <param name="cadPoint">The cadPoint<see cref="CadPoint"/>.</param>
        public void SubAnchor(CadPoint cadPoint)
        {
            cadPoint.PropertyChanged += Point_PropertyChanged;
        }

        /// <summary>
        /// The GetNotThisPoint.
        /// </summary>
        /// <param name="cadPoint">The cadPoint<see cref="CadPoint"/>.</param>
        /// <returns>The <see cref="CadPoint"/>.</returns>
        public CadPoint GetNotThisPoint(CadPoint cadPoint)
        {
            if (this.Point1 == cadPoint) return this.Point2;
            else if (this.Point2 == cadPoint) return this.Point1;
            return null;
        }

        /// <summary>
        /// The ToString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.ID}:{Value.ToString()}";
        }

        /// <summary>
        /// The Variable_PropertyChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.ComponentModel.PropertyChangedEventArgs"/>.</param>
        private void Variable_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.IsFix == false)
            {
                PreMagic(this.Point1, this.Point2, this.Vector);
            }
        }

        /// <summary>
        /// The Point_PropertyChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.ComponentModel.PropertyChangedEventArgs"/>.</param>
        private void Point_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Point")
            {
                if (sender is CadPoint cadPoint1 && this.GetNotThisPoint(cadPoint1) is CadPoint cadPoint2)
                {
                    PreMagic(cadPoint1, cadPoint2, this.Vector, cadPoint1 != this.Point1);
                }
                this.Changed?.Invoke(this, null);
            }
        }

        public void PreMagic(CadPoint cadPoint1, CadPoint cadPoint2, Vector2 vector2, bool Inversion = false)
        {
            if (this.Variable.Value > -1 
                && (cadPoint1.IsFix == false || cadPoint2.IsFix == false))
            {
                if (MakeMagic(cadPoint1, vector2, Inversion) == false)
                {
                    CadConstraint.RuntimeConstraits.Remove(this);
                    MakeMagic(cadPoint2, vector2, !Inversion);
                }
            }
        }

        /// <summary>
        /// The MakeMagic.
        /// </summary>
        /// <param name="cadPoint1">The cadPoint1<see cref="CadPoint"/>.</param>
        /// <param name="cadPoint2">The cadPoint2<see cref="CadPoint"/>.</param>
        private bool MakeMagic(CadPoint startPoint, Vector2 vector2, bool Inversion)
        {
            Console.WriteLine($"Lenth {this.ID} Vector {startPoint.ToString()}");
            CadPoint EndPoint = this.GetNotThisPoint(startPoint);
            if (EndPoint.IsFix == false)
            {
                if (CadConstraint.RuntimeConstraits.Contains(this) == false)
                {
                    CadConstraint.RuntimeConstraits.Add(this);
                    bool result = EndPoint.Update(
                            startPoint.X + vector2.X * this.Value * (Inversion == true ? -1 : 1),
                            startPoint.Y + vector2.Y * this.Value * (Inversion == true ? -1 : 1));
                    CadConstraint.RuntimeConstraits.Remove(this);
                    return result;
                }
            }
            else CadConstraint.RuntimeConstraits.Remove(this);

            return CadConstraint.RuntimeConstraits.Contains(this);
        }

        /// <summary>
        /// The GetInvertClone.
        /// </summary>
        /// <returns>The <see cref="ConstraintLenth"/>.</returns>
        public ConstraintLenth GetInvertClone()
        {
            return new ConstraintLenth(this.Point2, this.Point1, this.Value, this.IsSupport);
        }
    }
}
