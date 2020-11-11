using Microsoft.Azure.Relay;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SharpEventGrid
{
    public class EventGridCli : IEventBus// : IEventGridClient 
    {
        //private HttpClient _client;
        //private HttpRequest _httpRequest;
        private string _hybridRelayUrl;
        //private string key;
        
        public EventGridCli(string hybridRelayUrl)
        {
            _hybridRelayUrl = hybridRelayUrl;
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _ = HybridConnectionListener();
        }
        
        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public async Task HybridConnectionListener()
        {
            HybridConnectionListener listener = null;

            try
            {
                listener = new HybridConnectionListener(_hybridRelayUrl);
                listener.Connecting += (o, hce) =>
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"[{DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:MM:ss.fff")}] HybridConnection: Connecting, listener:{listener.Address}");
                    Console.ResetColor();
                };
                listener.Online += (o, hce) =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:MM:ss.fff")}] HybridConnection: Online, listener:{listener.Address}");
                    Console.ResetColor();
                };
                listener.Offline += (o, hce) =>
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:MM:ss.fff")}] HybridConnection: Offline, listener:{listener.Address}");
                    Console.ResetColor();
                };

                listener.RequestHandler = (context) =>
                {
                    try
                    {
                        if (!context.Request.Headers.AllKeys.Contains("Aeg-Event-Type", StringComparer.OrdinalIgnoreCase) || !string.Equals(context.Request.Headers["Aeg-Event-Type"], "Notification", StringComparison.CurrentCultureIgnoreCase))
                            throw new Exception("Received message is not for EventGrid subscriber");

                        string jsontext = null;
                        using (var reader = new StreamReader(context.Request.InputStream))
                        {
                            var jtoken = JToken.Parse(reader.ReadToEnd());
                            if (jtoken is JArray)
                                jsontext = jtoken.SingleOrDefault<JToken>().ToString(Newtonsoft.Json.Formatting.Indented);
                            else if (jtoken is JObject)
                                jsontext = jtoken.ToString(Newtonsoft.Json.Formatting.Indented);
                            else if (jtoken is JValue)
                                throw new Exception($"The payload (JValue) is not accepted. JValue={jtoken.ToString(Newtonsoft.Json.Formatting.None)}");
                        }

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"[{DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:MM:ss.fff")}] Headers: {string.Join(" | ", context.Request.Headers.AllKeys.Where(i => i.StartsWith("aeg-") || i.StartsWith("Content-Type")).Select(i => $"{i}={context.Request.Headers[i]}"))}");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{jsontext}");

                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:MM:ss.fff")}] HybridConnection: Message processing failed - {ex.Message}");
                    }
                    finally
                    {
                        context.Response.StatusCode = HttpStatusCode.NoContent;
                        context.Response.Close();
                        Console.ResetColor();
                    }
                };
                await listener.OpenAsync(TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:MM:ss.fff")}] Open HybridConnection failed - {ex.Message}");
                Console.ResetColor();
            }

            Console.ReadLine();

            if (listener != null)
                await listener.CloseAsync();
        }
    }
}
