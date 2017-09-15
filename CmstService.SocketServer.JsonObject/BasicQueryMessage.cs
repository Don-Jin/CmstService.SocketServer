using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class BasicQueryMessage
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
