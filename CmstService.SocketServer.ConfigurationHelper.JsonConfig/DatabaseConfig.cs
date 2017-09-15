using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public class DatabaseConfig : BasicConfig
    {
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }

        [JsonProperty("sqlExpressions")]
        public Dictionary<string, SqlExpression> SqlExpressions { get; set; }

        [JsonProperty("sqlAssemblies")]
        public Dictionary<string, SqlAssembly> SqlAssemblies { get; set; }

        public SqlExpression GetSql(string name)
        {
            if (!string.IsNullOrEmpty(name) && SqlExpressions.ContainsKey(name))
            {
                return SqlExpressions[name];
            }
            return null;
        }

        public string[] GetAssemblies()
        { 
            List<string> res = new List<string>();
            
            foreach (SqlAssembly asm in SqlAssemblies.Values)
            {
                res.Add(asm.Assembly);
            }
            return res.ToArray();
        }

        public SqlAssembly GetAssembly(string name)
        { 
            if (!string.IsNullOrEmpty(name) && SqlAssemblies.ContainsKey(name))
            {
                return SqlAssemblies[name];
            }
            return null;
        }

        public MethodInfo GetMethod(string assemblyName, string methodName)
        {
            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(methodName)) return null;

            if (SqlAssemblies.ContainsKey(assemblyName) && SqlAssemblies[assemblyName].Methods.ContainsKey(methodName))
            {
                return SqlAssemblies[assemblyName].Methods[methodName];
            }
            return null;
        }
    }
}
