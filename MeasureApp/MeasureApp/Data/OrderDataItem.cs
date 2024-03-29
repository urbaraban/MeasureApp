﻿using SQLite;
using System;

namespace SureOrder.Data
{
    public class OrderDataItem
    {
        [PrimaryKey, AutoIncrement]
        /// <summary>
        /// Name or other label
        /// </summary>
        /// 
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public double Area { get; set; }

        public double Perimetre { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Url image for picker
        /// </summary>
        public string ImagesUrls { get; set; }

        /// <summary>
        /// Location order.
        /// </summary>
        public string Adress { get; set; }

        public string Details { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// Url to xml file
        /// </summary>
        public string XmlUrl { get; set; }

        public double Cost { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.InWork;
    }

    public enum OrderStatus : int
    {
        InWork = 0,
        Solved = 1
    }
}
