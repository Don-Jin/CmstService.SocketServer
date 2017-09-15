using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using SuperSocket.Common;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common
{
    public class CommonGenericConfigCollection<TConfigElement, TConfigInterface> : GenericConfigurationElementCollectionBase<TConfigElement, TConfigInterface>, IEnumerable<TConfigInterface>
        where TConfigElement : ConfigurationElement, TConfigInterface, new()
    {


        // 添加单个元素
        public void AddNew(TConfigElement element)
        {
            if (element.ElementInformation.Type.GetInterface("ICommonMethod") != null)
            {
                (element as ICommonMethod).BeforeSave();
            }
            base.BaseAdd(element);
        }

        // 重载，批量添加
        public void AddNew(params TConfigElement[] elements)
        {
            foreach (TConfigElement el in elements)
            {
                this.AddNew(el);
            }
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void Clear()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.BaseRemoveAt(i);
            }
        }

        // 索引器
        public new TConfigElement this[string name]
        {
            get { return this.BaseGet(name) as TConfigElement; }
        }
    }
}
