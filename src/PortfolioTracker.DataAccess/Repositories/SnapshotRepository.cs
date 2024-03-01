using Microsoft.Azure.Cosmos;
using PortfolioTracker.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class SnapshotRepository
    {
        private readonly CosmosClient cosmosClient;
        public const string SnapshotsContainerId = "Snapshots";

        //TODO: move to the settings
        private const string DatabaseId = "PortfolioTracker";
        public SnapshotRepository(CosmosClient cosmosClient)
        {
            this.cosmosClient = cosmosClient;
        }

        public Task Upsert(Snapshot snapshot)
        {
            return cosmosClient.GetDatabase(DatabaseId).GetContainer(SnapshotsContainerId)
            .UpsertItemAsync(snapshot, new PartitionKey(snapshot.UserId));
        }

        public async Task<List<Snapshot>> Get(string userId)
        {
            var iterator = cosmosClient.GetDatabase(DatabaseId).GetContainer(SnapshotsContainerId).GetItemQueryIterator<Snapshot>($"Select * from c",
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(userId)
                });

            var results = new List<Snapshot>();

            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync();
                results.AddRange(result.Resource);
            }

            return results;
        }
    }
}
