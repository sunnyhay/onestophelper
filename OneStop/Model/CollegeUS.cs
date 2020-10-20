using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class CollegeUS
    {
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public string INSTNM { get; set; }
        public string CITY { get; set; }
        public string STABBR { get; set; }
        public string ZIP { get; set; }
        public string ST_FIPS { get; set; }
        public string REGION { get; set; }
        public string LOCALE { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string INSTURL { get; set; }
        public string NPCURL { get; set; }
        public string CONTROL { get; set; }
        public string ADMINURL { get; set; }
        public string FAIDURL { get; set; }
        public string APPLURL { get; set; }
        public string COUNTYNM { get; set; }
        public string HIGHEST_DEGREE { get; set; }

    }
}
