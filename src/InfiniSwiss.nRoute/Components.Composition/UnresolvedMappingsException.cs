using System;

namespace nRoute.Components.Composition
{
    [Serializable]
    public class UnresolvedMappingsException
         : Exception
    {
        // need to add more helpful message
        private const string UNRESOLVED_EXCEPTION_MESSAGE =
            "One or more mappings could not be resolved on target type '{0}' for attribute {1}.";

        public UnresolvedMappingsException(Type targetType, Type attributeType)
            : base(string.Format(UNRESOLVED_EXCEPTION_MESSAGE, targetType.FullName, attributeType.FullName))
        { }
    }
}
