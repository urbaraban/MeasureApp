using System.Windows.Input;
using Xamarin.Forms;

namespace SureMeasure.Orders
{
    using DrawEngine;
    using SureOrder.Data;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Defines the <see cref="Order" />.
    /// </summary>
    public class Order : INotifyPropertyChanged
    {
        public Contour SelectContour
        {
            get
            {
                if (this._selectcontour == null)
                {
                    if (this.Contours.Count < 1) this.Contours.Add(new Contour($"Contour {this.Contours.Count + 1}"));
                    this._selectcontour = this.Contours.First();
                }
                return this._selectcontour;
            }
            set
            {
                this._selectcontour = value;
                OnPropertyChanged("SelectContour");
            }
        }

        private Contour _selectcontour;

        /// <summary>
        /// Gets a value indicating whether IsAlive.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                foreach (Contour contour in this.Contours)
                {
                    return contour.Count > 0;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the DataItem.
        /// </summary>
        public OrderDataItem DataItem { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        public Order()
        {
            this.DataItem = new OrderDataItem()
            {
                ID = 0,
                Date = DateTime.Now,
                Name = "{NewOrder}",
                Details = string.Empty,
                PhoneNumber = string.Empty,
                XmlUrl = string.Empty,
                ImagesUrls = string.Empty
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="orderDataItem">The orderDataItem<see cref="OrderDataItem"/>.</param>
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
        /// Gets the ID
        /// Name or other label.
        /// </summary>
        public int ID { get => this.DataItem.ID; }

        /// <summary>
        /// Gets the Date.
        /// </summary>
        public string Date => this.DataItem.Date.ToLocalTime().ToString("d:M:y");

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
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
        /// Gets or sets the ImagesUrls
        /// Url image for picker.
        /// </summary>
        public ObservableCollection<string> ImagesUrls { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// The AddPhoto.
        /// </summary>
        /// <param name="Path">The Path<see cref="string"/>.</param>
        public void AddPhoto(string Path)
        {
            if (string.IsNullOrEmpty(Path) == false)
            {
                ImagesUrls.Add(Path);
                this.DataItem.ImagesUrls = string.Join('%', ImagesUrls).Replace("%%", "%");
                OnPropertyChanged("ImagesUrls");
            }
        }

        /// <summary>
        /// The RemovePhoto.
        /// </summary>
        /// <param name="path">The path<see cref="string"/>.</param>
        public void RemovePhoto(string path)
        {
            ImagesUrls.Remove(path);
            OnPropertyChanged("ImagesUrls");
        }

        /// <summary>
        /// Gets or sets the Location
        /// Location order..
        /// </summary>


        /// <summary>
        /// Gets or sets the Adress.
        /// </summary>
        public string Adress
        {
            get => this.DataItem.Adress;
            set
            {
                this.DataItem.Adress = value;
            }
        }

        /// <summary>
        /// Gets or sets the Details.
        /// </summary>
        public string Details
        {
            get => this.DataItem.Details;
            set
            {
                this.DataItem.Details = value;
                OnPropertyChanged("Details");
            }
        }

        /// <summary>
        /// Gets or sets the Phone.
        /// </summary>
        public string Phone
        {
            get => this.DataItem.PhoneNumber == null ? "+7999999" : this.DataItem.PhoneNumber;
            set
            {
                this.DataItem.PhoneNumber = value;
                OnPropertyChanged("Phone");
            }
        }

        /// <summary>
        /// Gets or sets the Cost.
        /// </summary>
        public double Cost
        {
            get => this.DataItem.Cost;
            set
            {
                this.DataItem.Cost = value;
                OnPropertyChanged("Cost");
            }
        }

        /// <summary>
        /// Gets the Perimetre.
        /// </summary>
        public double Perimetre
        {
            get
            {
                double perimetr = 0;
                foreach (Contour contour in this.Contours)
                {
                    perimetr += contour.Perimetr;
                }
                this.DataItem.Perimetre = Math.Round(perimetr / 1000, 1);
                return this.DataItem.Perimetre;
            }
        }

        /// <summary>
        /// Gets the Area.
        /// </summary>
        public double Area
        {
            get
            {
                double area = 0;
                foreach (Contour contour in this.Contours)
                {
                    area += contour.Area;
                }
                this.DataItem.Area = Math.Round(area / 1000000, 2, MidpointRounding.ToEven);
                return this.DataItem.Area;
            }
        }

        /// <summary>
        /// Defines the Contours.
        /// </summary>
        public ObservableCollection<Contour> Contours { get; set; } = new ObservableCollection<Contour>();

        /// <summary>
        /// Defines the PropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The OnPropertyChanged.
        /// </summary>
        /// <param name="prop">The prop<see cref="string"/>.</param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// The UpdateBinding.
        /// </summary>
        public void UpdateBinding()
        {
            OnPropertyChanged("Area");
            OnPropertyChanged("Perimetre");
            OnPropertyChanged("Sqare");
            OnPropertyChanged("ImagesUrls");
        }

        /// <summary>
        /// The ToString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string ToString()
        {
            return Name;
        }

        public ICommand AddContour => new Command(()=> 
        {
            Contour tempContor = new Contour($"Contour {this.Contours.Count + 1}");
            this.Contours.Add(tempContor);
            this.SelectContour = tempContor;
        });

        public ICommand RemoveContour =>  new Command(()=> 
        {
            if (this.Contours.Count > 1)
            {
                this.Contours.Remove(this.SelectContour);
                this.SelectContour = this.Contours.Last();
            }
            else
            {
                this.SelectContour.ClearContour();
            }
        });

        public ICommand Undo => new Command(() =>
        {
            this.SelectContour.Last().TryRemove();
        });

        public ICommand Redo => new Command(() =>
        {

        });


        public ICommand Clear => new Command(() => { this.SelectContour.ClearContour(); });


    }
}
