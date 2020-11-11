namespace nRoute.ApplicationServices
{
    public class ApplicationStateInfo
    {
        public ApplicationStateInfo(ApplicationState currentState)
        {
            CurrentState = currentState;
        }

        public ApplicationState CurrentState { get; }
    }
}
