using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class BalanceHistory
    {
        public long ID { get; set; }
        public string Address { get; set; }
        public double Balance { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
