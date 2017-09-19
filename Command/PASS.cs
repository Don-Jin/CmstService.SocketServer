using System;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using CmstService.SocketServer.DatabaseHelper;
using CmstService.SocketServer.ConfigurationHelper.JsonConfig;

namespace CmstService.SocketServer.Command
{
    // 用于登录、注册等验证
    public class PASS : JsonSubCommand<CmstSession, LoginMessage>
    {
        protected override void ExecuteJsonCommand(CmstSession session, LoginMessage commandInfo)
        {
            CmstServer server = session.AppServer;  // 会话所在服务器

            try
            { 
                switch (commandInfo.Type.ToLower())
                {
                    case "login":
                        // 空值检测
                        if (string.IsNullOrWhiteSpace(commandInfo.User)) { session.Send(server.JsonSerialize(new Error("Login", "用户名不能为空！"))); return; }
                        if (string.IsNullOrWhiteSpace(commandInfo.Password)) { session.Send(server.JsonSerialize(new Error("Login", "密码不能为空！"))); return; }

                        // 有效性检测
                        if (!server.UserInfo.ContainsKey(commandInfo.User)) { session.Send(server.JsonSerialize(new Error("Login", "用户名不存在！"))); return; }
                        if (!server.UserInfo[commandInfo.User].Password.Equals(commandInfo.Password)) { session.Send(server.JsonSerialize(new Error("Login", "登录密码错误！"))); return; }

                        // 登录成功，标记会话状态
                        session.User = commandInfo.User;

                        // 用户验证密钥 - 令牌
                        // 加密内容为：用户名|日期|登录IP
                        string token = Utility.AESCryptography(commandInfo.User + "|" + DateTime.Now.ToString() + "|" + session.RemoteEndPoint.Address.ToString());
                        
                        // 将令牌临时保留到服务器，以免客户端不支持本地存储时页面跳转登录失效
                        server.UserInfo[commandInfo.User].Token = token;
                        session.Send(server.JsonSerialize(new Success(new SessionMessage() { 
                            Token = token,
                            User = session.User,
                            Name = server.UserInfo[session.User].Name,
                            SubscriptionList = server.UserInfo[session.User].SubscriptionList
                        }, "登录成功！")));
                        break;
                    case "logout":

                        break;
                    case "signup":

                        break;
                    case "signin":

                        break;
                }
            }
            catch (Exception e) { session.Send(server.JsonSerialize(new Error(commandInfo.Type.ToLower(), e.Message))); }
        }
    }
}
