using nRoute.Internal;
using System;
using System.Threading.Tasks;

namespace nRoute.Components
{
    public class AsyncActionCommand<T>
         : CommandBase<T>, IAsyncCommand<T>
    {

        #region Declarations

        private readonly Func<T, bool> _canExecuteHandler;
        private readonly Func<T, Task> _executeHandler;
        private bool _isExecuting;

        #endregion

        public AsyncActionCommand(Func<T, Task> executeHandler)
            : this(executeHandler, null, true) { }

        public AsyncActionCommand(Func<T, Task> executeHandler, bool isActive)
            : this(executeHandler, null, isActive) { }

        public AsyncActionCommand(Func<T, Task> executeHandler, Func<T, bool> canExecuteHandler)
            : this(executeHandler, canExecuteHandler, true) { }

        public AsyncActionCommand(Func<T, Task> executeHandler, Func<T, bool> canExecuteHandler, bool isActive)
            : base(isActive)
        {
            Guard.ArgumentNotNull(executeHandler, "executeHandler");

            _executeHandler = executeHandler;
            _canExecuteHandler = canExecuteHandler;
        }

        public virtual async Task ExecuteAsync(T parameter)
        {
            // here we go directly to OnExecute - also we call OnCommandExecuted as this is not called from the base class
            if (CanExecute(parameter))
            {
                await OnExecuteAsync(parameter);
                OnCommandExecuted(new CommandEventArgs(null));
            }
        }

        protected async Task OnExecuteAsync(T parameter)
        {
            _isExecuting = true;
            try
            {
                await _executeHandler(parameter);
            }
            finally
            {
                _isExecuting = false;
            }
        }

        #region Override

        protected override bool OnCanExecute(T parameter)
        {
            return _canExecuteHandler != null ? _canExecuteHandler(parameter) : true && !_isExecuting;
        }

        protected async override void OnExecute(T parameter)
        {
            _isExecuting = true;
            try
            {
                await _executeHandler(parameter);
            }
            finally
            {
                _isExecuting = false;
            }
        }

        #endregion

    }
}