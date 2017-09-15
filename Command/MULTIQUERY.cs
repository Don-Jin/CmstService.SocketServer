using System;
using System.Data;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using Newtonsoft.Json;

namespace CmstService.SocketServer.Command
{
    public class MULTIQUERY : JsonSubCommand<CmstSession, MultiQueryMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, MultiQueryMessage commandInfo)
        {
            CmstServer server = session.AppServer;
            try
            { 
                DataSetMessage message = new DataSetMessage() { DataSet = new DataSet() };
                foreach (QueryMessage query in commandInfo.Querys)
                {
                    message.DataSet.Tables.Add(server.DBHelper.ExcuteQuery(query.Database, query.Query, query.Range));
                }
                session.Send(JsonConvert.SerializeObject(message));
            }
            catch (Exception e) { session.Send(e.Message); }
        }
    }
}
