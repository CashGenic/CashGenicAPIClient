using System.Collections.Generic;
using Newtonsoft.Json;

namespace CashGenicClient
{
    public class SystemStatus
    {
        [JsonProperty("events")]
        public List<Event> Events { get; set; }
    }

    public class Event
    {
        [JsonProperty("event")]
        public string EventEvent { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }


        [JsonProperty("value")]
        public long Value { get; set; }
    }


}