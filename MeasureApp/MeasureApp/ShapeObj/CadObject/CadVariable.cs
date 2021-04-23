using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeasureApp.ShapeObj
{
    public class CadVariable : INotifyPropertyChanged
    {
        private double _value;

        public double Value
        {
            get => this._value;
            set
            {
                this._value = value;
                OnPropertyChanged("Value");
            }
        }

        public string Name = string.Empty;

        /// <summary>
        /// Lenth or Angle flag
        /// </summary>
        public bool IsLenth { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString() => this._value.ToString();


        public CadVariable(double Value, bool IsLenth)
        {
            this._value = Value;
            this.IsLenth = IsLenth;
        }

        public CadVariable(double Value, string Name, bool IsLenth)
        {
            this._value = Value;
            this.Name = Name;
            this.IsLenth = IsLenth;
        }

        public void Update(double Value)
        {
            this._value = Value;
        }
    }
}
