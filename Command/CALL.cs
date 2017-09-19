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
            CmstServer server = session.AppServer;  // 会话所在服务器

            try
            {
                // 没有令牌，或令牌无效
                if (!Utility.IsLogin(session.User, session.RemoteEndPoint.Address.ToString(), commandInfo.Token)) 
                {
                    throw new Exception("与服务器的通信验证未通过，可尝试重新登录！");
                }

                session.Send(ReflectionHelper.MethodInvoke(new MethodConfig()
                {
                    DatabaseName = commandInfo.DatabaseName,
                    AssemblyName = commandInfo.AssemblyName,
                    MethodName = commandInfo.MethodName
                }, new ArgumentConfig() { 
                    Arguments = commandInfo.Arguments
                }));
            }
            catch (Exception e) { session.Send(server.JsonSerialize(new Error("CallError", e.Message))); }
        }
    }
}
