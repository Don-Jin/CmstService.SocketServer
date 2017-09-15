using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class ErrorMessage : BasicInfoMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Error : IMessage
    {
        public Error(string type, string message)
        {
            this.ErrorMessage = new ErrorMessage() { 
                Type = type,
                Message = message
            };
        }

        [JsonProperty("error")]
        public ErrorMessage ErrorMessage { get; private set; }
    }
}
