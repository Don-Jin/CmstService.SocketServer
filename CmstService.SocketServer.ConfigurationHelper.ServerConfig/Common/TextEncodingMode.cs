using System;
using CmstService.SocketServer.ConfigurationHelper;

namespace CmstService.SocketServer.ConfigurationHelper.ServerConfig.Common
{
    // 文本编码模式，字符串类型转换
    public class TextEncodingMode : PropertyGridStringConverter
    {
        public TextEncodingMode()
            : base(new string[2] { "ASCII", "UTF8" })
        { 
        
        }
    }
}
