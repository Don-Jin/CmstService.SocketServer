using System;
using System.Data;
using System.Collections.Generic;
using SuperSocket.WebSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using CmstService.SocketServer.DatabaseHelper;
using CmstService.SocketServer.ConfigurationHelper.JsonConfig;

namespace CmstService.SocketServer
{
    // Cmst Socket 通信服务
    public class CmstServer : WebSocketServer<CmstSession>
    {
        public CmstServer()
        {
            
        }
        
        // 为获取数据库连接等信息，需要重写服务装载方法
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            // 数据库配置
            try
            {
                // 数据库辅助对象
                JsonConfig jsonConf = ReflectionHelper.JsonConfig;
                
                // 数据库配置节
                DatabaseConfig dbConf = jsonConf.DatabaseConfig[jsonConf.SystemConfig.LoginDatabase];

                DataTable userInfo = CommonDatabaseHelper.ExcuteQuery(new List<QueryConfig>() { 
                    new QueryConfig() {
                        ConnectionString = dbConf.ConnectionString,
                        DatabaseType = dbConf.Type,
                        QueryList = new List<SqlExpression>() { 
                            dbConf.SqlExpressions[jsonConf.SystemConfig.LoginQuery]
                        }
                    }
                }).Tables[0];

                // 初始化用户登录验证信息

                List<string> subscriptionList = jsonConf.SystemConfig.SubscriptionList;

                // 检测订阅列表所有字段是否在表中，不在则移除
                foreach (string item in subscriptionList)
                {
                    if (!userInfo.Columns.Contains(item))
                    {
                        subscriptionList.Remove(item);
                    }
                }

                // 检测用户字段、密码字段
                if (!userInfo.Columns.Contains(jsonConf.SystemConfig.UserField) || !userInfo.Columns.Contains(jsonConf.SystemConfig.KeyField) || subscriptionList.Count < 1)
                {
                    throw new KeyNotFoundException("系统未在指定用户表中取得指定字段，请确认字段填写是否正确！");
                }

                // 创建对象
                this.UserInfo = new Dictionary<string, UserInfo>();

                // 赋值
                foreach (DataRow row in userInfo.Rows)
                {
                    // 用户订阅列表
                    List<string> list = new List<string>();

                    // 遍历数据，如果用户订阅了该项，则添加到 list
                    foreach (string item in subscriptionList)
                    {
                        if (Convert.ToBoolean(row[item.Trim()]))
                        {
                            list.Add(item.Trim());
                        }
                    }

                    this.UserInfo.Add(
                        row[jsonConf.SystemConfig.UserField].ToString().Trim(), 
                        new UserInfo() { 
                            Password = row[jsonConf.SystemConfig.KeyField].ToString().Trim(),
                            Name = row[jsonConf.SystemConfig.NameField].ToString().Trim(), 
                            Group = row[jsonConf.SystemConfig.GroupField].ToString().Trim(), 
                            SubscriptionList = list 
                        }
                    );
                }
            }
            catch (NullReferenceException e) { throw e; }
            catch (NotSupportedException e) { throw e; }
            catch (KeyNotFoundException e) { throw e; }
            catch (Exception e) { throw e; }
            return base.Setup(rootConfig, config);
        }

        // 本地化关闭原因
        public string GetLocaleCloseReason(CloseReason reason)
        {
            return Utility.GetLocaleCloseReason(Convert.ToInt32(reason));
        }

        // 如需提供用户登录验证，则临时缓存数据到内存
        // 形如：{ "user": {"password": "", "name": "", "group": "", "list": []} }
        public Dictionary<string, UserInfo> UserInfo { get; private set; }
    }

    public class UserInfo
    {
        // 用户密码
        public string Password { get; internal set; }

        // 用户姓名
        public string Name { get; internal set; }

        // 用户组
        public string Group { get; internal set; }

        // 客户端密钥，用于客户端不支持本地存储时临时存在服务器
        public string Token { internal get; internal set; }

        // 用户订阅列表
        public List<string> SubscriptionList { get; internal set; }
    }
}
