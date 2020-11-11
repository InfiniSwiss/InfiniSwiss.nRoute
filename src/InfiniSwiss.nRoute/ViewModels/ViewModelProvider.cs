using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewModels
{
    public class ViewModelProvider
         : IViewModelProvider
    {
        private readonly ViewModelMeta _meta;

        internal ViewModelProvider(ViewModelMeta meta)
        {
            Guard.ArgumentNotNull(meta, "meta");
            _meta = meta;
        }

        #region Properties

        public ViewModelMeta ViewModelMeta
        {
            get { return _meta; }
        }

        #endregion

        #region IViewModelProvider Implementation

        public Object CreateViewInstance()
        {
            return TypeBuilder.BuildType(_meta.ViewType);
        }

        public Object CreateViewModelInstance()
        {
            return TypeBuilder.BuildType(_meta.ViewModelType);
        }

        #endregion

    }
}
