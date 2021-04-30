﻿using SureMeasure.CadObjects;
using SureMeasure.ShapeObj.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace SureMeasure.Orders
{
    public class ContourPath : INotifyPropertyChanged, OrderObjectInt, IList<ConstraintLenth>
    {
        public bool IsSubstract { get; set; } = false;


        public Rect Bounds
        {
            get
            {
                CadPoint[] cadPoints = this.Points;
                double minX = cadPoints[0].X, minY = cadPoints[0].Y, maxX = cadPoints[0].X, maxY = cadPoints[0].Y;
                foreach(CadPoint cadPoint in cadPoints)
                {
                    minX = Math.Min(minX, cadPoint.X);
                    minY = Math.Min(minY, cadPoint.Y);
                    maxX = Math.Min(maxX, cadPoint.X);
                    maxY = Math.Min(maxY, cadPoint.Y);
                }
                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        /// <summary>
        /// Contour sqare
        /// </summary> 
        public double Area
        {
            get 
            {
                double area = 0;
                if (IsClosed == true)
                {
                    for (int i = 0; i < this.Points.Length; i += 1)
                    {
                        int index2 = (i + 1) % (Points.Length);

                        area += Points[i].X * Points[index2].Y - Points[i].Y * Points[index2].X;
                    }
                }
                return Math.Abs(area) / 2d;
            }
        }

        public double Perimeter
        {
            get
            {
                double lenth = 0;

                for (int i = 0; i < Lenths.Count; i += 1)
                {
                    lenth += Lenths[i].Lenth;
                }

                return lenth;
            }
        }

        /// <summary>
        /// Gabarit Sqare
        /// </summary>
        public double GabaritSqare
        {
            get => 0;
        }

        public bool IsClosed
        {
            get
            {
                if (this.Lenths.Count > 2)
                {
                   return this.Lenths[0].Point1 == this.Lenths[Lenths.Count - 1].Point2;
                }
                return false;
            }
        }


        private List<ConstraintLenth> Lenths = new List<ConstraintLenth>();

        public CadPoint[] Points
        {
            get
            {
                if (this.Lenths.Count > 0)
                {
                    CadPoint[] points = new CadPoint[this.Lenths.Count + (this.IsClosed == true ? 0 : 1)];
                    
                    for (int i = 0; i < Lenths.Count; i += 1)
                    {
                        points[i] = this.Lenths[i].Point1;
                    }
                    if (this.IsClosed == false) points[points.Length - 1] = this.Lenths[Lenths.Count - 1].Point2;
                    return points;
                }
                return null;
            }
        }

        public string Color = "#0000FF";

        public string ID { get; set; }

        public int Count => ((ICollection<ConstraintLenth>)Lenths).Count;

        public bool IsReadOnly => ((ICollection<ConstraintLenth>)Lenths).IsReadOnly;

        public ConstraintLenth this[int index] { get => ((IList<ConstraintLenth>)Lenths)[index]; set => ((IList<ConstraintLenth>)Lenths)[index] = value; }

        public ContourPath(string ID)
        {
            this.ID = ID;
        }

        public void Add(ConstraintLenth constraintLenth)
        {
            if (constraintLenth != null)
            {
                if (this.Lenths.Count > 0)
                {
                    for (int i = 0; i < Lenths.Count; i += 1)
                    {
                        if (Lenths[i].GetNotThisPoint(constraintLenth.Point1) != null)
                        {
                            if (i + 1 < this.Lenths.Count - 1)
                            {
                                Lenths.Insert(i + 1, constraintLenth);
                            }
                            else
                            {
                                Lenths.Add(constraintLenth);
                            }
                            return;
                        }
                    }
                }
                else
                {
                    this.Lenths.Add(constraintLenth);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString() => this.ID;

        internal bool CheckInsertLenth(ConstraintLenth constraintLenth)
        {
            foreach (ConstraintLenth constraint in this.Lenths)
            {
                if (constraintLenth.Point1.ID == constraint.Point1.ID ||
                    constraintLenth.Point2.ID == constraint.Point1.ID ||
                    constraintLenth.Point1.ID == constraint.Point2.ID ||
                    constraintLenth.Point2.ID == constraint.Point2.ID)
                    return true;
            }
            return false;
        }

        public int IndexOf(ConstraintLenth item)
        {
            return ((IList<ConstraintLenth>)Lenths).IndexOf(item);
        }

        public void Insert(int index, ConstraintLenth item)
        {
            ((IList<ConstraintLenth>)Lenths).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ConstraintLenth>)Lenths).RemoveAt(index);
        }

        public void Clear()
        {
            ((ICollection<ConstraintLenth>)Lenths).Clear();
        }

        public bool Contains(ConstraintLenth item)
        {
            return ((ICollection<ConstraintLenth>)Lenths).Contains(item);
        }

        public void CopyTo(ConstraintLenth[] array, int arrayIndex)
        {
            ((ICollection<ConstraintLenth>)Lenths).CopyTo(array, arrayIndex);
        }

        public bool Remove(ConstraintLenth item)
        {
            return ((ICollection<ConstraintLenth>)Lenths).Remove(item);
        }

        public IEnumerator<ConstraintLenth> GetEnumerator()
        {
            return ((IEnumerable<ConstraintLenth>)Lenths).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Lenths).GetEnumerator();
        }
    }
}