using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PortfolioTracker.EventStore.Core
{
    public interface IStoredEventSerializer
    {
        (JToken serializedEvent, string eventType) Serialize(IStoredEvent storedEvent);

        IStoredEvent Deserialize(JToken serializedEvent, string eventType);
    }

    public class JsonStoredEventSerializer : IStoredEventSerializer
    {
        private readonly IReadOnlyCollection<(string EventName, Type Type)> eventTypes;
        private readonly JsonSerializer jsonSerializer;

        public JsonStoredEventSerializer(IEnumerable<Assembly> assemblies, IEnumerable<JsonConverter> jsonConverters)
        {
            eventTypes = assemblies.SelectMany(x => x.GetTypes())
                .Where(x => typeof(IStoredEvent).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => (EventName: GetEventName(x), Type: x))
                .ToArray();

            jsonSerializer = JsonSerializer.CreateDefault();

            foreach (var jsonConverter in jsonConverters)
            {
                jsonSerializer.Converters.Add(jsonConverter);
            }
        }

        public JsonStoredEventSerializer(Assembly assembly, JsonConverter jsonConverter)
            : this(new[] { assembly }, new[] { jsonConverter }) { }

        public JsonStoredEventSerializer(Assembly assembly)
            : this(new[] { assembly }, Array.Empty<JsonConverter>()) { }

        public (JToken serializedEvent, string eventType) Serialize(IStoredEvent storedEvent)
        {
            var storedEventType = storedEvent.GetType();
            return (JToken.FromObject(storedEvent, jsonSerializer), eventTypes.Single(t => t.Type == storedEventType).EventName);
        }

        public IStoredEvent Deserialize(JToken serializedEvent, string eventType)
        {
            var eventTypes = this.eventTypes.Single(t => string.Equals(t.EventName, eventType, StringComparison.OrdinalIgnoreCase));
            return (IStoredEvent)serializedEvent.ToObject(eventTypes.Type, jsonSerializer);
        }

        private static string GetEventName(MemberInfo eventType)
        {
            var eventTypeAttribute = (EventNameAttribute)Attribute.GetCustomAttribute(eventType, typeof(EventNameAttribute));
            return eventTypeAttribute.Value;
        }
    }
}
