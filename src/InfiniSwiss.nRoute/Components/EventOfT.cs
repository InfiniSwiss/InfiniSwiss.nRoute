using System;

namespace nRoute.Components
{
    public class Event<T>
        where T
         : EventArgs
    {
        private readonly Object _sender;
        private readonly T _eventArgs;

        public Event(Object sender, T eventArgs)
        {
            _sender = sender;
            _eventArgs = eventArgs;
        }

        public Object Sender
        {
            get { return _sender; }
        }

        public T EventArgs
        {
            get { return _eventArgs; }
        }
    }
}
