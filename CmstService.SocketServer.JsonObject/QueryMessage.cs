using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class QueryMessage : BasicQueryMessage
    {
        [JsonProperty("queryList")]
        public List<QueryInfo> QueryList { get; set; }
    }

    public class QueryInfo
    {
        [JsonProperty("database")]
        public string DatabaseName { get; set; }

        [JsonProperty("query")]
        public string QueryName { get; set; }

        [JsonProperty("arguments")]
        public object[] Arguments { get; set; }
    }
}
