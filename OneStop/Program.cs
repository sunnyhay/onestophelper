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

namespace OneStopHelper
{
    public class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://localhost:8081";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "";

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
        private readonly string commonDatasetFilePath = "c:/Users/Administrator/Documents/CommonDataset";
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
                //await p.UpdateYearlyData("IPEDSCDEP");
                //await p.ValidateRankingData();
                //await p.UpdateCommonDataset();
                await p.UpdateYearlDataWithCommonDataset();
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

            for(int i = 0; i < universities.Count; i++)
            {
                var university = universities[i];
                var univName = university.INSTNM;
                var condition = $"c.INSTNM = \"{univName}\"";
                var targetItem = await QueryTableCustomConditionAsync<CollegeDataUS>("CollegeDataUS", condition);
                if (targetItem == default)
                {
                    Console.WriteLine($"Not found the university UNITID for {univName} with rank {university.Rank}");
                    university.UNITID = "UNKNOWN";
                } else
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
            
            for(int i = 0; i < list.Count; i++)
            {
                var college = list[i];
                for(int j = i+1; j < list.Count; j++)
                {
                    var target = list[j];
                    if (college.UNITID.Equals(target.UNITID))
                    {
                        Console.WriteLine("Found duplicate UNITID: " + target.UNITID);
                    }
                }
            }
        }

        public async Task UpdateCommonDataset()
        {
            var container = database.GetContainer("CommonDataset");
            int count = 0;
            foreach (string filename in Directory.EnumerateFiles(commonDatasetFilePath, "*.json"))
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
                    year = int.Parse(json["year"].ToString()),
                    Waiting = json["waiting"],
                    AdmDecision = json["admDecision"],
                    SatAct = json["satAct"],
                    Gpa = json["gpa"],
                    Apply = json["apply"],
                    Transfer = json["transfer"]
                };
                ItemResponse<CommonDatasetModel> res = null;
                try
                {
                    res = await container.ReadItemAsync<CommonDatasetModel>(model.Id, new PartitionKey(model.UNITID));
                    Console.WriteLine("Found the model with UNITID: " + res.Resource.UNITID);
                } catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {}
                if (res == null)
                {
                    Console.WriteLine("Not found the college " + json["name"]);
                    ItemResponse<CommonDatasetModel> result = await container.CreateItemAsync(model, new PartitionKey(model.UNITID));
                    Console.WriteLine("Created model in database with id: {0} Operation consumed {1} RUs.\n", result.Resource.Id, result.RequestCharge);
                }
                          
            }
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
                    while(innerIt.HasMoreResults)
                    {
                        var result = await innerIt.ReadNextAsync();
                        CollegeDataUSYearly dataItem = result.First();
                        Console.WriteLine($"Found corresponding college {dataItem.UNITID} in yearly data!");
                        dataItem.CommonData = item;
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
            //var destContainer = database.GetContainer(containerId);
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
                            item.APPLCN = targetItem.APPLCN;
                            item.APPLCNM = targetItem.APPLCNM;
                            item.APPLCNW = targetItem.APPLCNW;
                            item.ADMSSN = targetItem.ADMSSN;
                            item.ADMSSNM = targetItem.ADMSSNM;
                            item.ADMSSNW = targetItem.ADMSSNW;
                            item.ENRLT = targetItem.ENRLT;
                            item.ENRLM = targetItem.ENRLM;
                            item.ENRLW = targetItem.ENRLW;
                            item.ENRLFT = targetItem.ENRLFT;
                            item.ENRLFTM = targetItem.ENRLFTM;
                            item.ENRLFTW = targetItem.ENRLFTW;
                            item.ENRLPT = targetItem.ENRLPT;
                            item.ENRLPTM = targetItem.ENRLPTM;
                            item.ENRLPTW = targetItem.ENRLPTW;
                            item.SATNUM = targetItem.SATNUM;
                            item.SATPCT = targetItem.SATPCT;
                            item.ACTNUM = targetItem.ACTNUM;
                            item.ACTPCT = targetItem.ACTPCT;
                        }
                        else if (containerId.Equals("IPEDSAL"))
                        {
                            item.LPBOOKS = targetItem.LPBOOKS;
                            item.LEBOOKS = targetItem.LEBOOKS;
                            item.LEDATAB = targetItem.LEDATAB;
                            item.LPMEDIA = targetItem.LPMEDIA;
                            item.LEMEDIA = targetItem.LEMEDIA;
                            item.LPCLLCT = targetItem.LPCLLCT;
                            item.LECLLCT = targetItem.LECLLCT;
                            item.LTCLLCT = targetItem.LTCLLCT;
                            item.LPCRCLT = targetItem.LPCRCLT;
                            item.LECRCLT = targetItem.LECRCLT;
                            item.LTCRCLT = targetItem.LTCRCLT;
                        }
                        else if (containerId.Equals("IPEDSCDEP"))
                        {
                            item.Items = targetItem.Items;
                        }
                        else if (containerId.Equals("IPEDSDRVC"))
                        {
                            item.DOCDEGRS = targetItem.DOCDEGRS;
                            item.DOCDEGPP = targetItem.DOCDEGPP;
                            item.DOCDEGOT = targetItem.DOCDEGOT;
                            item.MASDEG = targetItem.MASDEG;
                            item.BASDEG = targetItem.BASDEG;
                            item.ASCDEG = targetItem.ASCDEG;
                            item.CERT4 = targetItem.CERT4;
                            item.CERT2 = targetItem.CERT2;
                            item.CERT1 = targetItem.CERT1;
                            item.PBACERT = targetItem.PBACERT;
                            item.PMACERT = targetItem.PMACERT;
                            item.SDOCDEG = targetItem.SDOCDEG;
                            item.SMASDEG = targetItem.SMASDEG;
                            item.SBASDEG = targetItem.SBASDEG;
                            item.SASCDEG = targetItem.SASCDEG;
                            item.SBAMACRT = targetItem.SBAMACRT;
                            item.SCERT24 = targetItem.SCERT24;
                            item.SCERT1 = targetItem.SCERT1;
                        }
                        else if (containerId.Equals("IPEDSDRVEF"))
                        {
                            item.ENRTOT = targetItem.ENRTOT;
                            item.ENRFT = targetItem.ENRFT;
                            item.ENRPT = targetItem.ENRPT;
                            item.PCTENRWH = targetItem.PCTENRWH;
                            item.PCTENRBK = targetItem.PCTENRBK;
                            item.PCTENRHS = targetItem.PCTENRHS;
                            item.PCTENRAP = targetItem.PCTENRAP;
                            item.PCTENRAS = targetItem.PCTENRAS;
                            item.PCTENRAN = targetItem.PCTENRAN;
                            item.PCTENRUN = targetItem.PCTENRUN;
                            item.PCTENRNR = targetItem.PCTENRNR;
                            item.PCTENRW = targetItem.PCTENRW;
                            item.EFUGFT = targetItem.EFUGFT;
                            item.PCUENRWH = targetItem.PCUENRWH;
                            item.PCUENRBK = targetItem.PCUENRBK;
                            item.PCUENRHS = targetItem.PCUENRHS;
                            item.PCUENRAP = targetItem.PCUENRAP;
                            item.PCUENRAS = targetItem.PCUENRAS;
                            item.PCUENRAN = targetItem.PCUENRAN;
                            item.PCUENR2M = targetItem.PCUENR2M;
                            item.PCUENRUN = targetItem.PCUENRUN;
                            item.PCUENRNR = targetItem.PCUENRNR;
                            item.PCUENRW = targetItem.PCUENRW;
                            item.EFGRAD = targetItem.EFGRAD;
                            item.PCGENRWH = targetItem.PCGENRWH;
                            item.PCGENRBK = targetItem.PCGENRBK;
                            item.PCGENRHS = targetItem.PCGENRHS;
                            item.PCGENRAP = targetItem.PCGENRAP;
                            item.PCGENRAN = targetItem.PCGENRAN;
                            item.PCGENR2M = targetItem.PCGENR2M;
                            item.PCGENRUN = targetItem.PCGENRUN;
                            item.PCGENRNR = targetItem.PCGENRNR;
                            item.PCGENRW = targetItem.PCGENRW;
                        }
                        else if (containerId.Equals("IPEDSDRVGR"))
                        {
                            item.GRRTTOT = targetItem.GRRTTOT;
                            item.GRRTM = targetItem.GRRTM;
                            item.GRRTW = targetItem.GRRTW;
                            item.GRRTAN = targetItem.GRRTAN;
                            item.GRRTAP = targetItem.GRRTAP;
                            item.GRRTAS = targetItem.GRRTAS;
                            item.GRRTNH = targetItem.GRRTNH;
                            item.GRRTBK = targetItem.GRRTBK;
                            item.GRRTHS = targetItem.GRRTHS;
                            item.GRRTWH = targetItem.GRRTWH;
                            item.GRRT2M = targetItem.GRRT2M;
                            item.GRRTUN = targetItem.GRRTUN;
                            item.GRRTNR = targetItem.GRRTNR;
                            item.TRRTTOT = targetItem.TRRTTOT;
                            item.GBA4RTT = targetItem.GBA4RTT;
                            item.GBA5RTT = targetItem.GBA5RTT;
                            item.GBA6RTT = targetItem.GBA6RTT;
                            item.GBA6RTM = targetItem.GBA6RTM;
                            item.GBA6RTW = targetItem.GBA6RTW;
                            item.GBA6RTAN = targetItem.GBA6RTAN;
                            item.GBA6RTAP = targetItem.GBA6RTAP;
                            item.GBA6RTAS = targetItem.GBA6RTAS;
                            item.GBA6RTNH = targetItem.GBA6RTNH;
                            item.GBA6RTBK = targetItem.GBA6RTBK;
                            item.GBA6RTHS = targetItem.GBA6RTHS;
                            item.GBA6RTWH = targetItem.GBA6RTWH;
                            item.GBA6RT2M = targetItem.GBA6RT2M;
                            item.GBA6RTUN = targetItem.GBA6RTUN;
                            item.GBA6RTNR = targetItem.GBA6RTNR;
                            item.GBATRRT = targetItem.GBATRRT;
                        }
                        else if (containerId.Equals("IPEDSDRVIC"))
                        {
                            item.CINDON = targetItem.CINDON;
                            item.CINSON = targetItem.CINSON;
                            item.COTSON = targetItem.COTSON;
                            item.CINDOFF = targetItem.CINDOFF;
                            item.CINSOFF = targetItem.CINSOFF;
                            item.COTSOFF = targetItem.COTSOFF;
                            item.CINDFAM = targetItem.CINDFAM;
                            item.CINSFAM = targetItem.CINSFAM;
                            item.COTSFAM = targetItem.COTSFAM;
                        }
                        else if (containerId.Equals("IPEDSIC"))
                        {
                            item.FT_UG = targetItem.FT_UG;
                            item.FT_FTUG = targetItem.FT_FTUG;
                            item.PT_UG = targetItem.PT_UG;
                            item.PT_FTUG = targetItem.PT_FTUG;
                            item.ROOM = targetItem.ROOM;
                            item.ROOMCAP = targetItem.ROOMCAP;
                            item.BOARD = targetItem.BOARD;
                            item.ROOMAMT = targetItem.ROOMAMT;
                            item.BOARDAMT = targetItem.BOARDAMT;
                            item.RMBRDAMT = targetItem.RMBRDAMT;
                            item.APPLFEEU = targetItem.APPLFEEU;
                            item.APPLFEEG = targetItem.APPLFEEG;
                        }
                        else if (containerId.Equals("IPEDSICAY"))
                        {
                            item.TUITION1 = targetItem.TUITION1;
                            item.FEE1 = targetItem.FEE1;
                            item.HRCHG1 = targetItem.HRCHG1;
                            item.TUITION2 = targetItem.TUITION2;
                            item.FEE2 = targetItem.FEE2;
                            item.HRCHG2 = targetItem.HRCHG2;
                            item.TUITION3 = targetItem.TUITION3;
                            item.FEE3 = targetItem.FEE3;
                            item.HRCHG3 = targetItem.HRCHG3;
                            item.TUITION5 = targetItem.TUITION5;
                            item.FEE5 = targetItem.FEE5;
                            item.HRCHG5 = targetItem.HRCHG5;
                            item.TUITION6 = targetItem.TUITION6;
                            item.FEE6 = targetItem.FEE6;
                            item.HRCHG6 = targetItem.HRCHG6;
                            item.TUITION7 = targetItem.TUITION7;
                            item.FEE7 = targetItem.FEE7;
                            item.HRCHG7 = targetItem.HRCHG7;
                            item.ISPROF1 = targetItem.ISPROF1;
                            item.ISPFEE1 = targetItem.ISPFEE1;
                            item.OSPROF1 = targetItem.OSPROF1;
                            item.OSPFEE1 = targetItem.OSPFEE1;
                            item.ISPROF2 = targetItem.ISPROF2;
                            item.ISPFEE2 = targetItem.ISPFEE2;
                            item.OSPROF2 = targetItem.OSPROF2;
                            item.OSPFEE2 = targetItem.OSPFEE2;
                            item.ISPROF3 = targetItem.ISPROF3;
                            item.ISPFEE3 = targetItem.ISPFEE3;
                            item.OSPROF3 = targetItem.OSPROF3;
                            item.OSPFEE3 = targetItem.OSPFEE3;
                            item.ISPROF4 = targetItem.ISPROF4;
                            item.ISPFEE4 = targetItem.ISPFEE4;
                            item.OSPROF4 = targetItem.OSPROF4;
                            item.OSPFEE4 = targetItem.OSPFEE4;
                            item.ISPROF5 = targetItem.ISPROF5;
                            item.ISPFEE5 = targetItem.ISPFEE5;
                            item.OSPROF5 = targetItem.OSPROF5;
                            item.OSPFEE5 = targetItem.OSPFEE5;
                            item.ISPROF6 = targetItem.ISPROF6;
                            item.ISPFEE6 = targetItem.ISPFEE6;
                            item.OSPROF6 = targetItem.OSPROF6;
                            item.OSPFEE6 = targetItem.OSPFEE6;
                            item.ISPROF7 = targetItem.ISPROF7;
                            item.ISPFEE7 = targetItem.ISPFEE7;
                            item.OSPROF7 = targetItem.OSPROF7;
                            item.OSPFEE7 = targetItem.OSPFEE7;
                            item.ISPROF8 = targetItem.ISPROF8;
                            item.ISPFEE8 = targetItem.ISPFEE8;
                            item.OSPROF8 = targetItem.OSPROF8;
                            item.OSPFEE8 = targetItem.OSPFEE8;
                            item.ISPROF9 = targetItem.ISPROF9;
                            item.ISPFEE9 = targetItem.ISPFEE9;
                            item.OSPROF9 = targetItem.OSPROF9;
                            item.OSPFEE9 = targetItem.OSPFEE9;
                            item.CHG1AT0 = targetItem.CHG1AT0;
                            item.CHG1AF0 = targetItem.CHG1AF0;
                            item.CHG1AY0 = targetItem.CHG1AY0;
                            item.CHG1AT1 = targetItem.CHG1AT1;
                            item.CHG1AF1 = targetItem.CHG1AF1;
                            item.CHG1AY1 = targetItem.CHG1AY1;
                            item.CHG1AT2 = targetItem.CHG1AT2;
                            item.CHG1AF2 = targetItem.CHG1AF2;
                            item.CHG1AY2 = targetItem.CHG1AY2;
                            item.CHG1AT3 = targetItem.CHG1AT3;
                            item.CHG1AF3 = targetItem.CHG1AF3;
                            item.CHG1AY3 = targetItem.CHG1AY3;
                            item.CHG1TGTD = targetItem.CHG1TGTD;
                            item.CHG1FGTD = targetItem.CHG1FGTD;
                            item.CHG2AT0 = targetItem.CHG2AT0;
                            item.CHG2AF0 = targetItem.CHG2AF0;
                            item.CHG2AY0 = targetItem.CHG2AY0;
                            item.CHG2AT1 = targetItem.CHG2AT1;
                            item.CHG2AF1 = targetItem.CHG2AF1;
                            item.CHG2AY1 = targetItem.CHG2AY1;
                            item.CHG2AT2 = targetItem.CHG2AT2;
                            item.CHG2AF2 = targetItem.CHG2AF2;
                            item.CHG2AY2 = targetItem.CHG2AY2;
                            item.CHG2AT3 = targetItem.CHG2AT3;
                            item.CHG2AF3 = targetItem.CHG2AF3;
                            item.CHG2AY3 = targetItem.CHG2AY3;
                            item.CHG2TGTD = targetItem.CHG2TGTD;
                            item.CHG2FGTD = targetItem.CHG2FGTD;
                            item.CHG3AT0 = targetItem.CHG3AT0;
                            item.CHG3AF0 = targetItem.CHG3AF0;
                            item.CHG3AY0 = targetItem.CHG3AY0;
                            item.CHG3AT1 = targetItem.CHG3AT1;
                            item.CHG3AF1 = targetItem.CHG3AF1;
                            item.CHG3AY1 = targetItem.CHG3AY1;
                            item.CHG3AT2 = targetItem.CHG3AT2;
                            item.CHG3AF2 = targetItem.CHG3AF2;
                            item.CHG3AY2 = targetItem.CHG3AY2;
                            item.CHG3AT3 = targetItem.CHG3AT3;
                            item.CHG3AF3 = targetItem.CHG3AF3;
                            item.CHG3AY3 = targetItem.CHG3AY3;
                            item.CHG3TGTD = targetItem.CHG3TGTD;
                            item.CHG3FGTD = targetItem.CHG3FGTD;
                            item.CHG4AY0 = targetItem.CHG4AY0;
                            item.CHG4AY1 = targetItem.CHG4AY1;
                            item.CHG4AY2 = targetItem.CHG4AY2;
                            item.CHG4AY3 = targetItem.CHG4AY3;
                            item.CHG5AY0 = targetItem.CHG5AY0;
                            item.CHG5AY1 = targetItem.CHG5AY1;
                            item.CHG5AY2 = targetItem.CHG5AY2;
                            item.CHG5AY3 = targetItem.CHG5AY3;
                            item.CHG6AY0 = targetItem.CHG6AY0;
                            item.CHG6AY1 = targetItem.CHG6AY1;
                            item.CHG6AY2 = targetItem.CHG6AY2;
                            item.CHG6AY3 = targetItem.CHG6AY3;
                            item.CHG7AY0 = targetItem.CHG7AY0;
                            item.CHG7AY1 = targetItem.CHG7AY1;
                            item.CHG7AY2 = targetItem.CHG7AY2;
                            item.CHG7AY3 = targetItem.CHG7AY3;
                            item.CHG8AY0 = targetItem.CHG8AY0;
                            item.CHG8AY1 = targetItem.CHG8AY1;
                            item.CHG8AY2 = targetItem.CHG8AY2;
                            item.CHG8AY3 = targetItem.CHG8AY3;
                            item.CHG9AY0 = targetItem.CHG9AY0;
                            item.CHG9AY1 = targetItem.CHG9AY1;
                            item.CHG9AY2 = targetItem.CHG9AY2;
                            item.CHG9AY3 = targetItem.CHG9AY3;
                        }
                        else if (containerId.Equals("IPEDSSSIS"))
                        {
                            item.SISTOTL = targetItem.SISTOTL;
                            item.SISPROF = targetItem.SISPROF;
                            item.SISASCP = targetItem.SISASCP;
                            item.SISASTP = targetItem.SISASTP;
                            item.SISINST = targetItem.SISINST;
                            item.SISLECT = targetItem.SISLECT;
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
                        if (idNum%10000 == 0)
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
            using StreamReader sr = new StreamReader("c:/Users/Administrator/Downloads/Most-Recent-Cohorts-All-Data-Elements.csv");
            var num = 0;
            var localContainer = database.GetContainer("CollegeDataUSYearly");
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while ((currentLine = sr.ReadLine()) != null)
            {
                num++;
                if (num == 1)
                    continue;
                //Console.WriteLine("Working on line number: " + (num - 1));
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

                CollegeDataUSYearly item = new CollegeDataUSYearly
                {

                    UNITID = UNITID,
                    Id = (num - 1).ToString(),
                    year = 2019,
                    ADM_RATE = ADM_RATE,
                    ADM_RATE_ALL = ADM_RATE_ALL,
                    SATVR25 = SATVR25,
                    SATVR75 = SATVR75,
                    SATMT25 = SATMT25,
                    SATMT75 = SATMT75,
                    SATWR25 = SATWR25,
                    SATWR75 = SATWR75,
                    SATVRMID = SATVRMID,
                    SATMTMID = SATMTMID,
                    SATWRMID = SATWRMID,
                    ACTCM25 = ACTCM25,
                    ACTCM75 = ACTCM75,
                    ACTEN25 = ACTEN25,
                    ACTEN75 = ACTEN75,
                    ACTMT25 = ACTMT25,
                    ACTMT75 = ACTMT75,
                    ACTWR25 = ACTWR25,
                    ACTWR75 = ACTWR75,
                    ACTCMMID = ACTCMMID,
                    ACTENMID = ACTENMID,
                    ACTMTMID = ACTMTMID,
                    ACTWRMID = ACTWRMID,
                    SAT_AVG = SAT_AVG,
                    UGDS = UGDS,
                    UGDS_WHITE = UGDS_WHITE,
                    UGDS_BLACK = UGDS_BLACK,
                    UGDS_HISP = UGDS_HISP,
                    UGDS_ASIAN = UGDS_ASIAN,
                    UGDS_AIAN = UGDS_AIAN,
                    UGDS_NHPI = UGDS_NHPI,
                    UGDS_2MOR = UGDS_2MOR,
                    UGDS_NRA = UGDS_NRA,
                    UGDS_UNKN = UGDS_UNKN,
                    UGDS_WHITENH = UGDS_WHITENH,
                    UGDS_BLACKNH = UGDS_BLACKNH,
                    UGDS_API = UGDS_API,
                    NPT4_PUB = NPT4_PUB,
                    NPT4_PRIV = NPT4_PRIV,
                    NPT41_PUB = NPT41_PUB,
                    NPT42_PUB = NPT42_PUB,
                    NPT43_PUB = NPT43_PUB,
                    NPT44_PUB = NPT44_PUB,
                    NPT45_PUB = NPT45_PUB,
                    NPT41_PRIV = NPT41_PRIV,
                    NPT42_PRIV = NPT42_PRIV,
                    NPT43_PRIV = NPT43_PRIV,
                    NPT44_PRIV = NPT44_PRIV,
                    NPT45_PRIV = NPT45_PRIV,
                    NUM4_PUB = NUM4_PUB,
                    NUM41_PUB = NUM41_PUB,
                    NUM42_PUB = NUM42_PUB,
                    NUM43_PUB = NUM43_PUB,
                    NUM44_PUB = NUM44_PUB,
                    NUM45_PUB = NUM45_PUB,
                    NUM41_PRIV = NUM41_PRIV,
                    NUM42_PRIV = NUM42_PRIV,
                    NUM43_PRIV = NUM43_PRIV,
                    NUM44_PRIV = NUM44_PRIV,
                    NUM45_PRIV = NUM45_PRIV,
                    NUM4_PRIV = NUM4_PRIV,
                    COSTT4_A = COSTT4_A,
                    COSTT4_P = COSTT4_P,
                    TUITIONFEE_IN = TUITIONFEE_IN,
                    TUITIONFEE_OUT = TUITIONFEE_OUT,
                    FEMALE = FEMALE,
                    MARRIED = MARRIED,
                    DEPENDENT = DEPENDENT,
                    VETERAN = VETERAN,
                    FIRST_GEN = FIRST_GEN,
                    FAMINC = FAMINC,
                    MD_FAMINC = MD_FAMINC,
                    MEDIAN_HH_INC = MEDIAN_HH_INC,
                    POVERTY_RATE = POVERTY_RATE,
                    UNEMP_RATE = UNEMP_RATE,
                    GRAD_DEBT_MDN_SUPP = GRAD_DEBT_MDN_SUPP,
                    UGDS_MEN = UGDS_MEN,
                    UGDS_WOMEN = UGDS_WOMEN,
                    GRADS = GRADS,
                    COUNT_NWNE_P10 = COUNT_NWNE_P10,
                    COUNT_WNE_P10 = COUNT_WNE_P10,
                    MN_EARN_WNE_P10 = MN_EARN_WNE_P10,
                    MD_EARN_WNE_P10 = MD_EARN_WNE_P10,
                    COUNT_NWNE_P6 = COUNT_NWNE_P6,
                    COUNT_WNE_P6 = COUNT_WNE_P6,
                    MN_EARN_WNE_P6 = MN_EARN_WNE_P6,
                    MD_EARN_WNE_P6 = MD_EARN_WNE_P6,
                    COUNT_NWNE_P8 = COUNT_NWNE_P8,
                    COUNT_WNE_P8 = COUNT_WNE_P8,
                    MN_EARN_WNE_P8 = MN_EARN_WNE_P8,
                    MD_EARN_WNE_P8 = MD_EARN_WNE_P8
                };

                if (num < 30000)
                {
                    //Console.WriteLine($"unitid: {UNITID}");
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
            await CreateContainerAsync();
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
            var sqlQueryText = $"SELECT * FROM c WHERE {condition}" ;
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
            while(it.HasMoreResults)
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

    }
}
