using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class RedirectMessage
    {
        // 跳转URI
        [JsonProperty("href")]
        public string[] Href { get; set; }

        // 本地存储信息
        [JsonProperty("cookie")]
        public SessionMessage Cookie { get; set; }
    }

    // 为了方便 JSON 序列化给客户端使用
    public class Redirect
    {
        public Redirect(RedirectMessage message)
        {
            this.RedirectMessage = message;
        }

        [JsonProperty("redirect")]
        public RedirectMessage RedirectMessage { get; private set; }
    }
}
