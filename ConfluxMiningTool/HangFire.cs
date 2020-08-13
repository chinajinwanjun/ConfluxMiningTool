﻿using ConfluxMiningTool.Models;
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
        private readonly IAccountRepository accountRepository;
        private readonly IConfiguration configuration;
        private readonly TrustNodeRepository trustNodeRepository;
        private readonly IDailyTrustedNodeRepository dailyTrustedNodeRepository;
        public HangFire(IBalanceHistoryRepository balanceHistory, IAccountRepository accountRepository, TrustNodeRepository trustNodeRepository, IDailyTrustedNodeRepository dailyTrustedNodeRepository, IConfiguration configuration)
        {
            this.balanceHistory = balanceHistory;
            this.accountRepository = accountRepository;
            this.trustNodeRepository = trustNodeRepository;
            this.configuration = configuration;
            this.dailyTrustedNodeRepository = dailyTrustedNodeRepository;
        }
        public HangFire()
        {
        }
        public void AddJob()
        {
            RecurringJob.AddOrUpdate(() => WriteMiningData(), "0 * * * *", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            RecurringJob.AddOrUpdate(() => UpdateLatAndLon(), "*/5 * * * *", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            RecurringJob.AddOrUpdate(() => WriteDailyTrustNode(), "30 23 * * *", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
        }

        public void WriteMiningData()
        {
            //team
            HttpClient http = new HttpClient();
            var teamAddressList = new List<string> { "0x1C2B0CDE31b96E52cf5236e75e8bbFC0Dac038E5", "0x1a8fbf02d66daafa902885dd958650d6865d4fcc", "0x10933e0d887f62d1f00fc7b93575e89e82cbd640", "0x19b9e817ec206241b3bd71cf2152d9695d2e439b" };

            int usedTotal = 0;
            foreach (var teamAddress in teamAddressList)
            {
                Regex regex = new Regex("\"balance\":\"(\\d+)\"");
                var str = http.GetAsync($@"http://confluxscan.io/contract-manager/api/account/token/list?address={teamAddress.ToLower()}").Result.Content.ReadAsStringAsync().Result;
                var balance = regex.Matches(str)[0].Groups[1].ToString();
                balanceHistory.Add(new BalanceHistory
                {
                    Address = teamAddress.ToLower(),
                    Balance = Convert.ToDouble(balance),
                    CreatedTime = DateTime.Now,
                });
                usedTotal += int.Parse(balance);
            }
            // total
            {
                Regex regex = new Regex("\"totalSupply\":\"(\\d+)\"");
                var str = http.GetAsync($@"http://confluxscan.io/api/token/query?address=0x8f50e31a4e3201b2f7aa720b3754dfa585b4dbfa").Result.Content.ReadAsStringAsync().Result;
                var balance = regex.Matches(str)[0].Groups[1].ToString();
                balanceHistory.Add(new BalanceHistory
                {
                    Address = "0x8f50e31a4e3201b2f7aa720b3754dfa585b4dbfa",
                    Balance = Convert.ToDouble(balance),
                    CreatedTime = DateTime.Now,
                });
                var unusedTotal = int.Parse(balance) - usedTotal;
                balanceHistory.Add(new BalanceHistory
                {
                    Address = "0x8unuseda4e3201b2f7aa720b3754dfa585b4dbfa",
                    Balance = Convert.ToDouble(unusedTotal),
                    CreatedTime = DateTime.Now,
                });
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
            var ipList = trustNodeRepository.GetAllActive().Where(x => x.Lat == null && x.IPAddressList != null && x.IPAddressList.Length > 4).Select(x => x.IPAddressList).Take(10).ToList();
            foreach (var ip in ipList)
            {
                var api = $@"http://api.ipstack.com/{ip}?access_key=60f430bcae98649b74acb1bc34f1a8a5";
                var result = http.GetAsync(api).Result;
                var data = result.Content.ReadAsStringAsync().Result;
                var parsedData = JsonConvert.DeserializeObject<LatAndLon>(data);
                latAndLons.Add(parsedData);
            }
            trustNodeRepository.UpdateLatAndLon(latAndLons);



        }
    }
}
