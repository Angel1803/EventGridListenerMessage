using EventGrid.MessageEvent;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using SharpEventGrid;
using System;

namespace ListenerEventGrid
{
    class Program
    {
        private static ServiceCollection services;
        //private static IEventBus eventBus;

        static void Main(string[] args)
        {
            var hybridRelayUrl = "Endpoint=sb://relayeventgrid.servicebus.windows.net/;SharedAccessKeyName=relaysas;SharedAccessKey=j11GFk1GcOEXbwcpXuRvBSCzl7bOavQCn4abjsTrCRs=;EntityPath=eventgrid";
            //var eventGridUrl = "https://new-cars.southcentralus-1.eventgrid.azure.net/api/events";
            //var eventGridKey = "YJW+hGFNyCmLHfBycJDSwGqbg7Ds9ZU1mPZKXIksaxk=";

            services = new ServiceCollection();

            //Se crea la conexión
            services.AddSingleton<IEventBus, EventGridCli>(sp =>
            {
                var eventGridSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                return new EventGridCli(hybridRelayUrl);
            });

            services.AddTransient<EventMessageHandler>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            var eventBus = services.BuildServiceProvider().GetRequiredService<IEventBus>();
            eventBus.Subscribe<Event, EventMessageHandler>();

            Console.ReadKey();
        }
    }
}
