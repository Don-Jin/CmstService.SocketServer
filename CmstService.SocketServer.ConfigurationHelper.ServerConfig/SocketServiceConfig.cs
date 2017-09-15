using System;
using System.Reflection;
using System.Configuration;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig
{
    public class SocketServiceConfig : ConfigurationSection, IConfigurationSource, ICommonMethod
    {
        private List<Server> m_Servers = new List<Server>();
        private List<CmstTypeProvider<IAppServer>> m_ServerTypes = new List<CmstTypeProvider<IAppServer>>();
        private List<CmstTypeProvider<IConnectionFilter>> m_ConnectionFilters = new List<CmstTypeProvider<IConnectionFilter>>();
        private List<CmstTypeProvider<ILogFactory>> m_LogFactories = new List<CmstTypeProvider<ILogFactory>>();
        private List<CmstTypeProvider<IReceiveFilterFactory>> m_ReceiveFilterFactories = new List<CmstTypeProvider<IReceiveFilterFactory>>();
        private List<CmstTypeProvider<ICommandLoader>> m_CommandLoaders = new List<CmstTypeProvider<ICommandLoader>>();

        [ListToConfigElement(typeof(ServerCollection), "Servers", "AddNew")]
        [Category("通信服务"), DisplayName("服务器实例"), Description("配置服务器实例集合。")]
        public List<Server> ServerNodes
        {
            get { return this.m_Servers; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<IAppServer>>), "ServerTypes", "AddNew")]
        [Category("通信服务"), DisplayName("服务器实例类型"), Description("服务器实例的实现类，托管对象类型。")]
        public List<CmstTypeProvider<IAppServer>> ServerTypeNodes
        {
            get { return this.m_ServerTypes; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<IConnectionFilter>>), "ConnectionFilters", "AddNew")]
        [Category("通信服务"), DisplayName("连接过滤器")]
        public List<CmstTypeProvider<IConnectionFilter>> ConnectionFilterNodes
        {
            get { return this.m_ConnectionFilters; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<ILogFactory>>), "LogFactories", "AddNew")]
        [Category("通信服务"), DisplayName("日志工厂")]
        public List<CmstTypeProvider<ILogFactory>> LogFactoryNodes
        {
            get { return this.m_LogFactories; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<IReceiveFilterFactory>>), "ReceiveFilterFactories", "AddNew")]
        [Category("通信服务"), DisplayName("消息接收过滤器")]
        public List<CmstTypeProvider<IReceiveFilterFactory>> ReceiveFilterFactoryNodes
        {
            get { return this.m_ReceiveFilterFactories; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<ICommandLoader>>), "CommandLoaders", "AddNew")]
        [Category("通信服务"), DisplayName("命令加载器")]
        public List<CmstTypeProvider<ICommandLoader>> CommandLoaderNodes
        {
            get { return this.m_CommandLoaders; }
        }

        //[Category("通信服务"), DisplayName("服务器集合"), Description("服务器集合。")]
        [Browsable(false)]
        [ConfigurationProperty("servers")]
        public ServerCollection Servers
        {
            get { return this["servers"] as ServerCollection; }
            set { this["servers"] = value; }
        }

        //[Category("通信服务"), DisplayName("服务器配置集合"), Description("服务器配置集合。")]
        [Browsable(false)]
        [ConfigurationProperty("serverTypes")]
        public CmstTypeProviderCollection<CmstTypeProvider<IAppServer>> ServerTypes
        {
            get { return this["serverTypes"] as CmstTypeProviderCollection<CmstTypeProvider<IAppServer>>; }
            set { this["serverTypes"] = value; }
        }

        //[Category("通信服务"), DisplayName("连接过滤集合"), Description("连接过滤集合。")]
        [Browsable(false)]
        [ConfigurationProperty("connectionFilters", IsRequired = false)]
        public CmstTypeProviderCollection<CmstTypeProvider<IConnectionFilter>> ConnectionFilters
        {
            get { return this["connectionFilters"] as CmstTypeProviderCollection<CmstTypeProvider<IConnectionFilter>>; }
            set { this["connectionFilters"] = value; }
        }

        //[Category("通信服务"), DisplayName("日志工厂集合"), Description("日志工厂集合。")]
        [Browsable(false)]
        [ConfigurationProperty("logFactories", IsRequired = false)]
        public CmstTypeProviderCollection<CmstTypeProvider<ILogFactory>> LogFactories
        {
            get { return this["logFactories"] as CmstTypeProviderCollection<CmstTypeProvider<ILogFactory>>; }
            set { this["logFactories"] = value; }
        }

        //[Category("通信服务"), DisplayName("接收过滤工厂集合"), Description("消息接收过滤工厂集合。")]
        [Browsable(false)]
        [ConfigurationProperty("receiveFilterFactories", IsRequired = false)]
        public CmstTypeProviderCollection<CmstTypeProvider<IReceiveFilterFactory>> ReceiveFilterFactories
        {
            get { return this["receiveFilterFactories"] as CmstTypeProviderCollection<CmstTypeProvider<IReceiveFilterFactory>>; }
            set { this["receiveFilterFactories"] = value; }
        }

        //[Category("通信服务"), DisplayName("命令加载器集合"), Description("命令加载器集合。")]
        [Browsable(false)]
        [ConfigurationProperty("commandLoaders", IsRequired = false)]
        public CmstTypeProviderCollection<CmstTypeProvider<ICommandLoader>> CommandLoaders
        {
            get { return this["commandLoaders"] as CmstTypeProviderCollection<CmstTypeProvider<ICommandLoader>>; }
            set { this["commandLoaders"] = value; }
        }

        [Category("通信服务"), DisplayName("线程池最大工作线程数量")]
        [ConfigurationProperty("maxWorkingThreads", IsRequired = false, DefaultValue = -1)]
        public int MaxWorkingThreads
        {
            get { return (int)this["maxWorkingThreads"]; }
            set { this["maxWorkingThreads"] = value; }
        }

        [Category("通信服务"), DisplayName("线程池最小工作线程数量")]
        [ConfigurationProperty("minWorkingThreads", IsRequired = false, DefaultValue = -1)]
        public int MinWorkingThreads
        {
            get { return (int)this["minWorkingThreads"]; }
            set { this["minWorkingThreads"] = value; }
        }

        [Category("通信服务"), DisplayName("线程池最大空闲端口线程数量")]
        [ConfigurationProperty("maxCompletionPortThreads", IsRequired = false, DefaultValue = -1)]
        public int MaxCompletionPortThreads
        {
            get { return (int)this["maxCompletionPortThreads"]; }
            set { this["maxCompletionPortThreads"] = value; }
        }

        [Category("通信服务"), DisplayName("线程池最小空闲端口线程数量")]
        [ConfigurationProperty("minCompletionPortThreads", IsRequired = false, DefaultValue = -1)]
        public int MinCompletionPortThreads
        {
            get { return (int)this["minCompletionPortThreads"]; }
            set { this["minCompletionPortThreads"] = value; }
        }

        [Category("通信服务"), DisplayName("性能数据采集频率"), Description("性能数据采集频率 (单位为秒, 默认值: 60)。")]
        [ConfigurationProperty("performanceDataCollectInterval", IsRequired = false, DefaultValue = 60)]
        public int PerformanceDataCollectInterval
        {
            get { return (int)this["performanceDataCollectInterval"]; }
            set { this["performanceDataCollectInterval"] = value; }
        }

        [Category("通信服务"), DisplayName("禁用性能数据采集")]
        [ConfigurationProperty("disablePerformanceDataCollector", IsRequired = false, DefaultValue = false)]
        public bool DisablePerformanceDataCollector
        {
            get { return (bool)this["disablePerformanceDataCollector"]; }
            set { this["disablePerformanceDataCollector"] = value; }
        }

        [Category("通信服务"), DisplayName("服务器实例隔离级别")]
        [ConfigurationProperty("isolation", IsRequired = false, DefaultValue = IsolationMode.None)]
        public IsolationMode Isolation
        {
            get { return (IsolationMode)this["isolation"]; }
            set { this["isolation"] = value; }
        }

        [Category("通信服务"), DisplayName("默认日志工厂"), Description("默认logFactory的名字, 所有可用的 log factories定义在子节点 \"logFactories\" 之中。")]
        [ConfigurationProperty("logFactory", IsRequired = false, DefaultValue = "")]
        public string LogFactory
        {
            get { return (string)this["logFactory"]; }
            set { this["logFactory"] = value; }
        }

        [Browsable(false)]
        public NameValueCollection OptionElements { get; set; }

        public TConfig GetChildConfig<TConfig>(string childConfigName)
            where TConfig : ConfigurationElement, new()
        {
            return this.OptionElements.GetChildConfig<TConfig>(childConfigName);
        }

        IEnumerable<IServerConfig> IConfigurationSource.Servers
        { 
            get { return this.Servers; } 
        }

        IEnumerable<ITypeProvider> IConfigurationSource.ServerTypes
        {
            get { return this.ServerTypes; } 
        }

        IEnumerable<ITypeProvider> IConfigurationSource.ConnectionFilters
        {
            get { return this.ConnectionFilters; } 
        }

        IEnumerable<ITypeProvider> IConfigurationSource.LogFactories
        {
            get { return this.LogFactories; } 
        }

        IEnumerable<ITypeProvider> IConfigurationSource.ReceiveFilterFactories
        {
            get { return this.ReceiveFilterFactories; } 
        }

        IEnumerable<ITypeProvider> IConfigurationSource.CommandLoaders
        {
            get { return this.CommandLoaders; } 
        }

        public void BeforeOpen()
        {
            Utility.BeforeOpen(this);
            
            // 清空有关集合，那么所有配置在保存时都会被重新序列化
            this.Servers.Clear();
            this.ServerTypes.Clear();
            this.LogFactories.Clear();
            this.CommandLoaders.Clear();
            this.ConnectionFilters.Clear();
            this.ReceiveFilterFactories.Clear();
        }

        public void BeforeSave()
        {
            Utility.BeforeSave(this);
        }
    }
}
