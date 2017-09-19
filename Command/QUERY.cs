using System;
using System.Data;
using SuperSocket.WebSocket.SubProtocol;
using CmstService.SocketServer.JsonObject;
using Newtonsoft.Json;

namespace CmstService.SocketServer.Command
{
    // 用于调用指定SQL语句进行数据库操作
    public class QUERY : JsonSubCommand<CmstSession, QueryMessage>
    {
        // 命令主执行过程
        protected override void ExecuteJsonCommand(CmstSession session, QueryMessage commandInfo)
        {
            try
            { 
                /*if ()
                {
                
                }

                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(session.AppServer.DBHelper.ExcuteQuery(commandInfo.Database, commandInfo.Query, commandInfo.Range));
                session.Send(JsonConvert.SerializeObject(dataSet));*/
            }
            catch (Exception e) { session.Send(e.Message); }
        }
    }
}
