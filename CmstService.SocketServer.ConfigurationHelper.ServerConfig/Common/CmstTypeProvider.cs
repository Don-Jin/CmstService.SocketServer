using System;
using System.Configuration;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Generic;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using CmstService.SocketServer.ConfigurationHelper;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common
{
    public class CmstTypeProvider : ConfigurationElement, ITypeProvider
    {
        [Category("属性"), DisplayName("名称")]
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
            set {this["name"] = value; }
        }

        [Editor(typeof(PropertyGridFileEditor), typeof(UITypeEditor))]
        [Category("属性"), DisplayName("类型")]
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return this["type"] as string; }
            set { this["type"] = value; }
        }
    }

    public class CmstTypeProvider<T> : CmstTypeProvider, ITypeProvider
    {
        // 通常是一个接口的类型，通过反射获取实现了该接口的类
        [Browsable(false)]
        public Type Assembly
        {
            get { return typeof(T); }
        }
    }

    [ConfigurationCollection(typeof(CmstTypeProvider))]
    public class CmstTypeProviderCollection : CommonGenericConfigCollection<CmstTypeProvider, ITypeProvider>, IEnumerable<ITypeProvider>, IEnumerable
    {
        
    }

    // 泛型集合
    public class CmstTypeProviderCollection<TConfigElement> : CommonGenericConfigCollection<TConfigElement, ITypeProvider>, IEnumerable<ITypeProvider>, IEnumerable
        where TConfigElement : ConfigurationElement, ITypeProvider, new()
    {
        
    }

    // 此种泛型集合似乎效率略低，且耦合度高，并需要重写很多类
    /*[ConfigurationCollection(typeof(CmstTypeProvider<IAppServer>))]
    public class IAppServerTypeProviderCollection : CommonGenericConfigCollection<CmstTypeProvider<IAppServer>, ITypeProvider>, IEnumerable<ITypeProvider>, IEnumerable
    {
        public new IEnumerator<ITypeProvider> GetEnumerator()
        {
            int count = base.Count;

            for (int i = 0; i < count; i++)
            {
                yield return (ITypeProvider)base.BaseGet(i);
            }
        }
    }*/
}
