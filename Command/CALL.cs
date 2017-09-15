using System;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using CmstService.SocketServer.DatabaseHelper;
using CmstService.SocketServer.ConfigurationHelper.JsonConfig;

namespace CmstService.SocketServer.Command
{
    // 用于调用指定程序集的方法
    public class CALL : JsonSubCommand<CmstSession, FunctionMessage>
    {
        protected override void ExecuteJsonCommand(CmstSession session, FunctionMessage commandInfo)
        {
            throw new NotImplementedException();
        }
    }
}
