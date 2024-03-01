using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Azure.Cosmos;
using PortfolioTracker.DataAccess;
using PortfolioTracker.Domain;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.EventStore.Core;

namespace PortfolioTracker.ChangeFeed
{
    public static class AssetEventsFunction
    {
        [FunctionName(nameof(AssetEventsFunction))]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "PortfolioTracker",
            collectionName: "AssetEvents",
            ConnectionStringSetting = "CosmosDbConnectionString",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> input,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                try
                {
                    string sqlConnectionString = Environment.GetEnvironmentVariables()["SqlDbConnectionString"] as string;
                    if (sqlConnectionString == null) throw new Exception("SqlDbConnectionString not found");

                    string serviceBusConnection = Environment.GetEnvironmentVariables()["ServiceBusConnectionString"] as string;
                    if (sqlConnectionString == null) throw new Exception("ServiceBusConnectionString not found");

                    string cosmosConnectionString = Environment.GetEnvironmentVariables()["CosmosDbConnectionString"] as string;
                    using CosmosClient cosmosClient = new CosmosClient(cosmosConnectionString);
                    SnapshotRepository snapshotRepository = new SnapshotRepository(cosmosClient);
                    var eventStoreSerializer = new JsonStoredEventSerializer(typeof(AssetCreated).Assembly);
                    var eventStore = new AssetEventStore(eventStoreSerializer, cosmosClient.GetDatabase("PortfolioTracker"));
                    AssetArRepository assetArRepository = new(eventStore);

                    SqlDatabase sqlDatabase = new SqlDatabase(sqlConnectionString);
                    AssetRepository assetRepository = new(sqlDatabase);
                    TransactionRepository transactionRepository = new(sqlDatabase);
                    SnapshotTriggerSender snapshotTriggerSender = new SnapshotTriggerSender(serviceBusConnection);

                    foreach (var document in input)
                    {
                        AssetEvent @event = JsonConvert.DeserializeObject<AssetEvent>(document.ToString());
                        if (@event == null) return;

                        switch (@event.Name)
                        {
                            case nameof(AssetCreated):
                                {
                                    AssetCreated created = JsonConvert.DeserializeObject<AssetCreated>(@event.Data.ToString()!);
                                    await assetRepository.Create(created!);
                                    await snapshotTriggerSender.SendTriggersFromDate(created.CreatedAt, created.UserId);
                                    break;
                                }
                            case nameof(TransactionAdded):
                                {
                                    TransactionAdded added = JsonConvert.DeserializeObject<TransactionAdded>(@event.Data.ToString()!);
                                    await transactionRepository.Create(added!);
                                    Maybe<AssetAR> ar = await assetArRepository.Get(added.AssetId);
                                    if (ar.HasNoValue || ar.Value.Get().IsFailure) return;
                                    Asset asset = ar.Value.Get().Value;
                                    await assetRepository.Update(asset);
                                    await snapshotTriggerSender.SendTriggersFromDate(added.TransactionDate, added.UserId);
                                    break;
                                }
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class AssetEvent
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            public string Type { get; set; }
            public string CorrelationId { get; set; }
            public string StreamId { get; set; }
            public string Position { get; set; }
            public string Name { get; set; }
            public Object Data { get; set; }
            public string Metadata { get; set; }
            public DateTime CreatedUtc { get; set; }
        }
    }
}
