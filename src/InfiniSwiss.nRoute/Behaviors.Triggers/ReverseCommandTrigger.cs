using Microsoft.Xaml.Behaviors;
using nRoute.Components;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors.Triggers
{
    public class ReverseCommandTrigger
        : TriggerBase<DependencyObject>
    {
        public static readonly DependencyProperty ReverseCommandProperty =
            DependencyProperty.Register("ReverseCommand", typeof(IReverseCommand), typeof(ReverseCommandTrigger),
            new PropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public IReverseCommand ReverseCommand
        {
            get { return (IReverseCommand)GetValue(ReverseCommandProperty); }
            set { SetValue(ReverseCommandProperty, value); }
        }

        #endregion

        #region Handlers

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ReverseCommandTrigger)d).SetupHandlers(e.NewValue as IReverseCommand, e.OldValue as IReverseCommand);
        }

        #endregion

        #region Helpers

        private void SetupHandlers(IReverseCommand newCommand, IReverseCommand oldCommand)
        {
            // we detach or attach
            if (oldCommand != null)
                oldCommand.CommandExecuted -= new EventHandler<CommandEventArgs>(Command_CommandExecuted);
            if (newCommand != null)
                newCommand.CommandExecuted += new EventHandler<CommandEventArgs>(Command_CommandExecuted);
        }

        private void Command_CommandExecuted(object sender, CommandEventArgs e)
        {
            if (this.AssociatedObject == null) return;      // this is to ensure with initial/static set properties the trigger isn't invoked

            // when the command is executed we execute the trigger
            // note_, we pass through the command parameter
            base.InvokeActions(e.CommandParameter);
        }

        #endregion

    }
}
