using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewModels
{
    public class DefaultViewModelLocator
         : ResourceLocatorBase<Object, ViewModelMeta>
    {
        public DefaultViewModelLocator(ViewModelMeta meta)
            : base(meta.Name, meta)
        {
            Guard.ArgumentNotNull(meta, "meta");
        }

        #region Overrides

        public override object GetResourceInstance()
        {
            return CreateViewModelProvider();
        }

        #endregion

        #region Helpers

        protected ViewModelProvider CreateViewModelProvider()
        {
            return new ViewModelProvider(ResourceMeta);
        }

        #endregion

    }
}
