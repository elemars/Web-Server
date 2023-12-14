using Server.Controllers;
using Server.Util;
using Newtonsoft.Json;
using System.Text;
using Server.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPush;

namespace Server.Services
{
    public class AlarmMonitoringService : IAlarmMonitoringService
    {
        private static bool IsActive { get; set; } = false;
        public static string? PrivateKey { get; set; }
        public static string? PublicKey { get; set; }
        public static string? Subject { get; set; }
        private readonly IServiceScopeFactory scopeFactory;
        private readonly INewState _newState;

        public AlarmMonitoringService(INewState newState, IServiceScopeFactory scopeFactory)
        {
            _newState = newState;
            _newState.NewStateAvailable += HandleNewState;
            this.scopeFactory = scopeFactory;
        }
        public void SetAlarmStatus(bool active)
        {
            IsActive = active;
        }
        public bool GetAlarmStatus()
        {
            return IsActive;
        }
        public async void HandleNewState()
        {
            Console.WriteLine($"IsActive: {IsActive}");

            if (IsActive)
            {
                //wenn neuer Status, dann Alarm (Push Benachrichtigung)
                Console.WriteLine("Alarm!");
                await SendPushNotification("Alarm wurde ausgelöst!", "/warning.png");
                //warning.png stammt von <a href="https://www.flaticon.com/free-icons/alert" title="alert icons">Alert icons created by riajulislam - Flaticon</a>
            }
            else
            {
                //nur Benachrichtigung über neuen Daten
                Console.WriteLine("Neue Daten verfügbar");
                await SendPushNotification("Neue Daten verfügbar", "/icon-48.png");
            }
        }
        public async Task<bool> SendPushNotification(string message, string iconUrl)
        {
            var notification = new
            {
                title = "Alarmanlage",
                body = message,
                icon = iconUrl,
                vibrate = new List<int> { 200, 100, 200 }
            };
            var payload = JsonConvert.SerializeObject(notification);
            using (var scope = scopeFactory.CreateScope())
            {
                var _subscriptionContext = scope.ServiceProvider.GetRequiredService<PushSubscriptionContext>();
                try
                {
                    // Lesen der PushSubscriptionEntities aus der Datenbank
                    var list = await _subscriptionContext.PushSubscriptionEntities.ToListAsync();

                    if (list == null)
                    {
                        Console.WriteLine("Keine PushSubscriptionEntity gefunden.");
                        return false;
                    }
                    var webPushClient = new WebPushClient();
                    webPushClient.SetVapidDetails(Subject, PublicKey, PrivateKey);
                    //Push-Benachrichtigung an alle senden
                    foreach (var pushSubscriptionEntity in list)
                    {
                        // Erstellen der Push-Benachrichtigung
                        var pushSubscription = new PushSubscription(pushSubscriptionEntity.Url, pushSubscriptionEntity.P256dh, pushSubscriptionEntity.Auth);
                        Console.WriteLine("neue Subscription erstellt");

                        // Senden der Push-Benachrichtigung
                        await webPushClient.SendNotificationAsync(pushSubscription, payload);
                    }
                    return true;
                }
                catch (WebPushException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }
    }
}
