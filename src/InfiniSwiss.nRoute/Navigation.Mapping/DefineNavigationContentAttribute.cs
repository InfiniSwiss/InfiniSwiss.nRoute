using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Navigation.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public class DefineNavigationContentAttribute
         : MapNavigationContentAttribute
    {
        private readonly Type _contentType;

        public DefineNavigationContentAttribute(string url, Type contentType)
         : base(url)
        {
            Guard.ArgumentNotNull(contentType, "contentType");
            _contentType = contentType;
        }

        #region overrides 

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            // we switch the target type with our given content type
            return base.GetResourceLocator(_contentType);
        }

        #endregion

    }
}