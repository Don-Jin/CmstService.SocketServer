using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class SessionMessage : BasicQueryMessage
    {
        // 用户
        [JsonProperty("user")]
        public string User { get; set; }

        // 用户名
        [JsonProperty("name")]
        public string Name { get; set; }

        // 订阅消息列表
        [JsonProperty("subscriptionList")]
        public List<string> SubscriptionList { get; set; }
    }
}
