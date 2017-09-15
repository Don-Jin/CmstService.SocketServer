using System;
using System.Xml;
using System.Configuration;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig
{
    public class Server : ConfigurationElementBase, IServerConfig, ICommonMethod
    {
        private List<Listener> m_Listeners = new List<Listener>();
        private List<CommandAssembly> m_CommandAssemblies = new List<CommandAssembly>();
        private List<CertificateConfig> m_CertificateConfig = new List<CertificateConfig>();
        private List<CmstNameValueCollection> m_ExtensionOptions = new List<CmstNameValueCollection>();

        // 添加监听节点，非属性
        [ListToConfigElement(typeof(ListenerConfigCollection), "Listeners", "AddNew")]
        [Category("服务器配置"), DisplayName("监听列表"), Description("此配置节点用于支持一个服务器实例监听多个IP/端口组合。")]
        public List<Listener> ListenerNodes
        {
            get { return this.m_Listeners; }
        }

        // 添加命令集节点
        [ListToConfigElement(typeof(CommandAssemblyCollection), "CommandAssemblies", "AddNew")]
        [Category("服务器配置"), DisplayName("命令集列表")]
        public List<CommandAssembly> CommandAssemblyNodes
        {
            get { return this.m_CommandAssemblies; }
        }
        
        [Category("服务器配置"), DisplayName("安全证书配置")]
        public List<CertificateConfig> CertificateConfig
        {
            get { return this.m_CertificateConfig; }
        }

        [Category("服务器配置"), DisplayName("扩展选项")]
        public List<CmstNameValueCollection> ExtensionOptions
        {
            get { return this.m_ExtensionOptions; }
        }

        [Category("服务器配置"), DisplayName("服务器类型节点名"), Description("所选用的服务器类型在 serverTypes 节点的名字，配置节点 serverTypes 用于定义所有可用的服务器类型。")]
        [ConfigurationProperty("serverTypeName", IsRequired = false)]
        public string ServerTypeName
        {
            get { return this["serverTypeName"] as string; }
            set { this["serverTypeName"] = value; }
        }

        [InterfaceAssembly(typeof(IAppServer))]
        [Editor(typeof(PropertyGridFileEditor), typeof(UITypeEditor))]
        [Category("服务器配置"), DisplayName("服务器类型"), Description("服务器实例的类型的完整名称。")]
        [ConfigurationProperty("serverType", IsRequired = false)]
        public string ServerType
        {
            get { return this["serverType"] as string; }
            set { this["serverType"] = value; }
        }

        [Category("服务器配置"), DisplayName("接收过滤器"), Description("定义该实例所使用的接收过滤器工厂的名字。")]
        [ConfigurationProperty("receiveFilterFactory", IsRequired = false)]
        public string ReceiveFilterFactory
        {
            get { return this["receiveFilterFactory"] as string; }
            set { this["receiveFilterFactory"] = value; }
        }

        [Category("服务器配置"), DisplayName("监听IP"), Description("通信监听IP地址。")]
        [ConfigurationProperty("ip", IsRequired = false)]
        public string Ip
        {
            get { return this["ip"] as string; }
            set { this["ip"] = value; }
        }

        [Category("服务器配置"), DisplayName("监听端口"), Description("通信监听端口。")]
        [ConfigurationProperty("port", IsRequired = false)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [Category("服务器配置"), DisplayName("服务器运行模式"), Description("Socket服务器运行的模式, Tcp (默认) 或者 Udp。")]
        [ConfigurationProperty("mode", IsRequired = false, DefaultValue = "Tcp")]
        public SocketMode Mode
        {
            get { return (SocketMode)this["mode"]; }
            set { this["mode"] = value; }
        }

        [Category("服务器配置"), DisplayName("禁用服务器实例")]
        [ConfigurationProperty("disabled", DefaultValue = "false")]
        public bool Disabled
        {
            get { return (bool)this["disabled"]; }
            set { this["disabled"] = value; }
        }

        [Category("服务器配置"), DisplayName("消息发送超时时间")]
        [ConfigurationProperty("sendTimeOut", IsRequired = false, DefaultValue = 5000)]
        public int SendTimeOut
        {
            get { return (int)this["sendTimeOut"]; }
            set { this["sendTimeOut"] = value; }
        }

        [Category("服务器配置"), DisplayName("服务器最大连接数")]
        [ConfigurationProperty("maxConnectionNumber", IsRequired = false, DefaultValue = 100)]
        public int MaxConnectionNumber
        {
            get { return (int)this["maxConnectionNumber"]; }
            set { this["maxConnectionNumber"] = value; }
        }

        [Category("服务器配置"), DisplayName("接收缓冲区大小")]
        [ConfigurationProperty("receiveBufferSize", IsRequired = false, DefaultValue = 4096)]
        public int ReceiveBufferSize
        {
            get { return (int)this["receiveBufferSize"]; }
            set { this["receiveBufferSize"] = value; }
        }

        [Category("服务器配置"), DisplayName("发送缓冲区大小")]
        [ConfigurationProperty("sendBufferSize", IsRequired = false, DefaultValue = 2048)]
        public int SendBufferSize
        {
            get { return (int)this["sendBufferSize"]; }
            set { this["sendBufferSize"] = value; }
        }

        [Category("服务器配置"), DisplayName("启用同步发送")]
        [ConfigurationProperty("syncSend", IsRequired = false, DefaultValue = false)]
        public bool SyncSend
        {
            get { return (bool)this["syncSend"]; }
            set { this["syncSend"] = value; }
        }

        [Category("服务器配置"), DisplayName("记录命令执行"), Description("是否记录命令执行的记录。")]
        [ConfigurationProperty("logCommand", IsRequired = false, DefaultValue = false)]
        public bool LogCommand
        {
            get { return (bool)this["logCommand"]; }
            set { this["logCommand"] = value; }
        }

        [Category("服务器配置"), DisplayName("记录会话活动"), Description("是否记录会话的基本活动，如连接和断开。")]
        [ConfigurationProperty("logBasicSessionActivity", IsRequired = false, DefaultValue = true)]
        public bool LogBasicSessionActivity
        {
            get { return (bool)this["logBasicSessionActivity"]; }
            set { this["logBasicSessionActivity"] = value; }
        }

        [Category("服务器配置"), DisplayName("记录所有异常"), Description("是否记录所有Socket异常和错误。")]
        [ConfigurationProperty("logAllSocketException", IsRequired = false, DefaultValue = false)]
        public bool LogAllSocketException
        {
            get { return (bool)this["logAllSocketException"]; }
            set { this["logAllSocketException"] = value; }
        }

        [Category("服务器配置"), DisplayName("定时清空空闲会话"), Description("是否定时清空空闲会话，默认值是 false。")]
        [ConfigurationProperty("clearIdleSession", IsRequired = false, DefaultValue = false)]
        public bool ClearIdleSession
        {
            get { return (bool)this["clearIdleSession"]; }
            set { this["clearIdleSession"] = value; }
        }

        [Category("服务器配置"), DisplayName("清空空闲会话时间间隔"), Description("清空空闲会话的时间间隔, 默认值是120, 单位为秒。")]
        [ConfigurationProperty("clearIdleSessionInterval", IsRequired = false, DefaultValue = 120)]
        public int ClearIdleSessionInterval
        {
            get { return (int)this["clearIdleSessionInterval"]; }
            set { this["clearIdleSessionInterval"] = value; }
        }

        [Category("服务器配置"), DisplayName("会话空闲超时时间"), Description("会话空闲超时时间; 当此会话空闲时间超过此值，同时clearIdleSession被配置成true时，此会话将会被关闭; 默认值为300，单位为秒。")]
        [ConfigurationProperty("idleSessionTimeOut", IsRequired = false, DefaultValue = 300)]
        public int IdleSessionTimeOut
        {
            get { return (int)this["idleSessionTimeOut"]; }
            set { this["idleSessionTimeOut"] = value; }
        }

        public TConfig GetChildConfig<TConfig>(string childConfigName)
            where TConfig : ConfigurationElement, new()
        {
            return this.OptionElements.GetChildConfig<TConfig>(childConfigName);
        }

        [Browsable(false)]
        [ConfigurationProperty("certificate")]
        public ICertificateConfig Certificate
        {
            get { return (ICertificateConfig)this["certificate"]; }
            set { this["certificate"] = value; }
        }

        [TypeConverter(typeof(TransportMode))]
        [Category("服务器配置"), DisplayName("传输层加密协议"), Description(" Socket服务器所采用的传输层加密协议，默认值为空。")]
        [ConfigurationProperty("security", IsRequired = false, DefaultValue = "None")]
        public string Security
        {
            get { return (string)this["security"]; }
            set { this["security"] = value; }
        }

        [Category("服务器配置"), DisplayName("最大请求长度"), Description("最大允许的请求长度，默认值为1024。")]
        [ConfigurationProperty("maxRequestLength", IsRequired = false, DefaultValue = 1024)]
        public int MaxRequestLength
        {
            get { return (int)this["maxRequestLength"]; }
            set { this["maxRequestLength"] = value; }
        }

        [Category("服务器配置"), DisplayName("禁用会话快照"), Description("是否禁用会话快照, 默认值为 false。")]
        [ConfigurationProperty("disableSessionSnapshot", IsRequired = false, DefaultValue = false)]
        public bool DisableSessionSnapshot
        {
            get { return (bool)this["disableSessionSnapshot"]; }
            set { this["disableSessionSnapshot"] = value; }
        }

        [Category("服务器配置"), DisplayName("会话快照时间间隔"), Description("会话快照时间间隔, 默认值是 5, 单位为秒。")]
        [ConfigurationProperty("sessionSnapshotInterval", IsRequired = false, DefaultValue = 5)]
        public int SessionSnapshotInterval
        {
            get { return (int)this["sessionSnapshotInterval"]; }
            set { this["sessionSnapshotInterval"] = value; }
        }

        [Category("服务器配置"), DisplayName("连接过滤器名"), Description("定义该实例所使用的连接过滤器的名字，多个过滤器用 ',' 或者 ';' 分割开来。")]
        [ConfigurationProperty("connectionFilter", IsRequired = false)]
        public string ConnectionFilter
        {
            get { return (string)this["connectionFilter"]; }
            set { this["connectionFilter"] = value; }
        }

        [Category("服务器配置"), DisplayName("命令加载器名"), Description("定义该实例所使用的命令加载器的名字，多个过滤器用 ',' 或者 ';' 分割开来。")]
        [ConfigurationProperty("commandLoader", IsRequired = false)]
        public string CommandLoader
        {
            get { return (string)this["commandLoader"]; }
            set { this["commandLoader"] = value; }
        }

        [Category("服务器配置"), DisplayName("数据发送间隔"), Description("网络连接正常情况下的keep alive数据的发送间隔, 默认值为 600, 单位为秒。")]
        [ConfigurationProperty("keepAliveTime", IsRequired = false, DefaultValue = 600)]
        public int KeepAliveTime
        {
            get { return (int)this["keepAliveTime"]; }
            set { this["keepAliveTime"] = value; }
        }

        [Category("服务器配置"), DisplayName("探测包发送间隔"), Description("Keep alive失败之后, keep alive探测包的发送间隔，默认值为 60, 单位为秒。")]
        [ConfigurationProperty("keepAliveInterval", IsRequired = false, DefaultValue = 60)]
        public int KeepAliveInterval
        {
            get { return (int)this["keepAliveInterval"]; }
            set { this["keepAliveInterval"] = value; }
        }

        [Category("服务器配置"), DisplayName("监听队列大小")]
        [ConfigurationProperty("listenBacklog", IsRequired = false, DefaultValue = 100)]
        public int ListenBacklog
        {
            get { return (int)this["listenBacklog"]; }
            set { this["listenBacklog"] = value; }
        }

        [Category("服务器配置"), DisplayName("服务器实例启动顺序"), Description("服务器实例启动顺序, 将按照此值的顺序来启动多个服务器实例。")]
        [ConfigurationProperty("startupOrder", IsRequired = false, DefaultValue = 0)]
        public int StartupOrder
        {
            get { return (int)this["startupOrder"]; }
            set { this["startupOrder"] = value; }
        }

        [Category("服务器配置"), DisplayName("发送队列最大长度"), Description("发送队列最大长度, 默认值为5。")]
        [ConfigurationProperty("sendingQueueSize", IsRequired = false, DefaultValue = 5)]
        public int SendingQueueSize
        {
            get { return (int)this["sendingQueueSize"]; }
            set { this["sendingQueueSize"] = value; }
        }

        [Category("服务器配置"), DisplayName("日志工厂名"), Description("定义该实例所使用的日志工厂(LogFactory)的名字。")]
        [ConfigurationProperty("logFactory", IsRequired = false, DefaultValue = "")]
        public string LogFactory
        {
            get { return (string)this["logFactory"]; }
            set { this["logFactory"] = value; }
        }

        [TypeConverter(typeof(TextEncodingMode))]  
        [Category("服务器配置"), DisplayName("文本编码"), Description("文本的默认编码，默认值是 ASCII。")]
        [ConfigurationProperty("textEncoding", IsRequired = false, DefaultValue = "")]
        public string TextEncoding
        {
            get { return (string)this["textEncoding"]; }
            set { this["textEncoding"] = value; }
        }

        [Category("服务器配置"), DisplayName("服务器实例名称")]
        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "")]
        public new string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        [Category("服务器配置"), DisplayName("页面重定向地址")]
        [ConfigurationProperty("redirectHref")]
        public string RedirectHref
        {
            get { return this["redirectHref"] as string; }
            set { this["redirectHref"] = value; }
        }

        [Category("服务器配置"), DisplayName("连接过滤区间")]
        [ConfigurationProperty("ipRange")]
        public string IpRange
        {
            get { return this["ipRange"] as string; }
            set { this["ipRange"] = value; }
        }

        //[Category("服务器配置"), DisplayName("服务器监听集合"), Description("此配置节点用于支持一个服务器实例监听多个IP/端口组合。")]
        [Browsable(false)]
        [ConfigurationProperty("listeners", IsRequired = false)]
        public ListenerConfigCollection Listeners
        {
            get { return this["listeners"] as ListenerConfigCollection; }
            set { this["listeners"] = value; }
        }

        IEnumerable<IListenerConfig> IServerConfig.Listeners
        {
            get { return this.Listeners; }
        }

        //[Category("服务器配置"), DisplayName("服务器命令集"), Description("命令集，可扩展。")]
        [Browsable(false)]
        [ConfigurationProperty("commandAssemblies", IsRequired = false)]
        public CommandAssemblyCollection CommandAssemblies
        {
            get { return this["commandAssemblies"] as CommandAssemblyCollection; }
            set { this["commandAssemblies"] = value; }
        }

        IEnumerable<ICommandAssemblyConfig> IServerConfig.CommandAssemblies
        {
            get { return this.CommandAssemblies; }
        }

        public void BeforeOpen()
        {
            // 动态为 NameValueCollection 的两个实例 Options、OptionElements 添加 BrowsableAttribute 特性，将其隐藏
            TypeDescriptor.AddAttributes(typeof(NameValueCollection), new BrowsableAttribute(false));

            // 初始安全证书节点
            if (this.Certificate != null) this.CertificateConfig.Add(this.Certificate as CertificateConfig);

            Utility.BeforeOpen(this);
        }

        public void BeforeSave()
        {
            // 将扩展配置添加到节点属性中
            //Utility.SetXmlAttribute(this.ExtensionOptions, this.Options);

            // 扩展属性，经过多次测试均失败，由于内部实现也是利用 XMLWriter，所以考虑到实际使用中可扩展属性不多
            // 因此，直接以 Property 的形式予以扩展，今后如需完善，再研究吧
            // 毕竟只是为了可视化配置，又想使用 Configuration 的功能，不然完全可以以 XML 的方式读写，只需花时间写个辅助类即可
            // 那样，增删改查节点都方便
            
            // 保存安全证书节点
            //if (this.CertificateConfig.Count > 0) this.Certificate = this.CertificateConfig[0];
            if (this.CertificateConfig.Count > 0)
            {
                ConfigXmlDocument xml = new ConfigXmlDocument();
                xml.Load(this.CurrentConfiguration.FilePath);
                XmlNode node = xml.SelectSingleNode(string.Format("//server[@name='{0}']", this.Name));
                XmlElement child = xml.CreateElement("certificate");
                child.SetAttribute("filePath", "localhost.pfx");
                child.SetAttribute("password", "supersocket");

                node.AppendChild(child);

                xml.Save(xml.Filename);
            }

            Utility.BeforeSave(this);
        }
    }

    [ConfigurationCollection(typeof(Server), AddItemName = "server")]
    public class ServerCollection : CommonGenericConfigCollection<Server, IServerConfig>
    {
        
    }
}
