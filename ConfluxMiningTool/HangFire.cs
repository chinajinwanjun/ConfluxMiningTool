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
        public HangFire(IBalanceHistoryRepository balanceHistory,IAccountRepository accountRepository, IConfiguration configuration)
        {
            this.balanceHistory = balanceHistory;
            this.accountRepository = accountRepository;
            this.configuration = configuration;
        }
        public HangFire()
        {
        }
        public void AddJob()
        {
            RecurringJob.AddOrUpdate(() => WriteMiningData(),  "*/5 * * * *");
        }

        public void WriteMiningData()
        {
            foreach (var account in accountRepository.GetAccounts())
            {
                var balance = GetBalanceByAddress(account.Address);
                if (balance>1)
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

    }
}
