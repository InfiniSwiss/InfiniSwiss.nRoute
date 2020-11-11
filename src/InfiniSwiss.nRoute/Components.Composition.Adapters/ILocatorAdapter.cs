using System;

namespace nRoute.Components.Composition.Adapters
{
    /// <summary>
    /// Represents an adapter that helps resolve a specified of resource against the <see cref="ResourceLocator"/>
    /// </summary>
    public interface ILocatorAdapter
    {
        /// <summary>
        /// Resolves a resource request
        /// </summary>
        /// <param name="name">The optional name/key associated with resource</param>
        /// <returns>An instance of the requested resource</returns>
        Object Resolve(string name);
    }
}
