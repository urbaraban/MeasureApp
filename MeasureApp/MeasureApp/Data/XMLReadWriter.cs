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
                        contour.Add(new CadPoint()
                        {
                            ID = XPoint.Attribute("ID").Value,
                            X = double.Parse(XPoint.Attribute("X").Value, XMLReadWriter.dblfrm),
                            Y = double.Parse(XPoint.Attribute("Y").Value, XMLReadWriter.dblfrm),
                            IsSupport = bool.Parse(XPoint.Attribute("Support").Value),
                            IsFix = bool.Parse(XPoint.Attribute("Fix").Value)
                        });
                    }
                    //add lenth constraint
                    foreach (XElement XLenth in XContour.Element("Objects").Elements("Lenth"))
                    {
                        contour.Add(new ConstraintLenth(
                            contour.GetPointByName(XLenth.Attribute("P1").Value),
                            contour.GetPointByName(XLenth.Attribute("P2").Value),
                            double.Parse(XLenth.Attribute("Lenth").Value, XMLReadWriter.dblfrm),
                            bool.Parse(XLenth.Attribute("Support").Value)));
                    }
                    //add angle constraint
                    foreach (XElement XAngle in XContour.Element("Objects").Elements("Angle"))
                    {
                        contour.Add(new ConstraintAngle(
                            contour.GetLenthByName(XAngle.Attribute("Lenth1").Value),
                            contour.GetLenthByName(XAngle.Attribute("Lenth2").Value),
                            double.Parse(XAngle.Attribute("Angle").Value, XMLReadWriter.dblfrm)
                            ));
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
                if (cadObject is CadPoint cadPoint)
                {
                    return new XElement("Point",
                        new XAttribute("ID", cadPoint.ID),
                        new XAttribute("X", cadPoint.X),
                        new XAttribute("Y", cadPoint.Y),
                        new XAttribute("Support", cadPoint.IsSupport),
                        new XAttribute("Fix", cadPoint.IsFix));
                }
                else if (cadObject is ConstraintLenth constraintLenth)
                {
                    return new XElement("Lenth",
                        new XAttribute("P1", constraintLenth.Point1.ID),
                        new XAttribute("P2", constraintLenth.Point2.ID),
                        new XAttribute("Lenth", constraintLenth.Value),
                        new XAttribute("Support", constraintLenth.IsSupport),
                        new XAttribute("Select", constraintLenth.IsSelect));
                }
                else if (cadObject is ConstraintAngle constraintAngle)
                {
                    return new XElement("Angle",
                        new XAttribute("Lenth1", constraintAngle.Lenth1.ID),
                        new XAttribute("Lenth2", constraintAngle.Lenth2.ID),
                        new XAttribute("Angle", constraintAngle.Value));
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
