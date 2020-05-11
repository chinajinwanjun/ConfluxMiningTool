using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public interface ITrustNodeRepository
    {
        public void Store(string info);
        public List<TrustNode> GetAll();
        public List<string> GetTrustedWalletAddress();
    }
}
