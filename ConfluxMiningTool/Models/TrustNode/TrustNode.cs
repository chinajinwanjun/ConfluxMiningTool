using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class TrustNode
    {
        public long ID { get; set; }
        public string WalletAddress { get; set; }
        public string IPAddressList { get; set; }
        public string TrustedIPList { get; set; }

    }
    public class NodeIPAndLocation
    {
        public string WalletAddress { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
    }
    public class ActiveTrustNode
    {
        public long ID { get; set; }
        public string WalletAddress { get; set; }
        public string IPAddressList { get; set; }
        public string TrustedIPList { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }

    }
}
