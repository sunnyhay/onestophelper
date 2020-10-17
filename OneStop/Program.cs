using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System.IO;
using ExcelDataReader;
using OneStop.Model;
using System.Collections.Generic;

namespace OneStop
{
    public class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://onestoptest.documents.azure.com:443/";
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
        private readonly string containerId = "CommonCollegeUS";

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...\n");
                Program p = new Program();
                await p.StartOperationAsync();
                // await p.ReadCSV();
                //p.Update();
                //await p.UpdateFixedData();
                //await p.QueryItemsAsync("100654");
                //await p.AddIPEDSYearlyDataforADM("IPEDSADM");
                //await p.AddIPEDSYearlyDataforAL("IPEDSAL");
                //await p.AddIPEDSYearlyDataforCDEP("IPEDSCDEP");
                //await p.AddIPEDSYearlyDataforDRVC("IPEDSDRVC");
                //await p.AddIPEDSYearlyDataforDRVEF("IPEDSDRVEF");
                //await p.AddIPEDSYearlyDataforDRVGR("IPEDSDRVGR");
                //await p.AddIPEDSYearlyDataforDRVIC("IPEDSDRVIC");
                //await p.AddIPEDSYearlyDataforIC("IPEDSIC");
                //await p.AddIPEDSYearlyDataforICAY("IPEDSICAY");
                await p.AddIPEDSYearlyDataforSSIS("IPEDSSSIS");
                //await p.AddIPEDSCIPCODE("IPEDSCIPCODE2020");
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

        private double? RetrieveVal(object val)
        {
            double? result;
            if (val == null)
                result = null;
            else
                result = (double)val;
            return result;
        }

        public async Task AddIPEDSCIPCODE(string containerId)
        {
            var filePath = "c:/Users/Administrator/Documents/CIPCode2020.xlsx";
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

        public async Task AddIPEDSYearlyDataforAL(string containerId)
        {
            var filePath = "c:/Users/Administrator/Documents/AL2018.xlsx";
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
                    var year = 2018;
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
                            year = year,
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
        public async Task AddIPEDSYearlyDataforADM(string containerId)
        {
            var filePath = "c:/Users/Administrator/Documents/ADM2018.xlsx";
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
                    var year = 2018;
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
                            year = year,
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
        public async Task AddIPEDSYearlyDataforCDEP(string containerId)
        {
            var filePath = "c:/Users/Administrator/Documents/C2018DEP.xlsx";
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
                    var year = 2018;
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
                            year = year
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
            var filePath = "c:/Users/Administrator/Documents/DRVC2018.xlsx";
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
                    var year = 2018;
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
                            year = year,
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
            var filePath = "c:/Users/Administrator/Documents/DRVEF2018.xlsx";
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
                    var year = 2018;
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
                            year = year,
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
            var filePath = "c:/Users/Administrator/Documents/DRVGR2018.xlsx";
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
                    var year = 2018;
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
                            year = year,
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
            var filePath = "c:/Users/Administrator/Documents/DRVIC2018.xlsx";
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
                    var year = 2018;
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
                            year = year,
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
            var filePath = "c:/Users/Administrator/Documents/IC2018.xlsx";
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
                    var year = 2018;
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
                            year = year,
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
            var filePath = "c:/Users/Administrator/Documents/IC2018_AY.xlsx";
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
                    var year = 2018;
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
                            year = year,
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
            var filePath = "c:/Users/Administrator/Documents/S2018_SIS.xlsx";
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
                    var year = 2018;
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
                                year = year,
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

        public async Task UpdateFixedData()
        {
            var filePath = "c:/Users/Administrator/Documents/HD2018_copy.xlsx";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            // Auto-detect format, supports:
            //  - Binary Excel files (2.0-2003 format; *.xls)
            //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
            using var reader = ExcelReaderFactory.CreateReader(stream);
            // Choose one of either 1 or 2:

            // 1. Use the reader methods
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
                    var countynm = reader.GetString(67);  // something wrong in the delimitor, may use field 68 for this!
                    var id = (count - 1).ToString();
                    var idNum = count - 1;

                    if (idNum < 30000)
                    {
                        Console.WriteLine($"unitid: {unitid} with adminurl {adminurl}");
                        var partitionKey = new PartitionKey(unitid);
                        //ItemResponse<CollegeUS> res = await container.ReadItemAsync<CollegeUS>(id, partitionKey);
                        CollegeUS college = await QueryItemsAsync(unitid);
                        if (college == null)
                        {
                            continue;
                        }
                        Console.WriteLine("Working on college: " + college.INSTNM);
                        //Console.WriteLine($"unitid: {unitid} with adminurl {adminurl}");
                        //Console.WriteLine($"faidurl: {faidurl} with applurl {applurl}");
                        //Console.WriteLine($"countynm: {countynm} with highest degree {degree}");
                        //Console.WriteLine();
                        college.ADMINURL = adminurl;
                        college.FAIDURL = faidurl;
                        college.APPLURL = applurl;
                        college.COUNTYNM = countynm;
                        college.HIGHEST_DEGREE = degree;
                        var res = await container.ReplaceItemAsync(college, college.Id, partitionKey);
                        Console.WriteLine("Updated college: " + college.INSTNM);
                        Console.WriteLine();
                    }

                }
            } while (reader.NextResult());

            // 2. Use the AsDataSet extension method
            //var result = reader.AsDataSet();

            // The result of each spreadsheet is in result.Tables
        }

        // ADD THIS PART TO YOUR CODE
        /*
            Entry point to call methods that operate on Azure Cosmos DB resources in this sample
        */
        public async Task StartOperationAsync()
        {
            // Create a new instance of the Cosmos Client
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            GetDatabase();
            //await CreateDatabaseAsync();
            //await CreateContainerAsync();
        }

        private void GetDatabase()
        {
            database = cosmosClient.GetDatabase(databaseId);
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
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

        /// <summary>
        /// Create the container if it does not exist. 
        /// Specifiy "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private async Task CreateContainerAsync()
        {
            // Create a new container
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/UNITID");
            Console.WriteLine("Created Container: {0}\n", container.Id);
        }

        private void Update()
        {
            using StreamReader sr = new StreamReader("c:/Users/Administrator/Downloads/HD2018.csv");
            var num = 0;
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while ((currentLine = sr.ReadLine()) != null)
            {
                num++;
                if (num == 1)
                    continue;
                if (num < 40)
                {
                    Console.WriteLine("Working on line number: " + (num - 1));
                    var vals = currentLine.Split(',');
                    var unitid = vals[0];
                    var adminurl = vals[17];
                    var faidurl = vals[18];
                    var applurl = vals[19];
                    var hdegofr1 = vals[30];
                    string degree = GetDegree(hdegofr1);
                    var countynm = vals[67];  // something wrong in the delimitor, may use field 68 for this!
                    // TODO: need to combine degree value "Wrong" and Integer-type countynm for wrong indicator for some
                    // empty space or other error in previous URLs
                    // record each UNITID and later manually restore!!!
                    Console.WriteLine($"unitid: {unitid} with adminurl {adminurl}");
                    Console.WriteLine($"faidurl: {faidurl} with applurl {applurl}");
                    Console.WriteLine($"countynm: {countynm} with highest degree {degree} and original value {hdegofr1}");
                    Console.WriteLine();
                    //Console.WriteLine($"npcurl: {npcurl} with control {control}");
                    //Console.WriteLine($"st_fips: {st_fips} with region {region}");
                    //Console.WriteLine($"locale: {locale}");
                    //Console.WriteLine($"latitude: {latitude} with longitude {longitude}");
                }
            }
        }

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

        private async Task ReadCSV()
        {
            using StreamReader sr = new StreamReader("c:/Users/Administrator/Downloads/Most-Recent-Cohorts-All-Data-Elements.csv");
            var num = 0;
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while ((currentLine = sr.ReadLine()) != null)
            {
                num++;
                if (num == 1)
                    continue;
                Console.WriteLine("Working on line number: " + (num-1));
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
                CollegeUS college = new CollegeUS
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
                await AddItemsToContainerAsync(college);

                //Console.WriteLine($"unitid: {unitid} with name {instnm}");
                //Console.WriteLine($"city: {city} with stabbr {stabbr}");
                //Console.WriteLine($"zip: {zip} with insturl {insturl}");
                //Console.WriteLine($"npcurl: {npcurl} with control {control}");
                //Console.WriteLine($"st_fips: {st_fips} with region {region}");
                //Console.WriteLine($"locale: {locale}");
                //Console.WriteLine($"latitude: {latitude} with longitude {longitude}");
            }
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

        /// <summary>
        /// Run a query (using Azure Cosmos DB SQL syntax) against the container
        /// </summary>
        private async Task<CollegeUS> QueryItemsAsync(string partitionKey)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.UNITID = '{partitionKey}'";
            //Console.WriteLine("Running query: {0}\n", sqlQueryText);
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            var it = container.GetItemQueryIterator<CollegeUS>(queryDefinition);
            if (!it.HasMoreResults)
            {
                Console.WriteLine($"College with UNITID {partitionKey} is not found.");
                return null;
            }
            CollegeUS college = null;
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
        private async Task AddItemsToContainerAsync(CollegeUS college)
        {
            try
            {
                ItemResponse<CollegeUS> res = await container.CreateItemAsync<CollegeUS>(college, new PartitionKey(college.UNITID));
                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", res.Resource.Id, res.RequestCharge);
            }
            catch (CosmosException ex) 
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
