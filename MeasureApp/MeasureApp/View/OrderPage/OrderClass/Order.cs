using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.View.OrderPage.OrderClass
{
    public class Order : INotifyPropertyChanged
    {
        /// <summary>
        /// Name or other label
        /// </summary>
        public string Name
        {
            get => this._name;
            set
            {
                this._name = value;
                OnPropertyChanged("Name");
            }
        }
        private string _name = string.Empty;

        /// <summary>
        /// Url image for picker
        /// </summary>
        public string ImageUrl
        {
            get => this._imageurl;
            set
            {
                this._imageurl = value;
            }
        }
        private string _imageurl;

        /// <summary>
        /// Location order.
        /// </summary>
        public string Location
        {
            get => this._location;
            set
            {
                this._location = value;
            }
        }
        private string _location;

        public string Details
        {
            get => this._details;
            set
            {
                this._details = value;
                OnPropertyChanged("Details");
            }
        }
        private string _details;

        public List<Contour> Contours = new List<Contour>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
