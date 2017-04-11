using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SecurityHub.UI.DependencyProperties.Commanding
{
    /// <summary>
    /// This implementation of ICommandSink can serve as a base
    /// class for a ViewModel or as an object embedded in a ViewModels.  
    /// It provides a means of registering commands and their callback 
    /// methods, and will invoke those callbacks upon request.
    /// </summary>
    /// <remarks>Added by Nigel Shaw 11/07/2012.</remarks>
    public class CommandSink : ICommandSink
    {
        #region Fields

        /// <summary>
        /// List of commands and their corresponding CanExecute and Execute callbacks.
        /// </summary>
        /// <remarks>This dictionary stores a list of commands and their corresponding CanExecute and Execute callbacks in a
        /// struct for each command. The command is looked up in the dictionary and the corresponding callbacks are invoked to determine
        /// whether the command can execute and to execute the command. In each case the command callbacks are passed the 
        /// model object as a parameter and the callbacks are evaluated against that model object.</remarks>
        readonly Dictionary<ICommand, CommandCanExecuteAndExecuteCallbacks> commandToCallbacksMap = new Dictionary<ICommand, CommandCanExecuteAndExecuteCallbacks>();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Determine whether the command can execute. 
        /// </summary>
        /// <param name="command">The command object.</param>
        /// <param name="parameter">The model object to use as the parameter against which to run the callback to determine whether
        /// the command can execute.</param>
        /// <param name="handled">Whether the CanExecute is handled. If not, then the command will continue to bubble or tunnel.</param>
        /// <returns>Whether the command can execute.</returns>
        public virtual bool CanExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            VerifyArgument(command, "command");

            if (commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                return commandToCallbacksMap[command].CanExecute(parameter);
            }
            else
            {
                return (handled = false);
            }
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="command">The command object to execute.</param>
        /// <param name="parameter">The model object parameter to pass as a parameter when executing the command.</param>
        /// <param name="handled">Whether the execute was handled.</param>
        public virtual void ExecuteCommand(ICommand command, object parameter, out bool handled)
        {
            VerifyArgument(command, "command");

            if (commandToCallbacksMap.ContainsKey(command))
            {
                handled = true;
                commandToCallbacksMap[command].Execute(parameter);
            }
            else
            {
                handled = false;
            }
        }

        /// <summary>
        /// Register a command in the command to callbacks map.
        /// </summary>
        /// <param name="command">The command to register.</param>
        /// <param name="canExecute">A predicate callback that will run to determine whether the command can execute.</param>
        /// <param name="execute">An action callback that will run to execute the command.</param>
        public void RegisterCommand(ICommand command, Predicate<object> canExecute, Action<object> execute)
        {
            VerifyArgument(command, "command");
                VerifyArgument(canExecute, "canExecute");
                VerifyArgument(execute, "execute");

                commandToCallbacksMap[command] = new CommandCanExecuteAndExecuteCallbacks(canExecute, execute);
        }

        /// <summary>
        /// Remove a command from the command to callbacks map.
        /// </summary>
        /// <param name="command">The command to remove.</param>
        public void UnregisterCommand(ICommand command)
        {
            VerifyArgument(command, "command");

            if (commandToCallbacksMap.ContainsKey(command))
                commandToCallbacksMap.Remove(command);
        }

        /// <summary>
        /// Verify that an argument is not null and throw an argument exception if it is.
        /// </summary>
        /// <param name="arg">The argument to verify.</param>
        /// <param name="argName">The name of the argument.</param>
        static void VerifyArgument(object arg, string argName)
        {
            if (arg == null)
                    throw new ArgumentNullException(argName);
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Callbacks for CanExecute and Execute for a particular command. 
        /// </summary>
        /// <remarks>Stores a predicate callback that is called to verify whether CanExecute
        /// is true or not, and stores an action callback that is called to execute the
        /// command action.</remarks>
        private struct CommandCanExecuteAndExecuteCallbacks
        {
            #region Fields

            /// <summary>
            /// The predicate that is called to determine whether the command can execute or not. 
            /// </summary>
            public readonly Predicate<object> CanExecute;

            /// <summary>
            /// The action callback that's called to run the command's action. 
            /// </summary>
            public readonly Action<object> Execute;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initialize a new instance of a <see cref="CommandCanExecuteAndExecuteCallbacks"/>. 
            /// </summary>
            /// <param name="canExecute">A predicate that when run with the target model object as a parameter determines
            /// whether the command can execute.</param>
            /// <param name="execute">An action that is run with the target model object as a parameter 
            /// to execute the command's action.</param>
            public CommandCanExecuteAndExecuteCallbacks(Predicate<object> canExecute, Action<object> execute)
            {
                this.CanExecute = canExecute;
                this.Execute = execute;
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }
}