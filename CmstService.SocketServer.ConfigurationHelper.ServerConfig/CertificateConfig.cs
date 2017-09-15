using System;
using System.Configuration;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using SuperSocket.SocketBase.Config;
using CmstService.SocketServer.ConfigurationHelper;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig
{
    public class CertificateConfig : ConfigurationElement, ICertificateConfig
    {
        [Category("��ȫ֤��"), DisplayName("��ȫ֤��·��")]
        [ConfigurationProperty("filePath", IsRequired = false)]
        public string FilePath
        {
            get { return this["filePath"] as string; }
            set { this["filePath"] = value; }
        }

        [Category("��ȫ֤��"), DisplayName("��ȫ֤������")]
        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return this["password"] as string; }
            set { this["password"] = value; }
        }

        [Category("��ȫ֤��"), DisplayName("��ȫ֤��浵����")]
        [ConfigurationProperty("storeName", IsRequired = false)]
        public string StoreName
        {
            get { return this["storeName"] as string; }
            set { this["storeName"] = value; }
        }

        [Category("��ȫ֤��"), DisplayName("��ȫ֤��洢λ��")]
        [ConfigurationProperty("storeLocation", IsRequired = false, DefaultValue = "CurrentUser")]
        public StoreLocation StoreLocation
        {
            get { return (StoreLocation)this["storeLocation"]; }
            set { this["storeLocation"] = value; }
        }

        [Category("��ȫ֤��"), DisplayName("��ȫ֤��ָ��")]
        [ConfigurationProperty("thumbprint", IsRequired = false)]
        public string Thumbprint
        {
            get { return this["thumbprint"] as string; }
            set { this["thumbprint"] = value; }
        }

        [Category("��ȫ֤��"), DisplayName("�ͻ��˰�ȫ֤����֤"), Description("һ������ֵ����ȷ���Ƿ�ͨ����ȫ֤����֤�ͻ��ˡ�")]
        [ConfigurationProperty("clientCertificateRequired", IsRequired = false, DefaultValue = false)]
        public bool ClientCertificateRequired
        {
            get { return (bool)this["clientCertificateRequired"]; }
            set { this["clientCertificateRequired"] = value; }
        }

        [Category("��ȫ֤��"), DisplayName("X.509֤��")]
        [ConfigurationProperty("keyStorageFlags", IsRequired = false, DefaultValue = X509KeyStorageFlags.DefaultKeySet)]
        public X509KeyStorageFlags KeyStorageFlags
        {
            get { return (X509KeyStorageFlags)this["keyStorageFlags"]; }
            set { this["keyStorageFlags"] = value; }
        }
    }
}
