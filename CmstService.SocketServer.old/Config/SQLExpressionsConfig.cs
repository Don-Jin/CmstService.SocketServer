using System;
using System.Configuration;

namespace CmstService.SocketServer.Config
{
    // SQL语句配置
    public class SQLExpressionsConfig : ConfigurationElement
    {
        // SQL语句的标识名，应唯一，否则后面覆盖前面的
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        // SQL语句
        [ConfigurationProperty("sql", IsRequired = true)]
        public string SQL
        {
            get { return this["sql"] as string; }
            set { this["sql"] = value; }
        }

        // SQL语句执行后返回数据表的标识名称
        [ConfigurationProperty("tableName", IsRequired = true)]
        public string TableName
        {
            get { return this["tableName"] as string; }
            set { this["tableName"] = value; }
        }

        // SQL查询区间
        [ConfigurationProperty("limit")]
        public bool IsLimit
        {
            get { return (bool)this["limit"]; }
            set { this["limit"] = value; }
        }

        // SQL查询区间分割字符
        [ConfigurationProperty("delimiter")]
        public string Delimiter
        {
            get { return (string)this["delimiter"]; }
            set { this["delimiter"] = value; }
        }
    }
}
