using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Server.Services;

namespace Server.Mqtt
{
    public static class IncomingMessageHandler
    {
        private static readonly HttpClient _httpClient = new();
        private static readonly TokenService _tokenGenerator = new();

        public static async Task HandleMessage(MqttApplicationMessageReceivedEventArgs arg)
        {
            TimeZoneInfo gmtPlus1TimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            try
            {
                //Console.WriteLine(Program.SecretKey);
                var jwt = _tokenGenerator.GenerateLoginToken("MqttUser", "admin");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
                if (arg.ApplicationMessage.Topic == null) return;
                // empfangene Nachricht richtig einlesen
                var topic = arg.ApplicationMessage.Topic;
                var payloadString = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                dynamic payload = JObject.Parse(payloadString); //JSON Format aus String lesen
                // Ausgabe zum Testen
                //Console.WriteLine(payloadString);
                if (topic == null || payload == null)
                {
                    Console.WriteLine("Fehler beim Verarbeiten der MQTT-Nachricht");
                    return;
                }
                else if (topic.Contains("NFC")) //NFC Tag erkannt
                {
                    //Console.WriteLine($"UID: {payload.uid}");
                    //Console.WriteLine($"Länge: {payload.lenght}");

                    var urlUser = "https://localhost:7009/api/Account";
                    HttpResponseMessage FoundUser = await _httpClient.GetAsync($"{urlUser}/NFC/{payload.uid}?UID={payload.uid}");
                    if (FoundUser.IsSuccessStatusCode)
                    {
                        var teststring = await FoundUser.Content.ReadAsStringAsync();
                        var UserID = Convert.ToInt32(teststring);

                        Console.WriteLine($"UserID: {UserID}");
                    }
                }
                else          //Statusmeldung verarbeiten
                {
                    //Daten zwischenspeichern
                    string macAddress = Convert.ToString(payload.device);
                    decimal voltage = Convert.ToDecimal(payload.voltage);
                    int StateValue = Convert.ToInt16(payload.state);

                    if (string.IsNullOrEmpty(macAddress))   //Mac-Adresse nicht zuordenbar?
                    {
                        return;
                    }
                    var urlDev = "https://localhost:7009/api/Devices";
                    var urlSta = "https://localhost:7009/api/States";

                    //var content = new StringContent("{\"property\":{macAddress}}", Encoding.UTF8, "application/json");
                    //Primärschlüssl über Mac-Adresse herausfinden
                    HttpResponseMessage FoundDevice = await _httpClient.GetAsync($"{urlDev}/mac/{macAddress}?MACa={macAddress}");//MAC-Adresse senden und Primärschlüssel erhalten
                    if (FoundDevice.IsSuccessStatusCode)
                    {
                        var teststring = await FoundDevice.Content.ReadAsStringAsync();
                        var DevID = Convert.ToInt32(teststring);

                        //Batteriespannung mit Primärschlüssel speichern
                        string voltageString = Convert.ToString(voltage);
                        FoundDevice = await _httpClient.PutAsync($"{urlDev}/voltage/{DevID}/{voltage}/{StateValue}", null);
                        teststring = await FoundDevice.Content.ReadAsStringAsync();

                        //Zeitzone richtig einstellen
                        DateTime utcNow = DateTime.UtcNow;
                        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, gmtPlus1TimeZone);

                        //neuen State anlegen
                        var state = new
                        {
                            Deviceid = DevID,
                            Statevalue = StateValue,
                            Timestamp = localTime
                        };
                        //Status als JSON verpacken und in http Anfrage senden
                        var json = JsonConvert.SerializeObject(state);
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        FoundDevice = await _httpClient.PostAsync(urlSta, content);
                        //Antwort auswerten
                        //var x1 = FoundDevice.Content;
                        //Console.WriteLine($"Requestantwort: {x1}");
                        //var statusCode = FoundDevice.StatusCode;
                        //Console.WriteLine($"Statuscode: {statusCode}");
                        Console.WriteLine($"Http-Antwort: {FoundDevice}");

                        if (FoundDevice.IsSuccessStatusCode)
                        {
                            return;
                        }
                    }
                    Console.WriteLine("Fehler beim erstellen des neuen Statuses");
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
        }
    }
}
