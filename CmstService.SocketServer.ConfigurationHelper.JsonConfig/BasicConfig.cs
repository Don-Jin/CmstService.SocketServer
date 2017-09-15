using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public class BasicConfig
    {
        // 对于SQL操作指操作类型，即：INSERT|UPDATE|DELETE|SELECT
        // 对于数据配置指数据库类型，即：SQLSERVER
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
