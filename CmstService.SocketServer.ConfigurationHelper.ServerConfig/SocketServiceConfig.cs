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
        [Category("ͨ�ŷ���"), DisplayName("������ʵ��"), Description("���÷�����ʵ�����ϡ�")]
        public List<Server> ServerNodes
        {
            get { return this.m_Servers; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<IAppServer>>), "ServerTypes", "AddNew")]
        [Category("ͨ�ŷ���"), DisplayName("������ʵ������"), Description("������ʵ����ʵ���࣬�йܶ������͡�")]
        public List<CmstTypeProvider<IAppServer>> ServerTypeNodes
        {
            get { return this.m_ServerTypes; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<IConnectionFilter>>), "ConnectionFilters", "AddNew")]
        [Category("ͨ�ŷ���"), DisplayName("���ӹ�����")]
        public List<CmstTypeProvider<IConnectionFilter>> ConnectionFilterNodes
        {
            get { return this.m_ConnectionFilters; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<ILogFactory>>), "LogFactories", "AddNew")]
        [Category("ͨ�ŷ���"), DisplayName("��־����")]
        public List<CmstTypeProvider<ILogFactory>> LogFactoryNodes
        {
            get { return this.m_LogFactories; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<IReceiveFilterFactory>>), "ReceiveFilterFactories", "AddNew")]
        [Category("ͨ�ŷ���"), DisplayName("��Ϣ���չ�����")]
        public List<CmstTypeProvider<IReceiveFilterFactory>> ReceiveFilterFactoryNodes
        {
            get { return this.m_ReceiveFilterFactories; }
        }

        [ListToConfigElement(typeof(CmstTypeProviderCollection<CmstTypeProvider<ICommandLoader>>), "CommandLoaders", "AddNew")]
        [Category("ͨ�ŷ���"), DisplayName("���������")]
        public List<CmstTypeProvider<ICommandLoader>> CommandLoaderNodes
        {
            get { return this.m_CommandLoaders; }
        }

        //[Category("ͨ�ŷ���"), DisplayName("����������"), Description("���������ϡ�")]
        [Browsable(false)]
        [ConfigurationProperty("servers")]
        public ServerCollection Servers
        {
            get { return this["servers"] as ServerCollection; }
            set { this["servers"] = value; }
        }

        //[Category("ͨ�ŷ���"), DisplayName("���������ü���"), Description("���������ü��ϡ�")]
        [Browsable(false)]
        [ConfigurationProperty("serverTypes")]
        public CmstTypeProviderCollection<CmstTypeProvider<IAppServer>> ServerTypes
        {
            get { return this["serverTypes"] as CmstTypeProviderCollection<CmstTypeProvider<IAppServer>>; }
            set { this["serverTypes"] = value; }
        }

        //[Category("ͨ�ŷ���"), DisplayName("���ӹ��˼���"), Description("���ӹ��˼��ϡ�")]
        [Browsable(false)]
        [ConfigurationProperty("connectionFilters", IsRequired = false)]
        public CmstTypeProviderCollection<CmstTypeProvider<IConnectionFilter>> ConnectionFilters
        {
            get { return this["connectionFilters"] as CmstTypeProviderCollection<CmstTypeProvider<IConnectionFilter>>; }
            set { this["connectionFilters"] = value; }
        }

        //[Category("ͨ�ŷ���"), DisplayName("��־��������"), Description("��־�������ϡ�")]
        [Browsable(false)]
        [ConfigurationProperty("logFactories", IsRequired = false)]
        public CmstTypeProviderCollection<CmstTypeProvider<ILogFactory>> LogFactories
        {
            get { return this["logFactories"] as CmstTypeProviderCollection<CmstTypeProvider<ILogFactory>>; }
            set { this["logFactories"] = value; }
        }

        //[Category("ͨ�ŷ���"), DisplayName("���չ��˹�������"), Description("��Ϣ���չ��˹������ϡ�")]
        [Browsable(false)]
        [ConfigurationProperty("receiveFilterFactories", IsRequired = false)]
        public CmstTypeProviderCollection<CmstTypeProvider<IReceiveFilterFactory>> ReceiveFilterFactories
        {
            get { return this["receiveFilterFactories"] as CmstTypeProviderCollection<CmstTypeProvider<IReceiveFilterFactory>>; }
            set { this["receiveFilterFactories"] = value; }
        }

        //[Category("ͨ�ŷ���"), DisplayName("�������������"), Description("������������ϡ�")]
        [Browsable(false)]
        [ConfigurationProperty("commandLoaders", IsRequired = false)]
        public CmstTypeProviderCollection<CmstTypeProvider<ICommandLoader>> CommandLoaders
        {
            get { return this["commandLoaders"] as CmstTypeProviderCollection<CmstTypeProvider<ICommandLoader>>; }
            set { this["commandLoaders"] = value; }
        }

        [Category("ͨ�ŷ���"), DisplayName("�̳߳�������߳�����")]
        [ConfigurationProperty("maxWorkingThreads", IsRequired = false, DefaultValue = -1)]
        public int MaxWorkingThreads
        {
            get { return (int)this["maxWorkingThreads"]; }
            set { this["maxWorkingThreads"] = value; }
        }

        [Category("ͨ�ŷ���"), DisplayName("�̳߳���С�����߳�����")]
        [ConfigurationProperty("minWorkingThreads", IsRequired = false, DefaultValue = -1)]
        public int MinWorkingThreads
        {
            get { return (int)this["minWorkingThreads"]; }
            set { this["minWorkingThreads"] = value; }
        }

        [Category("ͨ�ŷ���"), DisplayName("�̳߳������ж˿��߳�����")]
        [ConfigurationProperty("maxCompletionPortThreads", IsRequired = false, DefaultValue = -1)]
        public int MaxCompletionPortThreads
        {
            get { return (int)this["maxCompletionPortThreads"]; }
            set { this["maxCompletionPortThreads"] = value; }
        }

        [Category("ͨ�ŷ���"), DisplayName("�̳߳���С���ж˿��߳�����")]
        [ConfigurationProperty("minCompletionPortThreads", IsRequired = false, DefaultValue = -1)]
        public int MinCompletionPortThreads
        {
            get { return (int)this["minCompletionPortThreads"]; }
            set { this["minCompletionPortThreads"] = value; }
        }

        [Category("ͨ�ŷ���"), DisplayName("�������ݲɼ�Ƶ��"), Description("�������ݲɼ�Ƶ�� (��λΪ��, Ĭ��ֵ: 60)��")]
        [ConfigurationProperty("performanceDataCollectInterval", IsRequired = false, DefaultValue = 60)]
        public int PerformanceDataCollectInterval
        {
            get { return (int)this["performanceDataCollectInterval"]; }
            set { this["performanceDataCollectInterval"] = value; }
        }

        [Category("ͨ�ŷ���"), DisplayName("�����������ݲɼ�")]
        [ConfigurationProperty("disablePerformanceDataCollector", IsRequired = false, DefaultValue = false)]
        public bool DisablePerformanceDataCollector
        {
            get { return (bool)this["disablePerformanceDataCollector"]; }
            set { this["disablePerformanceDataCollector"] = value; }
        }

        [Category("ͨ�ŷ���"), DisplayName("������ʵ�����뼶��")]
        [ConfigurationProperty("isolation", IsRequired = false, DefaultValue = IsolationMode.None)]
        public IsolationMode Isolation
        {
            get { return (IsolationMode)this["isolation"]; }
            set { this["isolation"] = value; }
        }

        [Category("ͨ�ŷ���"), DisplayName("Ĭ����־����"), Description("Ĭ��logFactory������, ���п��õ� log factories�������ӽڵ� \"logFactories\" ֮�С�")]
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
            
            // ����йؼ��ϣ���ô���������ڱ���ʱ���ᱻ�������л�
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
