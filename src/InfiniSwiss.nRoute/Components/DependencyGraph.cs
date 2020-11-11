using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nRoute.Components
{
    internal static class DependencyGraph
    {
        private const string DEPENDENCY_WITHKEY_NOTFOUND = "'{0}' resource's dependent resource with key '{1}' not found";

        #region Static Methods

        public static IEnumerable<T> ResolveTopologicalOrder<T, TKey>(IEnumerable<T> items,
            Func<T, TKey> keyResolver, Func<T, IEnumerable<TKey>> dependenciesResolver)
            where
                TKey : IEquatable<TKey>
        {
            Guard.ArgumentNotNull(items, "items");
            Guard.ArgumentNotNull(keyResolver, "keyResolver");
            Guard.ArgumentNotNull(dependenciesResolver, "dependenciesResolver");

            var _orderedNodes = new List<DependencyNode<T, TKey>>();
            var _dependencyNodes = (List<DependencyNode<T, TKey>>)null;
            _dependencyNodes = items.Select((i) =>
                new DependencyNode<T, TKey>(
                    i,
                    new Lazy<TKey>(() => keyResolver(i)),
                    new Lazy<ICollection<DependencyNode<T, TKey>>>(() =>
                    {
                        var _dependenciesKeys = dependenciesResolver(i);
                        if (_dependenciesKeys == null) return new List<DependencyNode<T, TKey>>();
                        return _dependenciesKeys.Select((k) =>
                        {
                            var _dependentNode = _dependencyNodes.FirstOrDefault((n) => n.Key.Equals(k));
                            if (_dependentNode == null)
                            {
                                throw new InvalidOperationException(string.Format(DEPENDENCY_WITHKEY_NOTFOUND, keyResolver(i), k));
                            }
                            return _dependencyNodes.First((n) => n.Key.Equals(k));

                        }).ToList();
                    })
                )).ToList();

            foreach (var _node in _dependencyNodes)
            {
                _node.Visited = false;
            }

            foreach (var _node in _dependencyNodes)
            {
                DependencyNode<T, TKey>.Visit(_node, _orderedNodes, _node);
            }

            return _orderedNodes.Select((n) => (T)n.Item);
        }

        #endregion

        #region Nested Class

        /// <remarks>
        /// - Adopted from http://www.patrickdewane.com/2009/03/topological-sort.html
        /// </remarks>
        private class DependencyNode<T, TKey>
            : IEquatable<DependencyNode<T, TKey>>
            where
                TKey : IEquatable<TKey>
        {
            private const string CYCLIC_DEPENDENCY_DETECTED = "Cyclic dependency detected between key '{0}', directed towards key '{1}'";

            private readonly T _item;
            private readonly Lazy<TKey> _key;
            private readonly Lazy<ICollection<DependencyNode<T, TKey>>> _dependencies;

            public DependencyNode(T item, Lazy<TKey> key, Lazy<ICollection<DependencyNode<T, TKey>>> dependencies)
            {
                Guard.ArgumentNotDefault(item, "item");
                Guard.ArgumentNotNull(key, "key");
                Guard.ArgumentNotNull(dependencies, "dependenciesKeys");
                _item = item;
                _key = key;
                _dependencies = dependencies;
            }

            #region Properties

            public T Item
            {
                get { return _item; }
            }

            public TKey Key
            {
                get { return _key.Value; }
            }

            public ICollection<DependencyNode<T, TKey>> Dependencies
            {
                get { return _dependencies.Value; }
            }

            public bool Visited { get; set; }

            public DependencyNode<T, TKey> Root { get; set; }

            #endregion

            #region IEquatable<DependencyInfo<T, TKey>> Members

            public bool Equals(DependencyNode<T, TKey> other)
            {
                return other != null && Key.Equals(other.Key);
            }

            #endregion

            #region Static Methods

            public static bool Visit(DependencyNode<T, TKey> dependencyNode, ICollection<DependencyNode<T, TKey>> dependencyNodes,
                DependencyNode<T, TKey> rootDependencyNode)
            {
                if (dependencyNode.Visited) return (!dependencyNode.Root.Equals(rootDependencyNode));

                dependencyNode.Visited = true;
                dependencyNode.Root = rootDependencyNode;

                foreach (var _dependency in dependencyNode.Dependencies)
                {
                    if (!Visit(_dependency, dependencyNodes, rootDependencyNode) && dependencyNode != rootDependencyNode)
                    {
                        throw new InvalidOperationException(string.Format(CYCLIC_DEPENDENCY_DETECTED, dependencyNode.Key, _dependency.Key));
                    }
                }

                dependencyNodes.Add(dependencyNode);
                return true;
            }

            #endregion

        }

        #endregion

    }
}
