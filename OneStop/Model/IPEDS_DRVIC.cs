﻿using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class IPEDS_DRVIC
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public int year { get; set; }
        public double? CINDON { get; set; }  //5
        public double? CINSON { get; set; }  //6
        public double? COTSON { get; set; }  //7
        public double? CINDOFF { get; set; }  //8
        public double? CINSOFF { get; set; }  //9
        public double? COTSOFF { get; set; }  //10
        public double? CINDFAM { get; set; }  //11
        public double? CINSFAM { get; set; }  //12
        public double? COTSFAM { get; set; }  //13
        
    }
}
