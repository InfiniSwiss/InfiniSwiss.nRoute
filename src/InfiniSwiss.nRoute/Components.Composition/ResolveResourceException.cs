using System;

namespace nRoute.Components.Composition
{
    public class ResolveResourceException
        : Exception
    {
        private readonly Type _resourceType;

        public ResolveResourceException(string message)
            : this(message, null, null) { }

        public ResolveResourceException(string message, Type resourceType)
            : this(message, resourceType, null) { }

        public ResolveResourceException(string message, Type resourceType, Exception innerException)
            : base(message, innerException)
        {
            _resourceType = resourceType;
        }

        #region Properties

        public Type ResourceType
        {
            get { return _resourceType; }
        }

        #endregion

    }
}