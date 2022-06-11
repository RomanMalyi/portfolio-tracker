using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PortfolioTracker.DataAccess;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.DataAccess.Repositories;

namespace PortfolioTracker.SnapshotTrigger
{
    public class SnapshotTriggerFunction
    {
        //TODO: change RunOnStartup to false for production
        [FunctionName(nameof(SnapshotTriggerFunction))]
        public async Task Run([TimerTrigger("0 0 2 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string sqlConnectionString = Environment.GetEnvironmentVariables()["SqlDbConnectionString"] as string;
            SqlDatabase sqlDatabase = new SqlDatabase(sqlConnectionString);
            UserRepository userRepository = new UserRepository();
            //TODO: add pagination throw all users
            var users = await userRepository.Get(0, 1000);

            string serviceBusConnection = Environment.GetEnvironmentVariables()["ServiceBusConnectionString"] as string;

            ServiceBusClient client = new ServiceBusClient(serviceBusConnection);
            ServiceBusSender sender = client.CreateSender("snapshot-trigger");
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var user in users)
            {
                messageBatch.TryAddMessage(new ServiceBusMessage(JsonConvert.SerializeObject(new SnapshotGenerationTrigger()
                {
                    UserId = user.Id,
                    Date = DateTime.Today
                })));
            }

            await sender.SendMessagesAsync(messageBatch);

            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
