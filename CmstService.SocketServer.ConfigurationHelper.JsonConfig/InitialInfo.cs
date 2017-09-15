using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public class InitialInfo : BasicConfig
    {
        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
