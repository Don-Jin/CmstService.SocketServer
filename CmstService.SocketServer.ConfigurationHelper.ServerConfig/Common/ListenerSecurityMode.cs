using System;
using CmstService.SocketServer.ConfigurationHelper;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common
{
    // 端口监听安全模式
    public class ListenerSecurityMode : PropertyGridStringConverter
    {
        public ListenerSecurityMode()
            : base(new string[4] { "None", "Default", "TLS", "SSL" })
        { 
        
        }
    }
}
