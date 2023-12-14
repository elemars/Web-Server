using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Protocol;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;

namespace Server.Mqtt
{
    public static class MqttClient
    {
        private static IManagedMqttClient _client = null!;

        public static void Initialize()
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(10))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithProtocolVersion(MqttProtocolVersion.V500)
                    .WithTcpServer("172.16.16.21", 1883)
                    .WithCleanSession()
                    .Build())
                .Build();

            _client = new MqttFactory().CreateManagedMqttClient();

            _client.ConnectedAsync += ClientOnConnectedAsync;
            _client.DisconnectedAsync += ClientOnDisconnectedAsync;
            _client.ApplicationMessageReceivedAsync += ClientOnApplicationMessageReceivedAsync;
            _client.ConnectingFailedAsync += ClientOnConnectingFailedAsync;
            _client.SynchronizingSubscriptionsFailedAsync += ClientOnSynchronizingSubscriptionsFailedAsync;

            _client.StartAsync(options);

            // topic used for alarms
            Subscribe("Fenstermonitoring/#");
        }
        private static Task ClientOnSynchronizingSubscriptionsFailedAsync(ManagedProcessFailedEventArgs arg)
        {
            return Task.CompletedTask;
        }
        private static Task ClientOnConnectingFailedAsync(ConnectingFailedEventArgs arg)
        {
            return Task.CompletedTask;
        }
        private static Task ClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            return IncomingMessageHandler.HandleMessage(arg);
        }
        private static Task ClientOnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            return Task.CompletedTask;
        }
        private static Task ClientOnConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            return Task.CompletedTask;
        }
        private static void Subscribe(string topic)
        {
            _client.SubscribeAsync(topic, MqttQualityOfServiceLevel.AtLeastOnce);
        }
        public static void Unsubscribe(string topic)
        {
            _client.UnsubscribeAsync(topic);
        }
        public static async Task Publish(string topic, string message)
        {
            await _client.EnqueueAsync(topic, message, MqttQualityOfServiceLevel.AtLeastOnce);
        }       
    }
}