using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class FunctionMessage : BasicQueryMessage
    {
        [JsonProperty("database")]
        public string DatabaseName { get; set; }

        [JsonProperty("assembly")]
        public string AssemblyName { get; set; }

        [JsonProperty("method")]
        public string MethodName { get; set; }

        [JsonProperty("arguments")]
        public List<object[]> Arguments { get; set; }
    }
}
