using SQLite;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace SureMeasure.Data
{
    public class OrderDataItem
    {
        private string imagesUrls = string.Empty;


        [PrimaryKey, AutoIncrement]
        /// <summary>
        /// Name or other label
        /// </summary>
        /// 
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Url image for picker
        /// </summary>
        public string ImagesUrls
        {
            get => imagesUrls == string.Empty ? "SureMeasureLogo.png" : imagesUrls;
            set
            {
                imagesUrls = string.Concat(value,'%');
            }
        }  

        /// <summary>
        /// Location order.
        /// </summary>
        public string Adress 
        {
            get => Location == null ? "Empty" : Location.Latitude.ToString();
        }
        public Location Location;

        public string Details { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// Url to xml file
        /// </summary>
        public string XmlUrl { get; set; }
    }
}
