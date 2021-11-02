using System;
using System.Windows.Input;

namespace VectorMaker.Utility
{
    public class CommandBase : ICommand
    {
        private Action<object> m_executeAction;
        private Predicate<object> m_predicateAction;

        public event EventHandler CanExecuteChanged;

        public CommandBase(Action<object> executeAction,Predicate<object> predicateAction)
        {
            m_executeAction = executeAction ?? throw new ArgumentNullException("No execute Action");
            m_predicateAction = predicateAction;
        }

        public CommandBase(Action<object> executeAction) : this(executeAction, null) { }

        public bool CanExecute(object parameter) => m_predicateAction == null || m_predicateAction(parameter);

        public void Execute(object parameter) => m_executeAction(parameter);
    }
}
