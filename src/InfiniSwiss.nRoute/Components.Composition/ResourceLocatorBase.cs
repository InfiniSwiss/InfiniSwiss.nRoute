using nRoute.Internal;
using System;

namespace nRoute.Components.Composition
{
    public abstract class ResourceLocatorBase<T, TMetadata>
        : IResourceLocator
        where
            T : class
        where
            TMetadata : class
    {
        private readonly string _resourceName;
        private readonly TMetadata _resourceMeta;

        protected ResourceLocatorBase(string resourceName, TMetadata resourceMeta)
        {
            Guard.ArgumentNotNullOrEmpty(resourceName, "resourceName");

            _resourceName = resourceName;
            _resourceMeta = resourceMeta;       // note_ can be null
        }

        #region Overridable

        public abstract T GetResourceInstance();

        public virtual TMetadata ResourceMeta
        {
            get { return _resourceMeta; }
        }

        #endregion

        #region IResourceLocator Members

        public virtual string ResourceName
        {
            get { return _resourceName; }
        }

        Object IResourceLocator.ResourceMeta
        {
            get { return _resourceMeta; }
        }

        object IResourceLocator.GetResourceInstance()
        {
            return this.GetResourceInstance();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Overridable

        protected virtual void Dispose(bool disposing) { }

        #endregion

    }
}
