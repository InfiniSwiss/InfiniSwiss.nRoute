using System;

namespace nRoute.Components
{
    public class ValueEventArgs<T>
         : EventArgs
    {

        public ValueEventArgs() { }

        public ValueEventArgs(T value)
            : this()
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}
