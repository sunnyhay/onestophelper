using Newtonsoft.Json;

namespace OneStop.Model
{
    public class IPEDS_ADM
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? APPLCN { get; set; }
        public double? APPLCNM { get; set; }
        public double? APPLCNW { get; set; }
        public double? ADMSSN { get; set; }
        public double? ADMSSNM { get; set; }
        public double? ADMSSNW { get; set; }
        public double? ENRLT { get; set; }
        public double? ENRLM { get; set; }
        public double? ENRLW { get; set; }
        public double? ENRLFT { get; set; }
        public double? ENRLFTM { get; set; }
        public double? ENRLFTW { get; set; }
        public double? ENRLPT { get; set; }
        public double? ENRLPTM { get; set; }
        public double? ENRLPTW { get; set; }
        public double? SATNUM { get; set; }
        public double? SATPCT { get; set; }
        public double? ACTNUM { get; set; }
        public double? ACTPCT { get; set; }

    }
}
