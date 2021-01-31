using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneStopHelper.Model
{
    public class EstimateData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public string INSTNM { get; set; }
        public double?[] GpaInterval { get; set; }
        public double? GpaAvg { get; set; }
        public double?[] SatReading { get; set; }
        public double?[] SatMath { get; set; }
        public double?[] SatWriting { get; set; }
        public double? SatReadingMid { get; set; }
        public double? SatMathMid { get; set; }
        public double? SatWritingMid { get; set; }
        public double? SatAvg { get; set; }
        public double?[] ActCumulative { get; set; }
        public double?[] ActEnglish { get; set; }
        public double?[] ActMath { get; set; }
        public double?[] ActWriting { get; set; }
        public double? ActCumulativeMid { get; set; }
        public double? ActEnglishMid { get; set; }
        public double? ActMathMid { get; set; }
        public double? ActWritingMid { get; set; }


    }
}
