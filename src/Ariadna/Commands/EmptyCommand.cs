using System;
using System.Windows.Input;

namespace Ariadna
{
    public class EmptyCommand : ICommand
    {
        public static EmptyCommand Instance = new EmptyCommand();

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {
        }

        public event EventHandler CanExecuteChanged;
    }
}