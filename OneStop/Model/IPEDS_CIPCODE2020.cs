using Newtonsoft.Json;

namespace OneStop.Model
{
    public class IPEDS_CIPCODE2020
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string CIPCode { get; set; }
        public string CIPTitle { get; set; }
        public string CIPDefinition { get; set; }
    }
}
