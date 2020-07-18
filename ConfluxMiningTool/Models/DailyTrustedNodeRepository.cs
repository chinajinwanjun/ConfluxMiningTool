using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{

    public class DailyTrustedNodeRepository:IDailyTrustedNodeRepository
    {
        private readonly AppDbContext db;
        public DailyTrustedNodeRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Save(List<string> wallletList)
        {
            List<DailyTrustedNode> dailyTrustedNodes = new List<DailyTrustedNode>();
            foreach (var wallet in wallletList)
            {
                dailyTrustedNodes.Add(new DailyTrustedNode { CreatedTime = DateTime.Now, WalletAddress = wallet });
            }
            db.DailyTrustedNode.AddRange(dailyTrustedNodes);
            db.SaveChanges();
        }
    }
}
