using System;
using System.Diagnostics;
using System.Windows.Input;

namespace SecurityHub.UI.DependencyProperties.Commanding
{
    /// <summary>
    /// Dan McCrady to Nigel: Got this from Josh Smith's Crack .NET project at codeplex.
    /// I'm assuming form the same location as your command sinking.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Predicate<object> _canExecute;
        readonly Action<object> _execute;

        #endregion Fields

        #region Constructors

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion Constructors

        #region Events

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        #endregion Events

        #region Methods

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion Methods
    }
}