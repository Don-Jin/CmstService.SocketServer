using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;

namespace CmstService.SocketServer.Config
{
    // 数据库信息配置元素组
    [ConfigurationCollection(typeof(DatabaseConfig))]
    public class DatabaseConfigCollection : ConfigurationElementCollection
    {
        // 索引器
        public DatabaseConfig this[int index]
        {
            get
            {
                return (DatabaseConfig)base.BaseGet(index);
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new DatabaseConfig this[string name]
        {
            get { return (DatabaseConfig)base.BaseGet(name); }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        // 创建新元素
        protected override ConfigurationElement CreateNewElement()
        {
            return new DatabaseConfig();
        }

        // 获取元素的键
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DatabaseConfig)element).Name;
        }

        // 重写元素标签名
        protected override string ElementName
        {
            get { return "database"; }
        }

        // 枚举器
        public new IEnumerator<DatabaseConfig> GetEnumerator()
        {
            int count = base.Count;

            for (int i = 0; i < count; i++)
            {
                yield return (DatabaseConfig)base.BaseGet(i);
            }
        }

        private object GetSelfProperty(string name)
        {
            return this.ElementInformation.Properties[name].Value;
        }

        private void SetSelfProperty(string name, object value)
        {
            this.ElementInformation.Properties[name].Value = value;
        }

        // 是否提供登录验证，默认为 false
        [ConfigurationProperty("needLogin")]
        public bool NeedLogin
        {
            get { return (bool)this.GetSelfProperty("needLogin"); }
            set { this.SetSelfProperty("needLogin", value); }
        }

        // 如果需要提供登录验证，则应当提供用户表所在数据库名，与子配置节 <database name="..."> 的 name 项匹配
        [ConfigurationProperty("loginDatabase")]
        public string LoginDatabase
        {
            get { return ((string)this.GetSelfProperty("loginDatabase")); }
            set { this.SetSelfProperty("loginDatabase", value); }
        }

        // 登录验证所用的SQL语句
        [ConfigurationProperty("loginQuery")]
        public string LoginQuery
        {
            get { return ((string)this.GetSelfProperty("loginQuery")); }
            set { this.SetSelfProperty("loginQuery", value); }
        }

        // 用户名所在字段
        [ConfigurationProperty("userField")]
        public string UserField
        {
            get { return ((string)this.GetSelfProperty("userField")); }
            set { this.SetSelfProperty("userField", value); }
        }

        // 密码所在字段
        [ConfigurationProperty("keyField")]
        public string KeyField
        {
            get { return ((string)this.GetSelfProperty("keyField")); }
            set { this.SetSelfProperty("keyField", value); }
        }

        // 用户组所在字段
        [ConfigurationProperty("groupField")]
        public string GroupField
        {
            get { return ((string)this.GetSelfProperty("groupField")); }
            set { this.SetSelfProperty("groupField", value); }
        }

        // 订阅列表字段集，指定为逗号分隔
        [ConfigurationProperty("subscriptionList")]
        public string SubscriptionList
        {
            get { return ((string)this.GetSelfProperty("subscriptionList")); }
            set { this.SetSelfProperty("subscriptionList", value); }
        }

        // 用户注册所在数据库
        [ConfigurationProperty("signupDatabase")]
        public string SignupDatabase
        {
            get { return ((string)this.GetSelfProperty("signupDatabase")); }
            set { this.SetSelfProperty("signupDatabase", value); }
        }

        // 用户注册所用SQL语句
        [ConfigurationProperty("signupQuery")]
        public string SignupQuery
        {
            get { return ((string)this.GetSelfProperty("signupQuery")); }
            set { this.SetSelfProperty("signupQuery", value); }
        }

        // 封装键值对
        // Key - 数据库名，Value - DBInfo
        public Dictionary<string, DBInfo> GetPackDetails()
        { 
            Dictionary<string, DBInfo> details = new Dictionary<string,DBInfo>();
            for (int i = 0; i < this.Count; i++)
            {
                DatabaseConfig conf = this[i];
                details.Add(conf.Name.ToLower(), new DBInfo(conf.Type.ToLower(), conf.ConnectionString, conf.SQLExpressions));
            }
            return details;
        }

        // 封装数据库信息
        public class DBInfo
        {
            public DBInfo(string name, string conn, SQLExpressionsConfigCollection sqls)
            {
                this.Name = name;
                this.ConnString = conn;
                this.SQLDetails = new Dictionary<string, SQLInfo>();
                this.SetSQLDetails(sqls);
            }

            private void SetSQLDetails(SQLExpressionsConfigCollection sqls)
            {
                foreach (SQLExpressionsConfig conf in sqls)
                {
                    this.SQLDetails.Add(conf.Name.ToLower(), new SQLInfo(conf));
                }
            }

            // 数据库名
            public string Name { get; private set; }

            // 数据库连接字符串
            public string ConnString { get; private set; }

            // 数据库连接实例
            public IDbConnection DBConnection { get; set; }

            // SQL语句关联字典
            public Dictionary<string, SQLInfo> SQLDetails { get; private set; }
        }

        // 封装SQL信息
        public class SQLInfo
        {
            public SQLInfo(SQLExpressionsConfig conf)
            {
                this.IsLimit = conf.IsLimit;
                this.Delimiter = conf.Delimiter;
                this.TableName = conf.TableName;
                this.SQL = conf.SQL;
            }

            // 是否区间查询
            public bool IsLimit { get; private set; }

            // 分割字符
            public string Delimiter { get; private set; }
            
            // 返回数据表的名称
            public string TableName { get; private set; }

            // SQL语句
            public string SQL { get; private set; }
        }
    }
}
