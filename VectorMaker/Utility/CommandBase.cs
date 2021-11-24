using System;
using System.Windows.Input;

namespace VectorMaker.Commands
{
   internal class CommandBase : ICommand
    {
        private Action<object> m_executeAction;
        private Predicate<object> m_predicateAction;

        #region Constructors
        public CommandBase(Action<object> executeAction) : this(executeAction, null) { }

        public CommandBase(Action<object> executeAction, Predicate<object> predicateAction)
        {
            m_executeAction = executeAction ?? throw new ArgumentNullException("No execute Action");
            m_predicateAction = predicateAction;
        }
        #endregion

        #region Icommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => m_predicateAction == null || m_predicateAction(parameter);

        public void Execute(object parameter) => m_executeAction(parameter);
        #endregion
    }
}
