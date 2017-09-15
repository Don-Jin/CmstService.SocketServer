using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class BasicInfoMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
