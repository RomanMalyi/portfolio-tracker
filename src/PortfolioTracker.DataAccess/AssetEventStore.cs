using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PortfolioTracker.EventStore;
using PortfolioTracker.EventStore.Core;

namespace PortfolioTracker.DataAccess
{
    public class AssetEventStore : IEventStore
    {
        private readonly Container container;
        private readonly IStoredEventSerializer storedEventSerializer;

        private const string StoredProcedureId = "AppendEvents";
        public const string AssetsContainerId = "AssetEvents";
        private readonly JsonSerializerSettings jsonSerializer;

        public AssetEventStore(IStoredEventSerializer storedEventSerializer, Database database)
        {
            this.storedEventSerializer = storedEventSerializer;
            InitializeAssetEventStoreContainer(database).GetAwaiter().GetResult();
            container = database.GetContainer(AssetsContainerId);

            jsonSerializer = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task InitializeAssetEventStoreContainer(Database database)
        {
            await database.CreateContainerIfNotExistsAsync(AssetsContainerId, "/StreamId");
        }

        public async Task InitializeAppendEventsStoredProcedure()
        {
            static async Task<string> GetStoredProcedure(string name)
            {
                var assembly = typeof(AssetEventStore).GetTypeInfo().Assembly;
                var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{name}");
                var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }

            var storedProcedureBody = await GetStoredProcedure("AppendEvents.js");

            var storedProcedure = new StoredProcedureProperties { Id = StoredProcedureId, Body = storedProcedureBody };
            try
            {
                await container.Scripts.CreateStoredProcedureAsync(storedProcedure);
            }
            catch (CosmosException dex) when (dex.StatusCode == HttpStatusCode.Conflict)
            {
                await container.Scripts.ReplaceStoredProcedureAsync(storedProcedure);
            }
        }

        public Task AppendToStream(string streamId, IReadOnlyCollection<IStoredEvent> events, long currentPosition)
            => AppendToStream(streamId, events, currentPosition == 0 ? ExpectedPosition.Any : ExpectedPosition.NewExact(++currentPosition));

        private async Task AppendToStream(
            string streamId,
            IReadOnlyCollection<IStoredEvent> events,
            ExpectedPosition position)
        {
            if (events.Count == 0)
            {
                return;
            }

            var result = await container.Scripts.ExecuteStoredProcedureAsync<List<Document>>(
                StoredProcedureId,
                new PartitionKey(streamId),
                new dynamic[]
                {
                    streamId,
                    ObjectToJToken(events.Select(x =>
                    {
                        var (data, eventName) = storedEventSerializer.Serialize(x);

                        return new EventWrite(
                            Guid.NewGuid(),
                            Guid.NewGuid().ToString(),
                            eventName,
                            data,
                            null);
                    })),

                    position.ToJToken()
                }
            );
        }

        private JToken ObjectToJToken(object obj)
        {
            return JToken.Parse(JsonConvert.SerializeObject(obj, jsonSerializer));
        }

        public async Task<EventStream<IStoredEvent>> LoadEventStream(string streamId)
        {

            QueryDefinition queryDefinition =
                new QueryDefinition("SELECT * FROM e WHERE e.StreamId = @streamId AND e.Type = 'Event' ORDER BY e.Position ASC")
                    .WithParameter("@streamId", streamId);

            var documentQuery = container.GetItemQueryIterator<EventWrite>(queryDefinition, requestOptions: new QueryRequestOptions
            { EnableScanInQuery = true, PartitionKey = new PartitionKey(streamId) });

            var res = new List<IStoredEvent>();

            while (documentQuery.HasMoreResults)
            {
                var result = await documentQuery.ReadNextAsync();
                res.AddRange(
                    result.Select(x => storedEventSerializer.Deserialize(x.Data,
                        x.Name)));
            }

            return new EventStream<IStoredEvent>(streamId, res);
        }

        private class EventWrite
        {
            public Guid Id { get; }
            public string CorrelationId { get; }
            public string Name { get; }
            public JToken Data { get; }
            public JToken Metadata { get; }

            public EventWrite(Guid id, string correlationId, string name, JToken data, JToken metadata)
            {
                Id = id;
                CorrelationId = correlationId;
                Name = name;
                Data = data;
                Metadata = metadata;
            }
        }
    }
}
