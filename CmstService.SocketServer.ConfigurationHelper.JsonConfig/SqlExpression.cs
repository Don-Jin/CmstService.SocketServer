using System;
using Newtonsoft.Json;

namespace CmstService.SocketServer.ConfigurationHelper.JsonConfig
{
    public class SqlExpression : BasicConfig
    {
        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("sql")]
        public string Sql { get; set; }

        // 分隔符，用于区间查询，或插入等的数据，分割后将用指定值替换
        // 如插入，给定值[1, 2, 3, 4, 5]
        // 给定语句："INSERT INTO table VALUES (-);"
        // 分割后：["INSERT INTO table VALUES (", ");"]
        // 最终拼接为："INSERT INTO table VALUES (1, 2, 3, 4, 5);"
        // 多个分隔符，用不含空格的逗号隔开，且在赋值时按照给定值顺序依次进行
        [JsonProperty("delimiter")]
        public string Delimiter { get; set; }
    }
}
