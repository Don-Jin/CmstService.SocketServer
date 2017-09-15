using System;
using System.Xml;
using System.Configuration;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using SuperSocket.SocketBase.Config;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common
{
    public sealed class Utility
    {
        private Utility() { }

        // 保存节点前，将 List 中的数据保存到相应的配置集合中
        public static void BeforeSave(object sender)
        {
            try
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(sender))
                {
                    // 用 as 转换，如果转换失败则会返回 null
                    ListToConfigElementAttribute attr = prop.Attributes[typeof(ListToConfigElementAttribute)] as ListToConfigElementAttribute;
                    
                    if (attr != null)
                    {
                        IList val = prop.GetValue(sender) as IList;
                        object[] args = new object[val.Count];
                        val.CopyTo(args, 0);
                        attr.InvokeMember(sender.GetType().GetProperty(attr.Name).GetValue(sender, null), args);
                    }
                }
            }
            catch { throw; }
        }

        // 初始 Section 实例，在获取实例后，将集合中的节点添加到相应 List 中，方便操作
        public static void BeforeOpen(object sender)
        {
            try
            {
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(sender))
                {
                    // 用 as 转换，如果转换失败则会返回 null
                    ListToConfigElementAttribute attr = prop.Attributes[typeof(ListToConfigElementAttribute)] as ListToConfigElementAttribute;
                    
                    // 1.处理有 ListToConfigElementAttribute 的属性
                    if (attr != null)
                    {
                        IList list = prop.GetValue(sender) as IList;
                        ICollection val = sender.GetType().GetProperty(attr.Name).GetValue(sender, null) as ICollection;
                        
                        foreach (var item in val)
                        {
                            if (item.GetType().GetInterface("ICommonMethod") != null)
                            {
                                (item as ICommonMethod).BeforeOpen();
                            }
                            list.Add(item);
                        }
                    }
                    // 2.处理无 1 中特性，且为泛型集合的属性，如：List<CertificateConfig> CertificateConfig
                    else if (prop.PropertyType.IsGenericType && (sender is Server))
                    {
                        Type[] args = prop.PropertyType.GetGenericArguments();
                        NameValueCollection options = (sender as Server).Options;

                        if (args[0].BaseType.Equals(typeof(NameValueCollection)) && options.Count > 0)
                        {
                            IList list = prop.GetValue(sender) as IList;

                            foreach (string key in options)
                            {
                                list.Add(new CmstNameValueCollection() { 
                                    Name = key,
                                    Value = options[key]
                                });
                            }
                        }
                    }
                }
            }
            catch { throw; }
        }

        // 将键值对添加到节点的属性列表中
        // 本想通过扩展到父类 Options 中，通过 SerializeElement 方法由系统添加，但发现有诸多问题
        // 1.不新增节点，旧节点无法添加属性
        // 2.新增节点，添加属性会报 XML 节点错误
        // 于是只好中规中矩通过 XML 节点操作来实现了，这样一来别说扩展属性，扩展节点也能实现了
        public static void SetXmlAttribute(List<CmstNameValueCollection> options, string configName, string nodeName)
        {
            if (options.Count > 0)
            {
                try
                {
                    // 以 XML 方式加载配置文件
                    ConfigXmlDocument xmldoc = new ConfigXmlDocument();
                    xmldoc.Load(configName);

                    // 获取当前对象所在节点
                    XmlNode xmlnode = xmldoc.SelectSingleNode(string.Format("//server[@name='{0}']", nodeName));
                    
                    // 批量新增属性
                    foreach (CmstNameValueCollection item in options)
                    {
                        // 创建新属性节点
                        XmlAttribute xmlattr = xmldoc.CreateAttribute(item.Name);

                        // 设置属性值
                        xmlattr.Value = item.Value;

                        // 将属性节点添加到父节点的属性节点集合中
                        xmlnode.Attributes.Append(xmlattr);
                    }

                    // 添加完属性后，重新以旧的文件名保存
                    xmldoc.Save(configName);
                }
                catch { throw; }
            }
        }

        public static void SetXmlAttribute(List<CmstNameValueCollection> options, NameValueCollection origin)
        {
            if (options.Count > 0)
            {
                try
                {
                    // 批量新增属性
                    foreach (CmstNameValueCollection item in options)
                    {
                        // 将属性节点添加到父节点的属性节点集合中
                        //origin.Add(item.Name, item.Value);
                        origin[item.Name] = item.Value;
                    }
                }
                catch { throw; }
            }
        }
    }
}
