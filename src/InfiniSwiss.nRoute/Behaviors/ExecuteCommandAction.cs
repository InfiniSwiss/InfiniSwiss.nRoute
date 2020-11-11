using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace nRoute.Behaviors
{
    [ContentProperty("Parameter")]
    public class ExecuteCommandAction
        : TriggerAction<UIElement>
    {
        private const double INTERACTIVITY_ENABLED = 1d;
        private const double INTERACTIVITY_DISABLED = 0.5d;

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ExecuteCommandAction),
            new PropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(object), typeof(ExecuteCommandAction),
            new PropertyMetadata(null, new PropertyChangedCallback(OnParameterChanged)));

        public static readonly DependencyProperty TriggerParameterConverterProperty =
            DependencyProperty.Register("TriggerParameterConverter", typeof(IValueConverter), typeof(ExecuteCommandAction),
            new PropertyMetadata(null));

        private bool _manageEnableState;

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public object Parameter
        {
            get { return GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public IValueConverter TriggerParameterConverter
        {
            get { return (IValueConverter)GetValue(TriggerParameterConverterProperty); }
            set { SetValue(TriggerParameterConverterProperty, value); }
        }

        [Category("Common Properties")]
        public bool ManageEnableState
        {
            get { return _manageEnableState; }
            set { _manageEnableState = value; }
        }

        #endregion

        #region Trigger Related

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateEnabledState();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            DisposeEnableState();
        }

        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null) return;

            // if a trigger parameter converter is specified, then we use that to get the command parameter
            // else we use the given parameter - note_ the parameter can be null
            var _parameter = TriggerParameterConverter != null ?
                TriggerParameterConverter.Convert(parameter, typeof(Object), this.AssociatedObject, CultureInfo.CurrentCulture) :
                this.Parameter;

            if (this.Command != null && this.Command.CanExecute(_parameter))
                this.Command.Execute(_parameter);
        }

        #endregion

        #region Handlers

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExecuteCommandAction)d).SetupEnableState(e.NewValue as ICommand, e.OldValue as ICommand);
        }

        private static void OnParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExecuteCommandAction)d).UpdateEnabledState();
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }

        #endregion

        #region Helpers

        private void SetupEnableState(ICommand newCommand, ICommand oldCommand)
        {
            if (!ManageEnableState) return;

            // we detach or attach
            if (oldCommand != null)
                oldCommand.CanExecuteChanged -= new EventHandler(Command_CanExecuteChanged);
            if (newCommand != null)
                newCommand.CanExecuteChanged += new EventHandler(Command_CanExecuteChanged);

            // and update
            UpdateEnabledState();
        }

        private void UpdateEnabledState()
        {
            if (!ManageEnableState || AssociatedObject == null || Command == null) return;

            // we get if it is enabled or not
            var _canExecute = this.Command.CanExecute(this.Parameter);
            AssociatedObject.IsEnabled = _canExecute;
        }

        private void DisposeEnableState()
        {
            if (!ManageEnableState || AssociatedObject == null || Command == null) return;

            if (AssociatedObject != null)
            {
                Command.CanExecuteChanged -= new EventHandler(Command_CanExecuteChanged);
            }
        }

        #endregion

    }
}
