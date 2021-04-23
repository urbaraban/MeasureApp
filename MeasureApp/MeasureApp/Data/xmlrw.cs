using MeasureApp.Orders;
using MeasureApp.ShapeObj;
using MeasureApp.ShapeObj.Constraints;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace MeasureApp.Data
{
    public static class xmlrw
    {
        private static NumberFormatInfo dblfrm = new NumberFormatInfo()
        {
            NegativeSign = "-"
        };

        public static Order Read(OrderDataItem dataItem)
        {
            if (File.Exists(dataItem.XmlUrl) == true)
            {
                XDocument xml = XDocument.Load(dataItem.XmlUrl);
                XElement Measure = xml.Root;
                Order order = new Order(dataItem);

                //Add Contour
                XElement XContours = Measure.Element("Contours");
                foreach(XElement XContour in XContours.Descendants("Contour"))
                {
                    Contour contour = new Contour(XContour.Attribute("ID").Value);
                    order.Contours.Add(contour);
                    //Add Paths
                    XElement XPaths = XContour.Element("Paths");
                    foreach (XElement XPath in XPaths.Descendants("Path"))
                    {
                        ContourPath contourPath = new ContourPath(XPath.Attribute("ID").Value);
                        foreach(XElement XPoint in XPath.Descendants("Point"))
                        {
                            contourPath.Points.Add(new CadPoint()
                            {
                                ID = XPoint.Attribute("ID").Value,
                                X = double.Parse(XPoint.Attribute("X").Value, xmlrw.dblfrm),
                                Y = double.Parse(XPoint.Attribute("Y").Value, xmlrw.dblfrm),
                                IsSupport = bool.Parse(XPoint.Attribute("Support").Value)
                            });
                        }
                        contour.Paths.Add(contourPath);
                    }
                    //add lenth constraint
                    XElement XLenths = XContour.Element("Lenths");
                    foreach (XElement XLenth in XLenths.Descendants("Lenth"))
                    {
                        contour.Lenths.Add(new ConstraintLenth(
                            contour.GetPointByName(XLenth.Attribute("P1").Value),
                            contour.GetPointByName(XLenth.Attribute("P2").Value),
                            double.Parse(XLenth.Attribute("Lenth").Value, xmlrw.dblfrm),
                            bool.Parse(XLenth.Attribute("Support").Value)));
                    }
                    //add angle constraint
                    XElement XAngles = XContour.Element("Angles");
                    foreach (XElement XAngle in XAngles.Descendants("Angle"))
                    {
                        contour.Angles.Add(new ConstraintAngle(
                            contour.GetLenthByName(XAngle.Attribute("Lenth1").Value),
                            contour.GetLenthByName(XAngle.Attribute("Lenth2").Value),
                            double.Parse(XAngle.Attribute("Angle").Value, xmlrw.dblfrm)
                            ));
                    }
                    
                }

                return order;
            }
            return null;
        }

        public static void Write(Order order)
        {
            XElement Measure = new XElement("Measure");

            XElement info = new XElement("Info");
            info.Add(new XElement("ID", order.ID));
            info.Add(new XElement("Name", order.Name));
            info.Add(new XElement("Details", order.Details));
            info.Add(new XElement("Phone", order.Phone));
            info.Add(new XElement("Location", order.Location));
            info.Add(new XElement("ImagesUrls", order.ImagesUrls));
            Measure.Add(info);

            XElement Contours = new XElement("Contours");
            //add contour to order
            foreach(Contour contour in order.Contours)
            {
                XElement XContour = new XElement("Contour", new XAttribute("ID", contour.ID));
                //Add contour element

                //add path and point
                XElement XPaths = new XElement("Paths");
                foreach (ContourPath contourPath in contour.Paths)
                {
                    XElement XPath = new XElement("Path", new XAttribute("ID", contourPath.ID));
                    foreach(CadPoint cadPoint in contourPath.Points)
                    {
                        XPath.Add(new XElement("Point",
                            new XAttribute("ID", cadPoint.ID),
                            new XAttribute("X", cadPoint.X),
                            new XAttribute("Y", cadPoint.Y),
                            new XAttribute("Support", cadPoint.IsSupport)));
                    }
                    XPaths.Add(XPath);
                }
                XContour.Add(XPaths);

                //add lenth
                XElement XLenths = new XElement("Lenths");
                foreach (ConstraintLenth lenth in contour.Lenths)
                {
                    XLenths.Add(new XElement("Lenth",
                        new XAttribute("P1", lenth.Point1.ID),
                        new XAttribute("P2", lenth.Point2.ID),
                        new XAttribute("Lenth", lenth.Lenth),
                        new XAttribute("Support", lenth.IsSupport)));
                }
                XContour.Add(XLenths);

                //add angle
                XElement Angles = new XElement("Angles");
                foreach(ConstraintAngle Angle in contour.Angles)
                {
                    Angles.Add(new XElement("Angle",
                        new XAttribute("Lenth1", Angle.anchorAnchor1.ID),
                        new XAttribute("Lenth2", Angle.anchorAnchor2.ID),
                        new XAttribute("Angle", Angle.Value)));
                }
                XContour.Add(Angles);

                Contours.Add(XContour);
            }
            Measure.Add(Contours);

            XDocument xml = new XDocument();
            xml.Add(Measure);
            //Check path
            if (File.Exists(order.DataItem.XmlUrl) == false)
            {
                order.DataItem.XmlUrl = Constants.NewOrderPath;
            }
             xml.Save(order.DataItem.XmlUrl);
 
        }

        public static void Remove(string path)
        {
            if (File.Exists(path) == true)
            {
                File.Delete(path);
            }
           
        }
    }
}
