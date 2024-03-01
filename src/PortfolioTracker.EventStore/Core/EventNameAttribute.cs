using System;

namespace PortfolioTracker.EventStore.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventNameAttribute : Attribute
    {
        public EventNameAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
