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

        // ��Ӽ����ڵ㣬������
        [ListToConfigElement(typeof(ListenerConfigCollection), "Listeners", "AddNew")]
        [Category("����������"), DisplayName("�����б�"), Description("�����ýڵ�����֧��һ��������ʵ���������IP/�˿���ϡ�")]
        public List<Listener> ListenerNodes
        {
            get { return this.m_Listeners; }
        }

        // �������ڵ�
        [ListToConfigElement(typeof(CommandAssemblyCollection), "CommandAssemblies", "AddNew")]
        [Category("����������"), DisplayName("����б�")]
        public List<CommandAssembly> CommandAssemblyNodes
        {
            get { return this.m_CommandAssemblies; }
        }
        
        [Category("����������"), DisplayName("��ȫ֤������")]
        public List<CertificateConfig> CertificateConfig
        {
            get { return this.m_CertificateConfig; }
        }

        [Category("����������"), DisplayName("��չѡ��")]
        public List<CmstNameValueCollection> ExtensionOptions
        {
            get { return this.m_ExtensionOptions; }
        }

        [Category("����������"), DisplayName("���������ͽڵ���"), Description("��ѡ�õķ����������� serverTypes �ڵ�����֣����ýڵ� serverTypes ���ڶ������п��õķ��������͡�")]
        [ConfigurationProperty("serverTypeName", IsRequired = false)]
        public string ServerTypeName
        {
            get { return this["serverTypeName"] as string; }
            set { this["serverTypeName"] = value; }
        }

        [InterfaceAssembly(typeof(IAppServer))]
        [Editor(typeof(PropertyGridFileEditor), typeof(UITypeEditor))]
        [Category("����������"), DisplayName("����������"), Description("������ʵ�������͵��������ơ�")]
        [ConfigurationProperty("serverType", IsRequired = false)]
        public string ServerType
        {
            get { return this["serverType"] as string; }
            set { this["serverType"] = value; }
        }

        [Category("����������"), DisplayName("���չ�����"), Description("�����ʵ����ʹ�õĽ��չ��������������֡�")]
        [ConfigurationProperty("receiveFilterFactory", IsRequired = false)]
        public string ReceiveFilterFactory
        {
            get { return this["receiveFilterFactory"] as string; }
            set { this["receiveFilterFactory"] = value; }
        }

        [Category("����������"), DisplayName("����IP"), Description("ͨ�ż���IP��ַ��")]
        [ConfigurationProperty("ip", IsRequired = false)]
        public string Ip
        {
            get { return this["ip"] as string; }
            set { this["ip"] = value; }
        }

        [Category("����������"), DisplayName("�����˿�"), Description("ͨ�ż����˿ڡ�")]
        [ConfigurationProperty("port", IsRequired = false)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [Category("����������"), DisplayName("����������ģʽ"), Description("Socket���������е�ģʽ, Tcp (Ĭ��) ���� Udp��")]
        [ConfigurationProperty("mode", IsRequired = false, DefaultValue = "Tcp")]
        public SocketMode Mode
        {
            get { return (SocketMode)this["mode"]; }
            set { this["mode"] = value; }
        }

        [Category("����������"), DisplayName("���÷�����ʵ��")]
        [ConfigurationProperty("disabled", DefaultValue = "false")]
        public bool Disabled
        {
            get { return (bool)this["disabled"]; }
            set { this["disabled"] = value; }
        }

        [Category("����������"), DisplayName("��Ϣ���ͳ�ʱʱ��")]
        [ConfigurationProperty("sendTimeOut", IsRequired = false, DefaultValue = 5000)]
        public int SendTimeOut
        {
            get { return (int)this["sendTimeOut"]; }
            set { this["sendTimeOut"] = value; }
        }

        [Category("����������"), DisplayName("���������������")]
        [ConfigurationProperty("maxConnectionNumber", IsRequired = false, DefaultValue = 100)]
        public int MaxConnectionNumber
        {
            get { return (int)this["maxConnectionNumber"]; }
            set { this["maxConnectionNumber"] = value; }
        }

        [Category("����������"), DisplayName("���ջ�������С")]
        [ConfigurationProperty("receiveBufferSize", IsRequired = false, DefaultValue = 4096)]
        public int ReceiveBufferSize
        {
            get { return (int)this["receiveBufferSize"]; }
            set { this["receiveBufferSize"] = value; }
        }

        [Category("����������"), DisplayName("���ͻ�������С")]
        [ConfigurationProperty("sendBufferSize", IsRequired = false, DefaultValue = 2048)]
        public int SendBufferSize
        {
            get { return (int)this["sendBufferSize"]; }
            set { this["sendBufferSize"] = value; }
        }

        [Category("����������"), DisplayName("����ͬ������")]
        [ConfigurationProperty("syncSend", IsRequired = false, DefaultValue = false)]
        public bool SyncSend
        {
            get { return (bool)this["syncSend"]; }
            set { this["syncSend"] = value; }
        }

        [Category("����������"), DisplayName("��¼����ִ��"), Description("�Ƿ��¼����ִ�еļ�¼��")]
        [ConfigurationProperty("logCommand", IsRequired = false, DefaultValue = false)]
        public bool LogCommand
        {
            get { return (bool)this["logCommand"]; }
            set { this["logCommand"] = value; }
        }

        [Category("����������"), DisplayName("��¼�Ự�"), Description("�Ƿ��¼�Ự�Ļ�����������ӺͶϿ���")]
        [ConfigurationProperty("logBasicSessionActivity", IsRequired = false, DefaultValue = true)]
        public bool LogBasicSessionActivity
        {
            get { return (bool)this["logBasicSessionActivity"]; }
            set { this["logBasicSessionActivity"] = value; }
        }

        [Category("����������"), DisplayName("��¼�����쳣"), Description("�Ƿ��¼����Socket�쳣�ʹ���")]
        [ConfigurationProperty("logAllSocketException", IsRequired = false, DefaultValue = false)]
        public bool LogAllSocketException
        {
            get { return (bool)this["logAllSocketException"]; }
            set { this["logAllSocketException"] = value; }
        }

        [Category("����������"), DisplayName("��ʱ��տ��лỰ"), Description("�Ƿ�ʱ��տ��лỰ��Ĭ��ֵ�� false��")]
        [ConfigurationProperty("clearIdleSession", IsRequired = false, DefaultValue = false)]
        public bool ClearIdleSession
        {
            get { return (bool)this["clearIdleSession"]; }
            set { this["clearIdleSession"] = value; }
        }

        [Category("����������"), DisplayName("��տ��лỰʱ����"), Description("��տ��лỰ��ʱ����, Ĭ��ֵ��120, ��λΪ�롣")]
        [ConfigurationProperty("clearIdleSessionInterval", IsRequired = false, DefaultValue = 120)]
        public int ClearIdleSessionInterval
        {
            get { return (int)this["clearIdleSessionInterval"]; }
            set { this["clearIdleSessionInterval"] = value; }
        }

        [Category("����������"), DisplayName("�Ự���г�ʱʱ��"), Description("�Ự���г�ʱʱ��; ���˻Ự����ʱ�䳬����ֵ��ͬʱclearIdleSession�����ó�trueʱ���˻Ự���ᱻ�ر�; Ĭ��ֵΪ300����λΪ�롣")]
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
        [Category("����������"), DisplayName("��������Э��"), Description(" Socket�����������õĴ�������Э�飬Ĭ��ֵΪ�ա�")]
        [ConfigurationProperty("security", IsRequired = false, DefaultValue = "None")]
        public string Security
        {
            get { return (string)this["security"]; }
            set { this["security"] = value; }
        }

        [Category("����������"), DisplayName("������󳤶�"), Description("�����������󳤶ȣ�Ĭ��ֵΪ1024��")]
        [ConfigurationProperty("maxRequestLength", IsRequired = false, DefaultValue = 1024)]
        public int MaxRequestLength
        {
            get { return (int)this["maxRequestLength"]; }
            set { this["maxRequestLength"] = value; }
        }

        [Category("����������"), DisplayName("���ûỰ����"), Description("�Ƿ���ûỰ����, Ĭ��ֵΪ false��")]
        [ConfigurationProperty("disableSessionSnapshot", IsRequired = false, DefaultValue = false)]
        public bool DisableSessionSnapshot
        {
            get { return (bool)this["disableSessionSnapshot"]; }
            set { this["disableSessionSnapshot"] = value; }
        }

        [Category("����������"), DisplayName("�Ự����ʱ����"), Description("�Ự����ʱ����, Ĭ��ֵ�� 5, ��λΪ�롣")]
        [ConfigurationProperty("sessionSnapshotInterval", IsRequired = false, DefaultValue = 5)]
        public int SessionSnapshotInterval
        {
            get { return (int)this["sessionSnapshotInterval"]; }
            set { this["sessionSnapshotInterval"] = value; }
        }

        [Category("����������"), DisplayName("���ӹ�������"), Description("�����ʵ����ʹ�õ����ӹ����������֣������������ ',' ���� ';' �ָ����")]
        [ConfigurationProperty("connectionFilter", IsRequired = false)]
        public string ConnectionFilter
        {
            get { return (string)this["connectionFilter"]; }
            set { this["connectionFilter"] = value; }
        }

        [Category("����������"), DisplayName("�����������"), Description("�����ʵ����ʹ�õ���������������֣������������ ',' ���� ';' �ָ����")]
        [ConfigurationProperty("commandLoader", IsRequired = false)]
        public string CommandLoader
        {
            get { return (string)this["commandLoader"]; }
            set { this["commandLoader"] = value; }
        }

        [Category("����������"), DisplayName("���ݷ��ͼ��"), Description("����������������µ�keep alive���ݵķ��ͼ��, Ĭ��ֵΪ 600, ��λΪ�롣")]
        [ConfigurationProperty("keepAliveTime", IsRequired = false, DefaultValue = 600)]
        public int KeepAliveTime
        {
            get { return (int)this["keepAliveTime"]; }
            set { this["keepAliveTime"] = value; }
        }

        [Category("����������"), DisplayName("̽������ͼ��"), Description("Keep aliveʧ��֮��, keep alive̽����ķ��ͼ����Ĭ��ֵΪ 60, ��λΪ�롣")]
        [ConfigurationProperty("keepAliveInterval", IsRequired = false, DefaultValue = 60)]
        public int KeepAliveInterval
        {
            get { return (int)this["keepAliveInterval"]; }
            set { this["keepAliveInterval"] = value; }
        }

        [Category("����������"), DisplayName("�������д�С")]
        [ConfigurationProperty("listenBacklog", IsRequired = false, DefaultValue = 100)]
        public int ListenBacklog
        {
            get { return (int)this["listenBacklog"]; }
            set { this["listenBacklog"] = value; }
        }

        [Category("����������"), DisplayName("������ʵ������˳��"), Description("������ʵ������˳��, �����մ�ֵ��˳�����������������ʵ����")]
        [ConfigurationProperty("startupOrder", IsRequired = false, DefaultValue = 0)]
        public int StartupOrder
        {
            get { return (int)this["startupOrder"]; }
            set { this["startupOrder"] = value; }
        }

        [Category("����������"), DisplayName("���Ͷ�����󳤶�"), Description("���Ͷ�����󳤶�, Ĭ��ֵΪ5��")]
        [ConfigurationProperty("sendingQueueSize", IsRequired = false, DefaultValue = 5)]
        public int SendingQueueSize
        {
            get { return (int)this["sendingQueueSize"]; }
            set { this["sendingQueueSize"] = value; }
        }

        [Category("����������"), DisplayName("��־������"), Description("�����ʵ����ʹ�õ���־����(LogFactory)�����֡�")]
        [ConfigurationProperty("logFactory", IsRequired = false, DefaultValue = "")]
        public string LogFactory
        {
            get { return (string)this["logFactory"]; }
            set { this["logFactory"] = value; }
        }

        [TypeConverter(typeof(TextEncodingMode))]  
        [Category("����������"), DisplayName("�ı�����"), Description("�ı���Ĭ�ϱ��룬Ĭ��ֵ�� ASCII��")]
        [ConfigurationProperty("textEncoding", IsRequired = false, DefaultValue = "")]
        public string TextEncoding
        {
            get { return (string)this["textEncoding"]; }
            set { this["textEncoding"] = value; }
        }

        [Category("����������"), DisplayName("������ʵ������")]
        [ConfigurationProperty("name", IsRequired = true, DefaultValue = "")]
        public new string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        [Category("����������"), DisplayName("ҳ���ض����ַ")]
        [ConfigurationProperty("redirectHref")]
        public string RedirectHref
        {
            get { return this["redirectHref"] as string; }
            set { this["redirectHref"] = value; }
        }

        [Category("����������"), DisplayName("���ӹ�������")]
        [ConfigurationProperty("ipRange")]
        public string IpRange
        {
            get { return this["ipRange"] as string; }
            set { this["ipRange"] = value; }
        }

        //[Category("����������"), DisplayName("��������������"), Description("�����ýڵ�����֧��һ��������ʵ���������IP/�˿���ϡ�")]
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

        //[Category("����������"), DisplayName("���������"), Description("���������չ��")]
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
            // ��̬Ϊ NameValueCollection ������ʵ�� Options��OptionElements ��� BrowsableAttribute ���ԣ���������
            TypeDescriptor.AddAttributes(typeof(NameValueCollection), new BrowsableAttribute(false));

            // ��ʼ��ȫ֤��ڵ�
            if (this.Certificate != null) this.CertificateConfig.Add(this.Certificate as CertificateConfig);

            Utility.BeforeOpen(this);
        }

        public void BeforeSave()
        {
            // ����չ������ӵ��ڵ�������
            //Utility.SetXmlAttribute(this.ExtensionOptions, this.Options);

            // ��չ���ԣ�������β��Ծ�ʧ�ܣ������ڲ�ʵ��Ҳ������ XMLWriter�����Կ��ǵ�ʵ��ʹ���п���չ���Բ���
            // ��ˣ�ֱ���� Property ����ʽ������չ������������ƣ����о���
            // �Ͼ�ֻ��Ϊ�˿��ӻ����ã�����ʹ�� Configuration �Ĺ��ܣ���Ȼ��ȫ������ XML �ķ�ʽ��д��ֻ�軨ʱ��д�������༴��
            // ��������ɾ�Ĳ�ڵ㶼����
            
            // ���氲ȫ֤��ڵ�
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
