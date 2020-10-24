using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class IPEDS_CIPCODE2020
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string CIPCode { get; set; }  // partition key
        public string CIPTitle { get; set; }
        public string CIPDefinition { get; set; }
    }
}
