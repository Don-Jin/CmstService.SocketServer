using System;
using System.Configuration;

namespace CmstService.SocketServer.Config
{
    // 数据库信息配置
    public class DatabaseConfig : ConfigurationElement
    {
        // 数据库类型属性
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
        
        // 数据库名称属性
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        // 数据库连接字符串属性
        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }

        // SQL语句
        [ConfigurationProperty("sqlExpressions")]
        public SQLExpressionsConfigCollection SQLExpressions
        {
            get { return this["sqlExpressions"] as SQLExpressionsConfigCollection; }
        }
    }
}
