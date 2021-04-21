using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.Canvas;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.Orders
{
    public class Contour : INotifyPropertyChanged, OrderObjectInt
    {
        public event EventHandler<object> ObjectAdded;

        public string ID { get; set; } = "Contour";

        public List<ConstraintLenth> Lenths = new List<ConstraintLenth>();

        public List<ConstraintAngle> Angles = new List<ConstraintAngle>();

        public List<ContourPath> Paths = new List<ContourPath>();

        public List<CadPoint> Points
        {
            get
            {
                List<CadPoint> cadPoints = new List<CadPoint>();
                foreach (ContourPath contourPath in Paths)
                {
                    cadPoints.AddRange(contourPath.Points);
                }
                return cadPoints;
            }
        }

        /// <summary>
        /// Perimetr controur
        /// </summary>
        public double Perimetr => CalcPerimetr();

        private double CalcPerimetr()
        {
            double lenth = 0;

            foreach (ConstraintLenth lenthConstrait in Lenths)
            {
                if (lenthConstrait.IsSupport == false)
                {
                    lenth += lenthConstrait.Lenth;
                }
            }
            return lenth;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public DrawMethod Method
        {
            get => this._method;
            set
            {
                this._method = value;
                if (this._method == DrawMethod.FromPoint)
                {
                    this.BasePoint = this.LastPoint;
                }
                OnPropertyChanged("Method");
            }
        }
        private DrawMethod _method = DrawMethod.StepByStep;

        public ConstraintLenth BaseLenthConstrait { get; internal set; }
        public CadPoint BasePoint { get; internal set; }
        public CadPoint LastPoint { get; internal set; }

        public Contour(string Name)
        {
            this.ID = Name;
            Lenths = new List<ConstraintLenth>();
            Angles = new List<ConstraintAngle>();
            Paths = new List<ContourPath>();
            Paths.Add(new ContourPath(Paths.Count.ToString()));
        }
       
        /// <summary>
        /// Get CadPoint for name. Search in all path.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public CadPoint GetPointByName(string ID)
        {
            foreach (CadPoint cadPoint in this.Points)
            {
                if (cadPoint.ID == ID) return cadPoint;
            }
            return null;
        }

        public ConstraintLenth GetLenthByName(string ID)
        {
            foreach (ConstraintLenth constraintLenth in this.Lenths)
            {
                if (constraintLenth.ID == ID) return constraintLenth;
            }
            return null;
        }

        public object Add(object Object, int PathIndex, bool Last = true)
        {
            if (Object is CadObject cadObject)
            {
                cadObject.Selected += CadObject_Selected;
                cadObject.Removed += CadObject_Removed;
                cadObject.LastObject += CadObject_LastObject;
            }

            if (Object is CadPoint cadPoint)
            {
                this.Paths[PathIndex].Points.Add(cadPoint);
                if (Last == true)
                {
                    if (this._method == DrawMethod.StepByStep || this.BasePoint == null)
                    {
                        cadPoint.IsSelect = true;
                    }
                    else
                    {
                        this.LastPoint = cadPoint;
                    }
                    
                }
                ObjectAdded?.Invoke(this.Paths[PathIndex], Object);
                return Object;
            }
            else if (Object is ConstraintLenth lenthConstrait)
            {
                this.Lenths.Add(lenthConstrait);
                if (Last == true && (Method == DrawMethod.StepByStep || this.BaseLenthConstrait == null))
                {
                    lenthConstrait.IsSelect = true;
                }
                ObjectAdded?.Invoke(this, Object);
                return Object;
            }
            else if(Object is ConstraintAngle angleConstrait)
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

        private void CadObject_LastObject(object sender, bool e)
        {
            if (e == true)
            {
                if (sender is CadPoint cadPoint)
                {
                    if (this._method == DrawMethod.StepByStep)
                    {
                        this.LastPoint = cadPoint;
                        this.BasePoint = cadPoint;
                    }
                    else
                    {
                        this.LastPoint = cadPoint;
                    }
                }
                if (sender is ConstraintLenth lenthConstrait)
                {
                    if (this.BaseLenthConstrait != null)
                    {
                        this.BaseLenthConstrait.IsSelect = false;
                    }
                    this.BaseLenthConstrait = lenthConstrait;
                }
            }
        }

        private void CadObject_Removed(object sender, bool e)
        {
            this.Remove(sender);
        }

        private void CadObject_Selected(object sender, bool e)
        {
            if (e == true)
            {
                if (sender is CadPoint cadPoint)
                {
                    if (this._method == DrawMethod.StepByStep || this.BasePoint == null)
                    {
                        if (this.BasePoint != null) this.LastPoint.IsSelect = false;
                        this.LastPoint = cadPoint;
                        this.BasePoint = cadPoint;
                    }
                }
                if (sender is ConstraintLenth lenthConstrait)
                {
                    if (this.BaseLenthConstrait != null)
                    {
                        this.BaseLenthConstrait.IsSelect = false;
                    }
                    this.BaseLenthConstrait = lenthConstrait;
                }
            }
            else
            {
                BasePoint = BasePoint == sender ? null : BasePoint;
                BaseLenthConstrait = BaseLenthConstrait == sender ? null : BaseLenthConstrait;
            }
        }

        /// <summary>
        /// Remove select object
        /// </summary>
        /// <param name="Object"></param>
        public void Remove (object Object)
        {
            if (Object is CadObject cadObject)
            {
                cadObject.Selected -= CadObject_Selected;
                cadObject.Removed -= CadObject_Removed;
            }

            if (Object is CadPoint cadPoint)
            {
                foreach( ContourPath contourPath in this.Paths)
                {
                    contourPath.Points.Remove(cadPoint);
                }
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
        /// Clear contour and set Base and Last object to null
        /// </summary>
        public void Clear()
        {
            this.BasePoint = null;
            this.BaseLenthConstrait = null;
            this.LastPoint = null;
        }

        public string GetNewPointName()
        {
            string result = string.Empty;
            int count = 0;
            foreach (ContourPath contourPath in this.Paths)
            {
                count += contourPath.Points.Count;
            }

            result = $"{Convert.ToChar(65 * ((count / 678) > 0 ? 1 : 0) + count / 678)}" +
                $"{Convert.ToChar(65 * ((count / 26) > 0 ? 1 : 0) + count / 26)}" +
                $"{Convert.ToChar(65 + count % 26)}";

            return result.Trim('\0');
        }

        public override string ToString()
        {
            return this.ID;
        }
    }
}
