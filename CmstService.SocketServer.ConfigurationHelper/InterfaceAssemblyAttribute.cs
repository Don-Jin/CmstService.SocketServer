using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace CmstService.SocketServer.ConfigurationHelper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
    public class InterfaceAssemblyAttribute : Attribute
    {
        public InterfaceAssemblyAttribute() { }

        public InterfaceAssemblyAttribute(Type type)
        {
            this.Assembly = type;
        }

        public Type Assembly { get; set; }
    }
}
