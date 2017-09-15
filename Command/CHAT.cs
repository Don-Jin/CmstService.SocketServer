using System;
using System.Collections.Generic;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using CmstService.SocketServer.CommandFilterAttribute;

namespace CmstService.SocketServer.Command
{
    // 用于会话之间的通话
    public class CHAT : JsonSubCommand<CmstSession, ChatMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, ChatMessage commandInfo)
        {
            CmstServer server = session.AppServer;
            // 设置消息属性
            commandInfo.SendTime = DateTime.Now;

            // 查找接收者，将信息发送到指定会话
            foreach (CmstSession receiveSession in server.GetSessions(s => s.Name == commandInfo.Receiver))
            {
                receiveSession.Send(server.JsonSerialize(new Message(commandInfo)));
            }
        }
    }
}
