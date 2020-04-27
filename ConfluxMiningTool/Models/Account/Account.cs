using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class Account
    {
        public long ID { get; set; }
        public string Address { get; set; }
        public string OS { get; set; }
        public string CPU { get; set; }
        public string Memory { get; set; }
        public string Disk { get; set; }
        public string PhoneNumber { get; set; }
    }
}
