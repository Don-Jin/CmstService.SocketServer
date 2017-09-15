using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class LoginMessage : SessionMessage
    {
        // 模式，login - 登录，logout - 登出，signup - 注册, signin - 登录验证
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("job")]
        public string Job { get; set; }
    }
}
