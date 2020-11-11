namespace nRoute.Controllers.Mapping
{
    public interface IActionInvoker
    {
        void InvokeAction(ControllerContext actionContext, string actionName);
    }
}