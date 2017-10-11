using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI
{
   
    public class Command : ICommand
    {
        private Action _action;

        public Command(Action action)
        {
            _action = action;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true; 
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }

        #endregion
    }

    public class CommandString : ICommand
    {
        private Action<string> _action;

        public CommandString(Action<string> action)
        {
            _action = action;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if(parameter != null)
                _action(parameter.ToString());
        }

        #endregion
    }
}
