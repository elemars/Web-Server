namespace Server.Services
{
    public interface IAlarmMonitoringService
    {
        public void SetAlarmStatus(bool active);
        public bool GetAlarmStatus();
        public void HandleNewState();

        public Task<bool> SendPushNotification(string message, string iconUrl);
    }
}
