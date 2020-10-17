using Newtonsoft.Json;

namespace OneStop.Model
{
    public class IPEDS_IC
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? FT_UG { get; set; }  //24
        public double? FT_FTUG { get; set; }  //25
        public double? PT_UG { get; set; }  //27
        public double? PT_FTUG { get; set; }  //28
        public double? ROOM { get; set; }  //89
        public double? ROOMCAP { get; set; }  //90
        public double? BOARD { get; set; }  //91
        public double? ROOMAMT { get; set; }  //93
        public double? BOARDAMT { get; set; }  //94
        public double? RMBRDAMT { get; set; }  //95
        public double? APPLFEEU { get; set; }  //96
        public double? APPLFEEG { get; set; }  //97
        
    }
}
