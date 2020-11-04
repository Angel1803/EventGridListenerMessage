using EventGrid.MessageEvent;
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
        private static IEventBus eventBus;

        static void Main(string[] args)
        {
            Console.WriteLine("***EventGrid Listener Iniciado***");
            var eventGridUrl = "https://new-cars.southcentralus-1.eventgrid.azure.net/api/events";
            var eventGridKey = "YJW+hGFNyCmLHfBycJDSwGqbg7Ds9ZU1mPZKXIksaxk=";

            services = new ServiceCollection();

            //Se crea la conexión
            var clientConnection = new EventGridClient(new Uri(eventGridUrl), eventGridKey);

            ConfigureEventBusSuscribe(services);
        }

        public static void ConfigureEventBusSuscribe(IServiceCollection services)
        {
            //Llamamos al método genérico de la interfaz de IEventBus "Suscribe"
            eventBus.Subscribe<Event, EventMessageHandler>();
        }
    }
}
