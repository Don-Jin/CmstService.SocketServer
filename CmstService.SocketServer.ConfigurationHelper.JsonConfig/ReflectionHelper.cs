using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using CmstService.SocketServer.JsonObject;


namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public sealed class ReflectionHelper
    {
        static ReflectionHelper()
        {
            if (m_JsonConfig == null)
            {
                Initialize();
            }
        }

        // 动态加载的程序集
        private static Dictionary<string, Assembly> m_Assemblies = new Dictionary<string, Assembly>();

        // 反序列化后得到的对象
        private static JsonConfig m_JsonConfig = null;

        public static JsonConfig JsonConfig
        {
            get { return m_JsonConfig; }
        }

        public static void Initialize()
        {
            string jsonFile = AppDomain.CurrentDomain.BaseDirectory + "\\JsonConfig.json";  // JSON 配置文件
            
            // 文件不存在则抛出异常
            if (!File.Exists(jsonFile))
            {
                throw new FileNotFoundException("服务器所依赖的配置文件不存在，请联系管理员修复！");
            }

            string jsonContents = File.ReadAllText(jsonFile);

            // 文件内容不为空则继续处理
            if (jsonContents.Length > 0) 
            {
                m_JsonConfig = JsonConvert.DeserializeObject<JsonConfig>(jsonContents);

                foreach (DatabaseConfig dbconf in m_JsonConfig.DatabaseConfig.Values)
                {
                    foreach (string key in dbconf.SqlAssemblies.Keys)
                    {
                        SqlAssembly sa = dbconf.SqlAssemblies[key];
                        Assembly asm = Assembly.Load(sa.Assembly);
                        m_Assemblies.Add(key, asm);

                        // 部分程序集可能需要初始化
                        if (sa.HasInitial())
                        {
                            foreach (InitialInfo iinfo in sa.Initial)
                            {
                                switch (iinfo.Type)
                                {
                                    case "field":
                                        FieldInfo field = asm.GetType(iinfo.Class).GetField(iinfo.Name);
                                        field.SetValue(field, iinfo.Value);
                                        break;
                                    case "property":
                                        PropertyInfo property = asm.GetType(iinfo.Class).GetProperty(iinfo.Name);
                                        property.SetValue(property, iinfo.Value, null);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string MethodInvoke(MethodConfig methodConf, ArgumentConfig argumentConf)
        {
            // 返回结果
            List<object> result = new List<object>();

            // 获取方法信息
            MethodInfo method = m_JsonConfig.DatabaseConfig[methodConf.DatabaseName].GetMethod(methodConf.AssemblyName, methodConf.MethodName);
            System.Reflection.MethodInfo func = null;

            // 支持多组参数，多次执行
            foreach (object[] args in argumentConf.Arguments)
            {
                // 处理参数信息
                List<Type> m_params = new List<Type>();

                // 实例化该参数类
                List<object> argInstance = new List<object>();

                foreach (ArgumentInfo argInfo in method.Arguments)
                {
                    // 1.如果 properties 为空，则说明是系统原生结构，如：System.Int32、System.String等
                    // 2.如果不为空，则说明需要实例化该类，然后把值赋给对应 properties 中的属性名
                    Type m_Type = null;

                    if (argInfo.Properties.Length > 0)
                    {
                        m_Type = m_Assemblies[methodConf.AssemblyName].GetType(argInfo.Class);

                        // 实例化该参数类
                        object instance = Activator.CreateInstance(m_Type);

                        for (int i = 0, len = argInfo.Properties.Length; i < len; i++)
                        {
                            m_Type.GetProperty(argInfo.Properties[i]).SetValue(instance, args[i], null);
                        }
                        
                        argInstance.Add(instance);
                    }
                    else
                    {
                        m_Type = Type.GetType(argInfo.Class);
                        argInstance.Add(args[0]);
                    }
                    
                    // 添加到参数列表
                    m_params.Add(m_Type);
                }

                if (func == null)
                {
                    func = m_Assemblies[methodConf.AssemblyName].GetType(method.Class).GetMethod(method.Method, m_params.ToArray());
                }
                
                result.Add(func.Invoke(func, argInstance.ToArray()));
            }
            
            // 通过反射获取指定方法，并执行
            return JsonConvert.SerializeObject(result);
        }

        public static MethodInfo GetMethod(string databaseName, string assemblyName, string methodName)
        {
            return JsonConfig.DatabaseConfig[databaseName].SqlAssemblies[assemblyName].Methods[methodName];
        }

        public static QueryConfig GetSqls(string databaseName, string[] sqlNames)
        {
            DatabaseConfig dbConf = m_JsonConfig.DatabaseConfig[databaseName];
            QueryConfig query = new QueryConfig() { 
                ConnectionString = dbConf.ConnectionString,
                DatabaseType = dbConf.Type,
                QueryList = new List<SqlExpression>()
            };

            foreach (string name in sqlNames)
            {
                query.QueryList.Add(dbConf.SqlExpressions[name]);
            }

            return query;
        }

        public static QueryConfig2 GetSqls(string databaseName, List<QueryInfo> queryInfoList)
        {
            DatabaseConfig dbConf = m_JsonConfig.DatabaseConfig[databaseName];
            QueryConfig2 query = new QueryConfig2()
            {
                ConnectionString = dbConf.ConnectionString,
                DatabaseType = dbConf.Type,
                QueryList = new List<CommandConfig>()
            };

            foreach (QueryInfo queryInfo in queryInfoList)
            {
                CommandConfig cmdConf = new CommandConfig() {
                    SqlExpression = dbConf.SqlExpressions[queryInfo.QueryName],
                    Arguments = queryInfo.Arguments
                };

                // 处理参数
                if (cmdConf.ArgumentKeys != null && (queryInfo.Arguments == null || queryInfo.Arguments.Length < 1))
                {
                    continue;
                }

                query.QueryList.Add(cmdConf);
            }

            return query;
        }
    }

    public class JsonConfig
    {
        [JsonProperty("databaseConfig")]
        public Dictionary<string, DatabaseConfig> DatabaseConfig { get; set; }

        [JsonProperty("systemConfig")]
        public SystemConfig SystemConfig { get; set; }
    }

    public class MethodConfig
    {
        public string DatabaseName { get; set; }

        public string AssemblyName { get; set; }

        public string MethodName { get; set; }
    }

    public class ArgumentConfig
    {
        public List<object[]> Arguments { get; set; }
    }

    public class QueryConfig
    {
        public string DatabaseType { get; set; }

        public string ConnectionString { get; set; }

        public List<SqlExpression> QueryList { get; set; }
    }

    public class CommandConfig
    {
        public SqlExpression SqlExpression { get; set; }

        public object[] Arguments { get; set; }

        public List<string> ArgumentKeys
        {
            get
            {
                List<string> keys = new List<string>();
                foreach (Match mat in new Regex(@"@[a-zA-Z0-9]+").Matches(SqlExpression.Sql))
                {
                    keys.Add(mat.Value);
                }
                return keys.Count > 0 ? keys : null;
            }
        }
    }

    public class QueryConfig2
    {
        public string DatabaseType { get; set; }

        public string ConnectionString { get; set; }

        public List<CommandConfig> QueryList { get; set; }
    }
}
