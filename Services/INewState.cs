namespace Server.Services
{
    public interface INewState
    {
        event Action NewStateAvailable;
        public void HandleEvent();
    }
}
