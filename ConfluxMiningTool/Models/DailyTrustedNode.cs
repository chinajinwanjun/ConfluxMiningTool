using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class DailyTrustedNode
    {
        public long ID { get; set; }
        public string WalletAddress { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
