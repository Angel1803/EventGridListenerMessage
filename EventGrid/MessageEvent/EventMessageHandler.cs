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
        public async Task Handle(Event @event)
        {
            Console.WriteLine(@event.Data.ToString());
            await Task.CompletedTask;
        }
    }
}
