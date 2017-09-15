using System;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using CmstService.SocketServer.DatabaseHelper;
using CmstService.SocketServer.ConfigurationHelper.JsonConfig;

namespace CmstService.SocketServer.Command
{
    public class SIGNUP : JsonSubCommand<CmstSession, LoginMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, LoginMessage commandInfo)
        {
            CmstServer server = session.AppServer;  // 会话所在服务器

            // 空值检测
            if (string.IsNullOrWhiteSpace(commandInfo.User)) { session.Send(server.JsonSerialize(new Error("Login", "用户名不能为空！"))); return; }
            if (string.IsNullOrWhiteSpace(commandInfo.Password)) { session.Send(server.JsonSerialize(new Error("Login", "密码不能为空！"))); return; }

            // 有效性检测
            if (server.UserInfo.ContainsKey(commandInfo.User)) { session.Send(server.JsonSerialize(new Error("Login", "用户名已存在！"))); return; }
            
            try
            {
                // 将注册信息写入数据库
                // 数据库辅助对象
                JsonConfig jsonConf = ReflectionHelper.JsonConfig;

                // 数据库配置项
                DatabaseConfig dbConf = jsonConf.DatabaseConfig[jsonConf.SystemConfig.SignupDatabase];
                CommonDatabaseHelper.ExcuteNonQuery();
                server.DBHelper.ExcuteNonQuery(dbConf.SignupDatabase, dbConf.SignupQuery, new object[3] { commandInfo.User, commandInfo.Password, commandInfo.Department });
                
                // 注册成功，标记会话状态
                // 1.刷新服务器用户信息
                lock (server.UserInfo)
                {
                    server.InitUserInfo(dbConf, server.DBHelper.ExcuteQuery(dbConf.LoginDatabase, dbConf.LoginQuery));
                }

                // 2.给注册用户发送重定向信息
                // 客户端收到后，应将 Cookie 信息写入本地存储，无论是 Cookie、Web Storage、IndexedDB等
                string token = Utility.AESCryptography(session.RemoteEndPoint.Address.ToString() + '|' + commandInfo.User);
                Redirect redirect = new Redirect(new RedirectMessage()
                {
                    Href = Utility.GetRedirectUrl(server.Config.Options.GetValues("redirectHref")[0], server.UserInfo[commandInfo.User].Group, commandInfo.User, token),
                    Cookie = new CookieMessage() { User = commandInfo.User, Token = token }
                });

                // 将重定向信息发送到客户端，支持多页面
                foreach (CmstSession ss in server.GetSessions(s => s.RemoteEndPoint.Address == session.RemoteEndPoint.Address))
                {
                    ss.Send(server.JsonSerialize(redirect));
                }
            }
            catch (Exception e) { session.Send(server.JsonSerialize(new Error(e.GetType().Name, e.Message))); }
        }
    }
}
