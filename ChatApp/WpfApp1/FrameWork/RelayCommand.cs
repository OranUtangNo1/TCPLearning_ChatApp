using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatAppCore.FrameWork
{
    internal class RelayCommand :ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute = null;

        public RelayCommand(Action executeMethod)
        {
            this._execute = executeMethod;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
            :this(executeMethod)
        {
            this._canExecute = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) 
        {
            return this._canExecute != null ? this._canExecute() : true;
        }

        public void Execute(object parameter) 
        {
            _execute();
        }
    }
}
