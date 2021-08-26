using DrawEngine;
using DrawEngine.CadObjects;
using DrawEngine.Constraints;
using SureMeasure.Orders;
using SureOrder.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SureMeasure.Data
{
    public static class XMLReadWriter
    {
        private static NumberFormatInfo dblfrm = new NumberFormatInfo()
        {
            NegativeSign = "-"
        };

        public async static Task<Order> Read(OrderDataItem dataItem)
        {
            if (File.Exists(dataItem.XmlUrl) == true)
            {
                XDocument xml = XDocument.Load(dataItem.XmlUrl);
                XElement Measure = xml.Root;
                Order order = new Order(dataItem);

                //Add Contour
                XElement XContours = Measure.Element("Contours");
                foreach(XElement XContour in XContours.Elements("Contour"))
                {
                    Contour contour = new Contour(XContour.Attribute("ID").Value);
                    order.Contours.Add(contour);
                    //Add Paths

                    foreach (XElement XPoint in XContour.Element("Objects").Elements("Point"))
                    {
                        contour.Add(new CadAnchor()
                        {
                            ID = XPoint.Attribute("ID").Value,
                            Point = new CadPoint()
                            {
                                X = double.Parse(XPoint.Attribute("X").Value, XMLReadWriter.dblfrm),
                                Y = double.Parse(XPoint.Attribute("Y").Value, XMLReadWriter.dblfrm)
                            },
                            IsSupport = bool.Parse(XPoint.Attribute("Support").Value),
                            IsFix = bool.Parse(XPoint.Attribute("Fix").Value)
                        });
                    }
                    //add lenth constraint
                    foreach (XElement XLenth in XContour.Element("Objects").Elements("Lenth"))
                    {
                        OrientationStat orientationStat = (OrientationStat)int.Parse(XLenth.Attribute("Orientation").Value);
                        contour.Add(new LenthConstraint()
                        {
                            Anchor1 = contour.GetPointByName(XLenth.Attribute("P1").Value),
                            Anchor2 = contour.GetPointByName(XLenth.Attribute("P2").Value),
                            Variable = new CadVariable(double.Parse(XLenth.Attribute("Lenth").Value, XMLReadWriter.dblfrm)),
                            Orientation = (OrientationStat)int.Parse(XLenth.Attribute("Orientation").Value),
                            IsSupport = bool.Parse(XLenth.Attribute("Support").Value),
                            IsFix = bool.Parse(XLenth.Attribute("Fix").Value)
                        });
                    }
                    //add angle constraint
                    foreach (XElement XAngle in XContour.Element("Objects").Elements("Angle"))
                    {
                        contour.Add(new AngleConstraint()
                        {
                            Lenth1 = contour.GetLenthByName(XAngle.Attribute("Lenth1").Value),
                            Lenth2 = contour.GetLenthByName(XAngle.Attribute("Lenth2").Value),
                            Variable = new CadVariable(double.Parse(XAngle.Attribute("Angle").Value, XMLReadWriter.dblfrm)),
                            IsFix = bool.Parse(XAngle.Attribute("Fix").Value)
                        });                          
                    }
                }

                return order;
            }
            return null;
        }

        public static void Write(Order order, string Path)
        {
            XElement Measure = new XElement("Measure");

            XElement info = new XElement("Info");
            info.Add(new XElement("ID", order.ID));
            info.Add(new XElement("Name", order.Name));
            info.Add(new XElement("Details", order.Details));
            info.Add(new XElement("Phone", order.Phone));
            info.Add(new XElement("Adress", order.Adress));
            info.Add(new XElement("ImagesUrls", string.Concat(order.ImagesUrls, '%')));
            Measure.Add(info);

            XElement Contours = new XElement("Contours");
            //add contour to order
            foreach(Contour contour in order.Contours)
            {
                XElement XContour = new XElement("Contour", new XAttribute("ID", contour.ID));
                //Add contour element
                XElement XObjects = new XElement("Objects");
                foreach(ICadObject cadObject in contour)
                {
                    XObjects.Add(GetObjectElement(cadObject));
                }
                XContour.Add(XObjects);
                Contours.Add(XContour);
            }
            Measure.Add(Contours);

            XDocument xml = new XDocument();
            xml.Add(Measure);
            //Check path
            if (File.Exists(order.DataItem.XmlUrl) == false)
            {
                order.DataItem.XmlUrl = Path;
            }
             xml.Save(order.DataItem.XmlUrl);

            static XElement GetObjectElement(ICadObject cadObject)
            {
                if (cadObject is CadAnchor cadPoint)
                {
                    return new XElement("Point",
                        new XAttribute("ID", cadPoint.ID),
                        new XAttribute("X", cadPoint.Point.X),
                        new XAttribute("Y", cadPoint.Point.Y),
                        new XAttribute("Support", cadPoint.IsSupport),
                        new XAttribute("Fix", cadPoint.IsFix));
                }
                else if (cadObject is LenthConstraint constraintLenth)
                {
                    return new XElement("Lenth",
                        new XAttribute("P1", constraintLenth.Anchor1.ID),
                        new XAttribute("P2", constraintLenth.Anchor2.ID),
                        new XAttribute("Lenth", constraintLenth.Value),
                        new XAttribute("Orientation", (int)constraintLenth.Orientation),
                        new XAttribute("Support", constraintLenth.IsSupport),
                        new XAttribute("Select", constraintLenth.IsSelect),
                        new XAttribute("Fix", constraintLenth.IsFix));
                }
                else if (cadObject is AngleConstraint constraintAngle)
                {
                    return new XElement("Angle",
                        new XAttribute("Lenth1", constraintAngle.Lenth1.ID),
                        new XAttribute("Lenth2", constraintAngle.Lenth2.ID),
                        new XAttribute("Angle", constraintAngle.Value),
                        new XAttribute("Fix", constraintAngle.IsFix));
                }
                return null;
            }
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
