using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System.IO;
using ExcelDataReader;
using OneStopHelper.Model;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Cosmos.Linq;

namespace OneStopHelper
{
    public class SatInput
    {
        public int Avg { get; set; }
        public int Read { get; set; }
        public int Math { get; set; }
        public int Wrt { get; set; }
    }
    public class ActInput
    {
        public int Cum { get; set; }
        public int Eng { get; set; }
        public int Math { get; set; }
        public int Wrt { get; set; }
    }
    /*
     * {
     *   gpa,
     *   sat: {avg, read, math, wrt},
     *   act: {cum, eng, math, wrt},
     *   rank
     * }
     */
    public class UserInput
    {
        public double Gpa { get; set; }
        public SatInput Sat { get; set; }
        public ActInput Act { get; set; }
        public int Rank { get; set; }
    }
    
    public class ComprehensiveInput
    {
        public int Ri { get; set; }
        public int Cl { get; set; }
        public int Gp { get; set; }
        public int Te { get; set; }
        public int Ex { get; set; }
        public int Ta { get; set; }
        public int Fi { get; set; }
        public int Vo { get; set; }
        public int Wo { get; set; }

    }
    public class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://localhost:8081";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private readonly string databaseId = "OneStop";
        private readonly string containerId = "CollegeDataUS";
        private readonly string rootPath = "c:/Users/Administrator/Documents/";
        private static readonly string commonDatasetFilePath = "c:/Users/Administrator/Documents/CommonDataset-University";
        private static readonly string commonDatasetCollegeFilePath = "c:/Users/Administrator/Documents/CommonDataset-College";
        private static readonly string collegeDatasetFilePath = "c:/Users/Administrator/Documents/CollegeData-University";
        private static readonly string collegeDatasetCollegeFilePath = "c:/Users/Administrator/Documents/CollegeData-College";
        private readonly int CURRENTYEAR = 2019;

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.StartOperationAsync();

                //await p.AddBasicCollegeData();
                //await p.UpdateCollegeDataWithIPEDSHD();
                //await p.AddScorecardYearlyData();

                //await p.AddIPEDSYearlyDataforADM("IPEDSADM");
                //await p.AddIPEDSYearlyDataforAL("IPEDSAL");
                //await p.AddIPEDSCIPCODE("IPEDSCIPCODE2020");
                //await p.AddIPEDSYearlyDataforCDEP("IPEDSCDEP");
                //await p.AddIPEDSYearlyDataforDRVC("IPEDSDRVC");
                //await p.AddIPEDSYearlyDataforDRVEF("IPEDSDRVEF");
                //await p.AddIPEDSYearlyDataforDRVGR("IPEDSDRVGR");
                //await p.AddIPEDSYearlyDataforDRVIC("IPEDSDRVIC");
                //await p.AddIPEDSYearlyDataforIC("IPEDSIC");
                //await p.AddIPEDSYearlyDataforICAY("IPEDSICAY");
                //await p.AddIPEDSYearlyDataforSSIS("IPEDSSSIS");
                //await p.UpdateYearlyData("IPEDSSSIS");
                //await p.ValidateRankingData();
                //await p.UpdateCommonDataset(commonDatasetCollegeFilePath);
                //await p.UpdateCollegeDataset(collegeDatasetCollegeFilePath);
                //await p.UpdateYearlDataWithCommonDataset();
                //await p.UpdateYearlDataWithCollegeDataset();
                //await p.UpdateYearlDataWithUSNewsRanking();
                //await p.UpdateYearlDataWithCollegeInfo();
                //p.ValidateCommonDatasetFile();
                //p.Estimate();
                //await p.AddMatchDataset();
                //await p.SplitColleges();
                await p.SearchAcademicScoreUDF();
                //await p.SearchComprehensiveScoreUDF();
                //await p.TryLinq();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        public async Task TryLinq()
        {
            UserInput model = new UserInput
            {
                Gpa = 3.6,
                Sat = new SatInput
                {
                    Avg = 1501,
                    Read = 640,
                    Math = 680,
                    Wrt = 660
                },
                Act = new ActInput
                {
                    Cum = 31,
                    Eng = 30,
                    Math = 31,
                    Wrt = 31
                },
                Rank = 25
            };
            var userInput = JsonConvert.SerializeObject(model);
            var srcContainer = database.GetContainer("MatchData");
            using FeedIterator<MatchData> setIterator = srcContainer.GetItemLinqQueryable<MatchData>()
                .Where(d => (int)CosmosLinq.InvokeUserDefinedFunction("AcademicMatch", new object[] { d, userInput }) == 4)
                .ToFeedIterator();
            //using FeedIterator<MatchData> setIterator = srcContainer.GetItemLinqQueryable<MatchData>()
            //          .Where(b => b.UNITID == "100751" || b.UNITID == "100663" || b.UNITID == "100937")
            //          .OrderBy(e => e.Rank)
            //          .ToFeedIterator();
            int count = 0;
            while (setIterator.HasMoreResults)
            {
                foreach (var item in await setIterator.ReadNextAsync())
                {
                    Console.WriteLine("college name: " + item.Name + " and rank: " + item.Rank);
                    count++;
                }
            }
            Console.WriteLine("Totally " + count + " colleges!");
        }

        public async Task SearchAcademicScoreUDF()
        {
            UserInput model = new UserInput
            {
                Gpa = 3.6,
                Sat = new SatInput
                {
                    Avg = 1501,
                    Read = 640,
                    Math = 680,
                    Wrt = 660
                },
                Act = new ActInput
                {
                    Cum = 31,
                    Eng = 30,
                    Math = 31,
                    Wrt = 31
                },
                Rank = 25
            };
            var userInput = JsonConvert.SerializeObject(model);
            var srcContainer = database.GetContainer("MatchData");
            var query = $"select * from c where udf.AcademicMatch(c, {userInput}) = 4 and c.RankType = 1 order by c.Rank offset 0 limit 20";
            var iterator = srcContainer.GetItemQueryIterator<MatchData>(query);
            int count = 0;
            List<string> ids = new List<string>();
            while (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync();
                foreach (var item in results)
                {
                    count++;
                    ids.Add(item.UNITID);
                    Console.WriteLine($"{item.Name} qualifies with UNITID {item.UNITID} with its rank {item.Rank}!");
                }
            }
            Console.WriteLine($"Total {count} colleges qualify current user input!");
            var insideStr = "";
            foreach(var id in ids)
            {
                insideStr += "\"" + id + "\",";
            }
            var query1 = $"select * from c where c.UNITID in ({insideStr.Substring(0, insideStr.Length-1)})";
            Console.WriteLine("new query: " + query1);
            var dstContainer = database.GetContainer("CollegeDataUSYearly");
            var iterator1 = dstContainer.GetItemQueryIterator<CollegeDataUSYearly>(query1);
            while (iterator1.HasMoreResults)
            {
                var results = await iterator1.ReadNextAsync();
                foreach(var item in results)
                {
                    Console.WriteLine($"{item.INSTNM} is shown here with its city {item.CITY}");
                }
            }
        }
        public async Task SearchComprehensiveScoreUDF()
        {
            ComprehensiveInput model = new ComprehensiveInput
            {
                Ri = 3, Cl = 3, Gp = 4, Te = 3, Ex = 2, Ta = 2, Fi = 1, Vo = 2, Wo = 1
            };
            var userInput = JsonConvert.SerializeObject(model);
            var srcContainer = database.GetContainer("MatchData");
            var query = $"select * from c where udf.ComprehensiveMatch(c, {userInput}) = 4";
            var iterator = srcContainer.GetItemQueryIterator<MatchData>(query);
            int count = 0;
            while (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync();
                foreach (var item in results)
                {
                    count++;
                    Console.WriteLine($"{item.Name} qualifies with UNITID {item.UNITID}!");
                }
            }
            Console.WriteLine($"Total {count} colleges qualify current comprehensive input!");
        }

        // try estimate of GPA, ranking and SAT/ACT
        public void Estimate()
        {
            double?[] input = new double?[]
            {
                58.18, 30.86, 5.93, 3.3, 1.43, 0.22, 0, 0.08, 0
            };
            double?[] input1 = new double?[]
            {
                36.9,20.2,18.6,10.3,8.1,5.6,0.3,0,0
            };
            double?[] input2 = new double?[]
            {
                null,23,16,13,15,25,8,0,0
            };
            double?[] input3 = new double?[]
            {
                70.55, 22.26, 4.94, 1.67, 0.42, 0.08, 0.08, 0, 0
            };
            

            var output = Utils.EstimateFromGpa(input3);
            foreach(var item in output.Children())
            {
                Console.WriteLine(item.ToString());
            }
        }
        // split the entire data into two parts: standard university/college and communit college
        // based on this condition:
        // 1. the college's highest degree is under bachelor, i.e. Associate or None
        // 2. name contains "Community College"

        // then continue to split another round of colleges by
        // 3. no common dataset or college data and sat/act are all none
        public async Task SplitColleges()
        {
            var targetContainer = database.GetContainer("SecondaryColleges");
            var srcContainer = database.GetContainer("CollegeDataUSYearly");
            // iterate each college in CollegeDataUSYearly and move community college to OtherColleges
            // iterate again to move all other colleges to SecondaryColleges
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = srcContainer.GetItemQueryIterator<CollegeDataUSYearly>(queryDefinition);
            int count = 0;
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                foreach (var item in res)
                {
                    var unitid = item.UNITID;
                    var name = item.INSTNM;
                    var commondataset = item.CommonData;
                    var collegedata = item.CollegeData;

                    var satvr25 = item.ScoreCard[0].SATVR25;
                    var satvr75 = item.ScoreCard[0].SATVR75;
                    var satmt25 = item.ScoreCard[0].SATMT25;
                    var satmt75 = item.ScoreCard[0].SATMT75;
                    var satwr25 = item.ScoreCard[0].SATWR25;
                    var satwr75 = item.ScoreCard[0].SATWR75;
                    var satvrmid = item.ScoreCard[0].SATVRMID;
                    var satmtmid = item.ScoreCard[0].SATMTMID;
                    var satwrmid = item.ScoreCard[0].SATWRMID;
                    var actcm25 = item.ScoreCard[0].ACTCM25;
                    var actcm75 = item.ScoreCard[0].ACTCM75;
                    var acten25 = item.ScoreCard[0].ACTEN25;
                    var acten75 = item.ScoreCard[0].ACTEN75;
                    var actmt25 = item.ScoreCard[0].ACTMT25;
                    var actmt75 = item.ScoreCard[0].ACTMT75;
                    var actwr25 = item.ScoreCard[0].ACTWR25;
                    var actwr75 = item.ScoreCard[0].ACTWR75;
                    var actcmmid = item.ScoreCard[0].ACTCMMID;
                    var actenmid = item.ScoreCard[0].ACTENMID;
                    var actmtmid = item.ScoreCard[0].ACTMTMID;
                    var actwrmid = item.ScoreCard[0].ACTWRMID;
                    var satavg = item.ScoreCard[0].SAT_AVG;

                    try
                    {
                        //if (name.Contains("Community College") || highestDegree == "Associate" || highestDegree == "None")
                        //{
                        //    //ItemResponse<CollegeDataUSYearly> res1 = await targetContainer.CreateItemAsync(item, new PartitionKey(unitid));
                        //    //Console.WriteLine("Created entry in database with id: {0} Operation consumed {1} RUs.\n", res1.Resource.Id, res1.RequestCharge);
                        //    //ItemResponse<CollegeDataUSYearly> res2 = await srcContainer.DeleteItemAsync<CollegeDataUSYearly>(item.Id, new PartitionKey(unitid));
                        //    //Console.WriteLine($"Removed {unitid}... from yearly data!");
                        //    //Console.WriteLine("Deleted entry in database with id: {0} Operation consumed {1} RUs.\n", res2.Resource.Id, res2.RequestCharge);
                        //    count++;
                        //}
                        if (commondataset == null && collegedata == null &&
                            satvr25 == null && satvr75 == null && satvrmid == null &&
                            satmt25 == null && satmt75 == null && satmtmid == null &&
                            satwr25 == null && satwr75 == null && satwrmid == null &&
                            actcm25 == null && actcm75 == null && actcmmid == null &&
                            acten25 == null && acten75 == null && actenmid == null &&
                            actmt25 == null && actmt75 == null && actmtmid == null &&
                            actwr25 == null && actwr75 == null && actwrmid == null &&
                            satavg == null
                            )
                        {
                            Console.WriteLine($"This college belongs to secondary part: {name}");
                            //ItemResponse<CollegeDataUSYearly> res1 = await targetContainer.CreateItemAsync(item, new PartitionKey(unitid));
                            //Console.WriteLine("Created entry in database with id: {0} Operation consumed {1} RUs.\n", res1.Resource.Id, res1.RequestCharge);
                            ItemResponse<CollegeDataUSYearly> res2 = await srcContainer.DeleteItemAsync<CollegeDataUSYearly>(item.Id, new PartitionKey(unitid));
                            Console.WriteLine($"Removed {unitid}... from yearly data!");
                            count++;
                        }
                    }
                    catch (CosmosException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                
            }
            Console.WriteLine($"Totally {count} colleges have been moved!");
        }

        public async Task AddMatchDataset()
        {
            var targetContainer = database.GetContainer("MatchData");
            var srcContainer = database.GetContainer("CollegeDataUSYearly");
            // iterate each college in CollegeDataUSYearly and add a new MatchData entry accordingly
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = srcContainer.GetItemQueryIterator<CollegeDataUSYearly>(queryDefinition);
            var path = "Text.json";
            using StreamWriter sw = File.AppendText(path);
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                foreach (var item in res)
                {

                    var unitid = item.UNITID;
                    Console.WriteLine($"Found this UNITID {unitid}");

                    MatchData entry = new MatchData
                    {
                        Id = unitid,
                        UNITID = unitid,
                        Name = item.INSTNM,
                        City = item.CITY,
                        STABBR = item.STABBR,
                        ZIP = item.ZIP,
                        STFIPS = int.Parse(item.ST_FIPS),
                        REGION = int.Parse(item.REGION),
                        LOCALE = int.Parse(item.LOCALE),
                        LAT = item.LATITUDE,
                        LONG = item.LONGITUDE,
                        Type = int.Parse(item.CONTROL),
                        AdRate = item.ScoreCard[0].ADM_RATE == null ? item.ScoreCard[0].ADM_RATE_ALL : item.ScoreCard[0].ADM_RATE,
                        Income = item.ScoreCard[0].MD_EARN_WNE_P6
                    };

                    if (item.HIGHEST_DEGREE == "Doctor")
                    {
                        entry.TopDeg = 1;
                    }
                    else if (item.HIGHEST_DEGREE == "Master")
                    {
                        entry.TopDeg = 2;
                    }
                    else
                    {
                        entry.TopDeg = 3;
                    }

                    if (item.ScoreCard[0].SATVR75 != null)
                    {
                        entry.SatReadHigh = Convert.ToInt32(item.ScoreCard[0].SATVR75);
                    }

                    if (item.ScoreCard[0].SATVR25 != null)
                    {
                        entry.SatReadLow = Convert.ToInt32(item.ScoreCard[0].SATVR25);
                    }

                    if (item.ScoreCard[0].SATMT75 != null)
                    {
                        entry.SatMathHigh = Convert.ToInt32(item.ScoreCard[0].SATMT75);
                    }

                    if (item.ScoreCard[0].SATMT25 != null)
                    {
                        entry.SatMathLow = Convert.ToInt32(item.ScoreCard[0].SATMT25);
                    }

                    if (item.ScoreCard[0].SATWR75 != null)
                    {
                        entry.SatWrtHigh = Convert.ToInt32(item.ScoreCard[0].SATWR75);
                    }

                    if (item.ScoreCard[0].SATWR25 != null)
                    {
                        entry.SatWrtLow = Convert.ToInt32(item.ScoreCard[0].SATWR25);
                    }

                    if (item.ScoreCard[0].SATVRMID != null)
                        entry.SatReadMid = Convert.ToInt32(item.ScoreCard[0].SATVRMID);
                    if (item.ScoreCard[0].SATMTMID != null)
                        entry.SatMathMid = Convert.ToInt32(item.ScoreCard[0].SATMTMID);
                    if (item.ScoreCard[0].SATWRMID != null)
                        entry.SatWrtMid = Convert.ToInt32(item.ScoreCard[0].SATWRMID);
                    if (item.ScoreCard[0].SAT_AVG != null)
                        entry.SatAvg = Convert.ToInt32(item.ScoreCard[0].SAT_AVG);
                    
                    if (item.ScoreCard[0].ACTCM75 != null)
                    {
                        entry.ActCumHigh = Convert.ToInt32(item.ScoreCard[0].ACTCM75);
                    }

                    if (item.ScoreCard[0].ACTCM25 != null)
                    {
                        entry.ActCumLow = Convert.ToInt32(item.ScoreCard[0].ACTCM25);
                    }
                    
                    if (item.ScoreCard[0].ACTEN75 != null)
                    {
                        entry.ActEngHigh = Convert.ToInt32(item.ScoreCard[0].ACTEN75);
                    }
                    
                    if (item.ScoreCard[0].ACTEN25 != null)
                    {
                        entry.ActEngLow = Convert.ToInt32(item.ScoreCard[0].ACTEN25);
                    }
                    
                    if (item.ScoreCard[0].ACTMT75 != null)
                    {
                        entry.ActMathHigh = Convert.ToInt32(item.ScoreCard[0].ACTMT75);
                    }
                    
                    if (item.ScoreCard[0].ACTMT25 != null)
                    {
                        entry.ActMathLow = Convert.ToInt32(item.ScoreCard[0].ACTMT25);
                    }
                    
                    if (item.ScoreCard[0].ACTWR75 != null)
                    {
                        entry.ActWrtHigh = Convert.ToInt32(item.ScoreCard[0].ACTWR75);
                    }
                    
                    if (item.ScoreCard[0].ACTWR25 != null)
                    {
                        entry.ActWrtLow = Convert.ToInt32(item.ScoreCard[0].ACTWR25);
                    }

                    if (item.ScoreCard[0].ACTCMMID != null)
                        entry.ActCumMid = Convert.ToInt32(item.ScoreCard[0].ACTCMMID);
                    if (item.ScoreCard[0].ACTENMID != null)
                        entry.ActEngMid = Convert.ToInt32(item.ScoreCard[0].ACTENMID);
                    if (item.ScoreCard[0].ACTMTMID != null)
                        entry.ActMathMid = Convert.ToInt32(item.ScoreCard[0].ACTMTMID);
                    if (item.ScoreCard[0].ACTWRMID != null)
                        entry.ActWrtMid = Convert.ToInt32(item.ScoreCard[0].ACTWRMID);
                    
                    if (item.IPEDSDRVEF != null && item.IPEDSDRVEF[0].ENRTOT != null)
                        entry.ENRTOT = Convert.ToInt32(item.IPEDSDRVEF[0].ENRTOT);

                    if (item.ScoreCard[0].NPT4_PUB != null)
                        entry.NetPrice = item.ScoreCard[0].NPT4_PUB;
                    else
                        entry.NetPrice = item.ScoreCard[0].NPT4_PRIV;

                    if (item.Rank != null)
                    {
                        entry.Rank = item.Rank[0].Rank;
                        entry.RankType = item.Rank[0].Category == "Public University" ? 1 : 0;
                    }

                    if (item.IPEDSSSIS != null && item.IPEDSSSIS[0].SISTOTL != null
                        && entry.ENRTOT != null)
                    {
                        entry.FacRatio = (double?)Math.Round((decimal)(entry.ENRTOT / item.IPEDSSSIS[0].SISTOTL), 2);
                    }


                    if (item.CommonData != null
                        && item.CommonData[0] != null
                        && item.CommonData[0].Gpa != null
                        && item.CommonData[0].Gpa["p375"] != null
                        && item.CommonData[0].Gpa["p35"] != null)
                    {
                        dynamic Gpa = new JObject();
                        var gpa = item.CommonData[0].Gpa;
                        var input = new double?[] {
                                gpa["p4"], gpa["p375"], gpa["p35"],gpa["p325"],gpa["p3"],
                                gpa["p25"],gpa["p2"],gpa["p1"],gpa["pBelow1"],
                            };
                        // change JArray as output from this method!!!
                        var output = Utils.EstimateFromGpa(input);
                        //Console.WriteLine(output[0]);
                        Gpa.vals = output;
                        Gpa.avg = gpa["avg"];
                        entry.Gpa = Gpa;
                        entry.GpaAvg = gpa["avg"];
                    }

                    if (item.CollegeData != null
                        && item.CollegeData[0] != null
                        && item.CollegeData[0].Gpa != null
                        && item.CollegeData[0].Gpa["p375"] != null
                        && item.CollegeData[0].Gpa["p35"] != null)
                    {
                        dynamic Gpa = new JObject();
                        var gpa = item.CollegeData[0].Gpa;
                        var input = new double?[] {
                                gpa["p4"], gpa["p375"], gpa["p35"],gpa["p325"],gpa["p3"],
                                gpa["p25"],gpa["p2"],gpa["p1"],gpa["pBelow1"],
                            };
                        var output = Utils.EstimateFromGpa(input);
                        //Console.WriteLine(output[0]);
                        Gpa.vals = output;
                        Gpa.avg = gpa["avg"];
                        entry.Gpa = Gpa;
                        entry.GpaAvg = gpa["avg"];
                    }

                    if (item.CommonData != null && item.CommonData[0].AdmissionDecision != null)
                    {
                        dynamic Factors = new JObject();
                        var vSet = item.CommonData[0].AdmissionDecision["veryImportant"];
                        var vSetList = new List<string>();
                        foreach (var elem in vSet)
                        {
                            vSetList.Add((string)elem);
                        }
                        var iSet = item.CommonData[0].AdmissionDecision["important"];
                        var iSetList = new List<string>();
                        foreach (var elem in iSet)
                        {
                            iSetList.Add((string)elem);
                        }
                        var cSet = item.CommonData[0].AdmissionDecision["considered"];
                        var cSetList = new List<string>();
                        foreach (var elem in cSet)
                        {
                            cSetList.Add((string)elem);
                        }
                        if (vSetList.Count > 0)
                        {
                            var arr = new JArray();
                            foreach (var elem in vSetList)
                            {
                                arr.Add(new JValue(GetFactorBack(elem)));
                            }
                            Factors.v = arr;
                        }
                        if (iSetList.Count > 0)
                        {
                            var arr = new JArray();
                            foreach (var elem in iSetList)
                            {
                                arr.Add(new JValue(GetFactorBack(elem)));
                            }
                            Factors.i = arr;
                        }
                        if (cSetList.Count > 0)
                        {
                            var arr = new JArray();
                            foreach (var elem in cSetList)
                            {
                                arr.Add(new JValue(GetFactorBack(elem)));
                            }
                            Factors.c = arr;
                        }
                        entry.Factors = Factors;
                    }

                    if (item.CollegeData != null && item.CollegeData[0].AdmissionDecision != null)
                    {
                        dynamic Factors = new JObject();
                        var vSet = item.CollegeData[0].AdmissionDecision["veryImportant"];
                        var vSetList = new List<string>();
                        foreach (var elem in vSet)
                        {
                            vSetList.Add((string)elem);
                        }
                        var iSet = item.CollegeData[0].AdmissionDecision["important"];
                        var iSetList = new List<string>();
                        foreach (var elem in iSet)
                        {
                            iSetList.Add((string)elem);
                        }
                        var cSet = item.CollegeData[0].AdmissionDecision["considered"];
                        var cSetList = new List<string>();
                        foreach (var elem in cSet)
                        {
                            cSetList.Add((string)elem);
                        }
                        if (vSetList.Count > 0)
                        {
                            var arr = new JArray();
                            foreach (var elem in vSetList)
                            {
                                arr.Add(new JValue(GetFactorBack(elem)));
                            }
                            Factors.v = arr;
                        }
                        if (iSetList.Count > 0)
                        {
                            var arr = new JArray();
                            foreach (var elem in iSetList)
                            {
                                arr.Add(new JValue(GetFactorBack(elem)));
                            }
                            Factors.i = arr;
                        }
                        if (cSetList.Count > 0)
                        {
                            var arr = new JArray();
                            foreach (var elem in cSetList)
                            {
                                arr.Add(new JValue(GetFactorBack(elem)));
                            }
                            Factors.c = arr;
                        }
                        entry.Factors = Factors;
                    }

                    try
                    {
                        //ItemResponse<MatchData> res1 = await targetContainer.CreateItemAsync(entry, new PartitionKey(unitid));
                        //Console.WriteLine("Created entry in database with id: {0} Operation consumed {1} RUs.\n", res1.Resource.Id, res1.RequestCharge);
                        // write them into a file to see how much the cache data is
                        var jsonStr = JsonConvert.SerializeObject(entry);
                        //Console.WriteLine(jsonStr);
                        sw.WriteLine(jsonStr);
                    }
                    catch (CosmosException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    //if (unitid == "102270")
                    //{
                        
                    //}

                }
            }
        }
        private static int GetFactorBack(string factor)
        {
            /* admission factors
            * 19 elements: 
            * 0 Rigor of secondary school record (Ri 3 levels);
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
            switch (factor)
            {
                case "Rigor of secondary school record":
                    return 0;
                case "Class rank":
                    return 1;
                case "Academic GPA":
                    return 2;
                case "Standardized test scores":
                    return 3;
                case "Application Essay":
                    return 9;
                case "Recommendation(s)":
                    return 10;
                case "Interview":
                    return 11;
                case "Extracurricular activities":
                    return 4;
                case "Talent/ability":
                    return 5;
                case "Character/personal qualities":
                    return 12;
                case "First generation":
                    return 6;
                case "Alumni/ae relation":
                    return 13;
                case "Geographical residence":
                    return 14;
                case "State residency":
                    return 15;
                case "Religious affiliation/commitment":
                    return 16;
                case "Racial/ethnic status":
                    return 17;
                case "Volunteer work":
                    return 7;
                case "Work experience":
                    return 8;
                case "Level of applicant’s interest":
                    return 18;
                default:
                    return -1;

            }
        }

        /// <summary>
        /// Add the basic college data without yearly info from College ScoreCard dataset
        /// </summary>
        /// <returns></returns>
        public async Task AddBasicCollegeData()
        {
            using StreamReader sr = new StreamReader($"{rootPath}Most-Recent-Cohorts-All-Data-Elements.csv");
            var num = 0;
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while ((currentLine = sr.ReadLine()) != null)
            {
                num++;
                if (num == 1)
                    continue;
                var vals = currentLine.Split(',');
                var unitid = vals[0];
                var instnm = vals[3];
                var city = vals[4];
                var stabbr = vals[5];
                var zip = vals[6];
                var insturl = vals[8];
                var npcurl = vals[9];
                var control = vals[16];
                var st_fips = vals[17];
                var region = vals[18];
                var locale = vals[19];
                var latitude = vals[21];
                var longitude = vals[22];
                CollegeDataUS college = new CollegeDataUS
                {
                    Id = (num - 1).ToString(),
                    UNITID = unitid,
                    INSTNM = instnm,
                    CITY = city,
                    STABBR = stabbr,
                    ZIP = zip,
                    INSTURL = insturl,
                    NPCURL = npcurl,
                    CONTROL = control,
                    ST_FIPS = st_fips,
                    REGION = region,
                    LOCALE = locale,
                    LATITUDE = latitude,
                    LONGITUDE = longitude
                };
                if (num % 100 == 0)
                    Console.WriteLine("Working on line number: " + (num - 1));
                //Console.WriteLine("unitid: " + unitid + " for num: " + num);
                await AddItemsToContainerAsync(college);
            }
        }

        // update the basic college data with IPEDS HD table content
        public async Task UpdateCollegeDataWithIPEDSHD()
        {
            var filePath = $"{rootPath}HD2018.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    var adminurl = reader.GetString(17);
                    var faidurl = reader.GetString(18);
                    var applurl = reader.GetString(19);
                    var hdegofr1 = reader.GetDouble(30);
                    var degree = GetDegree(hdegofr1 + "");
                    var countynm = reader.GetString(67);
                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    if (idNum % 100 == 0)
                        Console.WriteLine("Working on line num: " + idNum);

                    if (idNum < 30000)
                    {
                        //Console.WriteLine($"unitid: {unitid} with adminurl {adminurl}");
                        var partitionKey = new PartitionKey(unitid);
                        //ItemResponse<CollegeUS> res = await container.ReadItemAsync<CollegeUS>(id, partitionKey);
                        CollegeDataUS college = await QueryItemsAsync(unitid);
                        if (college == null)
                        {
                            continue;
                        }
                        //Console.WriteLine("Working on college: " + college.INSTNM);
                        college.ADMINURL = adminurl;
                        college.FAIDURL = faidurl;
                        college.APPLURL = applurl;
                        college.COUNTYNM = countynm;
                        college.HIGHEST_DEGREE = degree;
                        var res = await container.ReplaceItemAsync(college, college.Id, partitionKey);
                        //Console.WriteLine("Updated college: " + college.INSTNM);
                        //Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSCIPCODE(string containerId)
        {
            var filePath = $"{rootPath}CIPCode2020.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var cipCode = reader.GetString(1);
                    var cipTitle = reader.GetString(4);
                    var cipDefinition = reader.GetString(5);

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(cipCode);
                        IPEDS_CIPCODE2020 item = new IPEDS_CIPCODE2020
                        {
                            Id = id,
                            CIPCode = cipCode,
                            CIPTitle = cipTitle,
                            CIPDefinition = cipDefinition
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            ItemResponse<IPEDS_CIPCODE2020> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.CIPCode));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task UpdateRankingData()
        {
            var mainContainer = database.GetContainer("USNewsRanking");
            var res = await mainContainer.ReadItemAsync<USNewsRanking>("1", new PartitionKey("2021"));
            var item = res.Resource;
            Console.WriteLine("id: " + item.Id + " with year: " + item.year);
            var universities = item.Universities;
            var libertyColleges = item.LibertyColleges;

            for (int i = 0; i < universities.Count; i++)
            {
                var university = universities[i];
                var univName = university.INSTNM;
                var condition = $"c.INSTNM = \"{univName}\"";
                var targetItem = await QueryTableCustomConditionAsync<CollegeDataUS>("CollegeDataUS", condition);
                if (targetItem == default)
                {
                    Console.WriteLine($"Not found the university UNITID for {univName} with rank {university.Rank}");
                    university.UNITID = "UNKNOWN";
                }
                else
                {
                    Console.WriteLine($"Found the university UNITID {targetItem.UNITID} for {univName} with rank {university.Rank}");
                    university.UNITID = targetItem.UNITID;
                }
            }
            for (int i = 0; i < libertyColleges.Count; i++)
            {
                var libertyCollege = libertyColleges[i];
                var collegeName = libertyCollege.INSTNM;
                var condition = $"c.INSTNM = \"{collegeName}\"";
                var targetItem = await QueryTableCustomConditionAsync<CollegeDataUS>("CollegeDataUS", condition);
                if (targetItem == default)
                {
                    Console.WriteLine($"Not found the college UNITID for {collegeName} with rank {libertyCollege.Rank}");
                    libertyCollege.UNITID = "UNKNOWN";
                }
                else
                {
                    Console.WriteLine($"Found the college UNITID {targetItem.UNITID} for {collegeName} with rank {libertyCollege.Rank}");
                    libertyCollege.UNITID = targetItem.UNITID;
                }
            }
            ItemResponse<USNewsRanking> result = await mainContainer.UpsertItemAsync(item, new PartitionKey("2021"));
            Console.WriteLine($"Updated item in database with id: {0} Operation consumed {1} RUs.\n", result.Resource.Id, result.RequestCharge);

        }
        public async Task ValidateRankingData()
        {
            var mainContainer = database.GetContainer("USNewsRanking");
            var res = await mainContainer.ReadItemAsync<USNewsRanking>("1", new PartitionKey("2021"));
            var item = res.Resource;
            Console.WriteLine("id: " + item.Id + " with year: " + item.year);
            var universities = item.Universities;
            var libertyColleges = item.LibertyColleges;
            var list = universities.Concat(libertyColleges).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                var college = list[i];
                for (int j = i + 1; j < list.Count; j++)
                {
                    var target = list[j];
                    if (college.UNITID.Equals(target.UNITID))
                    {
                        Console.WriteLine("Found duplicate UNITID: " + target.UNITID);
                    }
                }
            }
        }

        public void ValidateCommonDatasetFile()
        {
            int count = 0;
            Dictionary<string, string> map = new Dictionary<string, string>();
            ISet<string> unitids = new HashSet<string>();

            // iteration 1 for empty UNITID or college name
            foreach (string filename in Directory.EnumerateFiles(commonDatasetFilePath, "*.json"))
            {
                count++;
                //Console.WriteLine("File name: " + filename);
                using StreamReader file = File.OpenText(filename);
                using JsonTextReader reader = new JsonTextReader(file);
                JObject json = (JObject)JToken.ReadFrom(reader);
                var unitid = json["UNITID"].ToString();
                var year = json["year"].ToString();
                var name = json["name"].ToString();
                if (name == "")
                {
                    Console.WriteLine($"File {filename} has empty university name");
                }
                if (unitid == "")
                {
                    Console.WriteLine($"File {filename} has empty UNITID");
                }
                //Console.WriteLine($"Current college is {name} with UNITID {unitid} in year {year}!");
                unitids.Add(unitid);
                if (map.ContainsKey(name))
                {
                    Console.WriteLine($"This college has duplicate name {name}!");
                }
                else
                {
                    map.Add(name, unitid);
                }
            }
            foreach (string filename in Directory.EnumerateFiles(commonDatasetCollegeFilePath, "*.json"))
            {
                count++;
                //Console.WriteLine("File name: " + filename);
                using StreamReader file = File.OpenText(filename);
                using JsonTextReader reader = new JsonTextReader(file);
                JObject json = (JObject)JToken.ReadFrom(reader);
                var unitid = json["UNITID"].ToString();
                var year = json["year"].ToString();
                var name = json["name"].ToString();
                if (name == "")
                {
                    Console.WriteLine($"File {filename} has empty university name");
                }
                if (unitid == "")
                {
                    Console.WriteLine($"File {filename} has empty UNITID");
                }
                //Console.WriteLine($"Current college is {name} with UNITID {unitid} in year {year}!");
                unitids.Add(unitid);
                if (map.ContainsKey(name))
                {
                    Console.WriteLine($"This college has duplicate name {name}!");
                }
                else
                {
                    map.Add(name, unitid);
                }
            }
            Console.WriteLine($"Totally {count} colleges are covered in Common Dataset!");
            Console.WriteLine($"Found map entries {map.Count}");

            // validate duplicate UNITID
            Console.WriteLine($"Unique UNITID number: {unitids.Count}");

        }
        public async Task UpdateCommonDataset(string inputFile)
        {
            var container = database.GetContainer("CommonDataset");
            int count = 0;
            foreach (string filename in Directory.EnumerateFiles(inputFile, "*.json"))
            {
                count++;
                //string contents = File.ReadAllText(file);
                Console.WriteLine("File name: " + filename);
                //Console.WriteLine(contents);
                using StreamReader file = File.OpenText(filename);
                using JsonTextReader reader = new JsonTextReader(file);
                JObject json = (JObject)JToken.ReadFrom(reader);
                //Console.WriteLine(json["waiting"]["hasWaiting"]);
                Console.WriteLine("Current college is " + json["year"].ToString());
                CommonDatasetModel model = new CommonDatasetModel
                {
                    Id = json["UNITID"].ToString(),
                    UNITID = json["UNITID"].ToString(),
                    Year = int.Parse(json["year"].ToString()),
                    Name = json["name"].ToString(),
                    Waiting = json["waiting"],
                    AdmissionReq = json["admReq"],
                    AdmissionDecision = json["admDecision"],
                    SatAct = json["satAct"],
                    ClassRank = json["classRankPercent"],
                    Gpa = json["gpa"],
                    Apply = json["apply"],
                    Transfer = json["transfer"]
                };
                ItemResponse<CommonDatasetModel> res = null;
                try
                {
                    res = await container.ReadItemAsync<CommonDatasetModel>(model.Id, new PartitionKey(model.UNITID));
                    Console.WriteLine("Found the model with UNITID: " + res.Resource.UNITID);
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) { }
                if (res == null)
                {
                    Console.WriteLine("Not found the college " + json["name"]);
                    ItemResponse<CommonDatasetModel> result = await container.CreateItemAsync(model, new PartitionKey(model.UNITID));
                    Console.WriteLine("Created model in database with id: {0} Operation consumed {1} RUs.\n", result.Resource.Id, result.RequestCharge);
                }

            }
            Console.WriteLine($"Updated {count} number of colleges for file {inputFile}");
        }
        public async Task UpdateCollegeDataset(string inputFile)
        {
            var container = database.GetContainer("CollegeDataset");
            int count = 0;
            foreach (string filename in Directory.EnumerateFiles(inputFile, "*.json"))
            {
                count++;
                //string contents = File.ReadAllText(file);
                Console.WriteLine("File name: " + filename);
                //Console.WriteLine(contents);
                using StreamReader file = File.OpenText(filename);
                using JsonTextReader reader = new JsonTextReader(file);
                JObject json = (JObject)JToken.ReadFrom(reader);
                //Console.WriteLine(json["waiting"]["hasWaiting"]);
                Console.WriteLine("Current college is " + json["year"].ToString());
                CollegeDatasetModel model = new CollegeDatasetModel
                {
                    Id = json["UNITID"].ToString(),
                    UNITID = json["UNITID"].ToString(),
                    Year = int.Parse(json["year"].ToString()),
                    Name = json["name"].ToString(),
                    Waiting = json["waiting"],
                    AdmissionReq = json["admReq"],
                    AdmissionDecision = json["admDecision"],
                    SatAct = json["satAct"],
                    ClassRank = json["classRankPercent"],
                    Gpa = json["gpa"],
                    Apply = json["apply"]
                };
                ItemResponse<CollegeDatasetModel> res = null;
                try
                {
                    res = await container.ReadItemAsync<CollegeDatasetModel>(model.Id, new PartitionKey(model.UNITID));
                    Console.WriteLine("Found the model with UNITID: " + res.Resource.UNITID);
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) { }
                if (res == null)
                {
                    Console.WriteLine("Not found the college " + json["name"]);
                    ItemResponse<CollegeDatasetModel> result = await container.CreateItemAsync(model, new PartitionKey(model.UNITID));
                    Console.WriteLine("Created model in database with id: {0} Operation consumed {1} RUs.\n", result.Resource.Id, result.RequestCharge);
                }

            }
            Console.WriteLine($"Updated {count} number of colleges for file {inputFile}");
        }

        public async Task UpdateYearlDataWithCommonDataset()
        {
            var targetContainer = database.GetContainer("CollegeDataUSYearly");
            var srcContainer = database.GetContainer("CommonDataset");
            // iterate each model in CommonDataset and update CollegeDataUSYearly accordingly
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = srcContainer.GetItemQueryIterator<CommonDatasetModel>(queryDefinition);
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                foreach (var item in res)
                {
                    Console.WriteLine($"Found this UNITID {item.UNITID}");
                    var filterQueryText = $"SELECT * FROM c where c.UNITID ='{item.UNITID}'";
                    QueryDefinition innerQueryDef = new QueryDefinition(filterQueryText);
                    var innerIt = targetContainer.GetItemQueryIterator<CollegeDataUSYearly>(innerQueryDef);
                    while (innerIt.HasMoreResults)
                    {
                        var result = await innerIt.ReadNextAsync();
                        CollegeDataUSYearly dataItem = result.First();
                        Console.WriteLine($"Found corresponding college {dataItem.UNITID} in yearly data!");
                        dataItem.CommonData = new CommonDatasetModel[] { item };
                        ItemResponse<CollegeDataUSYearly> outcome = await targetContainer.UpsertItemAsync(dataItem, new PartitionKey(dataItem.UNITID));
                        Console.WriteLine($"For UNITID {dataItem.UNITID} updated item in database with id: {0} Operation consumed {1} RUs.\n", outcome.Resource.Id, outcome.RequestCharge);
                    }
                }
            }
        }
        public async Task UpdateYearlDataWithCollegeDataset()
        {
            var targetContainer = database.GetContainer("CollegeDataUSYearly");
            var srcContainer = database.GetContainer("CollegeDataset");
            // iterate each model in CollegeDataset and update CollegeDataUSYearly accordingly
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = srcContainer.GetItemQueryIterator<CollegeDatasetModel>(queryDefinition);
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                foreach (var item in res)
                {
                    Console.WriteLine($"Found this UNITID {item.UNITID}");
                    var filterQueryText = $"SELECT * FROM c where c.UNITID ='{item.UNITID}'";
                    QueryDefinition innerQueryDef = new QueryDefinition(filterQueryText);
                    var innerIt = targetContainer.GetItemQueryIterator<CollegeDataUSYearly>(innerQueryDef);
                    while (innerIt.HasMoreResults)
                    {
                        var result = await innerIt.ReadNextAsync();
                        CollegeDataUSYearly dataItem = result.First();
                        Console.WriteLine($"Found corresponding college {dataItem.UNITID} in yearly data!");
                        dataItem.CollegeData = new CollegeDatasetModel[] { item };
                        ItemResponse<CollegeDataUSYearly> outcome = await targetContainer.UpsertItemAsync(dataItem, new PartitionKey(dataItem.UNITID));
                        Console.WriteLine($"For UNITID {dataItem.UNITID} updated item in database with id: {0} Operation consumed {1} RUs.\n", outcome.Resource.Id, outcome.RequestCharge);
                    }
                }
            }
        }
        public async Task UpdateYearlDataWithUSNewsRanking()
        {
            var targetContainer = database.GetContainer("CollegeDataUSYearly");
            var srcContainer = database.GetContainer("USNewsRanking");
            // iterate each college in USNewsRanking and update CollegeDataUSYearly accordingly
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = srcContainer.GetItemQueryIterator<USNewsRanking>(queryDefinition);
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                foreach (var item in res)
                {
                    var year = item.year;
                    Console.WriteLine($"Found this year {year}");
                    var universities = item.Universities;
                    foreach(var university in universities)
                    {
                        var UNITID = university.UNITID;
                        Console.WriteLine($"Found a college: {university.INSTNM} with " + UNITID);
                        USNewsRankingInYearly rank = new USNewsRankingInYearly
                        {
                            Year = int.Parse(year),
                            Category = "Public University",
                            Rank = university.Rank
                        };
                        var filterQueryText = $"SELECT * FROM c where c.UNITID ='{UNITID}'";
                        QueryDefinition innerQueryDef = new QueryDefinition(filterQueryText);
                        var innerIt = targetContainer.GetItemQueryIterator<CollegeDataUSYearly>(innerQueryDef);
                        while (innerIt.HasMoreResults)
                        {
                            var result = await innerIt.ReadNextAsync();
                            CollegeDataUSYearly dataItem = result.First();
                            Console.WriteLine($"Found corresponding college {dataItem.UNITID} in yearly data!");
                            dataItem.Rank = new USNewsRankingInYearly[] { rank };
                            ItemResponse<CollegeDataUSYearly> outcome = await targetContainer.UpsertItemAsync(dataItem, new PartitionKey(dataItem.UNITID));
                            Console.WriteLine($"For UNITID {dataItem.UNITID} updated item in database with id: {0} Operation consumed {1} RUs.\n", outcome.Resource.Id, outcome.RequestCharge);
                        }
                    }

                    var colleges = item.LibertyColleges;
                    foreach (var college in colleges)
                    {
                        var UNITID = college.UNITID;
                        Console.WriteLine($"Found a liberty arts college: {college.INSTNM} with " + UNITID);
                        USNewsRankingInYearly rank = new USNewsRankingInYearly
                        {
                            Year = int.Parse(year),
                            Category = "Liberty Arts College",
                            Rank = college.Rank
                        };
                        var filterQueryText = $"SELECT * FROM c where c.UNITID ='{UNITID}'";
                        QueryDefinition innerQueryDef = new QueryDefinition(filterQueryText);
                        var innerIt = targetContainer.GetItemQueryIterator<CollegeDataUSYearly>(innerQueryDef);
                        while (innerIt.HasMoreResults)
                        {
                            var result = await innerIt.ReadNextAsync();
                            CollegeDataUSYearly dataItem = result.First();
                            Console.WriteLine($"Found corresponding college {dataItem.UNITID} in yearly data!");
                            dataItem.Rank = new USNewsRankingInYearly[] { rank };
                            ItemResponse<CollegeDataUSYearly> outcome = await targetContainer.UpsertItemAsync(dataItem, new PartitionKey(dataItem.UNITID));
                            Console.WriteLine($"For UNITID {dataItem.UNITID} updated item in database with id: {0} Operation consumed {1} RUs.\n", outcome.Resource.Id, outcome.RequestCharge);
                        }
                    }
                }
            }
        }
        public async Task UpdateYearlDataWithCollegeInfo()
        {
            var targetContainer = database.GetContainer("CollegeDataUSYearly");
            var srcContainer = database.GetContainer("CollegeDataUS");
            // iterate each college in CollegeDataUS and update CollegeDataUSYearly accordingly
            var sqlQueryText = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = srcContainer.GetItemQueryIterator<CollegeDataUS>(queryDefinition);
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                foreach (var item in res)
                {
                    var collegeName = item.INSTNM;
                    var UNITID = item.UNITID;
                    
                    Console.WriteLine($"Found this name {collegeName} with UNITID: {UNITID}");
                    var filterQueryText = "SELECT * FROM c where c.UNITID = @UNITID";
                    QueryDefinition innerQueryDef = new QueryDefinition(filterQueryText)
                        .WithParameter("@UNITID", UNITID);
                    var innerIt = targetContainer.GetItemQueryIterator<CollegeDataUSYearly>(innerQueryDef);
                    while (innerIt.HasMoreResults)
                    {
                        var result = await innerIt.ReadNextAsync();
                        CollegeDataUSYearly dataItem = result.First();
                        Console.WriteLine($"Found corresponding college {dataItem.UNITID} in yearly data!");
                        dataItem.INSTNM = collegeName;
                        dataItem.CITY = item.CITY;
                        dataItem.STABBR = item.STABBR;
                        dataItem.ZIP = item.ZIP;
                        dataItem.ST_FIPS = item.ST_FIPS;
                        dataItem.REGION = item.REGION;
                        dataItem.LOCALE = item.LOCALE;
                        dataItem.LATITUDE = item.LATITUDE;
                        dataItem.LONGITUDE = item.LONGITUDE;
                        dataItem.INSTURL = item.INSTURL;
                        dataItem.NPCURL = item.NPCURL;
                        dataItem.CONTROL = item.CONTROL;
                        dataItem.ADMINURL = item.ADMINURL;
                        dataItem.FAIDURL = item.FAIDURL;
                        dataItem.APPLURL = item.APPLURL;
                        dataItem.COUNTYNM = item.COUNTYNM;
                        dataItem.HIGHEST_DEGREE = item.HIGHEST_DEGREE;
                        ItemResponse<CollegeDataUSYearly> outcome = await targetContainer.UpsertItemAsync(dataItem, new PartitionKey(dataItem.UNITID));
                        Console.WriteLine($"For UNITID {dataItem.UNITID} updated item in database with id: {0} Operation consumed {1} RUs.\n", outcome.Resource.Id, outcome.RequestCharge);
                    }

                }
            }
        }

        // each containerId corresponds to a prepared IPEDS table like IPEDSADM to update the main table
        public async Task UpdateYearlyData(string containerId)
        {
            var mainContainer = database.GetContainer("CollegeDataUSYearly");
            var sqlQueryText = $"SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = mainContainer.GetItemQueryIterator<CollegeDataUSYearly>(queryDefinition);
            int count = 0;
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                Console.WriteLine($"This set contains {res.Count} records!");
                count += res.Count;
                foreach (var item in res)
                {
                    var unitid = item.UNITID;
                    var targetItem = await QueryTableAsync<IPEDS>(containerId, unitid);
                    if (targetItem != default)
                    {
                        if (containerId.Equals("IPEDSADM"))
                        {
                            CollegeDataUSYearly_ADM obj = new CollegeDataUSYearly_ADM
                            {
                                Year = 2018,
                                APPLCN = targetItem.APPLCN,
                                APPLCNM = targetItem.APPLCNM,
                                APPLCNW = targetItem.APPLCNW,
                                ADMSSN = targetItem.ADMSSN,
                                ADMSSNM = targetItem.ADMSSNM,
                                ADMSSNW = targetItem.ADMSSNW,
                                ENRLT = targetItem.ENRLT,
                                ENRLM = targetItem.ENRLM,
                                ENRLW = targetItem.ENRLW,
                                ENRLFT = targetItem.ENRLFT,
                                ENRLFTM = targetItem.ENRLFTM,
                                ENRLFTW = targetItem.ENRLFTW,
                                ENRLPT = targetItem.ENRLPT,
                                ENRLPTM = targetItem.ENRLPTM,
                                ENRLPTW = targetItem.ENRLPTW,
                                SATNUM = targetItem.SATNUM,
                                SATPCT = targetItem.SATPCT,
                                ACTNUM = targetItem.ACTNUM,
                                ACTPCT = targetItem.ACTPCT
                            };
                            item.IPEDSADM = new CollegeDataUSYearly_ADM[] { obj };
                            
                        }
                        else if (containerId.Equals("IPEDSAL"))
                        {
                            CollegeDataUSYearly_AL obj = new CollegeDataUSYearly_AL
                            {
                                Year = 2018,
                                LPBOOKS = targetItem.LPBOOKS,
                                LEBOOKS = targetItem.LEBOOKS,
                                LEDATAB = targetItem.LEDATAB,
                                LPMEDIA = targetItem.LPMEDIA,
                                LEMEDIA = targetItem.LEMEDIA,
                                LPCLLCT = targetItem.LPCLLCT,
                                LECLLCT = targetItem.LECLLCT,
                                LTCLLCT = targetItem.LTCLLCT,
                                LPCRCLT = targetItem.LPCRCLT,
                                LECRCLT = targetItem.LECRCLT,
                                LTCRCLT = targetItem.LTCRCLT
                            };
                            item.IPEDSAL = new CollegeDataUSYearly_AL[] { obj };
                            
                        }
                        else if (containerId.Equals("IPEDSCDEP"))
                        {
                            item.CDEP = targetItem.Items;
                        }
                        else if (containerId.Equals("IPEDSDRVC"))
                        {
                            CollegeDataUSYearly_DRVC obj = new CollegeDataUSYearly_DRVC
                            {
                                Year = 2018,
                                DOCDEGRS = targetItem.DOCDEGRS,
                                DOCDEGPP = targetItem.DOCDEGPP,
                                DOCDEGOT = targetItem.DOCDEGOT,
                                MASDEG = targetItem.MASDEG,
                                BASDEG = targetItem.BASDEG,
                                ASCDEG = targetItem.ASCDEG,
                                CERT4 = targetItem.CERT4,
                                CERT2 = targetItem.CERT2,
                                CERT1 = targetItem.CERT1,
                                PBACERT = targetItem.PBACERT,
                                PMACERT = targetItem.PMACERT,
                                SDOCDEG = targetItem.SDOCDEG,
                                SMASDEG = targetItem.SMASDEG,
                                SBASDEG = targetItem.SBASDEG,
                                SASCDEG = targetItem.SASCDEG,
                                SBAMACRT = targetItem.SBAMACRT,
                                SCERT24 = targetItem.SCERT24,
                                SCERT1 = targetItem.SCERT1
                            };
                            item.IPEDSDRVC = new CollegeDataUSYearly_DRVC[] { obj };
                        }
                        else if (containerId.Equals("IPEDSDRVEF"))
                        {
                            CollegeDataUSYearly_DRVEF obj = new CollegeDataUSYearly_DRVEF
                            {
                                Year = 2018,
                                ENRTOT = targetItem.ENRTOT,
                                ENRFT = targetItem.ENRFT,
                                ENRPT = targetItem.ENRPT,
                                PCTENRWH = targetItem.PCTENRWH,
                                PCTENRBK = targetItem.PCTENRBK,
                                PCTENRHS = targetItem.PCTENRHS,
                                PCTENRAP = targetItem.PCTENRAP,
                                PCTENRAS = targetItem.PCTENRAS,
                                PCTENRAN = targetItem.PCTENRAN,
                                PCTENRUN = targetItem.PCTENRUN,
                                PCTENRNR = targetItem.PCTENRNR,
                                PCTENRW = targetItem.PCTENRW,
                                EFUGFT = targetItem.EFUGFT,
                                PCUENRWH = targetItem.PCUENRWH,
                                PCUENRBK = targetItem.PCUENRBK,
                                PCUENRHS = targetItem.PCUENRHS,
                                PCUENRAP = targetItem.PCUENRAP,
                                PCUENRAS = targetItem.PCUENRAS,
                                PCUENRAN = targetItem.PCUENRAN,
                                PCUENR2M = targetItem.PCUENR2M,
                                PCUENRUN = targetItem.PCUENRUN,
                                PCUENRNR = targetItem.PCUENRNR,
                                PCUENRW = targetItem.PCUENRW,
                                EFGRAD = targetItem.EFGRAD,
                                PCGENRWH = targetItem.PCGENRWH,
                                PCGENRBK = targetItem.PCGENRBK,
                                PCGENRHS = targetItem.PCGENRHS,
                                PCGENRAP = targetItem.PCGENRAP,
                                PCGENRAN = targetItem.PCGENRAN,
                                PCGENR2M = targetItem.PCGENR2M,
                                PCGENRUN = targetItem.PCGENRUN,
                                PCGENRNR = targetItem.PCGENRNR,
                                PCGENRW = targetItem.PCGENRW
                            };
                            item.IPEDSDRVEF = new CollegeDataUSYearly_DRVEF[] { obj };
                        }
                        else if (containerId.Equals("IPEDSDRVGR"))
                        {
                            CollegeDataUSYearly_DRVGR obj = new CollegeDataUSYearly_DRVGR
                            {
                                Year = 2018,
                                GRRTTOT = targetItem.GRRTTOT,
                                GRRTM = targetItem.GRRTM,
                                GRRTW = targetItem.GRRTW,
                                GRRTAN = targetItem.GRRTAN,
                                GRRTAP = targetItem.GRRTAP,
                                GRRTAS = targetItem.GRRTAS,
                                GRRTNH = targetItem.GRRTNH,
                                GRRTBK = targetItem.GRRTBK,
                                GRRTHS = targetItem.GRRTHS,
                                GRRTWH = targetItem.GRRTWH,
                                GRRT2M = targetItem.GRRT2M,
                                GRRTUN = targetItem.GRRTUN,
                                GRRTNR = targetItem.GRRTNR,
                                TRRTTOT = targetItem.TRRTTOT,
                                GBA4RTT = targetItem.GBA4RTT,
                                GBA5RTT = targetItem.GBA5RTT,
                                GBA6RTT = targetItem.GBA6RTT,
                                GBA6RTM = targetItem.GBA6RTM,
                                GBA6RTW = targetItem.GBA6RTW,
                                GBA6RTAN = targetItem.GBA6RTAN,
                                GBA6RTAP = targetItem.GBA6RTAP,
                                GBA6RTAS = targetItem.GBA6RTAS,
                                GBA6RTNH = targetItem.GBA6RTNH,
                                GBA6RTBK = targetItem.GBA6RTBK,
                                GBA6RTHS = targetItem.GBA6RTHS,
                                GBA6RTWH = targetItem.GBA6RTWH,
                                GBA6RT2M = targetItem.GBA6RT2M,
                                GBA6RTUN = targetItem.GBA6RTUN,
                                GBA6RTNR = targetItem.GBA6RTNR,
                                GBATRRT = targetItem.GBATRRT
                            };
                            item.IPEDSDRVGR = new CollegeDataUSYearly_DRVGR[] { obj };
                        }
                        else if (containerId.Equals("IPEDSDRVIC"))
                        {
                            CollegeDataUSYearly_DRVIC obj = new CollegeDataUSYearly_DRVIC
                            {
                                Year = 2018,
                                CINDON = targetItem.CINDON,
                                CINSON = targetItem.CINSON,
                                COTSON = targetItem.COTSON,
                                CINDOFF = targetItem.CINDOFF,
                                CINSOFF = targetItem.CINSOFF,
                                COTSOFF = targetItem.COTSOFF,
                                CINDFAM = targetItem.CINDFAM,
                                CINSFAM = targetItem.CINSFAM,
                                COTSFAM = targetItem.COTSFAM
                            };
                            item.IPEDSDRVIC = new CollegeDataUSYearly_DRVIC[] { obj };
                        }
                        else if (containerId.Equals("IPEDSIC"))
                        {
                            CollegeDataUSYearly_IC obj = new CollegeDataUSYearly_IC
                            {
                                Year = 2018,
                                FT_UG = targetItem.FT_UG,
                                FT_FTUG = targetItem.FT_FTUG,
                                PT_UG = targetItem.PT_UG,
                                PT_FTUG = targetItem.PT_FTUG,
                                ROOM = targetItem.ROOM,
                                ROOMCAP = targetItem.ROOMCAP,
                                BOARD = targetItem.BOARD,
                                ROOMAMT = targetItem.ROOMAMT,
                                BOARDAMT = targetItem.BOARDAMT,
                                RMBRDAMT = targetItem.RMBRDAMT,
                                APPLFEEU = targetItem.APPLFEEU,
                                APPLFEEG = targetItem.APPLFEEG
                            };
                            item.IPEDSIC = new CollegeDataUSYearly_IC[] { obj };
                        }
                        else if (containerId.Equals("IPEDSICAY"))
                        {
                            CollegeDataUSYearly_ICAY obj = new CollegeDataUSYearly_ICAY
                            {
                                Year = 2018,
                                TUITION1 = targetItem.TUITION1,
                                FEE1 = targetItem.FEE1,
                                HRCHG1 = targetItem.HRCHG1,
                                TUITION2 = targetItem.TUITION2,
                                FEE2 = targetItem.FEE2,
                                HRCHG2 = targetItem.HRCHG2,
                                TUITION3 = targetItem.TUITION3,
                                FEE3 = targetItem.FEE3,
                                HRCHG3 = targetItem.HRCHG3,
                                TUITION5 = targetItem.TUITION5,
                                FEE5 = targetItem.FEE5,
                                HRCHG5 = targetItem.HRCHG5,
                                TUITION6 = targetItem.TUITION6,
                                FEE6 = targetItem.FEE6,
                                HRCHG6 = targetItem.HRCHG6,
                                TUITION7 = targetItem.TUITION7,
                                FEE7 = targetItem.FEE7,
                                HRCHG7 = targetItem.HRCHG7,
                                ISPROF1 = targetItem.ISPROF1,
                                ISPFEE1 = targetItem.ISPFEE1,
                                OSPROF1 = targetItem.OSPROF1,
                                OSPFEE1 = targetItem.OSPFEE1,
                                ISPROF2 = targetItem.ISPROF2,
                                ISPFEE2 = targetItem.ISPFEE2,
                                OSPROF2 = targetItem.OSPROF2,
                                OSPFEE2 = targetItem.OSPFEE2,
                                ISPROF3 = targetItem.ISPROF3,
                                ISPFEE3 = targetItem.ISPFEE3,
                                OSPROF3 = targetItem.OSPROF3,
                                OSPFEE3 = targetItem.OSPFEE3,
                                ISPROF4 = targetItem.ISPROF4,
                                ISPFEE4 = targetItem.ISPFEE4,
                                OSPROF4 = targetItem.OSPROF4,
                                OSPFEE4 = targetItem.OSPFEE4,
                                ISPROF5 = targetItem.ISPROF5,
                                ISPFEE5 = targetItem.ISPFEE5,
                                OSPROF5 = targetItem.OSPROF5,
                                OSPFEE5 = targetItem.OSPFEE5,
                                ISPROF6 = targetItem.ISPROF6,
                                ISPFEE6 = targetItem.ISPFEE6,
                                OSPROF6 = targetItem.OSPROF6,
                                OSPFEE6 = targetItem.OSPFEE6,
                                ISPROF7 = targetItem.ISPROF7,
                                ISPFEE7 = targetItem.ISPFEE7,
                                OSPROF7 = targetItem.OSPROF7,
                                OSPFEE7 = targetItem.OSPFEE7,
                                ISPROF8 = targetItem.ISPROF8,
                                ISPFEE8 = targetItem.ISPFEE8,
                                OSPROF8 = targetItem.OSPROF8,
                                OSPFEE8 = targetItem.OSPFEE8,
                                ISPROF9 = targetItem.ISPROF9,
                                ISPFEE9 = targetItem.ISPFEE9,
                                OSPROF9 = targetItem.OSPROF9,
                                OSPFEE9 = targetItem.OSPFEE9,
                                CHG1AT0 = targetItem.CHG1AT0,
                                CHG1AF0 = targetItem.CHG1AF0,
                                CHG1AY0 = targetItem.CHG1AY0,
                                CHG1AT1 = targetItem.CHG1AT1,
                                CHG1AF1 = targetItem.CHG1AF1,
                                CHG1AY1 = targetItem.CHG1AY1,
                                CHG1AT2 = targetItem.CHG1AT2,
                                CHG1AF2 = targetItem.CHG1AF2,
                                CHG1AY2 = targetItem.CHG1AY2,
                                CHG1AT3 = targetItem.CHG1AT3,
                                CHG1AF3 = targetItem.CHG1AF3,
                                CHG1AY3 = targetItem.CHG1AY3,
                                CHG1TGTD = targetItem.CHG1TGTD,
                                CHG1FGTD = targetItem.CHG1FGTD,
                                CHG2AT0 = targetItem.CHG2AT0,
                                CHG2AF0 = targetItem.CHG2AF0,
                                CHG2AY0 = targetItem.CHG2AY0,
                                CHG2AT1 = targetItem.CHG2AT1,
                                CHG2AF1 = targetItem.CHG2AF1,
                                CHG2AY1 = targetItem.CHG2AY1,
                                CHG2AT2 = targetItem.CHG2AT2,
                                CHG2AF2 = targetItem.CHG2AF2,
                                CHG2AY2 = targetItem.CHG2AY2,
                                CHG2AT3 = targetItem.CHG2AT3,
                                CHG2AF3 = targetItem.CHG2AF3,
                                CHG2AY3 = targetItem.CHG2AY3,
                                CHG2TGTD = targetItem.CHG2TGTD,
                                CHG2FGTD = targetItem.CHG2FGTD,
                                CHG3AT0 = targetItem.CHG3AT0,
                                CHG3AF0 = targetItem.CHG3AF0,
                                CHG3AY0 = targetItem.CHG3AY0,
                                CHG3AT1 = targetItem.CHG3AT1,
                                CHG3AF1 = targetItem.CHG3AF1,
                                CHG3AY1 = targetItem.CHG3AY1,
                                CHG3AT2 = targetItem.CHG3AT2,
                                CHG3AF2 = targetItem.CHG3AF2,
                                CHG3AY2 = targetItem.CHG3AY2,
                                CHG3AT3 = targetItem.CHG3AT3,
                                CHG3AF3 = targetItem.CHG3AF3,
                                CHG3AY3 = targetItem.CHG3AY3,
                                CHG3TGTD = targetItem.CHG3TGTD,
                                CHG3FGTD = targetItem.CHG3FGTD,
                                CHG4AY0 = targetItem.CHG4AY0,
                                CHG4AY1 = targetItem.CHG4AY1,
                                CHG4AY2 = targetItem.CHG4AY2,
                                CHG4AY3 = targetItem.CHG4AY3,
                                CHG5AY0 = targetItem.CHG5AY0,
                                CHG5AY1 = targetItem.CHG5AY1,
                                CHG5AY2 = targetItem.CHG5AY2,
                                CHG5AY3 = targetItem.CHG5AY3,
                                CHG6AY0 = targetItem.CHG6AY0,
                                CHG6AY1 = targetItem.CHG6AY1,
                                CHG6AY2 = targetItem.CHG6AY2,
                                CHG6AY3 = targetItem.CHG6AY3,
                                CHG7AY0 = targetItem.CHG7AY0,
                                CHG7AY1 = targetItem.CHG7AY1,
                                CHG7AY2 = targetItem.CHG7AY2,
                                CHG7AY3 = targetItem.CHG7AY3,
                                CHG8AY0 = targetItem.CHG8AY0,
                                CHG8AY1 = targetItem.CHG8AY1,
                                CHG8AY2 = targetItem.CHG8AY2,
                                CHG8AY3 = targetItem.CHG8AY3,
                                CHG9AY0 = targetItem.CHG9AY0,
                                CHG9AY1 = targetItem.CHG9AY1,
                                CHG9AY2 = targetItem.CHG9AY2,
                                CHG9AY3 = targetItem.CHG9AY3
                            };
                            item.IPEDSICAY = new CollegeDataUSYearly_ICAY[] { obj };
                        }
                        else if (containerId.Equals("IPEDSSSIS"))
                        {
                            CollegeDataUSYearly_SSIS obj = new CollegeDataUSYearly_SSIS
                            {
                                Year = 2018,
                                SISTOTL = targetItem.SISTOTL,
                                SISPROF = targetItem.SISPROF,
                                SISASCP = targetItem.SISASCP,
                                SISASTP = targetItem.SISASTP,
                                SISINST = targetItem.SISINST,
                                SISLECT = targetItem.SISLECT
                            };
                            item.IPEDSSSIS = new CollegeDataUSYearly_SSIS[] { obj };
                        }
                        ItemResponse<CollegeDataUSYearly> result = await mainContainer.UpsertItemAsync(item, new PartitionKey(unitid));
                        Console.WriteLine($"For UNITID {unitid} updated item in database with id: {0} Operation consumed {1} RUs.\n", result.Resource.Id, result.RequestCharge);
                    }
                }
            }
        }

        public async Task AddIPEDSYearlyDataforADM(string containerId)
        {
            var filePath = $"{rootPath}ADM2018.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? applcn = RetrieveVal(reader.GetValue(10));
                    double? applcnm = RetrieveVal(reader.GetValue(11));
                    double? applcnw = RetrieveVal(reader.GetValue(12));
                    double? admssn = RetrieveVal(reader.GetValue(13));
                    double? admssnm = RetrieveVal(reader.GetValue(14));
                    double? admssnw = RetrieveVal(reader.GetValue(15));
                    double? enrlt = RetrieveVal(reader.GetValue(16));
                    double? enrlm = RetrieveVal(reader.GetValue(17));
                    double? enrlw = RetrieveVal(reader.GetValue(18));
                    double? enrlft = RetrieveVal(reader.GetValue(19));
                    double? enrlftm = RetrieveVal(reader.GetValue(20));
                    double? enrlftw = RetrieveVal(reader.GetValue(21));
                    double? enrlpt = RetrieveVal(reader.GetValue(22));
                    double? enrlptm = RetrieveVal(reader.GetValue(23));
                    double? enrlptw = RetrieveVal(reader.GetValue(24));
                    double? satnum = RetrieveVal(reader.GetValue(25));
                    double? satpct = RetrieveVal(reader.GetValue(26));
                    double? actnum = RetrieveVal(reader.GetValue(27));
                    double? actpct = RetrieveVal(reader.GetValue(28));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_ADM item = new IPEDS_ADM
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR,
                            APPLCN = applcn,
                            APPLCNM = applcnm,
                            APPLCNW = applcnw,
                            ADMSSN = admssn,
                            ADMSSNM = admssnm,
                            ADMSSNW = admssnw,
                            ENRLT = enrlt,
                            ENRLM = enrlm,
                            ENRLW = enrlw,
                            ENRLFT = enrlft,
                            ENRLFTM = enrlftm,
                            ENRLFTW = enrlftw,
                            ENRLPT = enrlpt,
                            ENRLPTM = enrlptm,
                            ENRLPTW = enrlptw,
                            SATNUM = satnum,
                            SATPCT = satpct,
                            ACTNUM = actnum,
                            ACTPCT = actpct
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            ItemResponse<IPEDS_ADM> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforAL(string containerId)
        {
            var filePath = $"{rootPath}AL2018.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? lpbooks = RetrieveVal(reader.GetValue(2));
                    double? lebooks = RetrieveVal(reader.GetValue(3));
                    double? ledatab = RetrieveVal(reader.GetValue(4));
                    double? lpmedia = RetrieveVal(reader.GetValue(5));
                    double? lemedia = RetrieveVal(reader.GetValue(6));
                    double? lpcllct = RetrieveVal(reader.GetValue(9));
                    double? lecllct = RetrieveVal(reader.GetValue(10));
                    double? ltcllct = RetrieveVal(reader.GetValue(11));
                    double? lpcrclt = RetrieveVal(reader.GetValue(12));
                    double? lecrclt = RetrieveVal(reader.GetValue(13));
                    double? ltcrclt = RetrieveVal(reader.GetValue(14));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_AL item = new IPEDS_AL
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR,
                            LPBOOKS = lpbooks,
                            LEBOOKS = lebooks,
                            LEDATAB = ledatab,
                            LPMEDIA = lpmedia,
                            LEMEDIA = lemedia,
                            LPCLLCT = lpcllct,
                            LECLLCT = lecllct,
                            LTCLLCT = ltcllct,
                            LPCRCLT = lpcrclt,
                            LECRCLT = lecrclt,
                            LTCRCLT = ltcrclt
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with ltcrclt: " + item.LTCRCLT);
                            ItemResponse<IPEDS_AL> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforCDEP(string containerId)
        {
            var filePath = $"{rootPath}C2018DEP.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    var CIPCODE = reader.GetString(1);
                    double? PTOTAL = RetrieveVal(reader.GetValue(2));
                    double? PTOTALDE = RetrieveVal(reader.GetValue(3));
                    double? PASSOC = RetrieveVal(reader.GetValue(4));
                    double? PASSOCDE = RetrieveVal(reader.GetValue(5));
                    double? PBACHL = RetrieveVal(reader.GetValue(6));
                    double? PBACHLDE = RetrieveVal(reader.GetValue(7));
                    double? PMASTR = RetrieveVal(reader.GetValue(8));
                    double? PMASTRDE = RetrieveVal(reader.GetValue(9));
                    double? PDOCRS = RetrieveVal(reader.GetValue(10));
                    double? PDOCRSDE = RetrieveVal(reader.GetValue(11));
                    double? PDOCPP = RetrieveVal(reader.GetValue(12));
                    double? PDOCPPDE = RetrieveVal(reader.GetValue(13));
                    double? PDOCOT = RetrieveVal(reader.GetValue(14));
                    double? PDOCOTDE = RetrieveVal(reader.GetValue(15));
                    double? PCERT1 = RetrieveVal(reader.GetValue(16));
                    double? PCERT1DE = RetrieveVal(reader.GetValue(17));
                    double? PCERT2 = RetrieveVal(reader.GetValue(18));
                    double? PPBACCDE = RetrieveVal(reader.GetValue(23));
                    double? PPMAST = RetrieveVal(reader.GetValue(24));
                    double? PPMASTDE = RetrieveVal(reader.GetValue(25));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 300000)
                    {
                        if (idNum % 10000 == 0)
                        {
                            Console.WriteLine("Working on line: " + idNum);
                        }
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_CDEP item = new IPEDS_CDEP
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR
                        };
                        IPEDS_CDEP_ITEM entry = new IPEDS_CDEP_ITEM
                        {
                            PTOTAL = PTOTAL,
                            PTOTALDE = PTOTALDE,
                            PASSOC = PASSOC,
                            PASSOCDE = PASSOCDE,
                            PBACHL = PBACHL,
                            PBACHLDE = PBACHLDE,
                            PMASTR = PMASTR,
                            PMASTRDE = PMASTRDE,
                            PDOCRS = PDOCRS,
                            PDOCRSDE = PDOCRSDE,
                            PDOCPP = PDOCPP,
                            PDOCPPDE = PDOCPPDE,
                            PDOCOT = PDOCOT,
                            PDOCOTDE = PDOCOTDE,
                            PCERT1 = PCERT1,
                            PCERT1DE = PCERT1DE,
                            PCERT2 = PCERT2,
                            PPBACCDE = PPBACCDE,
                            PPMAST = PPMAST,
                            PPMASTDE = PPMASTDE
                        };

                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            IPEDS_CDEP val = await QueryTableAsync<IPEDS_CDEP>("IPEDSCDEP", unitid);
                            if (val == default)
                            {
                                // create the item
                                //Console.WriteLine("Need to create the new item!");
                                var dict = new Dictionary<string, IPEDS_CDEP_ITEM>
                                {
                                    { CIPCODE, entry }
                                };
                                item.Items = dict;
                                ItemResponse<IPEDS_CDEP> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                                //Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                            }
                            else
                            {
                                val.Items.Add(CIPCODE, entry);
                                ItemResponse<IPEDS_CDEP> res = await localContainer.UpsertItemAsync(val, new PartitionKey(val.UNITID)); //await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                                //Console.WriteLine("Updated item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                                //Console.WriteLine("Found the item and then update it!");
                            }
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        //Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforDRVC(string containerId)
        {
            var filePath = $"{rootPath}DRVC2018.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? ASCDEG = RetrieveVal(reader.GetValue(1));
                    double? BASDEG = RetrieveVal(reader.GetValue(2));
                    double? MASDEG = RetrieveVal(reader.GetValue(3));
                    double? DOCDEGRS = RetrieveVal(reader.GetValue(4));
                    double? DOCDEGPP = RetrieveVal(reader.GetValue(5));
                    double? DOCDEGOT = RetrieveVal(reader.GetValue(6));
                    double? CERT1 = RetrieveVal(reader.GetValue(7));
                    double? CERT2 = RetrieveVal(reader.GetValue(8));
                    double? CERT4 = RetrieveVal(reader.GetValue(9));
                    double? PBACERT = RetrieveVal(reader.GetValue(10));
                    double? PMACERT = RetrieveVal(reader.GetValue(11));
                    double? SASCDEG = RetrieveVal(reader.GetValue(12));
                    double? SBASDEG = RetrieveVal(reader.GetValue(13));
                    double? SMASDEG = RetrieveVal(reader.GetValue(14));
                    double? SDOCDEG = RetrieveVal(reader.GetValue(15));
                    double? SCERT1 = RetrieveVal(reader.GetValue(16));
                    double? SCERT24 = RetrieveVal(reader.GetValue(17));
                    double? SBAMACRT = RetrieveVal(reader.GetValue(18));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_DRVC item = new IPEDS_DRVC
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR,
                            ASCDEG = ASCDEG,
                            BASDEG = BASDEG,
                            MASDEG = MASDEG,
                            DOCDEGRS = DOCDEGRS,
                            DOCDEGPP = DOCDEGPP,
                            DOCDEGOT = DOCDEGOT,
                            CERT1 = CERT1,
                            CERT2 = CERT2,
                            CERT4 = CERT4,
                            PBACERT = PBACERT,
                            PMACERT = PMACERT,
                            SASCDEG = SASCDEG,
                            SBASDEG = SBASDEG,
                            SMASDEG = SMASDEG,
                            SDOCDEG = SDOCDEG,
                            SCERT1 = SCERT1,
                            SCERT24 = SCERT24,
                            SBAMACRT = SBAMACRT
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            ItemResponse<IPEDS_DRVC> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforDRVEF(string containerId)
        {
            var filePath = $"{rootPath}DRVEF2018.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? ENRTOT = RetrieveVal(reader.GetValue(1));
                    double? ENRFT = RetrieveVal(reader.GetValue(3));
                    double? ENRPT = RetrieveVal(reader.GetValue(4));
                    double? PCTENRWH = RetrieveVal(reader.GetValue(5));
                    double? PCTENRBK = RetrieveVal(reader.GetValue(6));
                    double? PCTENRHS = RetrieveVal(reader.GetValue(7));
                    double? PCTENRAP = RetrieveVal(reader.GetValue(8));
                    double? PCTENRAS = RetrieveVal(reader.GetValue(9));
                    double? PCTENRAN = RetrieveVal(reader.GetValue(11));
                    double? PCTENRUN = RetrieveVal(reader.GetValue(13));
                    double? PCTENRNR = RetrieveVal(reader.GetValue(14));
                    double? PCTENRW = RetrieveVal(reader.GetValue(15));
                    double? EFUGFT = RetrieveVal(reader.GetValue(22));
                    double? PCUENRWH = RetrieveVal(reader.GetValue(32));
                    double? PCUENRBK = RetrieveVal(reader.GetValue(33));
                    double? PCUENRHS = RetrieveVal(reader.GetValue(34));
                    double? PCUENRAP = RetrieveVal(reader.GetValue(35));
                    double? PCUENRAS = RetrieveVal(reader.GetValue(36));
                    double? PCUENRAN = RetrieveVal(reader.GetValue(38));
                    double? PCUENR2M = RetrieveVal(reader.GetValue(39));
                    double? PCUENRUN = RetrieveVal(reader.GetValue(40));
                    double? PCUENRNR = RetrieveVal(reader.GetValue(41));
                    double? PCUENRW = RetrieveVal(reader.GetValue(42));
                    double? EFGRAD = RetrieveVal(reader.GetValue(43));
                    double? PCGENRWH = RetrieveVal(reader.GetValue(46));
                    double? PCGENRBK = RetrieveVal(reader.GetValue(47));
                    double? PCGENRHS = RetrieveVal(reader.GetValue(48));
                    double? PCGENRAP = RetrieveVal(reader.GetValue(49));
                    double? PCGENRAN = RetrieveVal(reader.GetValue(52));
                    double? PCGENR2M = RetrieveVal(reader.GetValue(53));
                    double? PCGENRUN = RetrieveVal(reader.GetValue(54));
                    double? PCGENRNR = RetrieveVal(reader.GetValue(55));
                    double? PCGENRW = RetrieveVal(reader.GetValue(56));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_DRVEF item = new IPEDS_DRVEF
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR,
                            ENRTOT = ENRTOT,
                            ENRFT = ENRFT,
                            ENRPT = ENRPT,
                            PCTENRWH = PCTENRWH,
                            PCTENRBK = PCTENRBK,
                            PCTENRHS = PCTENRHS,
                            PCTENRAP = PCTENRAP,
                            PCTENRAS = PCTENRAS,
                            PCTENRAN = PCTENRAN,
                            PCTENRUN = PCTENRUN,
                            PCTENRNR = PCTENRNR,
                            PCTENRW = PCTENRW,
                            EFUGFT = EFUGFT,
                            PCUENRWH = PCUENRWH,
                            PCUENRBK = PCUENRBK,
                            PCUENRHS = PCUENRHS,
                            PCUENRAP = PCUENRAP,
                            PCUENRAS = PCUENRAS,
                            PCUENRAN = PCUENRAN,
                            PCUENR2M = PCUENR2M,
                            PCUENRUN = PCUENRUN,
                            PCUENRNR = PCUENRNR,
                            PCUENRW = PCUENRW,
                            EFGRAD = EFGRAD,
                            PCGENRWH = PCGENRWH,
                            PCGENRBK = PCGENRBK,
                            PCGENRHS = PCGENRHS,
                            PCGENRAP = PCGENRAP,
                            PCGENRAN = PCGENRAN,
                            PCGENR2M = PCGENR2M,
                            PCGENRUN = PCGENRUN,
                            PCGENRNR = PCGENRNR,
                            PCGENRW = PCGENRW
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            ItemResponse<IPEDS_DRVEF> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforDRVGR(string containerId)
        {
            var filePath = $"{rootPath}DRVGR2018.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? GRRTTOT = RetrieveVal(reader.GetValue(1));
                    double? GRRTM = RetrieveVal(reader.GetValue(2));
                    double? GRRTW = RetrieveVal(reader.GetValue(3));
                    double? GRRTAN = RetrieveVal(reader.GetValue(4));
                    double? GRRTAP = RetrieveVal(reader.GetValue(5));
                    double? GRRTAS = RetrieveVal(reader.GetValue(6));
                    double? GRRTNH = RetrieveVal(reader.GetValue(7));
                    double? GRRTBK = RetrieveVal(reader.GetValue(8));
                    double? GRRTHS = RetrieveVal(reader.GetValue(9));
                    double? GRRTWH = RetrieveVal(reader.GetValue(10));
                    double? GRRT2M = RetrieveVal(reader.GetValue(11));
                    double? GRRTUN = RetrieveVal(reader.GetValue(12));
                    double? GRRTNR = RetrieveVal(reader.GetValue(13));
                    double? TRRTTOT = RetrieveVal(reader.GetValue(14));
                    double? GBA4RTT = RetrieveVal(reader.GetValue(15));
                    double? GBA5RTT = RetrieveVal(reader.GetValue(16));
                    double? GBA6RTT = RetrieveVal(reader.GetValue(17));
                    double? GBA6RTM = RetrieveVal(reader.GetValue(18));
                    double? GBA6RTW = RetrieveVal(reader.GetValue(19));
                    double? GBA6RTAN = RetrieveVal(reader.GetValue(20));
                    double? GBA6RTAP = RetrieveVal(reader.GetValue(21));
                    double? GBA6RTAS = RetrieveVal(reader.GetValue(22));
                    double? GBA6RTNH = RetrieveVal(reader.GetValue(23));
                    double? GBA6RTBK = RetrieveVal(reader.GetValue(24));
                    double? GBA6RTHS = RetrieveVal(reader.GetValue(25));
                    double? GBA6RTWH = RetrieveVal(reader.GetValue(26));
                    double? GBA6RT2M = RetrieveVal(reader.GetValue(27));
                    double? GBA6RTUN = RetrieveVal(reader.GetValue(28));
                    double? GBA6RTNR = RetrieveVal(reader.GetValue(29));
                    double? GBATRRT = RetrieveVal(reader.GetValue(30));


                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_DRVGR item = new IPEDS_DRVGR
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR,
                            GRRTTOT = GRRTTOT,
                            GRRTM = GRRTM,
                            GRRTW = GRRTW,
                            GRRTAN = GRRTAN,
                            GRRTAP = GRRTAP,
                            GRRTAS = GRRTAS,
                            GRRTNH = GRRTNH,
                            GRRTBK = GRRTBK,
                            GRRTHS = GRRTHS,
                            GRRTWH = GRRTWH,
                            GRRT2M = GRRT2M,
                            GRRTUN = GRRTUN,
                            GRRTNR = GRRTNR,
                            TRRTTOT = TRRTTOT,
                            GBA4RTT = GBA4RTT,
                            GBA5RTT = GBA5RTT,
                            GBA6RTT = GBA6RTT,
                            GBA6RTM = GBA6RTM,
                            GBA6RTW = GBA6RTW,
                            GBA6RTAN = GBA6RTAN,
                            GBA6RTAP = GBA6RTAP,
                            GBA6RTAS = GBA6RTAS,
                            GBA6RTNH = GBA6RTNH,
                            GBA6RTBK = GBA6RTBK,
                            GBA6RTHS = GBA6RTHS,
                            GBA6RTWH = GBA6RTWH,
                            GBA6RT2M = GBA6RT2M,
                            GBA6RTUN = GBA6RTUN,
                            GBA6RTNR = GBA6RTNR,
                            GBATRRT = GBATRRT
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            ItemResponse<IPEDS_DRVGR> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforDRVIC(string containerId)
        {
            var filePath = $"{rootPath}DRVIC2018.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? CINDON = RetrieveVal(reader.GetValue(5));
                    double? CINSON = RetrieveVal(reader.GetValue(6));
                    double? COTSON = RetrieveVal(reader.GetValue(7));
                    double? CINDOFF = RetrieveVal(reader.GetValue(8));
                    double? CINSOFF = RetrieveVal(reader.GetValue(9));
                    double? COTSOFF = RetrieveVal(reader.GetValue(10));
                    double? CINDFAM = RetrieveVal(reader.GetValue(11));
                    double? CINSFAM = RetrieveVal(reader.GetValue(12));
                    double? COTSFAM = RetrieveVal(reader.GetValue(13));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_DRVIC item = new IPEDS_DRVIC
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR,
                            CINDON = CINDON,
                            CINSON = CINSON,
                            COTSON = COTSON,
                            CINDOFF = CINDOFF,
                            CINSOFF = CINSOFF,
                            COTSOFF = COTSOFF,
                            CINDFAM = CINDFAM,
                            CINSFAM = CINSFAM,
                            COTSFAM = COTSFAM
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            ItemResponse<IPEDS_DRVIC> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforIC(string containerId)
        {
            var filePath = $"{rootPath}IC2018.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? FT_UG = RetrieveVal(reader.GetValue(24));
                    double? FT_FTUG = RetrieveVal(reader.GetValue(25));
                    double? PT_UG = RetrieveVal(reader.GetValue(27));
                    double? PT_FTUG = RetrieveVal(reader.GetValue(28));
                    double? ROOM = RetrieveVal(reader.GetValue(89));
                    double? ROOMCAP = RetrieveVal(reader.GetValue(90));
                    double? BOARD = RetrieveVal(reader.GetValue(91));
                    double? ROOMAMT = RetrieveVal(reader.GetValue(93));
                    double? BOARDAMT = RetrieveVal(reader.GetValue(94));
                    double? RMBRDAMT = RetrieveVal(reader.GetValue(95));
                    double? APPLFEEU = RetrieveVal(reader.GetValue(96));
                    double? APPLFEEG = RetrieveVal(reader.GetValue(97));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_IC item = new IPEDS_IC
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR,
                            FT_UG = FT_UG,
                            FT_FTUG = FT_FTUG,
                            PT_UG = PT_UG,
                            PT_FTUG = PT_FTUG,
                            ROOM = ROOM,
                            ROOMCAP = ROOMCAP,
                            BOARD = BOARD,
                            ROOMAMT = ROOMAMT,
                            BOARDAMT = BOARDAMT,
                            RMBRDAMT = RMBRDAMT,
                            APPLFEEU = APPLFEEU,
                            APPLFEEG = APPLFEEG
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            ItemResponse<IPEDS_IC> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforICAY(string containerId)
        {
            var filePath = $"{rootPath}IC2018_AY.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? TUITION1 = RetrieveVal(reader.GetValue(1));
                    double? FEE1 = RetrieveVal(reader.GetValue(2));
                    double? HRCHG1 = RetrieveVal(reader.GetValue(3));
                    double? TUITION2 = RetrieveVal(reader.GetValue(4));
                    double? FEE2 = RetrieveVal(reader.GetValue(5));
                    double? HRCHG2 = RetrieveVal(reader.GetValue(6));
                    double? TUITION3 = RetrieveVal(reader.GetValue(7));
                    double? FEE3 = RetrieveVal(reader.GetValue(8));
                    double? HRCHG3 = RetrieveVal(reader.GetValue(9));
                    double? TUITION5 = RetrieveVal(reader.GetValue(10));
                    double? FEE5 = RetrieveVal(reader.GetValue(11));
                    double? HRCHG5 = RetrieveVal(reader.GetValue(12));
                    double? TUITION6 = RetrieveVal(reader.GetValue(13));
                    double? FEE6 = RetrieveVal(reader.GetValue(14));
                    double? HRCHG6 = RetrieveVal(reader.GetValue(15));
                    double? TUITION7 = RetrieveVal(reader.GetValue(16));
                    double? FEE7 = RetrieveVal(reader.GetValue(17));
                    double? HRCHG7 = RetrieveVal(reader.GetValue(18));
                    double? ISPROF1 = RetrieveVal(reader.GetValue(19));
                    double? ISPFEE1 = RetrieveVal(reader.GetValue(20));
                    double? OSPROF1 = RetrieveVal(reader.GetValue(21));
                    double? OSPFEE1 = RetrieveVal(reader.GetValue(22));
                    double? ISPROF2 = RetrieveVal(reader.GetValue(23));
                    double? ISPFEE2 = RetrieveVal(reader.GetValue(24));
                    double? OSPROF2 = RetrieveVal(reader.GetValue(25));
                    double? OSPFEE2 = RetrieveVal(reader.GetValue(26));
                    double? ISPROF3 = RetrieveVal(reader.GetValue(27));
                    double? ISPFEE3 = RetrieveVal(reader.GetValue(28));
                    double? OSPROF3 = RetrieveVal(reader.GetValue(29));
                    double? OSPFEE3 = RetrieveVal(reader.GetValue(30));
                    double? ISPROF4 = RetrieveVal(reader.GetValue(31));
                    double? ISPFEE4 = RetrieveVal(reader.GetValue(32));
                    double? OSPROF4 = RetrieveVal(reader.GetValue(33));
                    double? OSPFEE4 = RetrieveVal(reader.GetValue(34));
                    double? ISPROF5 = RetrieveVal(reader.GetValue(35));
                    double? ISPFEE5 = RetrieveVal(reader.GetValue(36));
                    double? OSPROF5 = RetrieveVal(reader.GetValue(37));
                    double? OSPFEE5 = RetrieveVal(reader.GetValue(38));
                    double? ISPROF6 = RetrieveVal(reader.GetValue(39));
                    double? ISPFEE6 = RetrieveVal(reader.GetValue(40));
                    double? OSPROF6 = RetrieveVal(reader.GetValue(41));
                    double? OSPFEE6 = RetrieveVal(reader.GetValue(42));
                    double? ISPROF7 = RetrieveVal(reader.GetValue(43));
                    double? ISPFEE7 = RetrieveVal(reader.GetValue(44));
                    double? OSPROF7 = RetrieveVal(reader.GetValue(45));
                    double? OSPFEE7 = RetrieveVal(reader.GetValue(46));
                    double? ISPROF8 = RetrieveVal(reader.GetValue(47));
                    double? ISPFEE8 = RetrieveVal(reader.GetValue(48));
                    double? OSPROF8 = RetrieveVal(reader.GetValue(49));
                    double? OSPFEE8 = RetrieveVal(reader.GetValue(50));
                    double? ISPROF9 = RetrieveVal(reader.GetValue(51));
                    double? ISPFEE9 = RetrieveVal(reader.GetValue(52));
                    double? OSPROF9 = RetrieveVal(reader.GetValue(53));
                    double? OSPFEE9 = RetrieveVal(reader.GetValue(54));
                    double? CHG1AT0 = RetrieveVal(reader.GetValue(55));
                    double? CHG1AF0 = RetrieveVal(reader.GetValue(56));
                    double? CHG1AY0 = RetrieveVal(reader.GetValue(57));
                    double? CHG1AT1 = RetrieveVal(reader.GetValue(58));
                    double? CHG1AF1 = RetrieveVal(reader.GetValue(59));
                    double? CHG1AY1 = RetrieveVal(reader.GetValue(60));
                    double? CHG1AT2 = RetrieveVal(reader.GetValue(61));
                    double? CHG1AF2 = RetrieveVal(reader.GetValue(62));
                    double? CHG1AY2 = RetrieveVal(reader.GetValue(63));
                    double? CHG1AT3 = RetrieveVal(reader.GetValue(64));
                    double? CHG1AF3 = RetrieveVal(reader.GetValue(65));
                    double? CHG1AY3 = RetrieveVal(reader.GetValue(66));
                    double? CHG1TGTD = RetrieveVal(reader.GetValue(67));
                    double? CHG1FGTD = RetrieveVal(reader.GetValue(68));
                    double? CHG2AT0 = RetrieveVal(reader.GetValue(69));
                    double? CHG2AF0 = RetrieveVal(reader.GetValue(70));
                    double? CHG2AY0 = RetrieveVal(reader.GetValue(71));
                    double? CHG2AT1 = RetrieveVal(reader.GetValue(72));
                    double? CHG2AF1 = RetrieveVal(reader.GetValue(73));
                    double? CHG2AY1 = RetrieveVal(reader.GetValue(74));
                    double? CHG2AT2 = RetrieveVal(reader.GetValue(75));
                    double? CHG2AF2 = RetrieveVal(reader.GetValue(76));
                    double? CHG2AY2 = RetrieveVal(reader.GetValue(77));
                    double? CHG2AT3 = RetrieveVal(reader.GetValue(78));
                    double? CHG2AF3 = RetrieveVal(reader.GetValue(79));
                    double? CHG2AY3 = RetrieveVal(reader.GetValue(80));
                    double? CHG2TGTD = RetrieveVal(reader.GetValue(81));
                    double? CHG2FGTD = RetrieveVal(reader.GetValue(82));
                    double? CHG3AT0 = RetrieveVal(reader.GetValue(83));
                    double? CHG3AF0 = RetrieveVal(reader.GetValue(84));
                    double? CHG3AY0 = RetrieveVal(reader.GetValue(85));
                    double? CHG3AT1 = RetrieveVal(reader.GetValue(86));
                    double? CHG3AF1 = RetrieveVal(reader.GetValue(87));
                    double? CHG3AY1 = RetrieveVal(reader.GetValue(88));
                    double? CHG3AT2 = RetrieveVal(reader.GetValue(89));
                    double? CHG3AF2 = RetrieveVal(reader.GetValue(90));
                    double? CHG3AY2 = RetrieveVal(reader.GetValue(91));
                    double? CHG3AT3 = RetrieveVal(reader.GetValue(92));
                    double? CHG3AF3 = RetrieveVal(reader.GetValue(93));
                    double? CHG3AY3 = RetrieveVal(reader.GetValue(94));
                    double? CHG3TGTD = RetrieveVal(reader.GetValue(95));
                    double? CHG3FGTD = RetrieveVal(reader.GetValue(96));
                    double? CHG4AY0 = RetrieveVal(reader.GetValue(97));
                    double? CHG4AY1 = RetrieveVal(reader.GetValue(98));
                    double? CHG4AY2 = RetrieveVal(reader.GetValue(99));
                    double? CHG4AY3 = RetrieveVal(reader.GetValue(100));
                    double? CHG5AY0 = RetrieveVal(reader.GetValue(101));
                    double? CHG5AY1 = RetrieveVal(reader.GetValue(102));
                    double? CHG5AY2 = RetrieveVal(reader.GetValue(103));
                    double? CHG5AY3 = RetrieveVal(reader.GetValue(104));
                    double? CHG6AY0 = RetrieveVal(reader.GetValue(105));
                    double? CHG6AY1 = RetrieveVal(reader.GetValue(106));
                    double? CHG6AY2 = RetrieveVal(reader.GetValue(107));
                    double? CHG6AY3 = RetrieveVal(reader.GetValue(108));
                    double? CHG7AY0 = RetrieveVal(reader.GetValue(109));
                    double? CHG7AY1 = RetrieveVal(reader.GetValue(110));
                    double? CHG7AY2 = RetrieveVal(reader.GetValue(111));
                    double? CHG7AY3 = RetrieveVal(reader.GetValue(112));
                    double? CHG8AY0 = RetrieveVal(reader.GetValue(113));
                    double? CHG8AY1 = RetrieveVal(reader.GetValue(114));
                    double? CHG8AY2 = RetrieveVal(reader.GetValue(115));
                    double? CHG8AY3 = RetrieveVal(reader.GetValue(116));
                    double? CHG9AY0 = RetrieveVal(reader.GetValue(117));
                    double? CHG9AY1 = RetrieveVal(reader.GetValue(118));
                    double? CHG9AY2 = RetrieveVal(reader.GetValue(119));
                    double? CHG9AY3 = RetrieveVal(reader.GetValue(120));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        var partitionKey = new PartitionKey(unitid);
                        IPEDS_ICAY item = new IPEDS_ICAY
                        {
                            Id = id,
                            UNITID = unitid,
                            year = CURRENTYEAR,
                            TUITION1 = TUITION1,
                            FEE1 = FEE1,
                            HRCHG1 = HRCHG1,
                            TUITION2 = TUITION2,
                            FEE2 = FEE2,
                            HRCHG2 = HRCHG2,
                            TUITION3 = TUITION3,
                            FEE3 = FEE3,
                            HRCHG3 = HRCHG3,
                            TUITION5 = TUITION5,
                            FEE5 = FEE5,
                            HRCHG5 = HRCHG5,
                            TUITION6 = TUITION6,
                            FEE6 = FEE6,
                            HRCHG6 = HRCHG6,
                            TUITION7 = TUITION7,
                            FEE7 = FEE7,
                            HRCHG7 = HRCHG7,
                            ISPROF1 = ISPROF1,
                            ISPFEE1 = ISPFEE1,
                            OSPROF1 = OSPROF1,
                            OSPFEE1 = OSPFEE1,
                            ISPROF2 = ISPROF2,
                            ISPFEE2 = ISPFEE2,
                            OSPROF2 = OSPROF2,
                            OSPFEE2 = OSPFEE2,
                            ISPROF3 = ISPROF3,
                            ISPFEE3 = ISPFEE3,
                            OSPROF3 = OSPROF3,
                            OSPFEE3 = OSPFEE3,
                            ISPROF4 = ISPROF4,
                            ISPFEE4 = ISPFEE4,
                            OSPROF4 = OSPROF4,
                            OSPFEE4 = OSPFEE4,
                            ISPROF5 = ISPROF5,
                            ISPFEE5 = ISPFEE5,
                            OSPROF5 = OSPROF5,
                            OSPFEE5 = OSPFEE5,
                            ISPROF6 = ISPROF6,
                            ISPFEE6 = ISPFEE6,
                            OSPROF6 = OSPROF6,
                            OSPFEE6 = OSPFEE6,
                            ISPROF7 = ISPROF7,
                            ISPFEE7 = ISPFEE7,
                            OSPROF7 = OSPROF7,
                            OSPFEE7 = OSPFEE7,
                            ISPROF8 = ISPROF8,
                            ISPFEE8 = ISPFEE8,
                            OSPROF8 = OSPROF8,
                            OSPFEE8 = OSPFEE8,
                            ISPROF9 = ISPROF9,
                            ISPFEE9 = ISPFEE9,
                            OSPROF9 = OSPROF9,
                            OSPFEE9 = OSPFEE9,
                            CHG1AT0 = CHG1AT0,
                            CHG1AF0 = CHG1AF0,
                            CHG1AY0 = CHG1AY0,
                            CHG1AT1 = CHG1AT1,
                            CHG1AF1 = CHG1AF1,
                            CHG1AY1 = CHG1AY1,
                            CHG1AT2 = CHG1AT2,
                            CHG1AF2 = CHG1AF2,
                            CHG1AY2 = CHG1AY2,
                            CHG1AT3 = CHG1AT3,
                            CHG1AF3 = CHG1AF3,
                            CHG1AY3 = CHG1AY3,
                            CHG1TGTD = CHG1TGTD,
                            CHG1FGTD = CHG1FGTD,
                            CHG2AT0 = CHG2AT0,
                            CHG2AF0 = CHG2AF0,
                            CHG2AY0 = CHG2AY0,
                            CHG2AT1 = CHG2AT1,
                            CHG2AF1 = CHG2AF1,
                            CHG2AY1 = CHG2AY1,
                            CHG2AT2 = CHG2AT2,
                            CHG2AF2 = CHG2AF2,
                            CHG2AY2 = CHG2AY2,
                            CHG2AT3 = CHG2AT3,
                            CHG2AF3 = CHG2AF3,
                            CHG2AY3 = CHG2AY3,
                            CHG2TGTD = CHG2TGTD,
                            CHG2FGTD = CHG2FGTD,
                            CHG3AT0 = CHG3AT0,
                            CHG3AF0 = CHG3AF0,
                            CHG3AY0 = CHG3AY0,
                            CHG3AT1 = CHG3AT1,
                            CHG3AF1 = CHG3AF1,
                            CHG3AY1 = CHG3AY1,
                            CHG3AT2 = CHG3AT2,
                            CHG3AF2 = CHG3AF2,
                            CHG3AY2 = CHG3AY2,
                            CHG3AT3 = CHG3AT3,
                            CHG3AF3 = CHG3AF3,
                            CHG3AY3 = CHG3AY3,
                            CHG3TGTD = CHG3TGTD,
                            CHG3FGTD = CHG3FGTD,
                            CHG4AY0 = CHG4AY0,
                            CHG4AY1 = CHG4AY1,
                            CHG4AY2 = CHG4AY2,
                            CHG4AY3 = CHG4AY3,
                            CHG5AY0 = CHG5AY0,
                            CHG5AY1 = CHG5AY1,
                            CHG5AY2 = CHG5AY2,
                            CHG5AY3 = CHG5AY3,
                            CHG6AY0 = CHG6AY0,
                            CHG6AY1 = CHG6AY1,
                            CHG6AY2 = CHG6AY2,
                            CHG6AY3 = CHG6AY3,
                            CHG7AY0 = CHG7AY0,
                            CHG7AY1 = CHG7AY1,
                            CHG7AY2 = CHG7AY2,
                            CHG7AY3 = CHG7AY3,
                            CHG8AY0 = CHG8AY0,
                            CHG8AY1 = CHG8AY1,
                            CHG8AY2 = CHG8AY2,
                            CHG8AY3 = CHG8AY3,
                            CHG9AY0 = CHG9AY0,
                            CHG9AY1 = CHG9AY1,
                            CHG9AY2 = CHG9AY2,
                            CHG9AY3 = CHG9AY3,
                        };
                        try
                        {
                            //Console.WriteLine("current item: " + item.UNITID + " with actpct: " + item.ACTPCT);
                            ItemResponse<IPEDS_ICAY> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                            Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                        }
                        catch (CosmosException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());
        }

        public async Task AddIPEDSYearlyDataforSSIS(string containerId)
        {
            var filePath = $"{rootPath}S2018_SIS.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            int count = 0;
            do
            {
                while (reader.Read())
                {
                    count++;
                    //Console.WriteLine("current line: " + count);
                    if (count == 1)
                        continue;
                    var unitid = reader.GetDouble(0).ToString();
                    double? FACSTAT = RetrieveVal(reader.GetValue(1));
                    double? SISTOTL = RetrieveVal(reader.GetValue(2));
                    double? SISPROF = RetrieveVal(reader.GetValue(3));
                    double? SISASCP = RetrieveVal(reader.GetValue(4));
                    double? SISASTP = RetrieveVal(reader.GetValue(5));
                    double? SISINST = RetrieveVal(reader.GetValue(6));
                    double? SISLECT = RetrieveVal(reader.GetValue(7));

                    var id = (count - 1).ToString();
                    var idNum = count - 1;
                    var localContainer = GetContainer(containerId);
                    if (idNum < 30000)
                    {
                        if (FACSTAT == 0)
                        {
                            var partitionKey = new PartitionKey(unitid);
                            IPEDS_SSIS item = new IPEDS_SSIS
                            {
                                Id = id,
                                UNITID = unitid,
                                year = CURRENTYEAR,
                                SISTOTL = SISTOTL,
                                SISPROF = SISPROF,
                                SISASCP = SISASCP,
                                SISASTP = SISASTP,
                                SISINST = SISINST,
                                SISLECT = SISLECT
                            };
                            try
                            {
                                //Console.WriteLine("current item: " + item.UNITID + " with ltcrclt: " + item.LTCRCLT);
                                ItemResponse<IPEDS_SSIS> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                            }
                            catch (CosmosException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            Console.WriteLine();
                        }
                    }

                }
            } while (reader.NextResult());
        }

        /// <summary>
        /// Add yearly data from college scorecard 
        /// </summary>
        /// <returns></returns>
        public async Task AddScorecardYearlyData()
        {
            using StreamReader sr = new StreamReader("c:/Users/Administrator/Downloads/CollegeScorecard-data-12022020.csv");
            var num = 0;
            var localContainer = database.GetContainer("CollegeDataUSYearly");
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while ((currentLine = sr.ReadLine()) != null)
            {
                num++;
                if (num == 1)
                    continue;
                Console.WriteLine("Working on line number: " + (num - 1));
                var vals = currentLine.Split(',');
                var UNITID = vals[0];
                var ADM_RATE = vals[36];
                var ADM_RATE_ALL = vals[37];
                var SATVR25 = vals[38];
                var SATVR75 = vals[39];
                var SATMT25 = vals[40];
                var SATMT75 = vals[41];
                var SATWR25 = vals[42];
                var SATWR75 = vals[43];
                var SATVRMID = vals[44];
                var SATMTMID = vals[45];
                var SATWRMID = vals[46];
                var ACTCM25 = vals[47];
                var ACTCM75 = vals[48];
                var ACTEN25 = vals[49];
                var ACTEN75 = vals[50];
                var ACTMT25 = vals[51];
                var ACTMT75 = vals[52];
                var ACTWR25 = vals[53];
                var ACTWR75 = vals[54];
                var ACTCMMID = vals[55];
                var ACTENMID = vals[56];
                var ACTMTMID = vals[57];
                var ACTWRMID = vals[58];
                var SAT_AVG = vals[59];
                var UGDS = vals[290];
                var UGDS_WHITE = vals[292];
                var UGDS_BLACK = vals[293];
                var UGDS_HISP = vals[294];
                var UGDS_ASIAN = vals[295];
                var UGDS_AIAN = vals[296];
                var UGDS_NHPI = vals[297];
                var UGDS_2MOR = vals[298];
                var UGDS_NRA = vals[299];
                var UGDS_UNKN = vals[300];
                var UGDS_WHITENH = vals[301];
                var UGDS_BLACKNH = vals[302];
                var UGDS_API = vals[303];
                var NPT4_PUB = vals[316];
                var NPT4_PRIV = vals[317];
                var NPT41_PUB = vals[320];
                var NPT42_PUB = vals[321];
                var NPT43_PUB = vals[322];
                var NPT44_PUB = vals[323];
                var NPT45_PUB = vals[324];
                var NPT41_PRIV = vals[325];
                var NPT42_PRIV = vals[326];
                var NPT43_PRIV = vals[327];
                var NPT44_PRIV = vals[328];
                var NPT45_PRIV = vals[329];
                var NUM4_PUB = vals[352];
                var NUM4_PRIV = vals[353];
                var NUM41_PUB = vals[356];
                var NUM42_PUB = vals[357];
                var NUM43_PUB = vals[358];
                var NUM44_PUB = vals[359];
                var NUM45_PUB = vals[360];
                var NUM41_PRIV = vals[361];
                var NUM42_PRIV = vals[362];
                var NUM43_PRIV = vals[363];
                var NUM44_PRIV = vals[364];
                var NUM45_PRIV = vals[365];
                var COSTT4_A = vals[376];
                var COSTT4_P = vals[377];
                var TUITIONFEE_IN = vals[378];
                var TUITIONFEE_OUT = vals[379];
                var FEMALE = vals[1609];
                var MARRIED = vals[1610];
                var DEPENDENT = vals[1611];
                var VETERAN = vals[1612];
                var FIRST_GEN = vals[1613];
                var FAMINC = vals[1614];
                var MD_FAMINC = vals[1615];
                var MEDIAN_HH_INC = vals[1626];
                var POVERTY_RATE = vals[1627];
                var UNEMP_RATE = vals[1628];
                var GRAD_DEBT_MDN_SUPP = vals[1709];
                var UGDS_MEN = vals[1739];
                var UGDS_WOMEN = vals[1740];
                var GRADS = vals[1775];
                var COUNT_NWNE_P10 = vals[1636];
                var COUNT_WNE_P10 = vals[1637];
                var MN_EARN_WNE_P10 = vals[1638];
                var MD_EARN_WNE_P10 = vals[1639];
                var COUNT_NWNE_P6 = vals[1662];
                var COUNT_WNE_P6 = vals[1663];
                var MN_EARN_WNE_P6 = vals[1664];
                var MD_EARN_WNE_P6 = vals[1665];
                var COUNT_NWNE_P8 = vals[1693];
                var COUNT_WNE_P8 = vals[1694];
                var MN_EARN_WNE_P8 = vals[1695];
                var MD_EARN_WNE_P8 = vals[1696];

                CollegeDataUSYearly_Scorecard obj = new CollegeDataUSYearly_Scorecard
                {
                    Year = 2020,
                    ADM_RATE = ParseDoubleFromString(ADM_RATE),
                    ADM_RATE_ALL = ParseDoubleFromString(ADM_RATE_ALL),
                    SATVR25 = ParseDoubleFromString(SATVR25),
                    SATVR75 = ParseDoubleFromString(SATVR75),
                    SATMT25 = ParseDoubleFromString(SATMT25),
                    SATMT75 = ParseDoubleFromString(SATMT75),
                    SATWR25 = ParseDoubleFromString(SATWR25),
                    SATWR75 = ParseDoubleFromString(SATWR75),
                    SATVRMID = ParseDoubleFromString(SATVRMID),
                    SATMTMID = ParseDoubleFromString(SATMTMID),
                    SATWRMID = ParseDoubleFromString(SATWRMID),
                    ACTCM25 = ParseDoubleFromString(ACTCM25),
                    ACTCM75 = ParseDoubleFromString(ACTCM75),
                    ACTEN25 = ParseDoubleFromString(ACTEN25),
                    ACTEN75 = ParseDoubleFromString(ACTEN75),
                    ACTMT25 = ParseDoubleFromString(ACTMT25),
                    ACTMT75 = ParseDoubleFromString(ACTMT75),
                    ACTWR25 = ParseDoubleFromString(ACTWR25),
                    ACTWR75 = ParseDoubleFromString(ACTWR75),
                    ACTCMMID = ParseDoubleFromString(ACTCMMID),
                    ACTENMID = ParseDoubleFromString(ACTENMID),
                    ACTMTMID = ParseDoubleFromString(ACTMTMID),
                    ACTWRMID = ParseDoubleFromString(ACTWRMID),
                    SAT_AVG = ParseDoubleFromString(SAT_AVG),
                    UGDS = ParseDoubleFromString(UGDS),
                    UGDS_WHITE = ParseDoubleFromString(UGDS_WHITE),
                    UGDS_BLACK = ParseDoubleFromString(UGDS_BLACK),
                    UGDS_HISP = ParseDoubleFromString(UGDS_HISP),
                    UGDS_ASIAN = ParseDoubleFromString(UGDS_ASIAN),
                    UGDS_AIAN = ParseDoubleFromString(UGDS_AIAN),
                    UGDS_NHPI = ParseDoubleFromString(UGDS_NHPI),
                    UGDS_2MOR = ParseDoubleFromString(UGDS_2MOR),
                    UGDS_NRA = ParseDoubleFromString(UGDS_NRA),
                    UGDS_UNKN = ParseDoubleFromString(UGDS_UNKN),
                    UGDS_WHITENH = ParseDoubleFromString(UGDS_WHITENH),
                    UGDS_BLACKNH = ParseDoubleFromString(UGDS_BLACKNH),
                    UGDS_API = ParseDoubleFromString(UGDS_API),
                    NPT4_PUB = ParseDoubleFromString(NPT4_PUB),
                    NPT4_PRIV = ParseDoubleFromString(NPT4_PRIV),
                    NPT41_PUB = ParseDoubleFromString(NPT41_PUB),
                    NPT42_PUB = ParseDoubleFromString(NPT42_PUB),
                    NPT43_PUB = ParseDoubleFromString(NPT43_PUB),
                    NPT44_PUB = ParseDoubleFromString(NPT44_PUB),
                    NPT45_PUB = ParseDoubleFromString(NPT45_PUB),
                    NPT41_PRIV = ParseDoubleFromString(NPT41_PRIV),
                    NPT42_PRIV = ParseDoubleFromString(NPT42_PRIV),
                    NPT43_PRIV = ParseDoubleFromString(NPT43_PRIV),
                    NPT44_PRIV = ParseDoubleFromString(NPT44_PRIV),
                    NPT45_PRIV = ParseDoubleFromString(NPT45_PRIV),
                    NUM4_PUB = ParseDoubleFromString(NUM4_PUB),
                    NUM41_PUB = ParseDoubleFromString(NUM41_PUB),
                    NUM42_PUB = ParseDoubleFromString(NUM42_PUB),
                    NUM43_PUB = ParseDoubleFromString(NUM43_PUB),
                    NUM44_PUB = ParseDoubleFromString(NUM44_PUB),
                    NUM45_PUB = ParseDoubleFromString(NUM45_PUB),
                    NUM41_PRIV = ParseDoubleFromString(NUM41_PRIV),
                    NUM42_PRIV = ParseDoubleFromString(NUM42_PRIV),
                    NUM43_PRIV = ParseDoubleFromString(NUM43_PRIV),
                    NUM44_PRIV = ParseDoubleFromString(NUM44_PRIV),
                    NUM45_PRIV = ParseDoubleFromString(NUM45_PRIV),
                    NUM4_PRIV = ParseDoubleFromString(NUM4_PRIV),
                    COSTT4_A = ParseDoubleFromString(COSTT4_A),
                    COSTT4_P = ParseDoubleFromString(COSTT4_P),
                    TUITIONFEE_IN = ParseDoubleFromString(TUITIONFEE_IN),
                    TUITIONFEE_OUT = ParseDoubleFromString(TUITIONFEE_OUT),
                    FEMALE = ParseDoubleFromString(FEMALE),
                    MARRIED = ParseDoubleFromString(MARRIED),
                    DEPENDENT = ParseDoubleFromString(DEPENDENT),
                    VETERAN = ParseDoubleFromString(VETERAN),
                    FIRST_GEN = ParseDoubleFromString(FIRST_GEN),
                    FAMINC = ParseDoubleFromString(FAMINC),
                    MD_FAMINC = ParseDoubleFromString(MD_FAMINC),
                    MEDIAN_HH_INC = ParseDoubleFromString(MEDIAN_HH_INC),
                    POVERTY_RATE = ParseDoubleFromString(POVERTY_RATE),
                    UNEMP_RATE = ParseDoubleFromString(UNEMP_RATE),
                    GRAD_DEBT_MDN_SUPP = ParseDoubleFromString(GRAD_DEBT_MDN_SUPP),
                    UGDS_MEN = ParseDoubleFromString(UGDS_MEN),
                    UGDS_WOMEN = ParseDoubleFromString(UGDS_WOMEN),
                    GRADS = ParseDoubleFromString(GRADS),
                    COUNT_NWNE_P10 = ParseDoubleFromString(COUNT_NWNE_P10),
                    COUNT_WNE_P10 = ParseDoubleFromString(COUNT_WNE_P10),
                    MN_EARN_WNE_P10 = ParseDoubleFromString(MN_EARN_WNE_P10),
                    MD_EARN_WNE_P10 = ParseDoubleFromString(MD_EARN_WNE_P10),
                    COUNT_NWNE_P6 = ParseDoubleFromString(COUNT_NWNE_P6),
                    COUNT_WNE_P6 = ParseDoubleFromString(COUNT_WNE_P6),
                    MN_EARN_WNE_P6 = ParseDoubleFromString(MN_EARN_WNE_P6),
                    MD_EARN_WNE_P6 = ParseDoubleFromString(MD_EARN_WNE_P6),
                    COUNT_NWNE_P8 = ParseDoubleFromString(COUNT_NWNE_P8),
                    COUNT_WNE_P8 = ParseDoubleFromString(COUNT_WNE_P8),
                    MN_EARN_WNE_P8 = ParseDoubleFromString(MN_EARN_WNE_P8),
                    MD_EARN_WNE_P8 = ParseDoubleFromString(MD_EARN_WNE_P8)
                };
                CollegeDataUSYearly_Scorecard[] ScoreCardArr = new CollegeDataUSYearly_Scorecard[] { obj };

                CollegeDataUSYearly item = new CollegeDataUSYearly
                {

                    UNITID = UNITID,
                    Id = (num - 1).ToString(),
                    ScoreCard = ScoreCardArr
                };

                if (num < 30000)
                {
                    Console.WriteLine($"unitid: {UNITID}");
                    ItemResponse<CollegeDataUSYearly> res = await localContainer.CreateItemAsync(item, new PartitionKey(item.UNITID));
                    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
                }
            }
        }

        public async Task StartOperationAsync()
        {
            // Create a new instance of the Cosmos Client
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            GetDatabase();
            //await CreateDatabaseAsync();
            //await CreateContainerAsync();
        }
        // -----------------------------------------------------------------
        // Below are the helper methods
        // -----------------------------------------------------------------

        /// <summary>
        /// Return degree string with input value of HDEGOFR1 in IPEDS HD table
        /// </summary>
        private string GetDegree(string input)
        {
            string degree;
            switch (int.Parse(input))
            {
                case 11:
                case 12:
                case 13:
                case 14:
                    degree = "Doctor";
                    break;
                case 20:
                    degree = "Master";
                    break;
                case 30:
                    degree = "Bachelor";
                    break;
                case 40:
                    degree = "Associate";
                    break;
                case 0:
                case -3:
                    degree = "None";
                    break;
                default:
                    degree = "Wrong";
                    break;
            }
            return degree;
        }

        private void GetDatabase()
        {
            database = cosmosClient.GetDatabase(databaseId);
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", database.Id);
        }

        private Container GetContainer(string containerId)
        {
            return database.GetContainer(containerId);
        }

        private async Task CreateContainerAsync()
        {
            // Create a new container
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/UNITID");
            //Console.WriteLine("Created Container: {0}\n", container.Id);
        }

        /// <summary>
        /// Run a query (using Azure Cosmos DB SQL syntax) against the container
        /// </summary>
        private async Task<T> QueryTableAsync<T>(string containerName, string partitionKey)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.UNITID = '{partitionKey}'";
            //Console.WriteLine("Running query: {0}\n", sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var localContainer = database.GetContainer(containerName);
            var it = localContainer.GetItemQueryIterator<T>(queryDefinition);
            if (!it.HasMoreResults)
            {
                //Console.WriteLine($"UNITID {partitionKey} is not found.");
                return default;
            }
            T item = default;
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                if (res.Count == 0)
                {
                    //Console.WriteLine($"College with UNITID {partitionKey} is not found.");
                    return default;
                }
                foreach (var val in res)
                {
                    item = val;
                }
            }
            return item;
        }

        private async Task<T> QueryTableCustomConditionAsync<T>(string containerName, string condition)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE {condition}";
            //Console.WriteLine("Running query: {0}\n", sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var localContainer = database.GetContainer(containerName);
            var it = localContainer.GetItemQueryIterator<T>(queryDefinition);
            if (!it.HasMoreResults)
            {
                //Console.WriteLine($"UNITID {partitionKey} is not found.");
                return default;
            }
            T item = default;
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                if (res.Count == 0)
                {
                    //Console.WriteLine($"College with UNITID {partitionKey} is not found.");
                    return default;
                }
                foreach (var val in res)
                {
                    item = val;
                }
            }
            return item;
        }

        /// <summary>
        /// Run a query (using Azure Cosmos DB SQL syntax) against the container
        /// </summary>
        private async Task<CollegeDataUS> QueryItemsAsync(string partitionKey)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.UNITID = '{partitionKey}'";
            //Console.WriteLine("Running query: {0}\n", sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = container.GetItemQueryIterator<CollegeDataUS>(queryDefinition);
            if (!it.HasMoreResults)
            {
                Console.WriteLine($"College with UNITID {partitionKey} is not found.");
                return null;
            }
            CollegeDataUS college = null;
            while (it.HasMoreResults)
            {
                var res = await it.ReadNextAsync();
                if (res.Count == 0)
                {
                    Console.WriteLine($"College with UNITID {partitionKey} is not found.");
                    return null;
                }
                foreach (var item in res)
                {
                    college = item;
                    Console.WriteLine($"Found college {college.INSTNM}");
                }
            }
            return college;
        }

        /// <summary>
        /// Add items to the container
        /// </summary>
        private async Task AddItemsToContainerAsync(CollegeDataUS college)
        {
            try
            {
                ItemResponse<CollegeDataUS> res = await container.CreateItemAsync(college, new PartitionKey(college.UNITID));
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private double? RetrieveVal(object val)
        {
            double? result;
            if (val == null)
                result = null;
            else
                result = (double)val;
            return result;
        }

        private double? ParseDoubleFromString(string val)
        {
            double? result = null;
            if (val == null)
                result = null;
            else
            {
                if (val == "NULL" || val == "PrivacySuppressed")
                    result = null;
                else
                    try
                    {
                        result = double.Parse(val);
                    } catch(FormatException e)
                    {
                        Console.WriteLine("Now input is: " + val);
                        Console.WriteLine(e);
                    }
            }
            return result;
        }
    }
}
