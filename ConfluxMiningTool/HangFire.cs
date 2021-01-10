using ConfluxMiningTool.Models;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfluxMiningTool
{
    public class HangFire
    {
        private readonly IBalanceHistoryRepository balanceHistory;
        private readonly IMinerBlockRepository minerBlockRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IConfiguration configuration;
        private readonly TrustNodeRepository trustNodeRepository;
        private readonly IDailyTrustedNodeRepository dailyTrustedNodeRepository;
        public HangFire(IBalanceHistoryRepository balanceHistory, IAccountRepository accountRepository, TrustNodeRepository trustNodeRepository, IDailyTrustedNodeRepository dailyTrustedNodeRepository, IConfiguration configuration, IMinerBlockRepository minerBlockRepository, ITransactionRepository transactionRepository)
        {
            this.balanceHistory = balanceHistory;
            this.accountRepository = accountRepository;
            this.trustNodeRepository = trustNodeRepository;
            this.configuration = configuration;
            this.dailyTrustedNodeRepository = dailyTrustedNodeRepository;
            this.minerBlockRepository = minerBlockRepository;
            this.transactionRepository = transactionRepository;
        }
        public HangFire()
        {
        }
        public void AddJob()
        {
            //FCCFX
            RecurringJob.AddOrUpdate(() => StoreTransaction(), "*/2 * * * *", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            //Test Miner
            RecurringJob.AddOrUpdate(() => AddMiner(), "*/5 3,12,23,18,19 * * *", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            //Pool
            RecurringJob.AddOrUpdate(() => StorePoolData(), "*/10 * * * *", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            //RecurringJob.AddOrUpdate(() => BlockRate(), "0 * * * *", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
        }
        public class Block
        {
            public string address { get; set; }
            public int block_count { get; set; }
        }
        public void BlockRate()
        {
            return;
            HttpClient http = new HttpClient();
            var str = http.GetAsync($@"https://mining.confluxnetwork.org/test/get-miner-list").Result.Content.ReadAsStringAsync().Result;
            var blocks = JsonConvert.DeserializeObject<List<Block>>(str);
            var now = DateTime.Now.ToString("yyyyMMdd HHmm");
            foreach (var block in blocks)
            {
                if (block.address != null)
                {
                    minerBlockRepository.Add(new MinerBlock { Block = block.block_count, CreatedTime = now, Wallet = block.address });
                }
            }
        }

        public class RawTrans
        {
            public int total { get; set; }
            public List<Transaction> list { get; set; }
        }
        public class RawMiner
        {
            public List<Miner> list { get; set; }
        }
        public void StorePoolData()
        {
            HttpClient http = new HttpClient();
            var str = http.GetAsync($@"http://www.wabi.com/coins/conflux").Result.Content.ReadAsStringAsync().Result;
            Regex regex = new Regex("_blank\">(.+?)<\\/a>[\\s\\S]{3,100}data-sort=\"(\\d+)\"");
            List<PoolHashRate> poolHashRateList = new List<PoolHashRate>();
            foreach (Match match in regex.Matches(str))
            {
                var poolName = match.Groups[1].Value;
                var hashRate = match.Groups[2].Value;
                var poolHashRate = new PoolHashRate
                {
                    CreatedAt = DateTime.Now,
                    Hashrate = Convert.ToInt64(hashRate),
                    Name = poolName,
                };
                poolHashRateList.Add(poolHashRate);
            }
            transactionRepository.AddPoolHashRate(poolHashRateList);
        }
        public void AddMiner()
        {
            HttpClient http = new HttpClient();
            var str = http.GetAsync($@"https://testnet.confluxscan.io/v1/block?limit=100&skip=0").Result.Content.ReadAsStringAsync().Result;
            var rawMiners = JsonConvert.DeserializeObject<RawMiner>(str);
            foreach (var miner in rawMiners.list)
            {
                transactionRepository.AddMiner(miner);
            }
        }
        public void StoreTransaction()
        {
            HttpClient http = new HttpClient();
            var transactionHashs = transactionRepository.GetAllHash();
            for (int i = 0; i < 5; i++)
            {
                var str = http.GetAsync($@"https://confluxscan.io/v1/transfer?accountAddress=0x86d1f0072e8aa1a38d34b4bfa7521cdb5293849f&limit=100&skip={i * 100}").Result.Content.ReadAsStringAsync().Result;
                var rawTrans = JsonConvert.DeserializeObject<RawTrans>(str);
                foreach (var block in rawTrans.list)
                {
                    if (!transactionHashs.Contains(block.transactionHash))
                    {
                        transactionRepository.Add(new Transaction { from = block.from, transactionHash = block.transactionHash, value = block.value / 1000000000000000000 });
                    }
                }
            }
        }
        public void WriteMiningData()
        {
            //team
            HttpClient http = new HttpClient();
            var teamAddressList = new List<string> { "0x1C2B0CDE31b96E52cf5236e75e8bbFC0Dac038E5", "0x1a8fbf02d66daafa902885dd958650d6865d4fcc", "0x10933e0d887f62d1f00fc7b93575e89e82cbd640", "0x19b9e817ec206241b3bd71cf2152d9695d2e439b" };

            int usedTotal = 0;
            foreach (var teamAddress in teamAddressList)
            {
                Regex regex = new Regex("CC\",\"balance\":\"(\\d+)\"");
                var str = http.GetAsync($@"http://confluxscan.io/contract-manager/api/account/token/list?address={teamAddress.ToLower()}").Result.Content.ReadAsStringAsync().Result;
                var balance = regex.Matches(str)[0].Groups[1].ToString();
                //balanceHistory.Add(new BalanceHistory
                //{
                //    Address = teamAddress.ToLower(),
                //    Balance = Convert.ToDouble(balance),
                //    CreatedTime = DateTime.Now,
                //});
                usedTotal += int.Parse(balance);
            }
            // total
            {
                Regex regex = new Regex("\"totalSupply\":\"(\\d+)\"");
                var str = http.GetAsync($@"http://confluxscan.io/api/token/query?address=0x8f50e31a4e3201b2f7aa720b3754dfa585b4dbfa").Result.Content.ReadAsStringAsync().Result;
                var balance = regex.Matches(str)[0].Groups[1].ToString();
                //balanceHistory.Add(new BalanceHistory
                //{
                //    Address = "0x8f50e31a4e3201b2f7aa720b3754dfa585b4dbfa",
                //    Balance = Convert.ToDouble(balance),
                //    CreatedTime = DateTime.Now,
                //});
                var unusedTotal = int.Parse(balance) - usedTotal;
                //balanceHistory.Add(new BalanceHistory
                //{
                //    Address = "0x8unuseda4e3201b2f7aa720b3754dfa585b4dbfa",
                //    Balance = Convert.ToDouble(unusedTotal),
                //    CreatedTime = DateTime.Now,
                //});
            }

            //total
            foreach (var account in accountRepository.GetAccounts())
            {
                if (teamAddressList.Contains(account.Address))
                {
                    continue;
                }
                var balance = GetBalanceByAddress(account.Address);
                if (balance > 1)
                {
                    balanceHistory.Add(new BalanceHistory
                    {
                        Address = account.Address,
                        Balance = balance,
                        CreatedTime = DateTime.Now,
                    });
                }

            }
            //remove 10 days ago 
            foreach (var account in accountRepository.GetAccounts())
            {
                balanceHistory.Remove(account.Address);
            }
        }
        public double GetBalanceByAddress(string address)
        {
            try
            {
                HttpClient http = new HttpClient();
                dynamic json = JsonConvert.DeserializeObject(http.GetAsync($@"{configuration.GetSection("Url").Value}?address={address}").Result.Content.ReadAsStringAsync().Result);
                return Convert.ToDouble(json.result.balance.ToString()) / 1000000000000000000;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public void WriteDailyTrustNode()
        {
            //get today trusted node list
            var walletList = trustNodeRepository.GetTodayTrustedWalletAddressActive();
            dailyTrustedNodeRepository.Save(walletList);
        }

        public void UpdateLatAndLon()
        {
            HttpClient http = new HttpClient();
            List<LatAndLon> latAndLons = new List<LatAndLon>();

            //get ip list
            // http://13.75.113.21:4002/trusted-node-ip-list

            var url = "http://13.75.113.21:4002/trusted-node-ip-list";
            var result = http.GetAsync(url).Result;
            var data = result.Content.ReadAsStringAsync().Result;
            var mapIPList = trustNodeRepository.GetMapIPList();
            var index = 0;
            foreach (var ip in Regex.Matches(data, $@"\d+\.\d+\.\d+\.\d+").Select(x => x.ToString()))
            {
                if (index <= 10)
                {
                    if (!mapIPList.Contains(ip))
                    {
                        var api = $@"http://api.ipstack.com/{ip.ToString()}?access_key=60f430bcae98649b74acb1bc34f1a8a5";
                        result = http.GetAsync(api).Result;
                        data = result.Content.ReadAsStringAsync().Result;
                        var parsedData = JsonConvert.DeserializeObject<LatAndLon>(data);
                        if (
                             !string.IsNullOrEmpty(parsedData.latitude)
                            && !string.IsNullOrEmpty(parsedData.longitude)
                               && parsedData.latitude.Trim().Length > 0
                            && parsedData.longitude.Trim().Length > 0
                            )
                        {
                            trustNodeRepository.StoreMapNode(parsedData);
                            index++;
                        }
                    }
                }
            }

            //var ipList = trustNodeRepository.GetAllActive().Where(x => x.Lat == null && x.IPAddressList != null && x.IPAddressList.Length > 4).Select(x => x.IPAddressList).Take(10).ToList();
            //foreach (var ip in ipList)
            //{
            //    var api = $@"http://api.ipstack.com/{ip}?access_key=60f430bcae98649b74acb1bc34f1a8a5";
            //    result = http.GetAsync(api).Result;
            //    data = result.Content.ReadAsStringAsync().Result;
            //    var parsedData = JsonConvert.DeserializeObject<LatAndLon>(data);
            //    latAndLons.Add(parsedData);
            //}
            //trustNodeRepository.UpdateLatAndLon(latAndLons);
        }
    }
}