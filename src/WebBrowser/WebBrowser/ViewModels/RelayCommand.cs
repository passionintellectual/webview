using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WebBrowser.ViewModels
{
    public class RelayCommand<T> : ICommand where T:class 
    {
        private Action<T> _execute = null;
        private Predicate<T> _predicate = null;
        public RelayCommand(Action<T> action, Predicate<T> predicate)
        {
            _execute = action;
            _predicate = predicate;
        }
        public bool CanExecute(object parameter)
        {
            return _predicate.Invoke((T)parameter);
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            _execute.Invoke((T) parameter);
        }
    }
}
