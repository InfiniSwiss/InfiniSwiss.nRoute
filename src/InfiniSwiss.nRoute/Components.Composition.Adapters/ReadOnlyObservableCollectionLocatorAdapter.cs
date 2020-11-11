using nRoute.Components.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace nRoute.Components.Composition.Adapters
{
    public class ReadOnlyObservableCollectionLocatorAdapter<T>
        : ReadOnlyObservableCollection<T>, ILocatorAdapter
        where
            T : class
    {
        private readonly object _lock = new object();

        public ReadOnlyObservableCollectionLocatorAdapter()
            : base(new ObservableCollection<T>()) { }

        #region ILocatorAdapter Members

        public object Resolve(string name)
        {
            lock (_lock)
            {
                var _observableResourceLocators = new List<IResourceLocator>(Resource<T>.Catalog);
                var _weakReference = new WeakReference(this.Items);

                // NOTE_ we are not lifiting anything within the class, we are only holding a weak-reference to the inner IList..
                // so we don't have a strong-ref on the observable collection, which means it can be GC'ed independent of the handler
                var _handler = (Handler<NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>)null;
                _handler = new Handler<NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>((s, e) =>
                {
                    var _weakObservableResources = (IList<T>)null;
                    if (_weakReference != null && _weakReference.IsAlive)
                    {
                        _weakObservableResources = _weakReference.Target as IList<T>;
                    }

                    if (_weakObservableResources == null)
                    {
                        if (_handler != null) _handler.Dispose();
                        _handler = null;
                        _observableResourceLocators = null;
                        _weakReference = null;
                        return;
                    }

                    // note_ the resource locator only supports add/remove, and one item at a time
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        var _locator = (IResourceLocator)e.NewItems[0];
                        _observableResourceLocators.Add(_locator);
                        _weakObservableResources.Add((T)_locator.GetResourceInstance());
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        var _locator = (IResourceLocator)e.OldItems[0];
                        var _index = _observableResourceLocators.IndexOf(_locator);
                        _observableResourceLocators.RemoveAt(_index);
                        _weakObservableResources.RemoveAt(_index);
                    }
                },
                (h) =>
                {
                    Resource<T>.Catalog.CollectionChanged -= h;
                });

                // add the handler and the items
                Resource<T>.Catalog.CollectionChanged += _handler;

                // add the items
                foreach (var _locator in _observableResourceLocators)
                {
                    base.Items.Add((T)_locator.GetResourceInstance());
                }
            }
            return this;
        }

        #endregion


    }
}