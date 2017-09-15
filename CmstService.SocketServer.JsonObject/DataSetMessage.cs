using System;
using System.Data;
using Newtonsoft.Json;

namespace CmstService.SocketServer.JsonObject
{
    public class DataSetMessage
    {
        [JsonProperty("dataset")]
        public DataSet DataSet { get; set; }
    }
}
