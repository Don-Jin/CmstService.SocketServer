using System;
using System.Data;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using Newtonsoft.Json;

namespace CmstService.SocketServer.Command
{
    public class QUERY : JsonSubCommand<CmstSession, QueryMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, QueryMessage commandInfo)
        {
            try
            { 
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(session.AppServer.DBHelper.ExcuteQuery(commandInfo.Database, commandInfo.Query, commandInfo.Range));
                session.Send(JsonConvert.SerializeObject(dataSet));
            }
            catch (Exception e) { session.Send(e.Message); }
        }
    }
}
