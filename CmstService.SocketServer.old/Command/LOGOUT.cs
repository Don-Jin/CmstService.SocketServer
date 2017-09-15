using System;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using CmstService.SocketServer.CommandFilterAttribute;

namespace CmstService.SocketServer.Command
{
    [CommandValidateFilter]
    public class LOGOUT : JsonSubCommand<CmstSession, LoginMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, LoginMessage commandInfo)
        {
            throw new NotImplementedException();
        }
    }
}
