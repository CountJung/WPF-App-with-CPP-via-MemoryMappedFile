using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFAppCPPMMF
{
    public class ViewCommander : ICommand
    {
        private readonly Action<object>? action;
        private readonly Func<object, bool>? canExecute;

        public ViewCommander(Action<object>? act) : this(act, null)
        {

        }
        public ViewCommander(Action<object>? act, Func<object, bool>? canAct = null)
        {
            action = act;
            canExecute = canAct ?? (param => true);
        }
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => canExecute!(parameter!);
        
        public void Execute(object? parameter) => action!(parameter!);
        
    }
}
