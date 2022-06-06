using System;
using System.Collections.Generic;
using EnsureThat;

namespace PortfolioTracker.EventStore
{
    public class EventStream<TEvent>
    {
        public EventStream(string id, IReadOnlyList<TEvent> events)
        {
            EnsureArg.IsNotNullOrEmpty(id, nameof(id));
            EnsureArg.IsNotNull(events, nameof(events));

            Id = id;
            Events = events;
            Position = events.Count == 0 ? ExpectedPosition.NoStream : ExpectedPosition.NewExact(events.Count + 1);
        }

        public string Id { get; }

        public IReadOnlyList<TEvent> Events { get; }

        public ExpectedPosition Position { get; }

        public static EventStream<TEvent> Empty(string streamId) => new EventStream<TEvent>(
            streamId,
            Array.Empty<TEvent>());
    }
}
