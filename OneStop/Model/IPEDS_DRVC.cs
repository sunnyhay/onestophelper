using Newtonsoft.Json;

namespace OneStop.Model
{
    public class IPEDS_DRVC
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? DOCDEGRS { get; set; }
		public double? DOCDEGPP { get; set; }
		public double? DOCDEGOT { get; set; }
		public double? MASDEG { get; set; }
		public double? BASDEG { get; set; }
		public double? ASCDEG { get; set; }
		public double? CERT4 { get; set; }
		public double? CERT2 { get; set; }
		public double? CERT1 { get; set; }
		public double? PBACERT { get; set; }
		public double? PMACERT { get; set; }
		public double? SDOCDEG { get; set; }
		public double? SMASDEG { get; set; }
		public double? SBASDEG { get; set; }
		public double? SASCDEG { get; set; }
		public double? SBAMACRT { get; set; }
		public double? SCERT24 { get; set; }
		public double? SCERT1 { get; set; }

	}
}
