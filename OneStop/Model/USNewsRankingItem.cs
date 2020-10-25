using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class USNewsRankingItem
    {
        public string UNITID { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string INSTNM { get; set; }
        [JsonProperty(PropertyName = "rank")]
        public int Rank { get; set; }
    }
}
