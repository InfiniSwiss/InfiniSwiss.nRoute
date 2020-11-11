using nRoute.Internal;
using System;
using System.Threading.Tasks;

namespace nRoute.Components
{
    public class AsyncActionCommand
         : CommandBase, IAsyncCommand
    {
        private readonly Func<bool> _canExecuteHandler;
        private readonly Func<Task> _executeHandler;
        private bool _isExecuting;

        public AsyncActionCommand(Func<Task> executeHandler)
            : this(executeHandler, null, true) { }

        public AsyncActionCommand(Func<Task> executeHandler, bool isActive)
            : this(executeHandler, null, isActive) { }

        public AsyncActionCommand(Func<Task> executeHandler, Func<bool> canExecuteHandler)
            : this(executeHandler, canExecuteHandler, true) { }

        public AsyncActionCommand(Func<Task> executeHandler, Func<bool> canExecuteHandler, bool isActive)
            : base(isActive)
        {
            Guard.ArgumentNotNull(executeHandler, "executeHandler");

            _executeHandler = executeHandler;
            _canExecuteHandler = canExecuteHandler;
        }

        public virtual async Task ExecuteAsync()
        {
            // here we go directly to OnExecute - also we call OnCommandExecuted as this is not called from the base class
            if (CanExecute())
            {
                await OnExecuteAsync();
                OnCommandExecuted(new CommandEventArgs(null));
            }
        }

        protected async Task OnExecuteAsync()
        {
            _isExecuting = true;
            try
            {
                await _executeHandler();
            }
            finally
            {
                _isExecuting = false;
            }
        }

        #region Overrides

        protected override bool OnCanExecute()
        {
            return _canExecuteHandler != null ? _canExecuteHandler() : true && !_isExecuting;
        }

        protected async override void OnExecute()
        {
            _isExecuting = true;
            try
            {
                await _executeHandler();
            }
            finally
            {
                _isExecuting = false;
            }
        }

        #endregion

    }
}
