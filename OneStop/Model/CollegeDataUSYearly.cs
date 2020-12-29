using Newtonsoft.Json;
using System.Collections.Generic;

namespace OneStopHelper.Model
{
    public class CollegeDataUSYearly
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public CollegeDataUSYearly_Scorecard[] ScoreCard { get; set; }
        public CollegeDataUSYearly_ADM[] IPEDSADM { get; set; }
        public CollegeDataUSYearly_AL[] IPEDSAL { get; set; }
        public Dictionary<string, IPEDS_CDEP_ITEM> CDEP { get; set; } //key is CIPCODE in CDEP table
        public CollegeDataUSYearly_DRVC[] IPEDSDRVC { get; set; }
        public CollegeDataUSYearly_DRVEF[] IPEDSDRVEF { get; set; }
        public CollegeDataUSYearly_DRVGR[] IPEDSDRVGR { get; set; }
        public CollegeDataUSYearly_DRVIC[] IPEDSDRVIC { get; set; }
        public CollegeDataUSYearly_IC[] IPEDSIC { get; set; }
        public CollegeDataUSYearly_ICAY[] IPEDSICAY { get; set; }
        public CollegeDataUSYearly_SSIS[] IPEDSSSIS { get; set; }
        public CommonDatasetModel[] CommonData { get; set; }

    }
}
