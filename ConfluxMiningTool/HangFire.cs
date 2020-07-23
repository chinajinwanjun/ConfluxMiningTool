using ConfluxMiningTool.Models;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConfluxMiningTool
{
    public class HangFire
    {
        private readonly IBalanceHistoryRepository balanceHistory;
        private readonly IAccountRepository accountRepository;
        private readonly IConfiguration configuration;
        private readonly ITrustNodeRepository trustNodeRepository;
        private readonly IDailyTrustedNodeRepository dailyTrustedNodeRepository;
        public HangFire(IBalanceHistoryRepository balanceHistory, IAccountRepository accountRepository, ITrustNodeRepository trustNodeRepository, IDailyTrustedNodeRepository dailyTrustedNodeRepository, IConfiguration configuration)
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
            RecurringJob.AddOrUpdate(() => WriteDailyTrustNode(), "30 23 * * *", TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
        }

        public void WriteMiningData()
        {
            foreach (var account in accountRepository.GetAccounts())
            {
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
    }
}
