using System;
using CmstService.SocketServer.ConfigurationHelper;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common
{
    // 传输模式，字符串转换类型转换
    public class TransportMode : PropertyGridStringConverter
    {
        public TransportMode() 
            : base(new string[3] { "None", "TLS", "SSL" })
        { 
        
        }
    }
}
