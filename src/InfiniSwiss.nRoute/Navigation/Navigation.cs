using System;
using System.Windows;

namespace nRoute.Navigation
{
    public static class Navigation
    {

        #region Dependency Properties

        public static readonly DependencyProperty HandlerProperty = DependencyProperty.RegisterAttached(
            "Handler",
            typeof(INavigationHandler),
            typeof(Navigation),
            null);

        public static readonly DependencyProperty HandlerNameProperty = DependencyProperty.RegisterAttached(
            "HandlerName",
            typeof(string),
            typeof(Navigation),
            new PropertyMetadata(OnSetHandlerNameCallback));

        public static readonly DependencyProperty DefaultHandlerProperty = DependencyProperty.RegisterAttached(
            "DefaultHandler",
            typeof(bool),
            typeof(Navigation),
            new PropertyMetadata(false, OnSetDefaultHandlerCallback));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached(
            "Title",
            typeof(string),
            typeof(Navigation),
            null);

        #endregion

        #region Getters and Setters

        public static void SetHandler(DependencyObject dependencyObject, INavigationHandler handler)
        {
            dependencyObject.SetValue(HandlerProperty, handler);
        }

        public static INavigationHandler GetHandler(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return null;
            return dependencyObject.GetValue(HandlerProperty) as INavigationHandler;
        }

        public static void SetHandlerName(DependencyObject dependencyObject, string name)
        {
            dependencyObject.SetValue(HandlerNameProperty, name);
        }

        public static string GetHandlerName(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return null;
            return Convert.ToString(dependencyObject.GetValue(HandlerNameProperty));
        }

        public static void SetDefaultHandler(DependencyObject dependencyObject, bool isDefault)
        {
            dependencyObject.SetValue(DefaultHandlerProperty, isDefault);
        }

        public static bool GetDefaultHandler(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return false;
            return Convert.ToBoolean(dependencyObject.GetValue(DefaultHandlerProperty));
        }

        public static void SetTitle(DependencyObject dependencyObject, string title)
        {
            dependencyObject.SetValue(TitleProperty, title);
        }

        public static string GetTitle(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return null;
            return Convert.ToString(dependencyObject.GetValue(TitleProperty));
        }

        #endregion

        #region Handlers

        private static void OnSetHandlerNameCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var _handler = NavigationService.GetNavigationHandler(dependencyObject);
            if (_handler == null) return;

            // if the handler had a old name registered, we remove it
            var _oldName = Convert.ToString(e.OldValue);
            if (!string.IsNullOrEmpty(_oldName) && NavigationService.IsNavigationHandlerRegistered(_oldName))
            {
                INavigationHandler _oldHandler = NavigationService.GetNavigationHandler(_oldName);
                if (_oldHandler != null)
                {
                    if (Object.ReferenceEquals(_oldHandler, _handler))
                    {
                        NavigationService.UnregisterNavigationHandler(_oldName);
                    }
                }
            }

            var _newName = Convert.ToString(e.NewValue);
            if (!string.IsNullOrEmpty(_newName))
            {
                NavigationService.RegisterNavigationHandler(_newName, _handler);
            }
        }

        private static void OnSetDefaultHandlerCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var _handler = NavigationService.GetNavigationHandler(dependencyObject);
            if (_handler == null) return;

            // if we are to set then set, else we check if it currently the default and then remove it
            if (Convert.ToBoolean(e.NewValue))
            {
                NavigationService.DefaultNavigationHandler = _handler;
            }
            else
            {
                if (Object.ReferenceEquals(NavigationService.DefaultNavigationHandler, _handler))
                {
                    NavigationService.DefaultNavigationHandler = null;
                }
            }
        }

        #endregion

    }
}
