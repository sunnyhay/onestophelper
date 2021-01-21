using Newtonsoft.Json;
using System.Collections.Generic;

namespace OneStopHelper.Model
{
    public class CollegeDataUSYearly
    {
        [JsonProperty(PropertyName = "id")]
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
        public CollegeDatasetModel[] CollegeData { get; set; }
        public USNewsRankingInYearly[] Rank { get; set; }

    }
}
