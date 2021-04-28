using SureMeasure.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace SureMeasure.Orders
{
    public class Order : INotifyPropertyChanged
    {
        public bool IsAlive
        {
            get
            {
                foreach (Contour contour in this.Contours)
                {
                    foreach(ContourPath contourPath in contour.Paths)
                    {
                        if (contourPath.Count > 0) return true;
                    }
                }
                return false;
            }
        }

        public OrderDataItem DataItem { get; }

        public Order()
        {
            this.DataItem = new OrderDataItem()
            {
                ID = 0,
                Date = DateTime.Now,
                Name = "Templ",
                Details = "Templ",
                PhoneNumber = "+7923251320",
                Location = new Location(),
                XmlUrl = string.Empty,
                ImagesUrls = string.Empty
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

        public string Date => this.DataItem.Date.ToLocalTime().ToString("d:M:y");

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
        public List<string> ImagesUrls
        {
            get => new List<string>(this.DataItem.ImagesUrls.Split('%'));
            set
            {
                this.DataItem.ImagesUrls = string.Concat(value, '%');
                OnPropertyChanged("ImagesUrls");
            }
        }


        /// <summary>
        /// Location order.
        /// </summary>
        public Location Location
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

        public string Phone
        {
            get => this.DataItem.PhoneNumber == null ? "+7999999" : this.DataItem.PhoneNumber;
            set
            {
                this.DataItem.PhoneNumber = value;
                OnPropertyChanged("Phone");
            }
        }

        public double Perimetr
        {
            get
            {
                double perimetr = 0;
                foreach (Contour contour in this.Contours)
                {
                    perimetr += contour.Perimetr;
                }
                return Math.Round(perimetr / 1000, 1);
            }
        }

        public double Area
        {
            get
            {
                double area = 0;
                foreach (Contour contour in this.Contours)
                {
                    area += contour.Area;
                }
                return Math.Round(area / 1000000, 2, MidpointRounding.ToEven);
            }
        }

        public List<Contour> Contours = new List<Contour>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void UpdateBinding()
        {
            OnPropertyChanged("Area");
            OnPropertyChanged("Perimetr");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
