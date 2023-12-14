namespace Server.Util
{
    public class PushBody
    {
        public string? pushEndpoint { get; set; }
        public string? p256dh { get; set; }
        public string? auth { get; set; }
        public string? subject { get; set; }
        public string? publicKey { get; set; }
        public string? privateKey { get; set; }
        public dynamic? payload { get; set; }
    }
}
