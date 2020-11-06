using EventGrid.MessageEvent;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SharpEventGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerEventGrid
{
    class Program
    {
        private static ServiceCollection services;
        //private static IEventBus eventBus;

        static void Main(string[] args)
        {
            Console.WriteLine("***EventGrid Listener Iniciado***");

            var eventGridUrl = "https://new-cars.southcentralus-1.eventgrid.azure.net/api/events";
            var eventGridKey = "YJW+hGFNyCmLHfBycJDSwGqbg7Ds9ZU1mPZKXIksaxk=";
            //var resourceGroupName = "Demo-EventGrid";
            //var topicName = "new-cars";
            //var eventSubscriptionName = "queue-sub";

            services = new ServiceCollection();

            //Se crea la conexión
            //services.AddSingleton<IEventBus, EventGridCli>(sp =>
            //{
            //    var eventGridSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
            //    return new EventGridCli(new Uri(eventGridUrl), eventGridKey, resourceGroupName, topicName, eventSubscriptionName, eventGridUrl, eventGridSubcriptionsManager);
            //});

            //services.AddTransient<EventMessageHandler>();
            //services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            //var clientConnection = new EventGridCli(new Uri(eventGridUrl), eventGridKey, resourceGroupName, topicName, eventSubscriptionName, eventGridUrl);
            var clientConnection = new EventGridCli(new Uri(eventGridUrl), eventGridKey);
            clientConnection.Subscribe<Event, EventMessageHandler>();
            //ConfigureEventBusSuscribe(services);
            Console.ReadKey();
        }

        //public static void ConfigureEventBusSuscribe(IServiceCollection services)
        //{
        //    //Llamamos al método genérico de la interfaz de IEventBus "Suscribe"
        //    eventBus = services.BuildServiceProvider().GetService<IEventBus>();
        //    eventBus.Subscribe<Event, EventMessageHandler>();
        //}
    }
}
