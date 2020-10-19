using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class IPEDS_SSIS
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? SISTOTL { get; set; }
        public double? SISPROF { get; set; }
        public double? SISASCP { get; set; }
        public double? SISASTP { get; set; }
        public double? SISINST { get; set; }
        public double? SISLECT { get; set; }

    }
}
