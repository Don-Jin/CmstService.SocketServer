using System;
//using System.Data.SqlClient;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using CmstService.SocketServer.CommandFilterAttribute;
using Newtonsoft.Json;

namespace CmstService.SocketServer.Command
{
    [CommandValidateFilter]
    public class NONQUERY : JsonSubCommand<CmstSession, QueryMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, QueryMessage commandInfo)
        {
            try
            {
                string fetch = session.AppServer.DBHelper.ExcuteNonQuery(commandInfo.Database, commandInfo.Query, commandInfo.Range);
                session.Send(fetch);
            }
            //catch (InvalidOperationException e) { session.Send(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (SqlException e) { session.Send(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            catch (Exception e) { session.Send(e.Message); }
        }
    }
}
