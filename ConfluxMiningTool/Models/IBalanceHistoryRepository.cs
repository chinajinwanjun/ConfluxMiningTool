using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public interface IBalanceHistoryRepository
    {
        public void Add(BalanceHistory balanceHistory);
        public dynamic GetChartByAddress(string address);
    }
}
