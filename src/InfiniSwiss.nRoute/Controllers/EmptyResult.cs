namespace nRoute.Controllers
{
    public class EmptyResult
        : ActionResult
    {
        private static readonly EmptyResult EMPTY_RESULT_INSTANCE = new EmptyResult();

        public EmptyResult()
            : base() { }

        public override void ExecuteResult(ControllerContext context) { }

        #region Static Property

        public static EmptyResult Instance
        {
            get { return EMPTY_RESULT_INSTANCE; }
        }

        #endregion

    }
}
