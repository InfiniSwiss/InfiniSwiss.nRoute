using System;
using System.ComponentModel;

namespace nRoute.Components
{
    public class ValueRelay
         : INotifyPropertyChanged
    {
        private bool _isInitialized;
        private object _value;

        public event EventHandler<ValueEventArgs<object>> Initialize;

        public object Value
        {
            get
            {
                if (_value == null && !_isInitialized) InitializeValue();
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged.Notify(() => Value);
                }
                if (!_isInitialized) _isInitialized = true;
            }
        }

        #region INotifyPropertyChanged Related

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Helper

        private void InitializeValue()
        {
            if (_isInitialized) return;
            if (Initialize != null)
            {
                var _eventArgs = new ValueEventArgs<object>();
                Initialize(this, _eventArgs);
                Value = _eventArgs.Value;
            }
            return;
        }

        #endregion

    }
}