using SQLite;

namespace MeasureApp.Data
{
    public class OrderDataItem
    {
        private string imageUrl = string.Empty;
        private string location = string.Empty;

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
        public string ImageUrl { get => imageUrl == string.Empty ? "SureMeasureLogo.png": imageUrl;
            set
            {
                imageUrl = value;
            }
        }  

        /// <summary>
        /// Location order.
        /// </summary>
        public string Location { get => location == string.Empty ? "{Location_empty}" : location;
            set 
            {
                location = value;
            }
        }

        public string Details { get; set; }

        /// <summary>
        /// Url to xml file
        /// </summary>
        public string XmlUrl { get; set; }
    }
}
