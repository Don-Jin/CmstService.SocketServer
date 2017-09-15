using System;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using SuperSocket.Common;
using SuperSocket.SocketBase.Config;
using CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig
{
    public class Listener : ConfigurationElement, IListenerConfig
    {
        [Category("通信监听"), DisplayName("监听IP")]
        [ConfigurationProperty("ip", IsRequired = true)]
        public string Ip
        {
            get { return this["ip"] as string; }
            set { this["ip"] = value; }
        }

        [Category("通信监听"), DisplayName("监听端口")]
        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [Category("通信监听"), DisplayName("通信队列大小")]
        [ConfigurationProperty("backlog", IsRequired = false, DefaultValue = 100)]
        public int Backlog
        {
            get { return (int)this["backlog"]; }
            set { this["backlog"] = value; }
        }

        [TypeConverter(typeof(ListenerSecurityMode))]
        [Category("通信监听"), DisplayName("安全模式"), Description("安全模式, 可设置为：None/Default/Tls/Ssl/...")]
        [ConfigurationProperty("security", IsRequired = false)]
        public string Security
        {
            get { return (string)this["security"]; }
            set { this["security"] = value; }
        }
    }

    [ConfigurationCollection(typeof(Listener))]
    public class ListenerConfigCollection : CommonGenericConfigCollection<Listener, IListenerConfig>
    {
        
    }
}
