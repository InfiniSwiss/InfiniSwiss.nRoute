using nRoute.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace nRoute.Components.Composition
{
    public class Resource<T>
        : Resource, INotifyCollectionChanged, IEnumerable<IResourceLocator>
        where
            T : class
    {
        private const string RESOURCE_NOTNULLOREMPTY = "Resource name cannot be null or empty";
        private const string RESOURCE_NOTFOUND = "Resource of type {0} with name '{1}' not found.";
        private const string RESOURCE_ALREADYEXISTS = "Resource of type {0} with name '{1}' already exists.";

        private readonly static Resource<T> _resourceCatalog;
        private readonly Dictionary<string, IResourceLocator> _resources;
        private readonly Object _lock = new Object();

        private string _defaultResourceName;

        static Resource()
        {
            _resourceCatalog = new Resource<T>();
        }

        private Resource()
            : base(typeof(T))
        {
            _resources = new Dictionary<string, IResourceLocator>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region Properties

        public static Resource<T> Catalog
        {
            get { return _resourceCatalog; }
        }

        public T Default
        {
            get
            {
                return (T)ResolveResourceInstance(false);
            }
        }

        public T this[string name]
        {
            get
            {
                return (T)ResolveResourceInstance(name, true);
            }
        }

        #endregion

        #region Overrides

        protected override Object ResolveResourceInstance(bool throwIfNotFound)
        {
            lock (_lock)
            {
                if (_resources.Count == 0) return null;

                // if there is no default we set it 
                if (string.IsNullOrEmpty(_defaultResourceName))
                {
                    _defaultResourceName = _resources.First().Key;
                }

                // and return, note the outlock still prevails or applies
                return ResolveResourceInstance(_defaultResourceName, throwIfNotFound);
            }
        }

        protected override Object ResolveResourceInstance(string name, bool throwIfNotFound)
        {
            if (string.IsNullOrEmpty(name))
            {
                if (throwIfNotFound)
                {
                    Guard.ArgumentNotNull(name, "name");
                }
                else
                {
                    return null;
                }
            }

            lock (_lock)
            {
                if (!_resources.ContainsKey(name))
                {
                    if (throwIfNotFound)
                    {
                        throw new KeyNotFoundException(string.Format(RESOURCE_NOTFOUND, typeof(T).FullName, name));
                    }
                    else
                    {
                        return null;
                    }
                }
                return _resources[name].GetResourceInstance();
            }
        }

        protected override IResourceLocator ResolveResourceLocator(bool throwIfNotFound)
        {
            lock (_lock)
            {
                if (_resources.Count == 0) return null;

                // if there is no default we set it 
                if (string.IsNullOrEmpty(_defaultResourceName))
                {
                    _defaultResourceName = _resources.First().Key;
                }

                // and return, note_ the outlock still prevails or applies
                return ResolveResourceLocator(_defaultResourceName, throwIfNotFound);
            }
        }

        protected override IResourceLocator ResolveResourceLocator(string name, bool throwIfNotFound)
        {
            if (string.IsNullOrEmpty(name))
            {
                if (throwIfNotFound)
                {
                    Guard.ArgumentNotNull(name, "name");
                }
                else
                {
                    return null;
                }
            }

            lock (_lock)
            {
                if (!_resources.ContainsKey(name))
                {
                    if (throwIfNotFound)
                    {
                        throw new KeyNotFoundException(string.Format(RESOURCE_NOTFOUND, typeof(T).FullName, name));
                    }
                    else
                    {
                        return null;
                    }
                }
                return _resources[name];
            }
        }

        protected override bool IsAnyResourceRegistered()
        {
            lock (_lock)
            {
                return (_resources.Count > 0);
            }
        }

        protected override bool IsNamedResourceRegistered(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            lock (_lock)
            {
                return _resources.ContainsKey(name);
            }
        }

        protected override void RegisterResourceLocator(IResourceLocator locator, bool isDefault)
        {
            Guard.ArgumentNotNull(locator, "locator");
            Guard.ArgumentNotNullOrWhiteSpace(locator.ResourceName, "locator", RESOURCE_NOTNULLOREMPTY);

            // we get the name
            var _name = locator.ResourceName;
            lock (_lock)
            {
                // and check
                if (_resources.ContainsKey(_name))
                    throw new InvalidOperationException(string.Format(RESOURCE_ALREADYEXISTS, typeof(T).FullName, _name));

                // add the resource
                _resources.Add(_name, locator);
                if (isDefault) _defaultResourceName = _name;

                // notify collection changed
                var _collectionChanged = CollectionChanged;
                if (_collectionChanged != null) _collectionChanged(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, locator, -1));
            }
        }

        protected override void UnregisterResource(string name)
        {
            Guard.ArgumentNotNullOrEmpty(name, "name");

            lock (_lock)
            {
                // contains checks
                if (!_resources.ContainsKey(name))
                {
                    throw new KeyNotFoundException(string.Format(RESOURCE_NOTFOUND, typeof(T), name));
                }

                // we get the locator and dispose it to release any resources
                IResourceLocator _locator = _resources[name];
                if (_locator != null) _locator.Dispose();

                // and remove, and change the default resource name
                _resources.Remove(name);
                if (string.Equals(_defaultResourceName, name, StringComparison.InvariantCultureIgnoreCase))
                    _defaultResourceName = null;

                // notify collection changed
                var _collectionChanged = CollectionChanged;
                if (_collectionChanged != null) _collectionChanged(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _locator, -1));
            }
        }

        protected override void SetDefaultResourceLocator(string name)
        {
            Guard.ArgumentNotNullOrEmpty(name, "name");

            lock (_lock)
            {
                // contains checks
                if (!_resources.ContainsKey(name))
                    throw new KeyNotFoundException(string.Format(RESOURCE_NOTFOUND, typeof(T), name));
                _defaultResourceName = name;
            }
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerable<IResourceLocator> Members

        public IEnumerator<IResourceLocator> GetEnumerator()
        {
            IEnumerable<IResourceLocator> _valuesEnumerable = null;
            lock (_lock)
            {
                _valuesEnumerable = _resources.Values.ToArray();
            }
            return _valuesEnumerable.GetEnumerator();
        }

        #endregion

    }
}
