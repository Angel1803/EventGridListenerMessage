using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using SharpEventGrid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventGrid.MessageEvent
{
    public class EventMessageHandler : IIntegrationEventHandler<Event>
    {
        public Task Handle(Event @event)
        {
            Console.WriteLine(@event.Data.ToString());
            return Task.CompletedTask;
        }
    }
}
