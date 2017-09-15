using System;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer.Command
{
    // 无论是否登录都会在第一时间发送 SIGNIN 命令，以从服务器确认登录状态
    public class SIGNIN : JsonSubCommand<CmstSession, CookieMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, CookieMessage commandInfo)
        {
            CmstServer server = session.AppServer;
            
            // 验证用户
            // 被识别为匿名用户的4种情况：
            // 1.命令参数中用户名为 Null 或空
            // 2.命令参数中加密字符串为 Null 或空
            // 3.用户名在服务器用户列表中不存在
            // 4.用户未登录
            // 凡上述4种情况，本命令什么都不做
            if (string.IsNullOrWhiteSpace(commandInfo.User) ||
                string.IsNullOrWhiteSpace(commandInfo.Token) || 
                !server.UserInfo.ContainsKey(commandInfo.User) ||
                !Utility.IsLogin(session.RemoteEndPoint.Address.ToString() + "|" + commandInfo.User, commandInfo.Token))
            {
                return;
            }

            // 验证通过后则必然表名用户已登录，此时需要做2件事情：
            // 1.刷新会话属性
            // 2.回传会话消息（页面可能需要确定某些功能的可用性，如订阅列表）
            
            // 验证完毕，刷新会话状态，并回传会话信息
            session.Name = commandInfo.User;
            session.IsLoggedIn = true;
            session.SubscriptionList = server.UserInfo[commandInfo.User].SubscriptionList;
            session.Send(server.JsonSerialize(new Session(new SessionMessage() { User = session.Name, IsLoggedIn = session.IsLoggedIn, SubscriptionList = session.SubscriptionList })));
        }
    }
}
