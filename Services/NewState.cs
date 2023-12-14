namespace Server.Services
{
    public class NewState : INewState
    {
        public event Action NewStateAvailable;

        public void HandleEvent()
        {
            NewStateAvailable?.Invoke();
        }
    }
}
