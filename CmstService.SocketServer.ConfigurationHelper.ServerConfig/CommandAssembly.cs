using System;
using System.Configuration;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using SuperSocket.Common;
using SuperSocket.SocketBase.Config;
using CmstService.SocketServer.ConfigurationHelper;
using CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig
{
    public class CommandAssembly : ConfigurationElement, ICommandAssemblyConfig
    {
        [Editor(typeof(PropertyGridFileEditor), typeof(UITypeEditor))]
        [InterfaceAssembly(typeof(ICommandAssemblyConfig))]
        [Category("命令集"), DisplayName("命令程序集")]
        [ConfigurationProperty("assembly", IsRequired = false)]
        public string Assembly
        {
            get { return this["assembly"] as string; }
            set { this["assembly"] = value; }
        }
    }

    [ConfigurationCollection(typeof(CommandAssembly))]
    public class CommandAssemblyCollection : CommonGenericConfigCollection<CommandAssembly, ICommandAssemblyConfig>
    {
        
    }
}
