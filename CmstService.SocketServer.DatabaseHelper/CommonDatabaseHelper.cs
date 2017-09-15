using System;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CmstService.SocketServer.JsonObject;
using CmstService.SocketServer.ConfigurationHelper.JsonConfig;

namespace CmstService.SocketServer.DatabaseHelper
{
    public sealed class CommonDatabaseHelper
    {
        private static IDbConnection GetDBConnection(string name, string connStr)
        { 
            switch(name.ToLower())
            {
                case "sqlserver": return GetSQLServerConnection(connStr);
                case "sqlite": return GetSQLiteConnection(connStr);
                default: throw new NotSupportedException("抱歉，" + name.ToUpper() + " 数据库当前不被支持！");
            }
        }

        // SQL Server 连接实例
        private static SqlConnection GetSQLServerConnection(string connString)
        {
            return new SqlConnection(connString);
        }

        // SQLite 连接实例
        private static SQLiteConnection GetSQLiteConnection(string connString)
        {
            return new SQLiteConnection(connString);
        }

        // 处理SqlCommand
        private static void InitDbCommand(List<string> keys, object[] args, string type, ref IDbCommand command)
        {
            if (keys != null)
            {
                for (int i = 0, len = keys.Count; i < len; i++)
                {
                    switch (type.ToLower())
                    {
                        case "sqlserver":
                            (command as SqlCommand).Parameters.AddWithValue(keys[i], args[i]);
                            break;
                        case "sqlite":
                            (command as SQLiteCommand).Parameters.AddWithValue(keys[i], args[i]);
                            break;
                    }
                }
            }
        }

        // 查询语句 - 拼接好的SQL
        public static DataSet ExcuteQuery(List<QueryConfig> list)
        {
            DataSet dataset = new DataSet();
            
            try
            {
                foreach (QueryConfig queryConf in list)
                {
                    // 获取数据库连接
                    IDbConnection connection = GetDBConnection(queryConf.DatabaseType, queryConf.ConnectionString);

                    // 批次查询
                    foreach (SqlExpression query in queryConf.QueryList)
                    {
                        if (!string.IsNullOrEmpty(query.Sql) && query.Type.ToUpper().Equals("SELECT"))
                        {
                            // 创建新表，并指定返回数据表的名称
                            DataTable tbl = new DataTable(query.TableName);

                            // 查询数据，并填充数据表
                            switch (queryConf.DatabaseType.ToLower())
                            {
                                case "sqlserver":
                                    new SqlDataAdapter(query.Sql, connection as SqlConnection).Fill(tbl);
                                    break;
                                case "sqlite":
                                    new SQLiteDataAdapter(query.Sql, connection as SQLiteConnection).Fill(tbl);
                                    break;
                            }
                            
                            // 将表添加到数据集中
                            dataset.Tables.Add(tbl);
                        }
                    }
                }
            }
            //catch (InvalidOperationException e) { throw e; }
            //catch (KeyNotFoundException e) { throw e; }
            catch (Exception e) { throw e; }
            return dataset;
        }

        // 查询语句 - 未拼接的SQL，利用SqlCommand过滤参数
        public static DataSet ExcuteQuery(List<QueryConfig2> list)
        {
            DataSet dataset = new DataSet();

            try
            {
                foreach (QueryConfig2 queryConf in list)
                {
                    // 获取数据库连接
                    IDbConnection connection = GetDBConnection(queryConf.DatabaseType, queryConf.ConnectionString);
                    IDbCommand command = null;

                    // 批次查询
                    foreach (CommandConfig cmdConf in queryConf.QueryList)
                    {
                        if (!string.IsNullOrEmpty(cmdConf.SqlExpression.Sql) && cmdConf.SqlExpression.Type.ToUpper().Equals("SELECT"))
                        {
                            // 创建新表，并指定返回数据表的名称
                            DataTable tbl = new DataTable(cmdConf.SqlExpression.TableName);

                            // 查询数据，并填充数据表
                            switch (queryConf.DatabaseType.ToLower())
                            {
                                case "sqlserver":
                                    command = new SqlCommand(cmdConf.SqlExpression.Sql, connection as SqlConnection);
                                    InitDbCommand(cmdConf.ArgumentKeys, cmdConf.Arguments, queryConf.DatabaseType, ref command);
                                    new SqlDataAdapter(command as SqlCommand).Fill(tbl);
                                    break;
                                case "sqlite":
                                    command = new SQLiteCommand(cmdConf.SqlExpression.Sql, connection as SQLiteConnection);
                                    InitDbCommand(cmdConf.ArgumentKeys, cmdConf.Arguments, queryConf.DatabaseType, ref command);
                                    new SQLiteDataAdapter(command as SQLiteCommand).Fill(tbl);
                                    break;
                            }

                            // 将表添加到数据集中
                            dataset.Tables.Add(tbl);
                        }
                    }
                }
            }
            //catch (InvalidOperationException e) { throw e; }
            //catch (KeyNotFoundException e) { throw e; }
            catch (Exception e) { throw e; }
            return dataset;
        }

        // 非查询语句 - 拼接好的SQL
        public static List<IMessage> ExcuteNonQuery(List<QueryConfig> list)
        {
            int fetch = -1;
            List<IMessage> res = new List<IMessage>();
            IDbCommand command = null;
            IDbConnection connection = null;
            try
            {
                foreach (QueryConfig queryConf in list)
                {
                    connection = GetDBConnection(queryConf.DatabaseType, queryConf.ConnectionString);
                    connection.Open();

                    foreach (SqlExpression query in queryConf.QueryList)
                    {
                        if (string.IsNullOrEmpty(query.Sql) || !"INSERT|UPDATE|DELETE".Contains(query.Type.ToUpper()))
                        {
                            res.Add(new Error("NullRefrenceException", "数据操作所指定的命令不存在或不合法，请确认后再提交！"));
                        }
                        else
                        {
                            try
                            { 
                                switch (queryConf.DatabaseType.ToLower())
                                {
                                    case "sqlserver":
                                        command = new SqlCommand(query.Sql, connection as SqlConnection, (connection as SqlConnection).BeginTransaction());
                                        break;
                                    case "sqlite":
                                        command = new SQLiteCommand(query.Sql, connection as SQLiteConnection, (connection as SQLiteConnection).BeginTransaction());
                                        break;
                                }
                        
                                fetch = command.ExecuteNonQuery();
                                command.Transaction.Commit();
                                if (fetch > 0)
                                {
                                    res.Add(new Success("恭喜，数据操作成功了！共 " + fetch.ToString() + " 行数据受影响！"));
                                }
                                else
                                {
                                    res.Add(new Error("InvalidOperationException", "抱歉，数据提交成功了，但是数据库操作失败了，请检查您所提交的数据以排查错误原因！"));
                                }
                            }
                            catch (Exception err)
                            {
                                command.Transaction.Rollback();
                                res.Add(new Error(err.GetType().Name, err.Message));
                            }
                        }
                    }
                }
            }
            //catch (InvalidOperationException e) { command.Transaction.Rollback(); throw e; }
            //catch (KeyNotFoundException e) { command.Transaction.Rollback(); throw e; }
            //catch (SqlException e) { command.Transaction.Rollback(); throw e; }
            catch (Exception e) { throw e; }
            finally
            {
                connection.Close();
            }
            return res;
        }

        // 非查询语句 - 未拼接的SQL，利用SqlCommand过滤参数
        public static List<IMessage> ExcuteNonQuery(List<QueryConfig2> list)
        {
            int fetch = -1;
            List<IMessage> res = new List<IMessage>();
            IDbCommand command = null;
            IDbConnection connection = null;
            try
            {
                foreach (QueryConfig2 queryConf in list)
                {
                    connection = GetDBConnection(queryConf.DatabaseType, queryConf.ConnectionString);
                    connection.Open();

                    foreach (CommandConfig cmdConf in queryConf.QueryList)
                    {
                        if (string.IsNullOrEmpty(cmdConf.SqlExpression.Sql) || !"INSERT|UPDATE|DELETE".Contains(cmdConf.SqlExpression.Type.ToUpper()))
                        {
                            res.Add(new Error("NullRefrenceException", "数据操作所指定的命令不存在或不合法，请确认后再提交！"));
                        }
                        else
                        {
                            try
                            {
                                switch (queryConf.DatabaseType.ToLower())
                                {
                                    case "sqlserver":
                                        command = new SqlCommand(cmdConf.SqlExpression.Sql, connection as SqlConnection, (connection as SqlConnection).BeginTransaction());
                                        break;
                                    case "sqlite":
                                        command = new SQLiteCommand(cmdConf.SqlExpression.Sql, connection as SQLiteConnection, (connection as SQLiteConnection).BeginTransaction());
                                        break;
                                }

                                // SQL预处理
                                InitDbCommand(cmdConf.ArgumentKeys, cmdConf.Arguments, queryConf.DatabaseType, ref command);
                                
                                fetch = command.ExecuteNonQuery();
                                command.Transaction.Commit();
                                if (fetch > 0)
                                {
                                    res.Add(new Success("恭喜，数据操作成功了！共 " + fetch.ToString() + " 行数据受影响！"));
                                }
                                else
                                {
                                    res.Add(new Error("InvalidOperationException", "抱歉，数据提交成功了，但是数据库操作失败了，请检查您所提交的数据以排查错误原因！"));
                                }
                            }
                            catch (Exception err)
                            {
                                command.Transaction.Rollback();
                                res.Add(new Error(err.GetType().Name, err.Message));
                            }
                        }
                    }
                }
            }
            //catch (InvalidOperationException e) { command.Transaction.Rollback(); throw e; }
            //catch (KeyNotFoundException e) { command.Transaction.Rollback(); throw e; }
            //catch (SqlException e) { command.Transaction.Rollback(); throw e; }
            catch (Exception e) { throw e; }
            finally
            {
                connection.Close();
            }
            return res;
        }
    }
}
