using nRoute.Components.Composition;
using System;

namespace nRoute.Navigation.Mapping
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class MapNavigationContentAttribute
         : MapNavigationBaseAttribute
    {
        public MapNavigationContentAttribute(string url)
         : base(url) { }

        #region Overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            return new DefaultNavigationContentLocator(new NavigationContentMeta(Url, targetType));
        }

        #endregion

    }
}
