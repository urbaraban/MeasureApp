using MeasureApp.ShapeObj.Constraints;
using MeasureApp.Tools;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace MeasureApp.ShapeObj
{
    public class Canvas : AbsoluteLayout
    {
        public AnchorAnchorLenth SelectedLine { get; internal set; }

        public Canvas()
        {
            this.ChildAdded += Canvas_ChildAdded;
        }

        private void Canvas_ChildAdded(object sender, ElementEventArgs e)
        {

        }

        public void Clear()
        {
            this.Children.Clear();
            this.SelectedLine = null;
        }

        public void Remove(CadObject cadObject)
        {
            this.Children.Remove(cadObject);
            this.SelectedLine = null;
        }

        public void AddLine(double lineLenth, double lineAngle)
        {
            if (SelectedLine == null)
            {
                CadAnchor cadAnchor1 = new CadAnchor(new CadPoint(100, 30), 7);
                CadAnchor cadAnchor2 = new CadAnchor(new CadPoint(lineLenth + 100, 30), 7);
                
                CadLine cadLine = new CadLine(new AnchorAnchorLenth(cadAnchor1, cadAnchor2, lineLenth));
                //new PointLineMerge(cadLine, cadAnchor1, 1);
                //new PointLineMerge(cadLine, cadAnchor2, 1);

                this.Children.Add(cadAnchor1);
                this.Children.Add(cadAnchor2);
                this.Children.Add(cadLine);

                cadLine.Update();

                SelectedLine = cadLine.Anchors;
            }
            else
            {
                if (SelectedLine is AnchorAnchorLenth anchorAnchor)
                {
                    CadAnchor cadAnchor2 = new CadAnchor(Sizing.GetPositionLineFromAngle(anchorAnchor.Anchor1.cadPoint, anchorAnchor.Anchor2.cadPoint, lineLenth, lineAngle));

                    CadLine cadLine = new CadLine(new AnchorAnchorLenth(anchorAnchor.Anchor2, cadAnchor2, lineLenth));
                    //new PointLineMerge(cadLine, anchorAnchor.Anchor2, 1);
                    //new PointLineMerge(cadLine, cadAnchor2, 1);

                    new AngleBetweenThreeAnchor(anchorAnchor, cadLine.Anchors, lineAngle);

                    this.Children.Add(cadAnchor2);
                    this.Children.Add(cadLine);



                    SelectedLine = cadLine.Anchors;
                }
            }
                // 

                

            
           // this.Children.Add(cadLine);
  

           // Point point2 = new Point(100, 100);

           /* 
                this.Children.Add(new CadLine(new Point(0, 0), point2, lineAngle));
                this.SelectedObject = (CadLine)this.Children.Last();
            }
  
                if (SelectedObject is CadLine cadLine)
                {
                    this.Children.Add(cadLine.GetPositionLineFromAngle((float)lineLenth, (float)lineAngle));
                    this.SelectedObject = (CadLine)this.Children.Last();
                }
            }*/

            
        }
    }
}
