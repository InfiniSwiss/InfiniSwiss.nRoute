using nRoute.Internal;
using System;

namespace nRoute.Navigation.Mapping
{
    public class NavigationContentMeta
         : NavigationMetaBase
    {
        private readonly Type _implementationType;

        public NavigationContentMeta(string url, Type implementationType)

         : base(url)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");
            _implementationType = implementationType;
        }

        #region Properties

        public Type ImplementationType
        {
            get { return _implementationType; }
        }

        #endregion

    }
}