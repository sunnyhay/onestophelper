using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class CollegeDatasetModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }

        [JsonProperty(PropertyName = "year")]
        public int Year { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "waiting")]
        public dynamic Waiting { get; set; }

        [JsonProperty(PropertyName = "admReq")]
        public dynamic AdmissionReq { get; set; }

        [JsonProperty(PropertyName = "satAct")]
        public dynamic SatAct { get; set; }

        [JsonProperty(PropertyName = "admDecision")]
        public dynamic AdmissionDecision { get; set; }

        [JsonProperty(PropertyName = "classRankPercent")]
        public dynamic ClassRank { get; set; }

        [JsonProperty(PropertyName = "gpa")]
        public dynamic Gpa { get; set; }

        [JsonProperty(PropertyName = "apply")]
        public dynamic Apply { get; set; }
                
    }
}
