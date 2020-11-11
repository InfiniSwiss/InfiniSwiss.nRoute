using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public class NavigationHandlerBehavior
         : Behavior<DependencyObject>
    {
        [Category("Common Properties")]
        public static readonly DependencyProperty HandlerNameProperty =
            DependencyProperty.Register("HandlerName", typeof(string), typeof(NavigationHandlerBehavior),
            new PropertyMetadata(null, OnHandlerNameChanged));

        [Category("Common Properties")]
        public static readonly DependencyProperty IsDefaultHandlerProperty =
            DependencyProperty.Register("IsDefaultHandler", typeof(bool), typeof(NavigationHandlerBehavior),
            new PropertyMetadata(false, OnIsDefaultHandlerChanged));

        #region Properties

        [Category("Common Properties")]
        public string HandlerName
        {
            get { return Convert.ToString(GetValue(HandlerNameProperty)); }
            set
            {
                SetValue(HandlerNameProperty, value);
            }
        }

        [Category("Common Properties")]
        public bool IsDefaultHandler
        {
            get { return Convert.ToBoolean(GetValue(IsDefaultHandlerProperty)); }
            set { SetValue(IsDefaultHandlerProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void OnAttached()
        {
            if (!string.IsNullOrEmpty(HandlerName))
            {
                Navigation.Navigation.SetHandlerName(this.AssociatedObject, HandlerName);
            }
            if (IsDefaultHandler)
            {
                Navigation.Navigation.SetDefaultHandler(this.AssociatedObject, IsDefaultHandler);
            }
        }

        #endregion

        #region Handlers

        private static void OnHandlerNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigationHandlerBehavior)d).UpdateHandlerName(Convert.ToString(e.NewValue));
        }

        private static void OnIsDefaultHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigationHandlerBehavior)d).UpdateIsDefaultHandler(Convert.ToBoolean(e.NewValue));
        }

        #endregion

        #region Helpers

        private void UpdateHandlerName(string value)
        {
            if (this.AssociatedObject != null)
            {
                Navigation.Navigation.SetHandlerName(this.AssociatedObject, value);
            }
        }

        private void UpdateIsDefaultHandler(bool value)
        {
            if (this.AssociatedObject != null)
            {
                Navigation.Navigation.SetDefaultHandler(this.AssociatedObject, value);
            }
        }

        #endregion

    }
}
