using nRoute.Internal;
using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace nRoute.Components
{
    [Serializable]
    //[DataContract]
    [ContentProperty("Value")]
    public class Parameter
        : IEquatable<Parameter>
    {
        private const string KEY_SETONCE_ONLY = "Parameter's key can only set once";

        private string _key;
        private object _value;

        public Parameter() { }

        public Parameter(string key, Object value)
        {
            this.Key = key;
            this.Value = value;
        }

        #region Properties

        //[DataMember]
        [Category("Common Properties")]
        public string Key
        {
            get { return _key; }
            set
            {
                Guard.ArgumentValue(_key != null, "Key", KEY_SETONCE_ONLY);
                _key = value;
            }
        }

        //[DataMember]
        [Category("Common Properties")]
        public Object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        #region IEquatable<Parameter> Members

        public bool Equals(Parameter other)
        {
            if (other == null) return false;
            return string.Equals(Key, other.Key, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

    }
}