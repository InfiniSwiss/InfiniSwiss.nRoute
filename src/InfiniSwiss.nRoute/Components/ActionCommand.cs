using nRoute.Internal;
using System;

namespace nRoute.Components
{
    public class ActionCommand
         : CommandBase
    {
        private readonly Func<bool> _canExecuteHandler;
        private readonly Action _executeHandler;

        public ActionCommand(Action executeHandler)
            : this(executeHandler, null, true) { }

        public ActionCommand(Action executeHandler, bool isActive)
            : this(executeHandler, null, isActive) { }

        public ActionCommand(Action executeHandler, Func<bool> canExecuteHandler)
            : this(executeHandler, canExecuteHandler, true) { }

        public ActionCommand(Action executeHandler, Func<bool> canExecuteHandler, bool isActive)
            : base(isActive)
        {
            Guard.ArgumentNotNull(executeHandler, "executeHandler");

            _executeHandler = executeHandler;
            _canExecuteHandler = canExecuteHandler;
        }

        #region Overrides

        protected override bool OnCanExecute()
        {
            return _canExecuteHandler != null ? _canExecuteHandler() : true;
        }

        protected override void OnExecute()
        {
            _executeHandler();
        }

        #endregion

    }
}
