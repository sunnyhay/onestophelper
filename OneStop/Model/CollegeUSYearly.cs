using Newtonsoft.Json;

namespace OneStopHelper.Model
{
    public class CollegeUSYearly
    {
        // some value may be "PrivacySuppressed" and so use string as generic type for each field
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public string year { get; set; }
        public string ADM_RATE { get; set; }
        public string ADM_RATE_ALL { get; set; }
        public string SATVR25 { get; set; }
        public string SATVR75 { get; set; }
        public string SATMT25 { get; set; }
        public string SATMT75 { get; set; }
        public string SATWR25 { get; set; }
        public string SATWR75 { get; set; }
        public string SATVRMID { get; set; }
        public string SATMTMID { get; set; }
        public string SATWRMID { get; set; }
        public string ACTCM25 { get; set; }
        public string ACTCM75 { get; set; }
        public string ACTEN25 { get; set; }
        public string ACTEN75 { get; set; }
        public string ACTMT25 { get; set; }
        public string ACTMT75 { get; set; }
        public string ACTWR25 { get; set; }
        public string ACTWR75 { get; set; }
        public string ACTCMMID { get; set; }
        public string ACTENMID { get; set; }
        public string ACTMTMID { get; set; }
        public string ACTWRMID { get; set; }
        public string SAT_AVG { get; set; }
        public string UGDS { get; set; }
        public string UGDS_WHITE { get; set; }
        public string UGDS_BLACK { get; set; }
        public string UGDS_HISP { get; set; }
        public string UGDS_ASIAN { get; set; }
        public string UGDS_AIAN { get; set; }
        public string UGDS_NHPI { get; set; }
        public string UGDS_2MOR { get; set; }
        public string UGDS_NRA { get; set; }
        public string UGDS_UNKN { get; set; }
        public string UGDS_WHITENH { get; set; }
        public string UGDS_BLACKNH { get; set; }
        public string UGDS_API { get; set; }
        public string NPT4_PUB { get; set; }
        public string NPT4_PRIV { get; set; }
        public string NPT41_PUB { get; set; }
        public string NPT42_PUB { get; set; }
        public string NPT43_PUB { get; set; }
        public string NPT44_PUB { get; set; }
        public string NPT45_PUB { get; set; }
        public string NPT41_PRIV { get; set; }
        public string NPT42_PRIV { get; set; }
        public string NPT43_PRIV { get; set; }
        public string NPT44_PRIV { get; set; }
        public string NPT45_PRIV { get; set; }
        public string NUM4_PUB { get; set; }
        public string NUM4_PRIV { get; set; }
        public string NUM41_PUB { get; set; }
        public string NUM42_PUB { get; set; }
        public string NUM43_PUB { get; set; }
        public string NUM44_PUB { get; set; }
        public string NUM45_PUB { get; set; }
        public string NUM41_PRIV { get; set; }
        public string NUM42_PRIV { get; set; }
        public string NUM43_PRIV { get; set; }
        public string NUM44_PRIV { get; set; }
        public string NUM45_PRIV { get; set; }
        public string COSTT4_A { get; set; }
        public string COSTT4_P { get; set; }
        public string TUITIONFEE_IN { get; set; }
        public string TUITIONFEE_OUT { get; set; }
        public string FEMALE { get; set; }
        public string MARRIED { get; set; }
        public string DEPENDENT { get; set; }
        public string VETERAN { get; set; }
        public string FIRST_GEN { get; set; }
        public string FAMINC { get; set; }
        public string MD_FAMINC { get; set; }
        public string MEDIAN_HH_INC { get; set; }
        public string POVERTY_RATE { get; set; }
        public string UNEMP_RATE { get; set; }
        public string GRAD_DEBT_MDN_SUPP { get; set; }
        public string UGDS_MEN { get; set; }
        public string UGDS_WOMEN { get; set; }
        public string GRADS { get; set; }
        public string COUNT_NWNE_P10 { get; set; }
        public string COUNT_WNE_P10 { get; set; }
        public string MN_EARN_WNE_P10 { get; set; }
        public string MD_EARN_WNE_P10 { get; set; }
        public string COUNT_NWNE_P6 { get; set; }
        public string COUNT_WNE_P6 { get; set; }
        public string MN_EARN_WNE_P6 { get; set; }
        public string MD_EARN_WNE_P6 { get; set; }
        public string COUNT_NWNE_P8 { get; set; }
        public string COUNT_WNE_P8 { get; set; }
        public string MN_EARN_WNE_P8 { get; set; }
        public string MD_EARN_WNE_P8 { get; set; }

    }
}
