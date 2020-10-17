using Newtonsoft.Json;

namespace OneStop.Model
{
    public class IPEDS_EFA
    {
		// not used
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? EFNRALM { get; set; }
		public double? EFNRALW { get; set; }
		public double? EFTOTLM { get; set; }
		public double? EFTOTLW { get; set; }
		public double? EFNRALT { get; set; }
		public double? EFTOTLT { get; set; }
		public double? EFAGE01 { get; set; }
		public double? EFAGE02 { get; set; }
		public double? EFAGE03 { get; set; }
		public double? EFAGE04 { get; set; }
		public double? EFAGE05 { get; set; }
		public double? EFAGE06 { get; set; }
		public double? EFAGE07 { get; set; }
		public double? EFAGE08 { get; set; }
		public double? EFAGE09 { get; set; }

	}
}
