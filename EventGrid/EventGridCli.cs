using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Management.EventGrid;
using Microsoft.Azure.Management.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;  
using System.Threading.Tasks;

namespace SharpEventGrid {
    public class EventGridCli : IEventBus// : IEventGridClient 
    {
        private HttpClient _client;
        //private HttpRequest _httpRequest;
        private readonly Uri _eventGridUri;
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";
        private readonly ILogger<EventGridCli> _logger;
        private string key;

        private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings 
        {   
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        //public EventGridCli(Uri eventGridUri, string sasKey, HttpMessageHandler messageHandler = null) 
        //{
        //    _eventGridUri = eventGridUri;
        //    _client = new HttpClient(messageHandler ?? new HttpClientHandler());
        //    _client.DefaultRequestHeaders.Add("aeg-sas-key", sasKey);
        //}

        //public EventGridCli(Uri eventGridUri, string sasKey, string resourceGroupName, string topicName, string eventSubscriptionName, string eventGridUrl, IEventBusSubscriptionsManager subsManager, HttpMessageHandler messageHandler = null)
        public EventGridCli(Uri eventGridUri, string sasKey, HttpMessageHandler messageHandler = null)
        {
            _eventGridUri = eventGridUri;
            _client = new HttpClient(messageHandler ?? new HttpClientHandler());
            _client.DefaultRequestHeaders.Add("aeg-sas-key", sasKey);
            key = sasKey;
            //------------------------------------------------------------------
            //_resourceGroupName = resourceGroupName;
            //_topicName = topicName;
            //_eventSubscriptionName = eventSubscriptionName;
            //_endpointUrl = eventGridUrl;
            //_subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager(); 
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            TopicCredentials topicCredentials = new TopicCredentials(key);
            EventGridSubscriber eventGridSubscriber = new EventGridSubscriber();
            //eventGridSubscriber.;
            



            //var eventName = _subsManager.GetEventKey<T>();
            //DoInternalSubscription(eventName);

            //EventSubscription eventSubscription = new EventSubscription();
            //var topic = eventSubscription.Topic;
            //topic.
            //Console.WriteLine(eventSubscription.Topic.ToString());

            //string token = await GetAuthorizationHeaderAsync();
            //TokenCredentials credential = new TokenCredentials(token);

            //EventGridManagementClient eventGridManagementClient = new EventGridManagementClient(credential)
            //{
            //    SubscriptionId = SubscriptionId, LongRunningOperationRetryTimeout = 2
            //};

            //_ = CreateEventGridEventSubscriptionAsync(_resourceGroupName, _topicName, _eventSubscriptionName, eventGridManagementClient, _endpointUrl);
        }

        //private void DoInternalSubscription(string eventName)
        //{
        //    var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
        //    if (!containsKey)
        //    {
        //        if (!_eventGridUri.IsConnected)
        //        {
        //            _persistentConnection.TryConnect();
        //        }

        //        using (var channel = _persistentConnection.CreateModel())
        //        {
        //            channel.QueueBind(queue: _queueName,
        //                              exchange: BROKER_NAME,
        //                              routingKey: eventName);
        //        }
        //    }
        //}

        //static async Task CreateEventGridEventSubscriptionAsync(string rgname, string topicName, string eventSubscriptionName, EventGridManagementClient eventGridMgmtClient, string endpointUrl)
        //{
        //    Topic topic = await eventGridMgmtClient.Topics.GetAsync(rgname, topicName);
        //    string eventSubscriptionScope = topic.Id;

        //    Console.WriteLine($"Creando un evento de suscripcion al Topic: {topicName}...");

        //    EventSubscription eventSubscription = new EventSubscription()
        //    {
        //        Destination = new WebHookEventSubscriptionDestination()
        //        {
        //            EndpointUrl = endpointUrl
        //        },
        //        // The below are all optional settings
        //        EventDeliverySchema = EventDeliverySchema.EventGridSchema,
        //        Filter = new EventSubscriptionFilter()
        //        {
        //            // By default, "All" event types are included
        //            IsSubjectCaseSensitive = false,
        //            SubjectBeginsWith = "",
        //            SubjectEndsWith = ""
        //        }
        //    };

        //    EventSubscription createdEventSubscription = await eventGridMgmtClient.EventSubscriptions.CreateOrUpdateAsync(eventSubscriptionScope, eventSubscriptionName, eventSubscription);
        //    Console.WriteLine("EventGrid event subscription created with name " + createdEventSubscription.Name);
        //}

        //static async Task<string> GetAuthorizationHeaderAsync()
        //{
        //    ClientCredential cc = new ClientCredential(ApplicationId, Password);
        //    var context = new AuthenticationContext("https://login.windows.net/" + TenantId);
        //    var result = await context.AcquireTokenAsync("https://management.azure.com/", cc);

        //    if (result == null)
        //    {
        //        throw new InvalidOperationException("Failed to obtain the JWT token. Please verify the values for your applicationId, Password, and Tenant.");
        //    }

        //    string token = result.AccessToken;
        //    return token;
        //}

        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            Console.WriteLine("Entre al async");
            log.LogInformation("C# HTTP trigger function processed a request.");
            string response = string.Empty;
            string requestContent = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"Received events: {requestContent}");
            Console.WriteLine(requestContent.ToString());
            EventGridSubscriber eventGridSubscriber = new EventGridSubscriber();

            EventGridEvent[] eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);
            

            foreach (EventGridEvent eventGridEvent in eventGridEvents)
            {
                if (eventGridEvent.Data is SubscriptionValidationEventData)
                {
                    var eventData = (SubscriptionValidationEventData)eventGridEvent.Data;
                    log.LogInformation($"Got SubscriptionValidation event data, validation code: {eventData.ValidationCode}, topic: {eventGridEvent.Topic}");
                    // Do any additional validation (as required) and then return back the below response

                    var responseData = new SubscriptionValidationResponse()
                    {
                        ValidationResponse = eventData.ValidationCode
                    };
                    return new OkObjectResult(responseData);
                }
            }
            return new OkObjectResult(response);
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
    }
}
