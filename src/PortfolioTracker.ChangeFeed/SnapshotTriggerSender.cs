using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using PortfolioTracker.DataAccess.Models;

namespace PortfolioTracker.ChangeFeed
{
    public class SnapshotTriggerSender
    {
        private readonly string serviceBusConnection;

        public SnapshotTriggerSender(string serviceBusConnection)
        {
            this.serviceBusConnection = serviceBusConnection;
        }

        public async Task SendTriggersFromDate(DateTimeOffset date, string userId)
        {
            ServiceBusClient client = new ServiceBusClient(serviceBusConnection);
            ServiceBusSender sender = client.CreateSender("snapshot-trigger");
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            DateOnly sendFrom = DateOnly.FromDateTime(date.DateTime);
            while (sendFrom <= today)
            {
                messageBatch.TryAddMessage(new ServiceBusMessage(JsonConvert.SerializeObject(new SnapshotGenerationTrigger()
                {
                    UserId = userId,
                    Date = DateTime.Parse(sendFrom.ToString())
                })));

                sendFrom = sendFrom.AddDays(1);
            }

            if (messageBatch.Count > 0)
                await sender.SendMessagesAsync(messageBatch);

            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
