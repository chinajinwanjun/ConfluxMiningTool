using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public interface IDailyTrustedNodeRepository
    {
        public void Save(List<string> wallletList);
    }
}
