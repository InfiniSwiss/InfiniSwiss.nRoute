using nRoute.Internal;
using System;

namespace nRoute.ViewModels
{
    public class ViewModelMeta
    {
        private readonly Type _viewType;
        private readonly Type _viewModelType;

        public ViewModelMeta(Type viewType, Type viewModelType)
        {
            Guard.ArgumentNotNull(viewType, "viewType");
            Guard.ArgumentNotNull(viewModelType, "viewModelType");

            _viewType = viewType;
            _viewModelType = viewModelType;
        }

        #region Properties

        public string Name
        {
            get { return _viewType.FullName; }
        }

        public Type ViewType
        {
            get
            {
                return _viewType;
            }
        }

        public Type ViewModelType
        {
            get
            {
                return _viewModelType;
            }
        }

        #endregion

    }
}
