using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace nRoute.Components
{
    public partial class DependencyParameterCollection
        : FreezableCollection<DependencyParameter>
    {
        private const string KEY_ALREADY_EXISTS = "Another DependencyParameter with key '{0}' exists";
        private const string PARAMETER_ALREADY_EXISTS = "DependencyParameter already exists in the collection";

        private HashSet<string> _keysSnapshot;

        public DependencyParameterCollection()
        {
            _keysSnapshot = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            ((INotifyCollectionChanged)this).CollectionChanged += DependencyParametersCollection_CollectionChanged;
        }

        #region IDictionary<string,object> Members

        public void Add(string key, object value)
        {
            Guard.ArgumentNotNull(key, "key");
            Add(new DependencyParameter(key, value));
        }

        public bool ContainsKey(string key)
        {
            return _keysSnapshot.Contains(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            Guard.ArgumentNotNull(key, "key");
            if (_keysSnapshot.Contains(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public ICollection<string> Keys
        {
            get { return this.Select((p) => p.Key).ToList(); }
        }

        public ICollection<object> Values
        {
            get { return this.Select((p) => p.Value).ToList(); }
        }

        public object this[string key]
        {
            get
            {
                return this.First((p) => string.Equals(p.Key, key, StringComparison.InvariantCultureIgnoreCase)).Value;
            }
            set
            {
                this.First((p) => string.Equals(p.Key, key, StringComparison.InvariantCultureIgnoreCase)).Value = value;
            }
        }

        #endregion

        #region Overridable

        protected virtual void ItemAdded(DependencyParameter parameter) { }

        protected virtual void ItemRemoved(DependencyParameter parameter) { }

        #endregion

        #region Handlers

        private void DependencyParametersCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (DependencyParameter _parameter in e.NewItems)
                    {
                        VerifyAdd(_parameter);
                        AddItemKey(_parameter.Key);
                        ItemAdded(_parameter);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (DependencyParameter _parameter in e.OldItems)
                    {
                        ItemRemoved(_parameter);
                        RemoveItemKey(_parameter.Key);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (DependencyParameter _parameter in e.OldItems)
                    {
                        ItemRemoved(_parameter);
                        RemoveItemKey(_parameter.Key);
                    }
                    foreach (DependencyParameter _parameter in e.NewItems)
                    {
                        VerifyAdd(_parameter);
                        AddItemKey(_parameter.Key);
                        ItemAdded(_parameter);
                        continue;
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (DependencyParameter _parameter in this)
                    {
                        ItemRemoved(_parameter);
                    }
                    var _items = this.ToArray();
                    _keysSnapshot.Clear();
                    _keysSnapshot = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                    foreach (DependencyParameter _parameter in this)
                    {
                        VerifyAdd(_parameter);
                        AddItemKey(_parameter.Key);
                        ItemAdded(_parameter);
                    }
                    break;

                default:
                    return;
            }
        }

        #endregion

        #region Helpers

        private void VerifyAdd(DependencyParameter item)
        {
            if (_keysSnapshot.Contains(item.Key, StringComparer.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException(PARAMETER_ALREADY_EXISTS);
            }
        }

        private void AddItemKey(string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, "parameter");
            Guard.ArgumentValue(_keysSnapshot.Contains(key), "parameter", KEY_ALREADY_EXISTS, key);
        }

        private void RemoveItemKey(string key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, "parameter");
            _keysSnapshot.Remove(key);
        }

        #endregion

        #region Operators

        public static implicit operator ParametersCollection(DependencyParameterCollection collection)
        {
            if (collection == null) return null;

            var _collection = new ParametersCollection();
            foreach (var _parameter in collection)
            {
                _collection.Add(_parameter.Key, _parameter.Value);
            }
            return _collection;
        }

        #endregion

    }
}
