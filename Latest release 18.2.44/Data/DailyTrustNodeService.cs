using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Latest_release_18._2._44.Data
{

    public class DailyTrustNodeService
    {
        private SoftwareDbContext db;
        public DailyTrustNodeService(SoftwareDbContext db)
        {
            this.db = db;
        }

        public List<DailyTrustedNodeDto> Get30DayPivotDataSource()
        {
            List<DailyTrustedNodeDto> dailyTrustedNodeDtoList = new List<DailyTrustedNodeDto>();
            var walletList = Get30DayWalletAddress();
            var DailyTrustedNodes = db.DailyTrustedNodes.ToList();

            foreach (var wallet in walletList)
            {
                for (int i = 0; i < 30; i++)
                {
                    if (DailyTrustedNodes.Exists(x =>  x.CreatedDate.Date == DateTime.Now.AddDays(-i).Date))
                    {
                        if (DailyTrustedNodes.Exists(x => x.WalletAddress == wallet && x.CreatedDate.Date == DateTime.Now.AddDays(-i).Date))
                        {
                            dailyTrustedNodeDtoList.Add(new DailyTrustedNodeDto { WalletAddress = wallet.Substring(0, 5) + "***", Value = 1, CreatedDate = DateTime.Now.AddDays(-i).ToString("MMM dd") });
                        }
                        else
                        {
                            dailyTrustedNodeDtoList.Add(new DailyTrustedNodeDto { WalletAddress = wallet.Substring(0, 5) + "***", Value = 0, CreatedDate = DateTime.Now.AddDays(-i).ToString("MMM dd") });
                        }
                    }
                  
                }
            }
            return dailyTrustedNodeDtoList;
        }

        public IEnumerable<string> Get30DayWalletAddress()
        {
            return db.DailyTrustedNodes.Where(x => x.CreatedDate > DateTime.Now.AddDays(-30)).Select(x => x.WalletAddress).Distinct();
        }

    }
}
