using System.Collections.Generic;
using System.Threading.Tasks;
using PortfolioTracker.EventStore.Core;

namespace PortfolioTracker.EventStore
{
    public interface IEventStore
    {
        Task AppendToStream(string streamId, IReadOnlyCollection<IStoredEvent> events, long currentPosition);

        Task<EventStream<IStoredEvent>> LoadEventStream(string streamId);
    }
}
