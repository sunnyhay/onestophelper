using Newtonsoft.Json;
using System.Collections.Generic;

namespace OneStopHelper.Model
{
    public class IPEDS
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UNITID { get; set; }
        public double year { get; set; }
        public double? APPLCN { get; set; }
        public double? APPLCNM { get; set; }
        public double? APPLCNW { get; set; }
        public double? ADMSSN { get; set; }
        public double? ADMSSNM { get; set; }
        public double? ADMSSNW { get; set; }
        public double? ENRLT { get; set; }
        public double? ENRLM { get; set; }
        public double? ENRLW { get; set; }
        public double? ENRLFT { get; set; }
        public double? ENRLFTM { get; set; }
        public double? ENRLFTW { get; set; }
        public double? ENRLPT { get; set; }
        public double? ENRLPTM { get; set; }
        public double? ENRLPTW { get; set; }
        public double? SATNUM { get; set; }
        public double? SATPCT { get; set; }
        public double? ACTNUM { get; set; }
        public double? ACTPCT { get; set; }
        public double? LPBOOKS { get; set; }
        public double? LEBOOKS { get; set; }
        public double? LEDATAB { get; set; }
        public double? LPMEDIA { get; set; }
        public double? LEMEDIA { get; set; }
        public double? LPCLLCT { get; set; }
        public double? LECLLCT { get; set; }
        public double? LTCLLCT { get; set; }
        public double? LPCRCLT { get; set; }
        public double? LECRCLT { get; set; }
        public double? LTCRCLT { get; set; }
        public Dictionary<string, IPEDS_CDEP_ITEM> Items { get; set; } //key is CIPCODE in CDEP table
        public double? DOCDEGRS { get; set; }
        public double? DOCDEGPP { get; set; }
        public double? DOCDEGOT { get; set; }
        public double? MASDEG { get; set; }
        public double? BASDEG { get; set; }
        public double? ASCDEG { get; set; }
        public double? CERT4 { get; set; }
        public double? CERT2 { get; set; }
        public double? CERT1 { get; set; }
        public double? PBACERT { get; set; }
        public double? PMACERT { get; set; }
        public double? SDOCDEG { get; set; }
        public double? SMASDEG { get; set; }
        public double? SBASDEG { get; set; }
        public double? SASCDEG { get; set; }
        public double? SBAMACRT { get; set; }
        public double? SCERT24 { get; set; }
        public double? SCERT1 { get; set; }
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
        public double? CINDON { get; set; }  //5
        public double? CINSON { get; set; }  //6
        public double? COTSON { get; set; }  //7
        public double? CINDOFF { get; set; }  //8
        public double? CINSOFF { get; set; }  //9
        public double? COTSOFF { get; set; }  //10
        public double? CINDFAM { get; set; }  //11
        public double? CINSFAM { get; set; }  //12
        public double? COTSFAM { get; set; }  //13
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
        public double? TUITION1 { get; set; }
        public double? FEE1 { get; set; }
        public double? HRCHG1 { get; set; }
        public double? TUITION2 { get; set; }
        public double? FEE2 { get; set; }
        public double? HRCHG2 { get; set; }
        public double? TUITION3 { get; set; }
        public double? FEE3 { get; set; }
        public double? HRCHG3 { get; set; }
        public double? TUITION5 { get; set; }
        public double? FEE5 { get; set; }
        public double? HRCHG5 { get; set; }
        public double? TUITION6 { get; set; }
        public double? FEE6 { get; set; }
        public double? HRCHG6 { get; set; }
        public double? TUITION7 { get; set; }
        public double? FEE7 { get; set; }
        public double? HRCHG7 { get; set; }
        public double? ISPROF1 { get; set; }
        public double? ISPFEE1 { get; set; }
        public double? OSPROF1 { get; set; }
        public double? OSPFEE1 { get; set; }
        public double? ISPROF2 { get; set; }
        public double? ISPFEE2 { get; set; }
        public double? OSPROF2 { get; set; }
        public double? OSPFEE2 { get; set; }
        public double? ISPROF3 { get; set; }
        public double? ISPFEE3 { get; set; }
        public double? OSPROF3 { get; set; }
        public double? OSPFEE3 { get; set; }
        public double? ISPROF4 { get; set; }
        public double? ISPFEE4 { get; set; }
        public double? OSPROF4 { get; set; }
        public double? OSPFEE4 { get; set; }
        public double? ISPROF5 { get; set; }
        public double? ISPFEE5 { get; set; }
        public double? OSPROF5 { get; set; }
        public double? OSPFEE5 { get; set; }
        public double? ISPROF6 { get; set; }
        public double? ISPFEE6 { get; set; }
        public double? OSPROF6 { get; set; }
        public double? OSPFEE6 { get; set; }
        public double? ISPROF7 { get; set; }
        public double? ISPFEE7 { get; set; }
        public double? OSPROF7 { get; set; }
        public double? OSPFEE7 { get; set; }
        public double? ISPROF8 { get; set; }
        public double? ISPFEE8 { get; set; }
        public double? OSPROF8 { get; set; }
        public double? OSPFEE8 { get; set; }
        public double? ISPROF9 { get; set; }
        public double? ISPFEE9 { get; set; }
        public double? OSPROF9 { get; set; }
        public double? OSPFEE9 { get; set; }
        public double? CHG1AT0 { get; set; }
        public double? CHG1AF0 { get; set; }
        public double? CHG1AY0 { get; set; }
        public double? CHG1AT1 { get; set; }
        public double? CHG1AF1 { get; set; }
        public double? CHG1AY1 { get; set; }
        public double? CHG1AT2 { get; set; }
        public double? CHG1AF2 { get; set; }
        public double? CHG1AY2 { get; set; }
        public double? CHG1AT3 { get; set; }
        public double? CHG1AF3 { get; set; }
        public double? CHG1AY3 { get; set; }
        public double? CHG1TGTD { get; set; }
        public double? CHG1FGTD { get; set; }
        public double? CHG2AT0 { get; set; }
        public double? CHG2AF0 { get; set; }
        public double? CHG2AY0 { get; set; }
        public double? CHG2AT1 { get; set; }
        public double? CHG2AF1 { get; set; }
        public double? CHG2AY1 { get; set; }
        public double? CHG2AT2 { get; set; }
        public double? CHG2AF2 { get; set; }
        public double? CHG2AY2 { get; set; }
        public double? CHG2AT3 { get; set; }
        public double? CHG2AF3 { get; set; }
        public double? CHG2AY3 { get; set; }
        public double? CHG2TGTD { get; set; }
        public double? CHG2FGTD { get; set; }
        public double? CHG3AT0 { get; set; }
        public double? CHG3AF0 { get; set; }
        public double? CHG3AY0 { get; set; }
        public double? CHG3AT1 { get; set; }
        public double? CHG3AF1 { get; set; }
        public double? CHG3AY1 { get; set; }
        public double? CHG3AT2 { get; set; }
        public double? CHG3AF2 { get; set; }
        public double? CHG3AY2 { get; set; }
        public double? CHG3AT3 { get; set; }
        public double? CHG3AF3 { get; set; }
        public double? CHG3AY3 { get; set; }
        public double? CHG3TGTD { get; set; }
        public double? CHG3FGTD { get; set; }
        public double? CHG4AY0 { get; set; }
        public double? CHG4AY1 { get; set; }
        public double? CHG4AY2 { get; set; }
        public double? CHG4AY3 { get; set; }
        public double? CHG5AY0 { get; set; }
        public double? CHG5AY1 { get; set; }
        public double? CHG5AY2 { get; set; }
        public double? CHG5AY3 { get; set; }
        public double? CHG6AY0 { get; set; }
        public double? CHG6AY1 { get; set; }
        public double? CHG6AY2 { get; set; }
        public double? CHG6AY3 { get; set; }
        public double? CHG7AY0 { get; set; }
        public double? CHG7AY1 { get; set; }
        public double? CHG7AY2 { get; set; }
        public double? CHG7AY3 { get; set; }
        public double? CHG8AY0 { get; set; }
        public double? CHG8AY1 { get; set; }
        public double? CHG8AY2 { get; set; }
        public double? CHG8AY3 { get; set; }
        public double? CHG9AY0 { get; set; }
        public double? CHG9AY1 { get; set; }
        public double? CHG9AY2 { get; set; }
        public double? CHG9AY3 { get; set; }
        public double? SISTOTL { get; set; }
        public double? SISPROF { get; set; }
        public double? SISASCP { get; set; }
        public double? SISASTP { get; set; }
        public double? SISINST { get; set; }
        public double? SISLECT { get; set; }

    }
}
