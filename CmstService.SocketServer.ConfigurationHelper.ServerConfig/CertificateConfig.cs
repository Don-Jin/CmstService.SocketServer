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
        [Category("安全证书"), DisplayName("安全证书路径")]
        [ConfigurationProperty("filePath", IsRequired = false)]
        public string FilePath
        {
            get { return this["filePath"] as string; }
            set { this["filePath"] = value; }
        }

        [Category("安全证书"), DisplayName("安全证书密码")]
        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return this["password"] as string; }
            set { this["password"] = value; }
        }

        [Category("安全证书"), DisplayName("安全证书存档名称")]
        [ConfigurationProperty("storeName", IsRequired = false)]
        public string StoreName
        {
            get { return this["storeName"] as string; }
            set { this["storeName"] = value; }
        }

        [Category("安全证书"), DisplayName("安全证书存储位置")]
        [ConfigurationProperty("storeLocation", IsRequired = false, DefaultValue = "CurrentUser")]
        public StoreLocation StoreLocation
        {
            get { return (StoreLocation)this["storeLocation"]; }
            set { this["storeLocation"] = value; }
        }

        [Category("安全证书"), DisplayName("安全证书指纹")]
        [ConfigurationProperty("thumbprint", IsRequired = false)]
        public string Thumbprint
        {
            get { return this["thumbprint"] as string; }
            set { this["thumbprint"] = value; }
        }

        [Category("安全证书"), DisplayName("客户端安全证书验证"), Description("一个布尔值，以确定是否通过安全证书验证客户端。")]
        [ConfigurationProperty("clientCertificateRequired", IsRequired = false, DefaultValue = false)]
        public bool ClientCertificateRequired
        {
            get { return (bool)this["clientCertificateRequired"]; }
            set { this["clientCertificateRequired"] = value; }
        }

        [Category("安全证书"), DisplayName("X.509证书")]
        [ConfigurationProperty("keyStorageFlags", IsRequired = false, DefaultValue = X509KeyStorageFlags.DefaultKeySet)]
        public X509KeyStorageFlags KeyStorageFlags
        {
            get { return (X509KeyStorageFlags)this["keyStorageFlags"]; }
            set { this["keyStorageFlags"] = value; }
        }
    }
}
