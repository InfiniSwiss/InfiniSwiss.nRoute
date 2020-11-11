using nRoute.Components.Composition;
using nRoute.Internal;
using System;
using System.Windows;
using System.Windows.Media;

namespace nRoute.ViewServices
{
    public class DefaultViewServiceLocator
         : ResourceLocatorBase<Object, ViewServiceMeta>
    {
        private readonly Object _lock = new Object();
        private Object _instance;

        public DefaultViewServiceLocator(ViewServiceMeta meta)
            : base(meta.ViewServiceName, meta)
        {
            Guard.ArgumentNotNull(meta, "meta");

            // if the lifetime is a singleton, then we initialize an instance immediately
            if (meta.Lifetime == ViewServiceLifetime.Singleton && meta.InitializationMode == InitializationMode.WhenAvailable)
            {
                InitializeInstance();
            }
        }

        #region IResourceLocator Members

        public override object GetResourceInstance()
        {
            return InitializeInstance();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: check if locks in disposer are a good idea? 
                lock (_lock)
                {
                    if (_instance != null)
                    {
                        _instance = null;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private Object InitializeInstance()
        {
            switch (ResourceMeta.Lifetime)
            {
                case ViewServiceLifetime.Singleton:
                    if (_instance != null) return _instance;
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = CreateObserverTypeInstance();
                        }
                    }
                    return _instance;
                case ViewServiceLifetime.PerInstance:
                    lock (_lock)
                    {
                        return CreateObserverTypeInstance();
                    }
                case ViewServiceLifetime.DiscoveredInstance:
                    lock (_lock)
                    {
                        // we search in the application's visual tree 
                        DependencyObject _viewServiceElement = null;
                        return (GetViewServiceElement(Application.Current.MainWindow, out _viewServiceElement)) ?
                                _viewServiceElement : null;
                    }
                case ViewServiceLifetime.SelfRegisteredInstance:
                    return ViewServicesInstancesManager.GetRegisteredInstance(ResourceMeta.ViewServiceType);
            }

            // all else
            throw new InvalidOperationException("Unable to resolve view service type.");
        }

        private bool GetViewServiceElement(DependencyObject element, out DependencyObject viewServiceElement)
        {
            // by default
            viewServiceElement = null;

            // we check if the given element is the implementation type we are looking for
            if (element == null) return false;
            if (ResourceMeta.ImplementationType == element.GetType())
            {
                // we return that the instance is the type
                viewServiceElement = element;
                return true;
            }

            // else we check it's children
            var _childCount = VisualTreeHelper.GetChildrenCount(element);
            if (_childCount == 0) return false;

            // we check each element's child
            for (int i = 0; i < _childCount; i++)
            {
                if (GetViewServiceElement(VisualTreeHelper.GetChild(element, i), out viewServiceElement))
                {
                    return true;
                }
            }

            // all else
            return false;
        }

        protected virtual Object CreateObserverTypeInstance()
        {
            return TypeBuilder.BuildType(ResourceMeta.ImplementationType);
        }

        #endregion

    }
}