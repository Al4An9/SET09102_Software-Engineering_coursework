using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NBMFS.Commands
{
    //Takes action when a button is clicked to make an event happen
    //inherits from ICommand interface
    public class RelayCommand : ICommand
    {
        private Action _action;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public RelayCommand(Action action)
        {
            _action = action;
        }
        //set to true to allow command to run 
        public bool CanExecute(object parameter)
        {
            return true;
        }
        //
        public void Execute(object parameter)
        {
            _action();
        }
    }
}
