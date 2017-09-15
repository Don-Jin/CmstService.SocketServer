using System;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer.Command
{
    public class LOGIN : JsonSubCommand<CmstSession, LoginMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, LoginMessage commandInfo)
        {
            CmstServer server = session.AppServer;  // 会话所在服务器
            
            // 空值检测
            if (string.IsNullOrWhiteSpace(commandInfo.User)) { session.Send(server.JsonSerialize(new Error("Login", "用户名不能为空！"))); return; }
            if (string.IsNullOrWhiteSpace(commandInfo.Password)) { session.Send(server.JsonSerialize(new Error("Login", "密码不能为空！"))); return; }

            // 有效性检测
            if (!server.UserInfo.ContainsKey(commandInfo.User)) { session.Send(server.JsonSerialize(new Error("Login", "用户名不存在！"))); return; }
            if (!server.UserInfo[commandInfo.User].Password.Equals(commandInfo.Password)) { session.Send(server.JsonSerialize(new Error("Login", "登录密码错误！"))); return; }

            try
            {
                // 登录成功，标记会话状态
                session.Name = commandInfo.User;
                session.IsLoggedIn = true;
                session.SubscriptionList = server.UserInfo[commandInfo.User].SubscriptionList;

                // 重定向信息
                // 客户端收到后，应将 Cookie 信息写入本地存储，无论是 Cookie、Web Storage、IndexedDB等
                string token = Utility.AESCryptography(session.RemoteEndPoint.Address.ToString() + '|' + commandInfo.User);
                Redirect redirect = new Redirect(new RedirectMessage() {
                    Href = Utility.GetRedirectUrl(server.Config.Options.GetValues("redirectHref")[0], server.UserInfo[commandInfo.User].Group, commandInfo.User, token),
                    Cookie = new CookieMessage() { User = commandInfo.User, Token = token }
                });

                // 登录成功，遍历会话列表发送重定向信息
                // 既然用 WebSocket，那么肯定是支持 H5 的浏览器，所以，综合考虑在服务端还是客户端存储登录状态，客户端要更优越一些
                // 毕竟服务器需要开辟单独的空间去存储，而在客户端浏览器，利用 H5 的 localStorage 这个类似 Cookie 的本地存储的键值对，
                // Local Storage的存储是长期的，Session Storage只能保持到会话结束
                // 那么在页面跳转继续保持连接状态就可以这么做：
                // 1、发送重定向信息到客户端，包含：重定向地址、服务器对用户名进行DES加密用来反向给服务器验证的字串
                // 2、客户端收到信息后，先将加密子串存储到Local Storage，然后执行重定向
                // 3、在重定向的页面建立通信，握手后用验证字串修改会话登录状态，然后返给客户端获得所有功能权限

                // 将重定向信息发送到客户端，支持多页面
                foreach (CmstSession ss in server.GetSessions(s => s.RemoteEndPoint.Address == session.RemoteEndPoint.Address))
                {
                    ss.Send(server.JsonSerialize(redirect));
                }
            }
            // 所有异常信息，throw 之后将会中断会话，故只在 DEBUG 阶段抛出，发布前所有会话的异常将被只被捕捉而不处理
            //catch (NotSupportedException e) { session.Send(server.JsonSerialize(new Error(e.GetType().Name, e.Message))); throw e; }
            //catch (ArgumentNullException e) { session.Send(server.JsonSerialize(new Error(e.GetType().Name, e.Message))); throw e; }
            //catch (ArgumentException e) { session.Send(server.JsonSerialize(new Error(e.GetType().Name, e.Message))); throw e; }
            //catch (NullReferenceException e) { session.Send(server.JsonSerialize(new Error(e.GetType().Name, e.Message))); throw e; }
            //catch (IndexOutOfRangeException e) { session.Send(server.JsonSerialize(new Error(e.GetType().Name, e.Message))); throw e; }
            catch (Exception e) { session.Send(server.JsonSerialize(new Error(e.GetType().Name, e.Message))); }
        }
    }
}
