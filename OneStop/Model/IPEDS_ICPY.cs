using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class IPEDS_ICPY
    {
        // not used
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? CIPTUIT1 { get; set; }
        public double? CIPSUPP1 { get; set; }
        public double? chg1py3 { get; set; }
        public double? chg4py3 { get; set; }
        public double? chg5py3 { get; set; }
        public double? chg6py3 { get; set; }
        public double? chg7py3 { get; set; }
        public double? chg8py3 { get; set; }
        public double? chg9py3 { get; set; }
        public double? TUITION1 { get; set; }
        public double? FEE1 { get; set; }
        public double? HRCHG1 { get; set; }
        public double? TUITION2 { get; set; }
        public double? FEE2 { get; set; }
        public double? HRCHG2 { get; set; }
        public double? TUITION3 { get; set; }
        public double? FEE3 { get; set; }
        public double? HRCHG3 { get; set; }
        public double? TUITION5 { get; set; }
        public double? FEE5 { get; set; }
        public double? HRCHG5 { get; set; }
        public double? TUITION6 { get; set; }
        public double? FEE6 { get; set; }
        public double? HRCHG6 { get; set; }
        public double? TUITION7 { get; set; }
        public double? FEE7 { get; set; }
        public double? HRCHG7 { get; set; }


    }
}
