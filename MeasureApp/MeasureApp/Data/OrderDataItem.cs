using SQLite;

namespace MeasureApp.Data
{
    public class OrderDataItem
    {
        [PrimaryKey, AutoIncrement]
        /// <summary>
        /// Name or other label
        /// </summary>
        /// 
        public int ID { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Url image for picker
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Location order.
        /// </summary>
        public string Location { get; set; }

        public string Details { get; set; }

        /// <summary>
        /// Url to xml file
        /// </summary>
        public string XmlUrl { get; set; }
    }
}
