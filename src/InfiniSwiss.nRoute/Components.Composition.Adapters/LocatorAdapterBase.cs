using nRoute.Internal;
using System;

namespace nRoute.Components.Composition.Adapters
{
    /// <summary>
    /// Represents an <see cref="ILocatorAdapter"/> implementation that adapts a <see cref="T"/> to <see cref="R"/>
    /// </summary>
    /// <typeparam name="T">The type of resource</typeparam>
    /// <typeparam name="R">The type that resource is adapted to</typeparam>
    public abstract class LocatorAdapterBase<T, R>
        : ILocatorAdapter
    {
        private readonly Func<string, R> _resolver;

        protected LocatorAdapterBase(Func<string, R> resolver)
        {
            Guard.ArgumentNotNull(resolver, "resolver");
            _resolver = resolver;
        }

        #region ILocatorAdapter Members

        object ILocatorAdapter.Resolve(string name)
        {
            return Resolve(name);
        }

        #endregion

        /// <summary>
        /// Resolves a request for a resource
        /// </summary>
        /// <param name="name">The optional name/key associated with resource</param>
        /// <returns>An instance of the requested resource of type <see cref="R"/></returns>
        public virtual R Resolve(string name)
        {
            return _resolver(name);
        }
    }
}
