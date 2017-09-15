using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class ChatMessage : BasicQueryMessage
    {
        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("sendtime")]
        public DateTime SendTime { get; set; }

        [JsonProperty("receiver")]
        public string Receiver { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class Message
    {
        public Message(ChatMessage message)
        {
            this.ChatMessage = message;
        }

        [JsonProperty("message")]
        public ChatMessage ChatMessage { get; private set; }
    }
}
