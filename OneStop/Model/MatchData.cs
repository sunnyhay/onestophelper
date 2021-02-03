using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class MatchData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string STABBR { get; set; }
        public string ZIP { get; set; }
        public int STFIPS { get; set; }
        public int REGION { get; set; }
        public int LOCALE { get; set; }
        public string LAT { get; set; }
        public string LONG { get; set; }
        // public 1; nonprofit private 2; for-profit private 3
        public int Type { get; set; }
        // Doctorate 1; Master 2; Bachelor 3
        public int TopDeg { get; set; }
        // {vals: [highly recom, recom, neutral, not recom], avg: double?}
        public dynamic Gpa { get; set; }
        // {read: [int 75%, int 25%], math: [int 75%, int 25%], wrt: [int 75%, int 25%],
        // readMid: int?, mathMid: int?, wrtMid: int?, avg: int?}
        public dynamic Sat { get; set; }
        // {cum: [int 75%, int 25%], eng: [int 75%, int 25%], math: [int 75%, int 25%], wrt: [int 75%, int 25%],
        // cumMid: int?, engMid: int?, mathMid: int?, wrtMid: int?}
        public dynamic Act { get; set; }
        // admission rate
        public double? AdRate { get; set; }
        // ratio about faculty : students
        public double? FacRatio { get; set; }
        // {type: 1 (national university or 0 liberty arts college), rank: int}
        public dynamic Rank { get; set; }
        // total enrollment
        public int? ENRTOT { get; set; }
        // average net price for this college
        public double? NetPrice { get; set; }
        // Median earnings of students working and not enrolled 6 years after entry
        public double? Income { get; set; }
        /* admission factors
         * {v: [], i: [], c:[]}
         * v -> very important; i -> important; c -> considered
         * 19 elements for 0 Rigor of secondary school record; 1 class rank; 2 GPA; 3 test scores; 
         * 4 essay; 5 recommendations; 6 interview; 7 extracurricular activities; 8 talent;
         * 9 character; 10 first generation; 11 alumni; 12 geographical residence; 13 state residency;
         * 14 religion; 15 race; 16 volunteer; 17 work experience; 18 interest.
         */
        public dynamic Factors { get; set; }
    }
}
