using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public interface ITrustNodeRepository
    {
        public void Store(string info,string bakIP);
        public List<TrustNode> GetAll();
        public List<string> GetTrustedWalletAddress();
        public List<string> GetTodayTrustedWalletAddressActive();
        public List<ActiveTrustNode> GetAllActive();
        public List<string> GetTrustedWalletAddressActive();
        public void UpdateLatAndLon();
        public List<NodeIPAndLocation> GetNodeIPAndLocation();
        public AppDbContext GetDb() ;
    }
}
