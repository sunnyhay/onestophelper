using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class IPEDS_DRVEF
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public int year { get; set; }
		public double? ENRTOT { get; set; } //1
		public double? ENRFT { get; set; } //3
		public double? ENRPT { get; set; } //4
		public double? PCTENRWH { get; set; }  //5
		public double? PCTENRBK { get; set; }  //6
		public double? PCTENRHS { get; set; }  //7
		public double? PCTENRAP { get; set; }  //8
		public double? PCTENRAS { get; set; }  //9
		public double? PCTENRAN { get; set; }  //11
		public double? PCTENRUN { get; set; }  //13
		public double? PCTENRNR { get; set; }  //14
		public double? PCTENRW { get; set; }  //15
		public double? EFUGFT { get; set; }  //22
		public double? PCUENRWH { get; set; }  //32
		public double? PCUENRBK { get; set; }  //33
		public double? PCUENRHS { get; set; }  //34
		public double? PCUENRAP { get; set; }  //35
		public double? PCUENRAS { get; set; }  //36
		public double? PCUENRAN { get; set; }  //38
		public double? PCUENR2M { get; set; }  //39
		public double? PCUENRUN { get; set; }  //40
		public double? PCUENRNR { get; set; }  //41
		public double? PCUENRW { get; set; }  //42
		public double? EFGRAD { get; set; }  //43
		public double? PCGENRWH { get; set; }  //46
		public double? PCGENRBK { get; set; }  //47
		public double? PCGENRHS { get; set; }  //48
		public double? PCGENRAP { get; set; }  //49
		public double? PCGENRAN { get; set; }  //52
		public double? PCGENR2M { get; set; }  //53
		public double? PCGENRUN { get; set; }  //54
		public double? PCGENRNR { get; set; }  //55
		public double? PCGENRW { get; set; }  //56
		
	}
}
