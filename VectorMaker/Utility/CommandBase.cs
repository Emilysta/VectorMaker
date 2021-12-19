using System;
using System.Windows.Input;

namespace VectorMaker.Commands
{
    /// <summary>
    /// This class is a bes for all commands created for MVVM pattern in WPF app.
    /// Implements <see cref="ICommand"/> interface.
    /// </summary>
   internal class CommandBase : ICommand
    {

        private Action<object> m_executeAction;
        private Predicate<object> m_predicateAction;

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="executeAction"></param>
        public CommandBase(Action<object> executeAction) : this(executeAction, null) { }
        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="predicateAction"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CommandBase(Action<object> executeAction, Predicate<object> predicateAction)
        {
            m_executeAction = executeAction ?? throw new ArgumentNullException("No execute Action");
            m_predicateAction = predicateAction;
        }
        #endregion

        #region Icommand Members
        /// <summary>
        /// Invoked when CanExecute state changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { }//CommandManager.RequerySuggested += value; }
            remove { }// CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Checks if command can be executed.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Returns true if command can be executed</returns>
        public bool CanExecute(object parameter) => m_predicateAction == null || m_predicateAction(parameter);

        /// <summary>
        /// Executes command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) => m_executeAction(parameter);
        #endregion
    }
}
