using MeasureApp.CadObjects;
using MeasureApp.CadObjects.Constraints;
using MeasureApp.CadObjects.Interface;
using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.Canvas;
using MeasureApp.ShapeObj.Constraints;
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

        public List<ConstraintLenth> Lenths { get; set; } = new List<ConstraintLenth>();

        public List<ConstraintAngle> Angles { get; set; } = new List<ConstraintAngle>();

        public List<ContourPath> Paths { get; set; } = new List<ContourPath>();

        public List<CadPoint> Points { get; set; } = new List<CadPoint>();

        /// <summary>
        /// Perimetr controur
        /// </summary>
        public double Perimetr {
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

        public double Area
        {
            get
            {
                double area = 0;
                foreach (ContourPath contourPath in Paths)
                {
                    area += contourPath.Area;
                }
                return 0;
            }
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
                if (cadPoint.ID == ID) 
                    return cadPoint;
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
                this.Points.Add(cadPoint);
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
                FindContourForLenth(lenthConstrait);
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

        private void FindContourForLenth (ConstraintLenth constraintLenth)
        {
            bool WithoutContour = true;
            if (constraintLenth.IsSupport == false)
            {
                foreach (ContourPath contourPath in this.Paths)
                {
                    if (contourPath.IsClosed == false)
                    {
                        if (contourPath.CheckInsertLenth(constraintLenth) == true)
                        {
                            contourPath.Lenths.Add(constraintLenth);
                            WithoutContour = false;
                        }
                    }
                }
                if (WithoutContour == true)
                {
                    this.Paths.Add(new ContourPath(this.Paths.Count.ToString())
                    {
                        Lenths = new List<ConstraintLenth>() { constraintLenth }
                    });
                }
            }
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

            result = $"{Convert.ToChar(65 * ((Points.Count / 678) > 0 ? 1 : 0) + Points.Count / 678)}" +
                $"{Convert.ToChar(65 * ((Points.Count / 26) > 0 ? 1 : 0) + Points.Count / 26)}" +
                $"{Convert.ToChar(65 + Points.Count % 26)}";

            return result.Trim('\0');
        }

        public override string ToString() => this.ID;
    }
}
