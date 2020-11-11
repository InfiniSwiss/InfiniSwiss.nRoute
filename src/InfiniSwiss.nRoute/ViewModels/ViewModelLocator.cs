using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.ViewModels
{
    public static class ViewModelLocator
    {

        #region Static Methods

        // Get ViewModel Related

        public static Object GetViewModel<TView>()
            where
                TView : class
        {
            return GetViewModel(typeof(TView));
        }

        public static Object GetViewModel(Type viewType)
        {
            Guard.ArgumentNotNull(viewType, "viewType");

            var _viewModelProvider = ResourceLocator.GetResource<IViewModelProvider>(viewType.FullName);
            return _viewModelProvider.CreateViewModelInstance();
        }

        // Try Get ViewModel Related

        public static bool TryGetViewModel<TView>(out object viewModelInstance)
            where
                TView : class
        {
            return TryGetViewModel(typeof(TView), out viewModelInstance);
        }

        public static bool TryGetViewModel(Type viewType, out object viewModelInstance)
        {
            Guard.ArgumentNotNull(viewType, "viewType");

            // we get the provider and then get the instance
            IViewModelProvider _viewModelProvider = null;
            var _response = ResourceLocator.TryGetResource<IViewModelProvider>(viewType.FullName, out _viewModelProvider);
            viewModelInstance = _viewModelProvider == null ? null : _viewModelProvider.CreateViewModelInstance();
            return _response;
        }

        // Registration Related

        public static bool IsViewModelRegistered<TView>()
            where
                TView : class
        {
            return IsViewModelRegistered(typeof(TView));
        }

        public static bool IsViewModelRegistered(Type viewType)
        {
            return ResourceLocator.IsResourceRegistered<IViewModelProvider>(viewType.FullName);
        }

        #endregion

    }
}