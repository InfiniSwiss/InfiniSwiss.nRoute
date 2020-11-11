using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace nRoute.Components
{
    [Serializable]
    public abstract class KeyedObservableCollection<TKey, T>
        : ObservableCollection<T>
        where
            T : class
    {
        private const string KEY_NOTFOUND = "Key '{0}' not found";
        private const string ITEM_WITHKEY_EXISTS = "Another Item with '{0}' Key already exists";
        private const string ITEMKEY_CANNOT_BENULL = "Item Key cannot be null";

        private readonly IEqualityComparer<TKey> _comparer;

        protected KeyedObservableCollection() : this(null) { }

        protected KeyedObservableCollection(IEqualityComparer<TKey> comparer)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        #region Overrides

        protected override void InsertItem(int index, T item)
        {
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentNotDefault(GetKeyForItem(item), "item", ITEMKEY_CANNOT_BENULL);
            Guard.ArgumentValue((GetItem(GetKeyForItem(item)) != null), "item", ITEM_WITHKEY_EXISTS, GetKeyForItem(item));
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentNotDefault(GetKeyForItem(item), "item", ITEMKEY_CANNOT_BENULL);
            Guard.ArgumentValue((GetItem(GetKeyForItem(item)) != null), "item", ITEM_WITHKEY_EXISTS, GetKeyForItem(item));
            base.SetItem(index, item);
        }

        #endregion

        #region Added Memembers

        public bool Contains(TKey key)
        {
            Guard.ArgumentNotDefault(key, "key");
            return (GetItem(key) != null);
        }

        public T this[TKey key]
        {
            get
            {
                var _item = GetItem(key);
                if (_item == null) throw new KeyNotFoundException(string.Format(KEY_NOTFOUND, key));
                return _item;
            }
        }

        #endregion

        #region MustOverride

        protected abstract TKey GetKeyForItem(T item);

        #endregion

        #region Helpers

        protected T GetItem(TKey key)
        {
            Guard.ArgumentNotDefault(key, "key");
            return this.Items.FirstOrDefault((i) => _comparer.Equals(GetKeyForItem(i), key));
        }

        #endregion

    }
}