using Microsoft.Xaml.Behaviors;
using nRoute.Components;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace nRoute.Behaviors
{
    public class BridgeCommandBehavior
        : Behavior<DependencyObject>
    {
        public static readonly DependencyProperty CommandRelayProperty =
            DependencyProperty.Register("CommandRelay", typeof(CommandRelay), typeof(BridgeCommandBehavior),
                new PropertyMetadata(null, OnCommandRelayChanged));

        public static readonly DependencyProperty CommandSourceProperty =
            DependencyProperty.Register("CommandSource", typeof(ICommand), typeof(BridgeCommandBehavior),
                new PropertyMetadata(null, OnCommandSourceChanged));

        #region Properties

        [Category("Common Properties")]
        public CommandRelay CommandRelay
        {
            get { return (CommandRelay)GetValue(CommandRelayProperty); }
            set { SetValue(CommandRelayProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.PropertyBinding)]
        public ICommand CommandSource
        {
            get { return (ICommand)GetValue(CommandSourceProperty); }
            set { SetValue(CommandSourceProperty, value); }
        }

        #endregion

        #region Handlers

        private static void OnCommandSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BridgeCommandBehavior)d).UpdateRelay();
        }

        private static void OnCommandRelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BridgeCommandBehavior)d).UpdateRelay();
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateRelay();
        }

        #endregion

        #region Helpers

        private void UpdateRelay()
        {
            if (CommandRelay != null && this.CommandSource != null)
            {
                CommandRelay.Command = CommandSource;
            }
        }

        #endregion

    }
}
