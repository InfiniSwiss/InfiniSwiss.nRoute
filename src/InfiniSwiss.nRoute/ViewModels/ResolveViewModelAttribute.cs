using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewModels
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = false)]
    public class ResolveViewModelAttribute
        : ResolveResourceAttribute
    {
        private const string VIEWMODEL_COULDNOT_RESOLVE = "Could not resolve ViewModel for View of type '{0}'";

        private Type _viewType;

        public ResolveViewModelAttribute(Type viewType)
            : base()
        {
            Guard.ArgumentNotNull(viewType, "viewType");
            _viewType = viewType;
        }

        #region Properties

        public Type ViewType
        {
            get { return _viewType; }
        }

        #endregion

        #region Overrides

        protected internal override object ResolveResource(Type targetType)
        {
            var _viewModelProvider = (IViewModelProvider)null;
            if (ResourceLocator.TryGetResource<IViewModelProvider>(_viewType.FullName, out _viewModelProvider))
            {
                return _viewModelProvider.CreateViewModelInstance();
            }
            else
            {
                if (!this.IsNullable)
                {
                    throw new ResolveResourceException(string.Format(VIEWMODEL_COULDNOT_RESOLVE, _viewType.FullName));
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

    }
}
