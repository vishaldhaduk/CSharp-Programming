using System;
using System.Windows;
using System.Windows.Input;

namespace SecurityHub.UI.DependencyProperties.Commanding
{
    /// <summary>
    /// A CommandBinding subclass that will attach its
    /// CanExecute and Executed events to the event handling
    /// methods on the object referenced by its CommandSink property.  
    /// </summary>
    /// <remarks>
    /// <para>Set the attached CommandSink property on the element 
    /// whose CommandBindings collection contains CommandSinkBindings.
    /// If you dynamically create an instance of this class and add it 
    /// to the CommandBindings of an element, you must explicitly set
    /// its CommandSink property.</para>
    /// <para>Added by Nigel Shaw 11/07/2012</para></remarks>
    public class CommandSinkBinding : CommandBinding
    {
        #region Fields

        /// <summary>
        /// The CommandSink dependency property.
        /// </summary>
        /// <remarks>This dependency property allows you to set the CommandSink property on 
        /// an element whose CommandBindings collection contains CommandSinkBindings.</remarks>
        public static readonly DependencyProperty CommandSinkProperty = 
            DependencyProperty.RegisterAttached(
            "CommandSink",
            typeof(ICommandSink),
            typeof(CommandSinkBinding),
            new UIPropertyMetadata(null, OnCommandSinkChanged));

        private ICommandSink commandSink;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The CommandSink object that is set on an element by using the 
        /// CommandSink dependency property.
        /// </summary>
        public ICommandSink CommandSink
        {
            get { return commandSink; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Cannot set CommandSink to null.");

                if (commandSink != null)
                    throw new InvalidOperationException("Cannot set CommandSink more than once.");

                commandSink = value;

                // Wire up the CanExecute routed event handler to the command sink's CanExecuteCommand
                // method.
                base.CanExecute += (sender, e) =>
                {
                    bool handled;
                    e.CanExecute = commandSink.CanExecuteCommand(e.Command, e.Parameter, out handled);
                    e.Handled = handled;
                };

                // Wire up the Executed routed event handler to the command sink's ExecuteCommand
                // method.
                base.Executed += (sender, e) =>
                {
                    bool handled;
                    commandSink.ExecuteCommand(e.Command, e.Parameter, out handled);
                    e.Handled = handled;
                };
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Get the command sink property. 
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The command sink property.</returns>
        public static ICommandSink GetCommandSink(DependencyObject obj)
        {
            return (ICommandSink)obj.GetValue(CommandSinkProperty);
        }

        /// <summary>
        /// Set the command sink property. 
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value to set.</param>
        public static void SetCommandSink(DependencyObject obj, ICommandSink value)
        {
            obj.SetValue(CommandSinkProperty, value);
        }

        // This method is necessary when the CommandSink attached property is set on an element
        // in a template, or any other situation in which the element's CommandBindings have not
        // yet had a chance to be created and added to its CommandBindings collection.
        static bool ConfigureDelayedProcessing(DependencyObject depObj, ICommandSink commandSink)
        {
            bool isDelayed = false;

            CommonElement elem = new CommonElement(depObj);
            if (elem.IsValid && !elem.IsFrameworkElementLoaded)
            {
                RoutedEventHandler handler = null;
                handler = delegate
                {
                    elem.Loaded -= handler;
                    ProcessCommandSinkChanged(depObj, commandSink);
                };
                elem.Loaded += handler;
                isDelayed = true;
            }

            return isDelayed;
        }

        /// <summary>
        /// Get the list of command bindings.
        /// </summary>
        /// <param name="depObj"></param>
        /// <returns></returns>
        static CommandBindingCollection GetCommandBindings(DependencyObject depObj)
        {
            var elem = new CommonElement(depObj);
            return elem.IsValid ? elem.CommandBindings : null;
        }

        /// <summary>
        /// Event handler for the changed event of the CommandSink dependency property.
        /// </summary>
        /// <param name="depObj">The CommandSink dependency object.</param>
        /// <param name="e">The changed event args.</param>
        /// <remarks>When the CommandSink is changed we need to run ConfigureDelayedProcessing 
        /// and if not using delayed processing, we process the command sink changed. </remarks>
        static void OnCommandSinkChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            ICommandSink commandSink = e.NewValue as ICommandSink;

            if (!ConfigureDelayedProcessing(depObj, commandSink))
                ProcessCommandSinkChanged(depObj, commandSink);
        }

        /// <summary>
        /// When the command sink changes, change the command bindings by setting
        /// each binding's command sink to the changed command sink.
        /// </summary>
        /// <param name="depObj">The object whose command sink changed.</param>
        /// <param name="commandSink">The new command sink.</param>
        static void ProcessCommandSinkChanged(DependencyObject depObj, ICommandSink commandSink)
        {
            CommandBindingCollection cmdBindings = GetCommandBindings(depObj);
            if (cmdBindings == null)
                throw new ArgumentException("The CommandSinkBinding.CommandSink attached property was set on an element that does not support CommandBindings.");

            foreach (CommandBinding cmdBinding in cmdBindings)
            {
                CommandSinkBinding csb = cmdBinding as CommandSinkBinding;
                if (csb != null && csb.CommandSink == null)
                    csb.CommandSink = commandSink;
            }
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// This class makes it easier to Write code that works 
        /// with the common members of both the FrameworkElement
        /// and FrameworkContentElement classes.
        /// </summary>
        private class CommonElement
        {
            #region Fields

            public readonly bool IsValid;

            readonly FrameworkContentElement frameworkContentElement;
            readonly FrameworkElement frameworkElement;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initialize a new instance of a <see cref="CommonElement"/>. 
            /// </summary>
            /// <param name="depObj">The dependency object to interpret as a FrameworkElement
            /// or FrameworkContentElement.</param>
            public CommonElement(DependencyObject depObj)
            {
                frameworkElement = depObj as FrameworkElement;
                frameworkContentElement = depObj as FrameworkContentElement;

                IsValid = frameworkElement != null || frameworkContentElement != null;
            }

            #endregion Constructors

            #region Events

            /// <summary>
            /// Loaded event. 
            /// </summary>
            public event RoutedEventHandler Loaded
            {
                add
                {
                    this.Verify();

                    if (frameworkElement != null)
                        frameworkElement.Loaded += value;
                    else
                        frameworkContentElement.Loaded += value;
                }
                remove
                {
                    this.Verify();

                    if (frameworkElement != null)
                        frameworkElement.Loaded -= value;
                    else
                        frameworkContentElement.Loaded -= value;
                }
            }

            #endregion Events

            #region Properties

            /// <summary>
            /// Collection of CommandBindings. 
            /// </summary>
            public CommandBindingCollection CommandBindings
            {
                get
                {
                    this.Verify();

                    if (frameworkElement != null)
                        return frameworkElement.CommandBindings;
                    else
                        return frameworkContentElement.CommandBindings;
                }
            }

            /// <summary>
            /// Whether the framework element is loaded. 
            /// </summary>
            public bool IsFrameworkElementLoaded
            {
                get
                {
                    this.Verify();

                    if (frameworkElement != null)
                        return frameworkElement.IsLoaded;
                    else
                        return frameworkContentElement.IsLoaded;
                }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Verify the command sink binding.
            /// </summary>
            void Verify()
            {
                if (!this.IsValid)
                    throw new InvalidOperationException("Cannot use an invalid CommmonElement.");
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}