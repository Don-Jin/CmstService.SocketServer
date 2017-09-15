using System;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CmstService.SocketServer.Config;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer
{
    public class DatabaseHelper
    {
        public DatabaseHelper(DatabaseConfigCollection dbconf)
        {
            if (dbconf == null || dbconf.GetPackDetails().Count < 1)
            {
                throw new NullReferenceException("系统需要配置文件中的数据库配置节，但是未找到，请确认正确添加后重新启动系统！");
            }
            this.InitDBConfig(dbconf);
        }

        private void InitDBConfig(DatabaseConfigCollection dbconf)
        {
            this.DBInfo = dbconf.GetPackDetails();
            foreach (string key in this.DBInfo.Keys)
            {
                DatabaseConfigCollection.DBInfo info = this.DBInfo[key];
                info.DBConnection = this.GetDBConnection(info.Name, info.ConnString);
            }
        }

        private IDbConnection GetDBConnection(string name, string connStr)
        { 
            switch(name.ToLower())
            {
                case "sqlserver": return this.GetSQLServerConnection(connStr);
                case "sqlite": return this.GetSQLiteConnection(connStr);
                default: throw new NotSupportedException("抱歉，" + name.ToUpper() + " 数据库当前不被支持！");
            }
        }

        private Dictionary<string, DatabaseConfigCollection.DBInfo> DBInfo = null;

        // SQL Server 连接实例
        private SqlConnection GetSQLServerConnection(string connString)
        {
            return new SqlConnection(connString);
        }

        // SQLite 连接实例
        private SQLiteConnection GetSQLiteConnection(string connString)
        {
            return new SQLiteConnection(connString);
        }

        private string GetSqlExpression(string dbname, string sqlname, params object[] args)
        {
            if (!this.DBInfo.ContainsKey(dbname) || !this.DBInfo[dbname].SQLDetails.ContainsKey(sqlname))
            {
                throw new KeyNotFoundException("数据操作所指定的数据库或命令不存在！");
            }

            DatabaseConfigCollection.SQLInfo sqlInfo = this.DBInfo[dbname].SQLDetails[sqlname];

            // 如果SQL语句为空则返回NULL
            if (sqlInfo.SQL.Trim().Length == 0) { return null; }

            // 非Limit查询，直接返回该SQL
            if (!sqlInfo.IsLimit) { return sqlInfo.SQL; }

            // 区间查询，则根据相应分割字符拼接，如果未提供字符则返回NULL
            if (sqlInfo.Delimiter.Trim().Length == 0) { return null; }

            // 替换分割字符
            int curIndex = 0;
            return Regex.Replace(sqlInfo.SQL, sqlInfo.Delimiter, new MatchEvaluator(delegate (Match target) {
                return args[curIndex].GetType().Equals(typeof(string)) ? "'" + args[curIndex++].ToString() + "'" : args[curIndex++].ToString();
            }));
        }

        // 查询语句
        public DataTable ExcuteQuery(string dbname, string sqlname, params object[] args)
        {
            DataTable tbl = new DataTable();
            try
            {
                string sql = this.GetSqlExpression(dbname, sqlname, args);
                if (sql != null)
                {
                    DatabaseConfigCollection.DBInfo dbInfo = this.DBInfo[dbname];
                    // 指定返回数据表的名称
                    tbl.TableName = dbInfo.SQLDetails[sqlname].TableName;
                    // 查询数据
                    switch (dbInfo.Name.ToLower())
                    {
                        case "sqlserver":
                            new SqlDataAdapter(sql, dbInfo.DBConnection as SqlConnection).Fill(tbl);
                            break;
                        case "sqlite":
                            new SQLiteDataAdapter(sql, dbInfo.DBConnection as SQLiteConnection).Fill(tbl);
                            break;
                    }
                }
            }
            //catch (InvalidOperationException e) { throw e; }
            //catch (KeyNotFoundException e) { throw e; }
            catch (Exception e) { throw e; }
            return tbl;
        }

        // 非查询语句
        public string ExcuteNonQuery(string dbname, string sqlname, params object[] args)
        {
            int fetch = -1;
            string res = "";
            IDbCommand command = null;
            IDbConnection connection = null;
            try
            {
                string sql = this.GetSqlExpression(dbname, sqlname, args);
                if (sql == null)
                {
                    return "数据操作所指定的命令不存在，请确认后再提交！";
                }
                DatabaseConfigCollection.DBInfo dbInfo = this.DBInfo[dbname];
                connection = dbInfo.DBConnection;
                connection.Open();
                switch (dbInfo.Name.ToLower())
                {
                    case "sqlserver":
                        command = new SqlCommand(sql, connection as SqlConnection, (connection as SqlConnection).BeginTransaction());
                        break;
                    case "sqlite":
                        command = new SQLiteCommand(sql, connection as SQLiteConnection, (connection as SQLiteConnection).BeginTransaction());
                        break;
                }
                fetch = command.ExecuteNonQuery();
                command.Transaction.Commit();
                res = fetch > 0 ? "谢天谢地，数据操作成功了！" : "抱歉，数据提交成功了，但是数据库操作失败了，请检查您所提交的数据以排查错误原因！";
            }
            //catch (InvalidOperationException e) { command.Transaction.Rollback(); throw e; }
            //catch (KeyNotFoundException e) { command.Transaction.Rollback(); throw e; }
            //catch (SqlException e) { command.Transaction.Rollback(); throw e; }
            catch (Exception e) { command.Transaction.Rollback(); throw e; }
            finally
            {
                connection.Close();
            }
            return res;
        }
    }
}
