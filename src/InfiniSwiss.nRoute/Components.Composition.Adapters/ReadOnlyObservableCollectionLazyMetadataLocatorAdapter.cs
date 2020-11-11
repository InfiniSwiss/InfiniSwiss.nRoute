using nRoute.Components.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace nRoute.Components.Composition.Adapters
{
    public class ReadOnlyObservableCollectionLazyMetadataLocatorAdapter<T, TMetadata>
        : ReadOnlyObservableCollection<Lazy<T, TMetadata>>, ILocatorAdapter
        where
            T : class
        where
            TMetadata : class
    {
        private readonly object _lock = new object();

        public ReadOnlyObservableCollectionLazyMetadataLocatorAdapter()
            : base(new ObservableCollection<Lazy<T, TMetadata>>()) { }

        #region ILocatorAdapter Members

        public object Resolve(string name)
        {
            lock (_lock)
            {
                var _weakReference = new WeakReference(this.Items);

                // NOTE_ we are not lifiting anything within the class, we are only holding a weak-reference to the inner IList..
                // so we don't have a strong-ref on the observable collection, which means it can be GC'ed independent of the handler
                var _handler = (Handler<NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>)null;
                _handler = new Handler<NotifyCollectionChangedEventArgs, NotifyCollectionChangedEventHandler>((s, e) =>
                {
                    var _weakObservableResources = (IList<Lazy<T, TMetadata>>)null;
                    if (_weakReference != null && _weakReference.IsAlive)
                    {
                        _weakObservableResources = _weakReference.Target as IList<Lazy<T, TMetadata>>;
                    }

                    if (_weakObservableResources == null)
                    {
                        if (_handler != null) _handler.Dispose();
                        _handler = null;
                        _weakReference = null;
                        return;
                    }

                    // note_ the resource locator only supports add/remove, and one item at a time
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        var _locator = (IResourceLocator)e.NewItems[0];
                        var _metadata = _locator.ResourceMeta as TMetadata;
                        if (_metadata != null && !_weakObservableResources.Any((r) => r.Metadata == _metadata))
                        {
                            _weakObservableResources.Add(new Lazy<T, TMetadata>(() => (T)_locator.GetResourceInstance(), _metadata));
                        }
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        var _locator = (IResourceLocator)e.NewItems[0];
                        var _metadata = _locator.ResourceMeta as TMetadata;
                        if (_metadata != null)
                        {
                            var _resource = _weakObservableResources.FirstOrDefault((r) => r.Metadata == _metadata);
                            if (_resource != null) _weakObservableResources.Remove(_resource);
                        }
                    }
                },
                (h) =>
                {
                    Resource<T>.Catalog.CollectionChanged -= h;
                });

                // add the handler and the items
                Resource<T>.Catalog.CollectionChanged += _handler;

                // create the lazy-metadata pairing
                var _lazyMetadataResources = Resource<T>.Catalog.Select((l) =>
                    new Lazy<T, TMetadata>(() => (T)l.GetResourceInstance(), l.ResourceMeta as TMetadata));

                // add the items
                foreach (var _lazyMetadataResource in _lazyMetadataResources)
                {
                    if (_lazyMetadataResource.Metadata != null)
                    {
                        this.Items.Add(_lazyMetadataResource);
                    }
                }
            }
            return this;
        }

        #endregion


    }
}