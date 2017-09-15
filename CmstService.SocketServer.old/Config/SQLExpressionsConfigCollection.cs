using System;
using System.Configuration;
using System.Collections.Generic;

namespace CmstService.SocketServer.Config
{
    // SQL语句配置元素组
    [ConfigurationCollection(typeof(SQLExpressionsConfig))]
    public class SQLExpressionsConfigCollection : ConfigurationElementCollection
    {
        // 索引器
        public SQLExpressionsConfig this[int index]
        {
            get
            {
                return (SQLExpressionsConfig)base.BaseGet(index);
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

        public new SQLExpressionsConfig this[string name]
        {
            get
            {
                return (SQLExpressionsConfig)base.BaseGet(name);
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        // 元素名
        protected override string ElementName
        {
            get { return "add"; }
        }

        // 创建新元素
        protected override ConfigurationElement CreateNewElement()
        {
            return new SQLExpressionsConfig();
        }

        // 获取元素的键
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SQLExpressionsConfig)element).Name;
        }

        // 枚举器
        public new IEnumerator<SQLExpressionsConfig> GetEnumerator()
        {
            int count = base.Count;

            for (int i = 0; i < count; i++)
            {
                yield return (SQLExpressionsConfig)base.BaseGet(i);
            }
        }
    }
}
