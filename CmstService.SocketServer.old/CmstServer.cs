using System;
using System.Net;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using SuperSocket.WebSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using CmstService.SocketServer.Config;

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
                // 数据库配置节
                DatabaseConfigCollection dbConf = config.GetChildConfig<DatabaseConfigCollection>("databases");
                
                // 数据库辅助对象
                this.DBHelper = new DatabaseHelper(dbConf);
                
                // 数据库配置节有三个自定属性，如果 NeedLogin = true，则必须给定非空 LoginDatabase、LoginQuery
                if (dbConf.NeedLogin)
                {
                    if (config.Options.GetValues("redirectHref").Length < 1)
                    {
                        throw new KeyNotFoundException("系统配置了用户登录验证，此项功能需要配置登录成功时的重定向地址，但系统并未取得该配置，请确认！");
                    }
                    this.InitUserInfo(dbConf, this.DBHelper.ExcuteQuery(dbConf.LoginDatabase, dbConf.LoginQuery));
                }
            }
            catch (NullReferenceException e) { throw e; }
            catch (NotSupportedException e) { throw e; }
            catch (KeyNotFoundException e) { throw e; }
            catch (Exception e) { throw e; }
            return base.Setup(rootConfig, config);
        }
        
        // 初始化用户登录验证信息
        public void InitUserInfo(DatabaseConfigCollection dbConf, DataTable table)
        {
            if (table == null)
            {
                throw new NullReferenceException("系统配置了用户登录验证，此项功能需要读取数据库的关联表，但系统无法获取该表，请确认配置文件中的信息填写是否正确！");
            }
            
            List<string> subscriptionList = dbConf.SubscriptionList.Split(',').ToList<string>();

            // 检测订阅列表所有字段是否在表中，不在则移除
            foreach (string item in subscriptionList)
            {
                if (!table.Columns.Contains(item))
                {
                    subscriptionList.Remove(item);
                }
            }

            // 检测用户字段、密码字段
            if (!table.Columns.Contains(dbConf.UserField) || !table.Columns.Contains(dbConf.KeyField) || subscriptionList.Count < 1)
            {
                throw new KeyNotFoundException("系统未在指定用户表中取得指定字段，请确认字段填写是否正确！");
            }

            // 创建对象
            this.UserInfo = new Dictionary<string, UserInfo>();

            // 赋值
            foreach (DataRow row in table.Rows)
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

                this.UserInfo.Add(row[dbConf.UserField].ToString().Trim(), new UserInfo() { Password = row[dbConf.KeyField].ToString().Trim(), Group = row[dbConf.GroupField].ToString().Trim(), SubscriptionList = list });
            }
        }

        // 本地化关闭原因
        public string GetLocaleCloseReason(CloseReason reason)
        {
            return Utility.GetLocaleCloseReason(Convert.ToInt32(reason));
        }

        // 数据库辅助对象
        public DatabaseHelper DBHelper { get; private set; }

        // 如需提供用户登录验证，则临时缓存数据到内存
        public Dictionary<string, UserInfo> UserInfo { get; private set; }
    }

    public class UserInfo
    {
        // 用户密码
        public string Password { get; internal set; }

        // 用户组
        public string Group { get; internal set; }

        // 用户订阅列表
        public List<string> SubscriptionList { get; internal set; }
    }
}
