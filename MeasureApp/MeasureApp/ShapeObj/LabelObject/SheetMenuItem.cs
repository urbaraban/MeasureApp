using System;
using System.Windows.Input;

namespace MeasureApp.ShapeObj.LabelObject
{
    public class SheetMenuItem : ICommand
    {
        public string Name { get; set; }
        public ICommand Command;

        public SheetMenuItem(ICommand Command, string Name)
        {
            this.Name = Name;
            this.Command = Command;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                Command.CanExecuteChanged += value;
            }

            remove
            {
                Command.CanExecuteChanged -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return Command.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            Command.Execute(parameter);
        }
    }
}
