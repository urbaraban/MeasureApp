namespace SureMeasure.Orders
{
    using SureMeasure.CadObjects;
    using SureMeasure.CadObjects.Constraints;
    using SureMeasure.CadObjects.Interface;
    using SureMeasure.ShapeObj;
    using SureMeasure.ShapeObj.Constraints;
    using SureMeasure.Tools;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Defines the <see cref="Contour" />.
    /// </summary>
    public class Contour : INotifyPropertyChanged, OrderObjectInt
    {
        /// <summary>
        /// Defines the ObjectAdded.
        /// </summary>
        public event EventHandler<object> ObjectAdded;

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public string ID { get; set; } = "Contour";

        /// <summary>
        /// Gets the Lenths.
        /// </summary>
        public ObservableCollection<ConstraintLenth> Lenths { get; private set; } = new ObservableCollection<ConstraintLenth>();

        /// <summary>
        /// Gets the Angles.
        /// </summary>
        public ObservableCollection<ConstraintAngle> Angles { get; private set; } = new ObservableCollection<ConstraintAngle>();

        /// <summary>
        /// Gets or sets the Paths.
        /// </summary>
        public ObservableCollection<ContourPath> Paths { get; set; } = new ObservableCollection<ContourPath>();

        /// <summary>
        /// Gets the Points.
        /// </summary>
        public ObservableCollection<CadPoint> Points { get; private set; } = new ObservableCollection<CadPoint>();

        /// <summary>
        /// Gets the Perimetr
        /// Perimetr controur.
        /// </summary>
        public double Perimetr
        {
            get
            {
                double lenth = 0;

                foreach (ContourPath contourPath in Paths)
                {
                    lenth += contourPath.Perimeter;
                }
                return lenth;
            }
        }

        /// <summary>
        /// Gets the Area.
        /// </summary>
        public double Area
        {
            get
            {
                double area = 0;
                foreach (ContourPath contourPath in Paths)
                {
                    area += contourPath.Area;
                }
                return area;
            }
        }

        /// <summary>
        /// Defines the PropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The OnPropertyChanged.
        /// </summary>
        /// <param name="prop">The prop<see cref="string"/>.</param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Gets or sets the SelectedDrawMethod.
        /// </summary>
        public DrawMethod SelectedDrawMethod
        {
            get => this._drawmethod;
            set
            {
                this._drawmethod = value;
                if (this._drawmethod == DrawMethod.FromPoint)
                {
                    this.BasePoint = this.LastPoint;
                }
                OnPropertyChanged("Method");
            }
        }

        /// <summary>
        /// Defines the _drawmethod.
        /// </summary>
        private DrawMethod _drawmethod = DrawMethod.StepByStep;

        /// <summary>
        /// Gets or sets the BaseLenthConstrait.
        /// </summary>
        public ConstraintLenth BaseLenthConstrait
        {
            get => this._baselenthconstrait;
            internal set
            {
                this._baselenthconstrait = value;
                if (this._baselenthconstrait != null)
                {
                    if (this._baselenthconstrait.IsSelect == false)
                    {
                        this._baselenthconstrait.IsSelect = true;
                    }
                    foreach (ConstraintLenth constraintLenth in this.Lenths)
                    {
                        if (constraintLenth != this._baselenthconstrait && constraintLenth.IsSelect == true)
                            constraintLenth.IsSelect = false;
                    }
                }
                OnPropertyChanged("BaseLenthConstrait");
            }
        }

        /// <summary>
        /// Defines the _baselenthconstrait.
        /// </summary>
        private ConstraintLenth _baselenthconstrait;

        /// <summary>
        /// Gets or sets the BasePoint.
        /// </summary>
        public CadPoint BasePoint
        {
            get => this._basepoint;
            internal set
            {
                this._basepoint = value;
                if (this._basepoint != null)
                {
                    if (this._basepoint.IsBase == false)
                    {
                        this._basepoint.IsBase = true;
                    }

                    foreach (CadPoint cadPoint in Points)
                    {
                        if (cadPoint != value && cadPoint.IsBase == true) cadPoint.IsBase = false;
                    }
                    if (this._lastpoint == null || this._drawmethod == DrawMethod.StepByStep)
                    {
                        this.LastPoint = value;
                    }
                }
                OnPropertyChanged("BasePoint");
            }
        }

        /// <summary>
        /// Defines the _basepoint.
        /// </summary>
        private CadPoint _basepoint;

        /// <summary>
        /// Gets or sets the LastPoint.
        /// </summary>
        public CadPoint LastPoint
        {
            get => this._lastpoint;
            internal set
            {
                this._lastpoint = value;
                if (this._lastpoint != null)
                {
                    if (this._lastpoint.IsSelect == false)
                    {
                        this._lastpoint.IsSelect = true;
                    }

                    foreach (CadPoint cadPoint in Points)
                    {
                        if (cadPoint != value && cadPoint.IsSelect == true) cadPoint.IsSelect = false;
                    }

                    if (this._basepoint == null && this._drawmethod == DrawMethod.FromPoint)
                    {
                        this.BasePoint = value;
                    }
                }
                OnPropertyChanged("LastPoint");
            }
        }

        /// <summary>
        /// Defines the _lastpoint.
        /// </summary>
        private CadPoint _lastpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="Contour"/> class.
        /// </summary>
        /// <param name="Name">The Name<see cref="string"/>.</param>
        public Contour(string Name)
        {
            this.ID = Name;
            Lenths = new ObservableCollection<ConstraintLenth>();
            Angles = new ObservableCollection<ConstraintAngle>();
            Paths = new ObservableCollection<ContourPath>();
            Paths.Add(new ContourPath(Paths.Count.ToString()));
        }

        /// <summary>
        /// Get CadPoint for name. Search in all path.
        /// </summary>
        /// <param name="ID">The ID<see cref="string"/>.</param>
        /// <returns>.</returns>
        public CadPoint GetPointByName(string ID)
        {
            foreach (CadPoint cadPoint in this.Points)
            {
                if (cadPoint.ID == ID)
                    return cadPoint;
            }
            return null;
        }

        /// <summary>
        /// The GetLenthByName.
        /// </summary>
        /// <param name="ID">The ID<see cref="string"/>.</param>
        /// <returns>The <see cref="ConstraintLenth"/>.</returns>
        public ConstraintLenth GetLenthByName(string ID)
        {
            foreach (ConstraintLenth constraintLenth in this.Lenths)
            {
                if (constraintLenth.ID == ID) return constraintLenth;
            }
            return null;
        }

        /// <summary>
        /// The Add.
        /// </summary>
        /// <param name="Object">The Object<see cref="object"/>.</param>
        /// <param name="Last">The Last<see cref="bool"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public object Add(object Object, bool Last)
        {
            if (Object is CadObject cadObject)
            {
                cadObject.Selected += CadObject_Selected;
                cadObject.Removed += CadObject_Removed;
            }

            if (Object is CadPoint cadPoint)
            {
                cadPoint.BaseObject += CadPoint_BaseObject; ;
                this.Points.Add(cadPoint);
                if (Last == true)
                {
                    if (this._drawmethod == DrawMethod.StepByStep)
                    {
                        this.BasePoint = cadPoint;
                    }
                    else
                    {
                        this.LastPoint = cadPoint;
                    }
                }
                ObjectAdded?.Invoke(this, Object);
                return Object;
            }
            else if (Object is ConstraintLenth lenthConstrait)
            {
                this.Lenths.Add(lenthConstrait);
                if (Last == true && (SelectedDrawMethod == DrawMethod.StepByStep || this.BaseLenthConstrait == null))
                {
                    this.BaseLenthConstrait = lenthConstrait;
                    FindContourForLenth(lenthConstrait);
                }

                ObjectAdded?.Invoke(this, Object);
                return Object;
            }
            else if (Object is ConstraintAngle angleConstrait)
            {
                this.Angles.Add(angleConstrait);
                ObjectAdded?.Invoke(this, Object);
                return Object;
            }
            else if (Object is ConstraitLabel constraitLabel)
            {
                this.ObjectAdded(this, constraitLabel);
            }
            return null;
        }

        /// <summary>
        /// Make line with anchor on canvas.
        /// </summary>
        /// <param name="tuple">item1 - lenth, item2 - angle.</param>
        /// <param name="Forced">Make line from label.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool BuildLine(Tuple<double, double> tuple, bool Forced = false)
        {
            //Если у нас нет линии привязки
            if ((this.BaseLenthConstrait == null) || (Forced == true))
            {
                CadPoint cadPoint1 = (CadPoint)this.Add(new CadPoint(0, 0, this.GetNewPointName()), false);
                cadPoint1.IsFix = !Forced;
                CadPoint cadPoint2 = (CadPoint)this.Add(new CadPoint(0, 0 + tuple.Item1, this.GetNewPointName()), true);
                cadPoint2.IsFix = !Forced;
                this.Add(new ConstraintLenth(cadPoint1, cadPoint2, tuple.Item1), true);
                return true;
            }
            else if (this.BasePoint != null)
            {
                CadPoint point1 = this.BaseLenthConstrait.GetNotThisPoint(this.BasePoint);
                if (point1 == null) return false;

                CadPoint point2 = Sizing.GetPositionLineFromAngle(point1, this.BasePoint, tuple.Item1, tuple.Item2 < 0 ? 90 : tuple.Item2);
                point2.ID = this.GetNewPointName();

                ConstraintLenth lenthConstrait =
                    new ConstraintLenth(this.BasePoint, point2, tuple.Item1,
                        this.SelectedDrawMethod == DrawMethod.FromPoint && this.LastPoint != this.BasePoint);

                if (tuple.Item2 > 0)
                {
                    ConstraintAngle constraintAngle =
                        (ConstraintAngle)this.Add(
                            new ConstraintAngle(this.BaseLenthConstrait, lenthConstrait, tuple.Item2), false);
                }

                if (this.SelectedDrawMethod == DrawMethod.FromPoint && this.BasePoint != this.LastPoint)
                {
                    this.Add(new ConstraintLenth(this.LastPoint, point2, -1), false);
                }

                this.Add(lenthConstrait, this.SelectedDrawMethod == DrawMethod.StepByStep);
                this.Add(point2, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// The CadPoint_BaseObject.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="bool"/>.</param>
        private void CadPoint_BaseObject(object sender, bool e)
        {
            if (e == true)
            {
                BasePoint = (CadPoint)sender;
            }
        }

        /// <summary>
        /// The FindContourForLenth.
        /// </summary>
        /// <param name="constraintLenth">The constraintLenth<see cref="ConstraintLenth"/>.</param>
        private void FindContourForLenth(ConstraintLenth constraintLenth)
        {
            bool WithoutContour = true;
            if (constraintLenth.IsSupport == false)
            {
                foreach (ContourPath contourPath in this.Paths)
                {
                    if (contourPath.IsClosed == false)
                    {
                        contourPath.Add(constraintLenth);
                        WithoutContour = false;
                    }
                }
                if (WithoutContour == true)
                {
                    this.Paths.Add(new ContourPath(this.Paths.Count.ToString())
                    {
                         constraintLenth
                    });
                }
            }
        }

        /// <summary>
        /// The CadObject_Removed.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="bool"/>.</param>
        private void CadObject_Removed(object sender, bool e)
        {
            this.Remove(sender);
        }

        /// <summary>
        /// The CadObject_Selected.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="bool"/>.</param>
        private void CadObject_Selected(object sender, bool e)
        {
            if (e == true)
            {
                if (sender is CadPoint cadPoint)
                {
                    if (_drawmethod == DrawMethod.StepByStep && this.BasePoint != cadPoint)
                        this.BasePoint = cadPoint;
                    else if (this.LastPoint != cadPoint)
                        this.LastPoint = cadPoint;
                }
                if (sender is ConstraintLenth lenthConstrait)
                {
                    this.BaseLenthConstrait = lenthConstrait;
                }
            }
            else
            {
                if (LastPoint == sender)
                {
                    LastPoint = null;
                }
                if (BasePoint == sender && BasePoint == LastPoint)
                {
                    BasePoint = null;
                }
                if (BaseLenthConstrait == sender)
                {
                    BaseLenthConstrait = null;
                }

            }
        }

        /// <summary>
        /// Remove select object.
        /// </summary>
        /// <param name="Object">.</param>
        public void Remove(object Object)
        {
            if (Object is CadObject cadObject)
            {
                cadObject.Selected -= CadObject_Selected;
                cadObject.Removed -= CadObject_Removed;
            }

            if (Object is CadPoint cadPoint)
            {
                this.Points.Remove(cadPoint);
                this.LastPoint = this.LastPoint == cadPoint ? null : this.LastPoint;
                this.BasePoint = this.BasePoint == cadPoint ? null : this.BasePoint;
            }
            else if (Object is ConstraintLenth lenthConstrait)
            {
                this.Lenths.Remove(lenthConstrait);
                this.BaseLenthConstrait = this.BaseLenthConstrait == lenthConstrait ? null : this.BaseLenthConstrait;
            }
            else if (Object is ConstraintAngle angleConstrait)
            {
                this.Angles.Remove(angleConstrait);
            }
        }

        /// <summary>
        /// Clear contour and set Base and Last object to null.
        /// </summary>
        public void Clear()
        {
            this.BasePoint = null;
            this.BaseLenthConstrait = null;
            this.LastPoint = null;
            this.Angles.Clear();
            this.Lenths.Clear();
            this.Points.Clear();
        }

        /// <summary>
        /// The GetBytes.
        /// </summary>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public byte[] GetBytes()
        {
            List<Byte> theBytes = new List<Byte>();

            theBytes.Add((byte)'2');
            theBytes.Add((byte)'C');
            theBytes.Add((byte)'U');
            theBytes.Add((byte)'T');
            theBytes.Add((byte)0);
            theBytes.Add((byte)0);
            theBytes.Add((byte)0);

            short PathCount = (short)this.Paths.Count;
            theBytes.Add((byte)((PathCount >> 8) & 0xff));
            theBytes.Add((byte)(PathCount & 0xff));

            foreach (ContourPath path in this.Paths)
            {
                theBytes.Add(path.IsClosed == true ? (byte)1 : (byte)0);
                short PointCount = (short)(path.Points == null ? 0 : path.Points.Length);
                theBytes.Add((byte)((PointCount >> 8) & 0xff));
                theBytes.Add((byte)(PointCount & 0xff));

                if (path.Points is CadPoint[] points)
                {
                    foreach (CadPoint point in points)
                    {
                        short posx = (short)point.X;
                        theBytes.Add((byte)((posx >> 8) & 0xff));
                        theBytes.Add((byte)(posx & 0xff));

                        short posy = (short)point.Y;
                        theBytes.Add((byte)((posy >> 8) & 0xff));
                        theBytes.Add((byte)(posy & 0xff));
                    }
                }
            }
            return theBytes.ToArray();
        }

        /// <summary>
        /// The GetNewPointName.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetNewPointName()
        {
            string result = string.Empty;

            result = $"{Convert.ToChar(65 * ((Points.Count / 678) > 0 ? 1 : 0) + Points.Count / 678)}" +
                $"{Convert.ToChar(65 * ((Points.Count / 26) > 0 ? 1 : 0) + Points.Count / 26)}" +
                $"{Convert.ToChar(65 + Points.Count % 26)}";

            return result.Trim('\0');
        }

        /// <summary>
        /// The ToString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString() => this.ID;

        /// <summary>
        /// Defines the DrawMethod.
        /// </summary>
        public enum DrawMethod
        {
            /// <summary>
            /// Defines the StepByStep.
            /// </summary>
            StepByStep,

            /// <summary>
            /// Defines the FromPoint.
            /// </summary>
            FromPoint
        }
    }
}
