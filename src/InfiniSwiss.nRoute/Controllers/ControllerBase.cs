using nRoute.Components;
using nRoute.Controllers.Mapping;
using nRoute.Internal;
using System;

namespace nRoute.Controllers
{
    public class ControllerBase : IController
    {
        private const string ACTION_KEY = "Action";
        private const string DEFAULT_ACTION_NAME = "Index";

        private ParametersCollection _viewDataDictionary;
        private string _action;
        private IActionInvoker _actionInvoker;

        #region Properties

        public IActionInvoker ActionInvoker
        {
            get { return _actionInvoker ?? new ControllerActionInvoker(this.GetType()); }
            set { _actionInvoker = value; }
        }

        public ControllerContext ActionContext { get; protected set; }

        public string Action
        {
            get { return _action; }
        }

        public ParametersCollection ViewData
        {
            get
            {
                if (_viewDataDictionary == null) _viewDataDictionary = new ParametersCollection();
                return _viewDataDictionary;
            }
            set
            {
                _viewDataDictionary = value;
            }
        }

        #endregion

        #region Overridable

        protected virtual void Initialize(ControllerContext actionContext)
        {
            Guard.ArgumentNotNull(actionContext, "actionContext");
            this.ActionContext = actionContext;
        }

        protected virtual void Execute()
        {
            ActionInvoker.InvokeAction(this.ActionContext, this.Action);
        }

        #endregion

        #region IController Members

        void IController.Execute(ControllerContext actionContext)
        {
            // Initialize
            Initialize(actionContext);

            // resolve the action name
            if (ActionContext.ResponseParameters.ContainsKey(ACTION_KEY))
            {
                _action = Convert.ToString(ActionContext.ResponseParameters[ACTION_KEY]);
            }

            // else set the default action name
            if (string.IsNullOrEmpty(_action))
            {
                _action = DEFAULT_ACTION_NAME;
            }

            // and execute
            Execute();
        }

        #endregion

    }
}