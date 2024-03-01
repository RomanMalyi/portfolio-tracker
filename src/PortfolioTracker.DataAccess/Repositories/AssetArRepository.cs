using System;
using System.Collections.Generic;
using PortfolioTracker.Domain;
using PortfolioTracker.EventStore;
using PortfolioTracker.EventStore.Core;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class AssetArRepository
    {
        private readonly IEventStore eventStore;

        public AssetArRepository(IEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        public async Task<Maybe<AssetAR>> Get(string assetId)
        {
            EventStream<IStoredEvent> stream = await eventStore.LoadEventStream(assetId);

            return !stream.Events.Any() ? Maybe<AssetAR>.None : new AssetAR(stream.Events, ExpectedPosition.GetPosition(stream.Position));
        }

        public async Task<Maybe<AssetAR>> GetByDate(string assetId, DateTimeOffset dateTime)
        {
            EventStream<IStoredEvent> stream = await eventStore.LoadEventStream(assetId);
            List<IStoredEvent> streamPart = stream.Events.Where(e => e.CreatedAt <= dateTime).ToList();

            return !stream.Events.Any() ? Maybe<AssetAR>.None : new AssetAR(streamPart, streamPart.Count);
        }

        public async Task Upsert(AssetAR assetAr)
        {
            await eventStore.AppendToStream(assetAr.AssetId, assetAr.Changes, assetAr.Version);
        }
    }
}
