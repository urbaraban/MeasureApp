using MeasureApp.Data;
using SQLite;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.Orders
{
    public class Order : INotifyPropertyChanged
    {
        public OrderDataItem DataItem { get; }

        public Order()
        {
            this.DataItem = new OrderDataItem()
            {
                ID = 0,
                Name = "Templ",
                Details = "Templ",
                Location = string.Empty,
                XmlUrl = string.Empty,
                ImageUrl = string.Empty
            };
        }

        public Order(OrderDataItem orderDataItem)
        {
            this.DataItem = orderDataItem;
        }
        /// <summary>
        /// Name or other label
        /// </summary>
        /// 
        public int ID { get => this.DataItem.ID; }

        public string Name
        {
            get => this.DataItem.Name;
            set
            {
                this.DataItem.Name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Url image for picker
        /// </summary>
        public string ImageUrl
        {
            get => this.DataItem.ImageUrl;
            set
            {
                this.DataItem.ImageUrl = value;
            }
        }


        /// <summary>
        /// Location order.
        /// </summary>
        public string Location
        {
            get => this.DataItem.Location;
            set
            {
                this.DataItem.Location = value;
            }
        }


        public string Details
        {
            get => this.DataItem.Details;
            set
            {
                this.DataItem.Details = value;
                OnPropertyChanged("Details");
            }
        }


        public List<Contour> Contours = new List<Contour>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
