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
         * 19 elements: 0 Rigor of secondary school record (Ri 3 levels);
         * 1 class rank (Cl 4 levels);
         * 2 GPA (Gp 4 levels);
         * 3 test scores (Te 3 levels);
         * 4 extracurricular activities (Ex 3 levels);
         * 5 talent (Ta 3 levels);
         * 6 first generation (Fi 2 levels);
         * 7 volunteer (Vo 3 levels);
         * 8 work experience (Wo 2 levels);
         * 9 essay; 
         * 10 recommendations; 11 interview; 12 character; 13 alumni; 14 geographical residence;
         * 15 state residency; 16 religion; 17 race; 18 interest. 
         */
        public dynamic Factors { get; set; }
    }
}
