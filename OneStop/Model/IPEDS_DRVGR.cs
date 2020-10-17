using Newtonsoft.Json;

namespace OneStop.Model
{
    public class IPEDS_DRVGR
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? GRRTTOT { get; set; }  //1
		public double? GRRTM { get; set; }  //2
		public double? GRRTW { get; set; }  //3
		public double? GRRTAN { get; set; }  //4
		public double? GRRTAP { get; set; }  //5
		public double? GRRTAS { get; set; }  //6
		public double? GRRTNH { get; set; }  //7
		public double? GRRTBK { get; set; }  //8
		public double? GRRTHS { get; set; }  //9
		public double? GRRTWH { get; set; }  //10
		public double? GRRT2M { get; set; }  //11
		public double? GRRTUN { get; set; }  //12
		public double? GRRTNR { get; set; }  //13
		public double? TRRTTOT { get; set; }  //14
		public double? GBA4RTT { get; set; }  //15
		public double? GBA5RTT { get; set; }  //16
		public double? GBA6RTT { get; set; }  //17
		public double? GBA6RTM { get; set; }  //18
		public double? GBA6RTW { get; set; }  //19
		public double? GBA6RTAN { get; set; }  //20
		public double? GBA6RTAP { get; set; }  //21
		public double? GBA6RTAS { get; set; }  //22
		public double? GBA6RTNH { get; set; }  //23
		public double? GBA6RTBK { get; set; }  //24
		public double? GBA6RTHS { get; set; }  //25
		public double? GBA6RTWH { get; set; }  //26
		public double? GBA6RT2M { get; set; }  //27
		public double? GBA6RTUN { get; set; }  //28
		public double? GBA6RTNR { get; set; }  //29
		public double? GBATRRT { get; set; }  //30
		
	}
}
