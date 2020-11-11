using Microsoft.Xaml.Behaviors;
using nRoute.Components.Routing;
using nRoute.Navigation;
using System;
using System.ComponentModel;
using System.Windows;

namespace nRoute.Behaviors
{
    public abstract class NavigationHandlerActionBase
        : TriggerAction<FrameworkElement>
    {
        private const string CANNOT_FIND_HANDLER =
            "Cannot find navigation handler named '{0}' either in the control name-scope or application handler's registery";

        public static readonly DependencyProperty HandlerProperty =
            DependencyProperty.Register("Handler", typeof(INavigationHandler), typeof(NavigationHandlerActionBase),
            new PropertyMetadata(null));

        public static readonly DependencyProperty HandlerNameProperty =
            DependencyProperty.Register("HandlerName", typeof(string), typeof(NavigationHandlerActionBase),
            new PropertyMetadata(null));

        #region Properties

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.ElementBinding)]
        public INavigationHandler Handler
        {
            get { return (INavigationHandler)GetValue(HandlerProperty); }
            set { SetValue(HandlerProperty, value); }
        }

        [Category("Common Properties")]
        [CustomPropertyValueEditor(CustomPropertyValueEditor.Element)]
        public string HandlerName
        {
            get { return Convert.ToString(GetValue(HandlerNameProperty)); }
            set { SetValue(HandlerNameProperty, value); }
        }

        #endregion

        #region Helpers

        protected virtual INavigationHandler ResolveHandler()
        {
            // five ways, we resolve
            // 1. if we have a navigation handler specified then we use that 
            // 2. if we have an handler name, we try and resolve as an element within the local name-scope
            // 3. if we have an handler name, we check if we have one registered globally
            // 4. we look up the visual tree for a INavigationHandler, either an implemented or attached one
            // 5. lastly, we check if we have a default application-wide container, if so we use that
            // else we return null which should throw an exception, however any class can override that 

            // OPTION 1. we check if we have a handler registered
            if (this.Handler != null) return this.Handler;

            // OPTION 2/3. we check if we have a handler element name specified in the visual tree and in the global registry
            if (!string.IsNullOrEmpty(this.HandlerName))
            {
                // we try and get the handler specifically named handler in the visual tree
                var _namedHandler = NavigationService.ResolveHandlerInVisualTree(this.AssociatedObject, this.HandlerName);
                if (_namedHandler != null) return _namedHandler;

                // else we try in the global registry
                _namedHandler = NavigationService.GetNavigationHandler(this.HandlerName);
                if (_namedHandler != null) return _namedHandler;

                // if a name is given and not found, we raise an exception
                throw new NavigationException(string.Format(CANNOT_FIND_HANDLER, this.HandlerName), ResponseStatus.HandlerNotFound);
            }

            // OPTION 4. else, we try and check the visual tree
            var _handler = NavigationService.ResolveHandlerInVisualTree(this.AssociatedObject);
            if (_handler != null) return _handler;

            // OPTION 5. lastly we check if the Application has navigation enabled
            return NavigationService.DefaultNavigationHandler;

            // all else
            //if (NavigationService.DefaultNavigationHandler != null) return NavigationService.DefaultNavigationHandler;
            //return null;
        }

        #endregion

    }
}