using System;

namespace nRoute.Components
{
    public class CommandEventArgs
         : EventArgs
    {
        private readonly Object _commandParameter;

        public CommandEventArgs(Object commandParameter)
        {
            _commandParameter = commandParameter;
        }

        public Object CommandParameter
        {
            get { return _commandParameter; }
        }
    }
}
