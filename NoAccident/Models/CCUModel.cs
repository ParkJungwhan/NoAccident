using System.Collections.Generic;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace NoAccident.Models
{
    public class CCUModel
    {
        [JsonProperty("List")]
        public List<Server> List { get; set; }
    }

    public class Server
    {
        [JsonProperty("CCU")]
        public long Ccu { get; set; }

        [JsonProperty("CPUR")]
        public double Cpur { get; set; }

        [JsonProperty("HDDR")]
        public double Hddr { get; set; }

        [JsonProperty("MEMR")]
        public double Memr { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}