using Newtonsoft.Json;
using System.Collections.Generic;

namespace OneStopHelper.Model
{
    public class USNewsRanking
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string year { get; set; }
        [JsonProperty(PropertyName = "universities")]
        public List<USNewsRankingItem> Universities { get; set; }
        [JsonProperty(PropertyName = "libertyColleges")]
        public List<USNewsRankingItem> LibertyColleges { get; set; }
    }
}
