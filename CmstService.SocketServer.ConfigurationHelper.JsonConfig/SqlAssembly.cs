using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public class SqlAssembly
    {
        [JsonProperty("assembly")]
        public string Assembly { get; set; }

        [JsonProperty("initial")]
        public InitialInfo[] Initial { get; set; }

        [JsonProperty("methods")]
        public Dictionary<string, MethodInfo> Methods { get; set; }

        public bool HasInitial()
        {
            return Initial != null && Initial.Length > 0;
        }
    }
}
