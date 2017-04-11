using System.Windows.Input;

namespace SecurityHub.UI.DependencyProperties.Commanding
{
    /// <summary>
    /// Represents an object that is capable of being notified of 
    /// a routed command execution by a CommandSinkBinding.  This
    /// interface is intended to be implemented by a ViewModel class
    /// that responds to a set of routed commands.
    /// </summary>
    /// <remarks>Added by Nigel Shaw 11/07/2012.</remarks>
    public interface ICommandSink
    {
        #region Methods

        /// <summary>Returns true if the specified command can be executed by the command sink.</summary>
        /// <param name="command">The command whose execution status is being queried.</param>
        /// <param name="parameter">An optional command parameter.</param>
        /// <param name="handled">Set to true if there is no need to continue querying for an execution status.</param>
        bool CanExecuteCommand(ICommand command, object parameter, out bool handled);

        /// <summary>Executes the specified command.</summary>
        /// <param name="command">The command being executed.</param>
        /// <param name="parameter">An optional command parameter.</param>
        /// <param name="handled">Set to true if the command has been executed and there is no need for others to respond.</param>
        void ExecuteCommand(ICommand command, object parameter, out bool handled);

        #endregion Methods
    }
}