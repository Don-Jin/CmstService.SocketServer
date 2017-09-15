using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public class SystemConfig
    {
        [JsonProperty("loginDatabase")]
        public string LoginDatabase { get; set; }

        [JsonProperty("loginQuery")]
        public string LoginQuery { get; set; }

        [JsonProperty("userField")]
        public string UserField { get; set; }

        [JsonProperty("nameField")]
        public string NameField { get; set; }

        [JsonProperty("keyField")]
        public string KeyField { get; set; }

        [JsonProperty("groupField")]
        public string GroupField { get; set; }

        [JsonProperty("subscriptionList")]
        public List<string> SubscriptionList { get; set; }

        [JsonProperty("signupDatabase")]
        public string SignupDatabase { get; set; }

        [JsonProperty("signupQuery")]
        public string SignupQuery { get; set; }
    }
}
