using nRoute.Components;
using nRoute.Internal;
using nRoute.Navigation.Containers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace nRoute.Navigation
{
    public static partial class NavigationService
    {
        private static WeakReference _defaultNavigationHandler;
        private static readonly Dictionary<string, WeakReference> _navigationHandlers;
        private static readonly Object _handlersLock = new Object();

        static NavigationService()
        {
            _navigationHandlers = new Dictionary<string, WeakReference>(StringComparer.InvariantCultureIgnoreCase);
        }

        internal static void Initialize()
        {
            SetupRoutes();
            MapAllRegisteredRoutes();
        }

        #region Application Containers Related

        public static INavigationHandler DefaultNavigationHandler
        {
            get
            {
                lock (_handlersLock)
                {
                    if (_defaultNavigationHandler == null || !_defaultNavigationHandler.IsAlive) return null;
                    var _handler = (INavigationHandler)_defaultNavigationHandler.Target;
                    return _handler;
                }
            }
            set
            {
                lock (_handlersLock)
                {
                    _defaultNavigationHandler = value != null ? new WeakReference(value) : null;
                }
            }
        }

        public static bool IsNavigationHandlerRegistered(string name)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");

            lock (_handlersLock)
            {
                return (_navigationHandlers.ContainsKey(name));
            }
        }

        public static INavigationHandler GetNavigationHandler(string name)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");

            lock (_handlersLock)
            {
                WeakReference _handlerRef = null;
                if (!_navigationHandlers.TryGetValue(name, out _handlerRef)) return null;
                if (_handlerRef == null || !_handlerRef.IsAlive) return null;
                return (INavigationHandler)_handlerRef.Target;
            }
        }

        //public static bool TryGetNavigationHandler(string name, out INavigationHandler handler)
        //{
        //    Guard.ArgumentNotNullOrWhiteSpace(name, "name");

        //    handler = null;
        //    lock (_handlersLock)
        //    {
        //        WeakReference _handlerRef = null;
        //        if (!_navigationHandlers.TryGetValue(name, out _handlerRef)) return false;
        //        if (_handlerRef == null || !_handlerRef.IsAlive) return false;
        //        handler = (INavigationHandler)_handlerRef.Target;
        //        return (handler != null);
        //    }
        //}

        public static void RegisterNavigationHandler(string name, INavigationHandler handler)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");
            Guard.ArgumentNotNull(handler, "handler");

            lock (_handlersLock)
            {
                if (_navigationHandlers.ContainsKey(name))
                {
                    _navigationHandlers[name] = new WeakReference(handler);
                }
                else
                {
                    _navigationHandlers.Add(name, new WeakReference(handler));
                }
            }
        }

        public static void UnregisterNavigationHandler(string name)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");

            lock (_handlersLock)
            {
                if (!_navigationHandlers.ContainsKey(name)) return;
                _navigationHandlers.Remove(name);
            }
        }

        #endregion

        #region Navigation Supporter & Container Related

        public static T GetSupporter<T>(INavigationContainer container)
            where
                T : class
        {
            if (container == null || container.Content == null) return null;
            return GetSupporter<T>(container.Content);
        }

        public static T GetSupporter<T>(Object content)
            where
                T : class
        {
            if (content == null) return null;

            // we see if the content implements the supporter, if not then we check for IResolve<T>
            var _supporter = content as T;
            if (_supporter == null)
            {
                var _supporterResolver = content as IResolve<T>;
                if (_supporterResolver != null) _supporter = _supporterResolver.Resolve();
            }

            // if we have the supporter then return
            if (_supporter != null) return _supporter;

            // else we check it it is contained in the data-content, in that case it has to be a frameworkElement
            var _frameworkElement = content as FrameworkElement;
            if (_frameworkElement == null) return null;

            // we check if it's data context (VM normally) implements T
            _supporter = _frameworkElement.DataContext as T;

            // if not, then we check if they implement IResolve<T>, through which we can also get it
            if (_supporter == null)
            {
                var _supporterResolver = _frameworkElement.DataContext as IResolve<T>;
                if (_supporterResolver != null) _supporter = _supporterResolver.Resolve();
            }

            // and return, even it is null
            return _supporter;
        }

        public static INavigationHandler GetNavigationHandler(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return null;
            var _handler = dependencyObject as INavigationHandler;
            return _handler ?? Navigation.GetHandler(dependencyObject);
        }

        public static INavigationContainer GetNavigationContainer(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return null;
            var _container = dependencyObject as INavigationContainer;
            return _container ?? Navigation.GetHandler(dependencyObject) as INavigationContainer;
        }

        public static INavigationHandler ResolveHandlerInVisualTree(FrameworkElement element, string handlerName)
        {
            Guard.ArgumentNotNull(element, "element");
            Guard.ArgumentNotNullOrEmpty(handlerName, "handlerName");

            // OPTION 2. we check if we have a handler element name specified
            // we find the first possible FrameworkElement in the VisualTree
            var _frameworkElement = element;       // I've left this as later we can extend this to a DO
            if (_frameworkElement != null)
            {
                // we try and get the given element (who is supposed to be navigation handler) by name
                var _handlerElement = _frameworkElement.FindName(handlerName) as DependencyObject;
                if (_handlerElement != null)
                {
                    var _elementHandler = NavigationService.GetNavigationHandler(_handlerElement);
                    if (_elementHandler != null)
                    {
                        return _elementHandler;
                    }
                }
            }

            // OPTION 3. we try against any globally registered handler
            INavigationHandler _registeredHandler = NavigationService.GetNavigationHandler(handlerName);
            if (_registeredHandler != null)
            {
                return _registeredHandler;
            }

            // return null!
            return null;
        }

        public static INavigationHandler ResolveHandlerInVisualTree(DependencyObject element)
        {
            Guard.ArgumentNotNull(element, "element");

            // OPTION 4. else, we try and check the visual tree
            DependencyObject _visualTreeElement = element;
            INavigationHandler _visualTreeHandler = NavigationService.GetNavigationHandler(_visualTreeElement);

            while (_visualTreeElement != null)
            {
                if (_visualTreeHandler != null)
                {
                    return _visualTreeHandler;
                }
                _visualTreeElement = VisualTreeHelper.GetParent(_visualTreeElement);
                _visualTreeHandler = NavigationService.GetNavigationHandler(_visualTreeElement);
            }

            return null;
        }

        #endregion

    }
}
