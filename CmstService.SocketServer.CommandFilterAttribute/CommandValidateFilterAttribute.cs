using System;
using SuperSocket.SocketBase;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer.CommandFilterAttribute
{
    // 按照官方的说法，应该是可以作为 AppServer 或 AppSession 的类特性
    // 但是照例做后，无法正确取得命令的Name，影响了命令过滤器的功能
    // 只好中规中矩的添加到指定命令的类上，为了方便扩展也只好单独作为一个库出现了
    public class CommandValidateFilterAttribute : SubCommandFilterAttribute
    {
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            CmstSession session = commandContext.Session as CmstSession;
            // 匿名用户无权执行UPDATE、INSERT、DELETE等数据库写入命令，只能执行SELECT、LOGIN等命令
            if (!session.IsLoggedIn)
            {
                commandContext.Cancel = true;
                session.Send(session.AppServer.JsonSerialize(new Error("PrivilegeRefusedException", "执行该命令需要登录权限，请登录后再执行！")));
            }
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {
            
        }
    }
}
