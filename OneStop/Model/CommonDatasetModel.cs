using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class CommonDatasetModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public int year { get; set; }
        [JsonProperty(PropertyName = "waiting")]
        public dynamic Waiting { get; set; }
        [JsonProperty(PropertyName = "admDecision")]
        public dynamic AdmDecision { get; set; }
        [JsonProperty(PropertyName = "satAct")]
        public dynamic SatAct { get; set; }
        [JsonProperty(PropertyName = "gpa")]
        public dynamic Gpa { get; set; }
        [JsonProperty(PropertyName = "apply")]
        public dynamic Apply { get; set; }
        [JsonProperty(PropertyName = "transfer")]
        public dynamic Transfer { get; set; }
    }
}
