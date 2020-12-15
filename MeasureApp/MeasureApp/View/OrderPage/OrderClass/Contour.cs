using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.Constraints;
using MeasureApp.ShapeObj.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.View.OrderPage.OrderClass
{
    public class Contour : INotifyPropertyChanged
    {
        public event EventHandler<object> ObjectAdded;

        public string ID = "Contour";

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
                    this.StartPoint = this.LastPoint;
                }
                OnPropertyChanged("Method");
            }
        }
        private DrawMethod _method = DrawMethod.StepByStep;

        public ConstraintLenth LastLenthConstrait { get; internal set; }
        public CadPoint StartPoint { get; internal set; }
        public CadPoint LastPoint { get; internal set; }

        public Contour(string Name)
        {
            this.ID = Name;
            Lenths = new List<ConstraintLenth>();
            Angles = new List<ConstraintAngle>();
            Paths = new List<ContourPath>();
            Paths.Add(new ContourPath());
        }
       

        public string GetLastID()
        {
            return string.Empty;
        }

        public object Add(object Object, int PathIndex)
        {
            if (Object is CadObject cadObject)
            {
                cadObject.Selected += CadObject_Selected;
                cadObject.Removed += CadObject_Removed;
            }

            if (Object is CadPoint cadPoint)
            {
                this.Paths[PathIndex].Points.Add(cadPoint);
                this.LastPoint = cadPoint;
                this.LastPoint.IsSelect = true;
                ObjectAdded?.Invoke(this.Paths[PathIndex], Object);
                return Object;
            }
            else if (Object is ConstraintLenth lenthConstrait)
            {
                this.Lenths.Add(lenthConstrait);
                this.LastLenthConstrait = lenthConstrait;
                this.LastLenthConstrait.IsSelect = true;
                ObjectAdded?.Invoke(this, Object);
                return Object;
            }
            else if(Object is ConstraintAngle angleConstrait)
            {
                this.Angles.Add(angleConstrait);
                ObjectAdded?.Invoke(this, Object);
                return Object;
            }
            return null;
        }

        private void CadObject_Removed(object sender, EventArgs e)
        {
            this.Remove(sender);
        }

        private void CadObject_Selected(object sender, bool e)
        {
            if (e == true)
            {
                if (sender is CadPoint cadPoint)
                {
                    if (this._method == DrawMethod.StepByStep)
                    {
                        if (this.LastPoint != null) this.LastPoint.IsSelect = false;
                        this.LastPoint = cadPoint;
                        this.StartPoint = cadPoint;
                    }
                    else
                    {
                        if (this.StartPoint != null) this.StartPoint.IsSelect = false;
                        this.StartPoint = cadPoint;
                        if (this.LastPoint == null) this.LastPoint = cadPoint;
                    }
                }
                if (sender is ConstraintLenth lenthConstrait)
                {
                    if (this.LastLenthConstrait != null)
                    {
                        this.LastLenthConstrait.IsSelect = false;
                    }
                    this.LastLenthConstrait = lenthConstrait;
                }
            }
        }

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
                this.StartPoint = this.StartPoint == cadPoint ? null : this.StartPoint;
            }
            else if (Object is ConstraintLenth lenthConstrait)
            {
                this.Lenths.Remove(lenthConstrait);
                this.LastLenthConstrait = this.LastLenthConstrait == lenthConstrait ? null : this.LastLenthConstrait;
            }
            else if (Object is ConstraintAngle angleConstrait)
            {
                this.Angles.Remove(angleConstrait);
            }
        }

        public void Clear()
        {
            this.StartPoint = null;
            this.LastLenthConstrait = null;
            this.LastPoint = null;
        }
    }
}
