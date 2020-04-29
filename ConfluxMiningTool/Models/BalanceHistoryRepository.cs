using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class Chart
    {
        public List<string> labels { get; set; }
        public List<D> datasets { get; set; }

        public class D
        {
            public List<double> data { get; set; }
        }
    }

    public class BalanceHistoryRepository : IBalanceHistoryRepository
    {
        private readonly AppDbContext db;
        public BalanceHistoryRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Add(BalanceHistory balanceHistory)
        {
            db.BalanceHistory.Add(balanceHistory);
            db.SaveChanges();
        }

        public dynamic GetChartByAddress(string addresses)
        {
            var chart = new Chart();
            var address = addresses.Split(',');
            var maxCount = 0;
            var maxAddress = "";
            foreach (var tmpAddress in address)
            {
                var currentCnt = db.BalanceHistory.Where(x => x.Address == tmpAddress).Count();
                if (currentCnt > maxCount)
                {
                    maxCount = currentCnt;
                    maxAddress = tmpAddress;
                }
            }
            var list = db.BalanceHistory.Where(x => x.Address == maxAddress).OrderByDescending(x => x.ID).Take(100).OrderBy(x => x.ID).ToList();

            chart.labels = new List<string>();
            chart.datasets = new List<Chart.D>();
            var xs = new List<double>();
            foreach (var item in list)
            {
                chart.labels.Add(item.CreatedTime.ToString("HH:mm"));
            }
            var pointLength = list.Count;
            foreach (var a in addresses.Split(','))
            {
                Chart.D d = new Chart.D();
                d.data = new List<double>();
                for (int i = 0; i < pointLength; i++)
                {
                    d.data.Add(0);
                }
                var balanceHistorys = db.BalanceHistory.Where(x => x.Address == a).OrderByDescending(x => x.ID).Take(100).OrderByDescending(x => x.ID).ToList();
                var index = 0;
                foreach (var balanceHistory in balanceHistorys)
                {
                    d.data[index] = Math.Round(balanceHistory.Balance, 2);
                    index++;
                }
                d.data.Reverse();
                chart.datasets.Add(d);
            }
            return chart;
        }
    }
}
