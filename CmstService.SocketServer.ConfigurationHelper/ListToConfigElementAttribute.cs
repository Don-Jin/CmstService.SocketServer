using System;
using System.Reflection;
using System.ComponentModel;

namespace CmstService.SocketServer.ConfigurationHelper
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public class ListToConfigElementAttribute : Attribute
    {
        public ListToConfigElementAttribute() {
            
        }

        public ListToConfigElementAttribute(Type type, string name, string invokeName)
        {
            this.Type = type;
            this.Name = name;
            this.InvokeName = invokeName;
        }

        // 将要挂载节点的类型
        public Type Type { get; set; }

        // 配置节点集合所在属性名
        public string Name { get; set; }

        // 集合新增所调用的方法名
        public string InvokeName { get; set; }

        // 调用指定方法
        public bool InvokeMember(object invokeTarget, object[] args)
        {
            try
            {
                this.Type.InvokeMember(this.InvokeName, BindingFlags.InvokeMethod, null, invokeTarget, args);
            }
            catch { return false; }

            return true;
        }
    }
}
