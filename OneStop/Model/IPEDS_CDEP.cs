using Newtonsoft.Json;
using System.Collections.Generic;

namespace OneStop.Model
{
    public class IPEDS_CDEP
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public Dictionary<string, IPEDS_CDEP_ITEM> Items { get; set; } //key is CIPCODE in CDEP table
	}
}
