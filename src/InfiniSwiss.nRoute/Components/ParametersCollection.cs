using nRoute.Internal;
using nRoute.Navigation.Containers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace nRoute.Components
{
    [Serializable]
    [KnownType(typeof(PageContentState))]
    [KnownType(typeof(PageContentState[]))]
    [KnownType(typeof(Object[][]))]
    //[CollectionDataContract(Name = "Parameters", ItemName = "Parameter")]
    //[KnownType(typeof(Parameter))]
    [XmlInclude(typeof(Parameter))]
    public class ParametersCollection
        : KeyedCollection<string, Parameter>
    {
        public ParametersCollection()
            : base(StringComparer.InvariantCultureIgnoreCase) { }

        public ParametersCollection(ParametersCollection collection)
            : this()
        {
            if (collection == null || collection.Count == 0) return;
            foreach (var _parameter in collection)
            {
                this.Add(_parameter);
            }
        }

        /// <remarks>This is not allowed as accessing non-public types outside declaring assembly is not allowed.</remarks>
        public ParametersCollection(object values)
            : this()
        {
            this.AddValues(values);
        }

        #region Overrides

        protected override string GetKeyForItem(Parameter item)
        {
            Guard.ArgumentNotNull(item, "item");
            return item.Key;
        }

        #endregion

        #region IDictionary<string,object> Members

        public void Add(string key, object value)
        {
            Guard.ArgumentNotNull(key, "key");
            Add(new Parameter(key, value));
        }

        public bool ContainsKey(string key)
        {
            return Contains(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            Guard.ArgumentNotNull(key, "key");
            if (Contains(key))
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
            get { return Items.Select((p) => p.Key).ToList(); }
        }

        public ICollection<object> Values
        {
            get { return Items.Select((p) => p.Value).ToList(); }
        }

        public new object this[string key]
        {
            get
            {
                return base[key].Value;
            }
            set
            {
                base[key].Value = value;
            }
        }

        #endregion

        #region Additional Methods

        private void AddValues(object values)
        {
            if (values != null)
            {
                // we get all the (public?) properties and read their values
                //BindingFlags.Public & BindingFlags.GetProperty doesn't work
                var _properties = values.GetType().GetProperties();

                foreach (var _prop in _properties)
                {
                    if (_prop.CanRead)
                        this.Add(_prop.Name, _prop.GetValue(values, null));
                }

                //foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
                //{
                //    object obj2 = descriptor.GetValue(values);
                //    this.Add(descriptor.Name, obj2);
                //}
            }
        }

        #endregion

    }
}