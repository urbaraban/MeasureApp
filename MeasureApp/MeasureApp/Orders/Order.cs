using SureMeasure.Data;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using System.IO;
using System.Collections.ObjectModel;

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
                Name = "{NewOrder}",
                Details = string.Empty,
                PhoneNumber = string.Empty,
                Location = new Location(),
                XmlUrl = string.Empty,
                ImagesUrls = string.Empty
            };
        }

        public Order(OrderDataItem orderDataItem)
        {
            this.DataItem = orderDataItem;

            string[] pathsImages = this.DataItem.ImagesUrls.Split('%');
            this.ImagesUrls.Clear();
            foreach (string path in pathsImages)
            {
                if (File.Exists(path) == true) this.ImagesUrls.Add(path);
            }
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
        public ObservableCollection<string> ImagesUrls { get; set; } = new ObservableCollection<string>();


        public void AddPhoto(string Path)
        {
            if (string.IsNullOrEmpty(Path) == false)
            {
                ImagesUrls.Add(Path);
                this.DataItem.ImagesUrls = string.Join('%', ImagesUrls).Replace("%%", "%");
                OnPropertyChanged("ImagesUrls");
            }
        }

        public void RemovePhoto(string path)
        {
            ImagesUrls.Remove(path);
            OnPropertyChanged("ImagesUrls");
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

        public string Adress
        {
            get => this.DataItem.Adress;
            set
            {
                this.DataItem.Adress = value;
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

        public ObservableCollection<Contour> Contours = new ObservableCollection<Contour>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void UpdateBinding()
        {
            OnPropertyChanged("Area");
            OnPropertyChanged("Perimetr");
            OnPropertyChanged("ImagesUrls");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
