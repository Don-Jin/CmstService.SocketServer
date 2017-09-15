using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public class ArgumentInfo
    {
        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("properties")]
        public string[] Properties { get; set; }
    }
}
