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


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString() => this._value.ToString();


        public CadVariable(double Value)
        {
            this._value = Value;
        }

        public CadVariable(double Value, string Name)
        {
            this._value = Value;
            this.Name = Name;
        }

        public void Update(double Value)
        {
            this._value = Value;
        }
    }
}
