using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortfolioTracker.DataAccess;

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

                    SqlDatabase sqlDatabase = new SqlDatabase(sqlConnectionString);
                    AssetRepository assetRepository = new(sqlDatabase);
                    TransactionRepository transactionRepository = new(sqlDatabase);

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
                                break;
                            }
                            case nameof(TransactionAdded):
                            {
                                TransactionAdded added = JsonConvert.DeserializeObject<TransactionAdded>(@event.Data.ToString()!);
                                await transactionRepository.Create(added!);
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
