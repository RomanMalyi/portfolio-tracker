using Newtonsoft.Json.Linq;

namespace PortfolioTracker.EventStore
{
    public abstract class ExpectedPosition
    {
        public static ExpectedPosition NoStream { get; } = new ExpectedPosition._NoStream();

        public static ExpectedPosition Any { get; } = new ExpectedPosition._Any();

        public static ExpectedPosition NewExact(long position) => new ExpectedPosition._Exact(position);

        public static long GetPosition(ExpectedPosition expectedPosition)
        {
            if (expectedPosition is _Exact exact)
            {
                return exact.Position - 1;
            }

            return 0;
        }

        public JToken ToJToken()
        {
            var json = new JObject();
            if (this is _NoStream)
            {
                json["mode"] = new JValue("noStream");
            }
            else if (this is _Any)
            {
                json["mode"] = new JValue("any");
            }
            else if (this is _Exact exact)
            {
                json["mode"] = new JValue("exact");
                json["position"] = new JValue(exact.Position);
            }

            return json;
        }

        private class _Any : ExpectedPosition
        {
        }

        private class _NoStream : ExpectedPosition
        {
        }

        private class _Exact : ExpectedPosition
        {
            internal readonly long Position;

            public _Exact(long position)
            {
                Position = position;
            }
        }
    }
}