using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class SuccessMessage : BasicInfoMessage
    {
        [JsonProperty("session")]
        public SessionMessage Session { get; set; }
    }

    public class Success : IMessage
    {
        public Success(string message)
        {
            this.SuccessMessage = new SuccessMessage()
            {
                Message = message
            };
        }

        public Success(SessionMessage session, string message)
            : this(message)
        {
            this.SuccessMessage.Session = session;
        }

        [JsonProperty("success")]
        public SuccessMessage SuccessMessage { get; private set; }
    }
}
