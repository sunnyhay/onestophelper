using Newtonsoft.Json;

namespace OneStop.Model
{
    public class IPEDS_AL
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? LPBOOKS { get; set; }
        public double? LEBOOKS { get; set; }
        public double? LEDATAB { get; set; }
        public double? LPMEDIA { get; set; }
        public double? LEMEDIA { get; set; }
        public double? LPCLLCT { get; set; }
        public double? LECLLCT { get; set; }
        public double? LTCLLCT { get; set; }
        public double? LPCRCLT { get; set; }
        public double? LECRCLT { get; set; }
        public double? LTCRCLT { get; set; }
        
    }
}
