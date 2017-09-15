using System;
using System.ComponentModel;
using System.Collections.Specialized;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common
{
    public class CmstNameValueCollection : NameValueCollection
    {
        [Category("扩展属性"), DisplayName("属性")]
        public string Name { get; set; }

        [Category("扩展属性"), DisplayName("属性值")]
        public string Value { get; set; }
    }
}
