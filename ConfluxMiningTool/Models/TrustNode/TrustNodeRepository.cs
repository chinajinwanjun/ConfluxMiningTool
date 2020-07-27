using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConfluxMiningTool.Models
{
    public class LatAndLon
    {
        public string query { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
    }
    public class TrustNodeRepository
    {
        public AppDbContext db;

        public TrustNodeRepository(AppDbContext db)
        {
            this.db = db;
        }
        public AppDbContext GetDb()
        {
            return db;
        }
        public List<TrustNode> GetAll()
        {
            return db.TrustNode.ToList();
        }
        public List<ActiveTrustNode> GetAllActive()
        {
            var threeDaysAgo = DateTime.Now.AddDays(-3);
            return db.ActiveTrustNode.Where(x => x.CreatedDate >= threeDaysAgo).ToList();
        }
        public List<string> GetTrustedWalletAddress()
        {
            List<string> luckTrustedWallet = new List<string>();
            var trustNodeList = db.TrustNode.ToList();
            foreach (var trustNode in trustNodeList)
            {
                var ipaddressList = trustNode.IPAddressList.Split(",");
                foreach (var ipaddress in ipaddressList)
                {
                    if (!string.IsNullOrEmpty(ipaddress))
                    {
                        if (trustNodeList.Where(x => x.TrustedIPList.Contains(ipaddress)).Count() > 0)
                        {
                            luckTrustedWallet.Add(trustNode.WalletAddress);
                        }
                    }

                }
            }
            return luckTrustedWallet.Distinct().ToList();
        }

        public List<string> GetTrustedWalletAddressActive()
        {
            List<string> luckTrustedWallet = new List<string>();
            var trustNodeList = db.ActiveTrustNode.ToList();
            foreach (var trustNode in trustNodeList)
            {
                var ipaddressList = trustNode.IPAddressList.Split(",");
                foreach (var ipaddress in ipaddressList)
                {
                    if (!string.IsNullOrEmpty(ipaddress))
                    {
                        if (trustNodeList.Where(x => x.TrustedIPList.Contains(ipaddress)).Count() > 0)
                        {
                            luckTrustedWallet.Add(trustNode.WalletAddress);
                        }
                    }

                }
            }
            return luckTrustedWallet.Distinct().ToList();
        }
        public List<string> GetTodayTrustedWalletAddressActive()
        {
            List<string> luckTrustedWallet = new List<string>();
            var ActiveTrustNode = db.ActiveTrustNode.ToList();
            var trustNodeList = ActiveTrustNode.Where(x => x.CreatedDate.Date == DateTime.Today).ToList();
            foreach (var trustNode in trustNodeList)
            {
                var ipaddressList = trustNode.IPAddressList.Split(",");
                foreach (var ipaddress in ipaddressList)
                {
                    if (!string.IsNullOrEmpty(ipaddress))
                    {
                        if (trustNodeList.Where(x => x.TrustedIPList.Contains(ipaddress)).Count() > 0)
                        {
                            luckTrustedWallet.Add(trustNode.WalletAddress);
                        }
                    }

                }
            }
            return luckTrustedWallet.Distinct().ToList();
        }
        public void Store(string info, string bakIP)
        {

            //info 1aeab1e09fa82a7aca916f8f9e8b45763a6d4d97|||66.154.104.2|||,13.75.113.21,20.43.79.41,13.69.186.42,34.201.56.239,35.177.114.1,139.224.68.249,15.185.81.4,139.162.100.191,13.229.52.100,52.175.52.111,123.57.45.90,39.97.208.206,182.92.92.126,39.97.223.148,101.201.127.86,59.110.70.5,13.67.73.51,34.222.188.35,18.228.10.237,202.182.127.199,61.184.85.5,34.216.73.138,47.88.79.174,137.135.25.219,18.163.111.230,52.231.28.50,23.96.108.96,13.115.14.17118
            if (info != null)
            {
                info = info.Replace("MMMMMM", "||||||");

                var trustNodes = info.Split("|||");
                if (trustNodes.Length == 3)
                {
                    var walletAddress = trustNodes[0];
                    if (walletAddress == null || walletAddress.Length == 0 || walletAddress.Length != 40 || !walletAddress.StartsWith("1"))
                    {
                        return;
                    }

                    var ipAddress = trustNodes[1];
                    if (ipAddress.Length < 4 && bakIP.Length >= 4)
                    {
                        ipAddress = bakIP;
                    }
                    var trustNodeIPList = trustNodes[2];
                    var trustNode = db.TrustNode.FirstOrDefault(x => x.WalletAddress == walletAddress);

                    #region active trust node

                    var activeTrustNode = db.ActiveTrustNode.FirstOrDefault(x => x.WalletAddress == walletAddress);
                    var lat = "";
                    var lon = "";
                    var ip = activeTrustNode.IPAddressList;
                    if (activeTrustNode != null)
                    {
                        if (ipAddress == ip)
                        {
                            lat = activeTrustNode.Lat;
                            lon = activeTrustNode.Lon;
                        }
                        db.ActiveTrustNode.Remove(activeTrustNode);
                    }
                    db.ActiveTrustNode.Add(new ActiveTrustNode
                    {
                        CreatedDate = DateTime.Now,
                        WalletAddress = walletAddress,
                        IPAddressList = ipAddress,
                        TrustedIPList = trustNodeIPList,
                        Lat = lat,
                        Lon = lon,
                    });
                    #endregion

                    if (trustNode == null)
                    {
                        db.TrustNode.Add(new TrustNode { WalletAddress = walletAddress, IPAddressList = string.Empty, TrustedIPList = string.Empty, });
                        db.SaveChanges();
                        trustNode = db.TrustNode.FirstOrDefault(x => x.WalletAddress == walletAddress);
                    }

                    //handle ip address
                    if (!trustNode.IPAddressList.Split(",").Contains(ipAddress))
                    {
                        trustNode.IPAddressList += "," + ipAddress;
                    }

                    //handle trust ip address
                    var oldTrustNodeList = trustNode.TrustedIPList.Split(",").ToList();
                    var newTrustNodeList = trustNodeIPList.Split(",").ToList();
                    oldTrustNodeList.AddRange(newTrustNodeList);
                    var sumTrustNodeList = oldTrustNodeList.Distinct().OrderBy(x => x);
                    trustNode.TrustedIPList = string.Join(",", sumTrustNodeList);
                    db.SaveChanges();
                }
            }
        }


        public void UpdateLatAndLon(List<LatAndLon> latAndLons)
        {
            foreach (var latAndLon in latAndLons)
            {
                var obj = db.ActiveTrustNode.FirstOrDefault(x => x.IPAddressList == latAndLon.query);
                if (obj != null)
                {
                    obj.Lat = latAndLon.lat;
                    obj.Lon = latAndLon.lon;
                }
            }
            db.SaveChanges();
        }
        public List<NodeIPAndLocation> GetNodeIPAndLocation()
        {
            List<NodeIPAndLocation> NodeIPAndLocationList = new List<NodeIPAndLocation>();
            var activeTrustNodeList = db.ActiveTrustNode.Where(x => x.Lat != null);
            foreach (var activeTrustNode in activeTrustNodeList)
            {
                NodeIPAndLocationList.Add(new NodeIPAndLocation
                {
                    Lat = activeTrustNode.Lat,
                    Lon = activeTrustNode.Lon,
                    WalletAddress = activeTrustNode.WalletAddress,
                });
            }
            return NodeIPAndLocationList;
        }
    }
}
