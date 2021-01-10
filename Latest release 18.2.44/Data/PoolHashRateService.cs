using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Latest_release_18._2._44.Data
{
    public class Record
    {
        public string CreatedAt { get; set; }
        public List<long> Values { get; set; }
    }
    public class PoolHashRateService
    {
        private SoftwareDbContext db;
        public PoolHashRateService(SoftwareDbContext db)
        {
            this.db = db;
        }
        public List<Record> Get()
        {
            var poolHashRate = db.PoolHashRates.OrderByDescending(x => x.ID).Take(1000).OrderBy(x=>x.ID).ToList();
            var createdAtList = poolHashRate.Select(x => x.CreatedAt.ToString("MM-dd HH:mm")).Distinct().ToList();
         
            List<Record> recordList = new List<Record>();
            foreach (var createdAt in createdAtList)
            {
                List<long> valueList = new List<long>();
                if (poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "Poolflare") != null)
                {
                    valueList.Add(poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "Poolflare").Hashrate);
                }
                if (poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "火币矿池") != null)
                {
                    valueList.Add(poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "火币矿池").Hashrate);
                }
                if (poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "蜜蜂矿池") != null)
                {
                    valueList.Add(poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "蜜蜂矿池").Hashrate);
                }
                if (poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "鱼池") != null)
                {
                    valueList.Add(poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "鱼池").Hashrate);
                }
                if (poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "独角兽矿池") != null)
                {
                    valueList.Add(poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "独角兽矿池").Hashrate);
                }
                if (poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "MatPool") != null)
                {
                    valueList.Add(poolHashRate.FirstOrDefault(x => x.CreatedAt.ToString("MM-dd HH:mm") == createdAt && x.Name == "MatPool").Hashrate);
                }
                recordList.Add(new Record { CreatedAt = createdAt, Values = valueList });
            }
            return recordList;
        }
    }
}
