using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public class MethodInfo : BasicConfig
    {
        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("arguments")]
        public ArgumentInfo[] Arguments { get; set; }
    }
}
