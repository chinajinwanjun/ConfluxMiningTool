using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConfluxMiningTool.Models;
using System.Net.Http;
using Newtonsoft.Json;

using Microsoft.EntityFrameworkCore;

namespace ConfluxMiningTool.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IBalanceHistoryRepository _balanceHistory;
        private readonly IAccountRepository accountRepository;
        private readonly TrustNodeRepository trustNodeRepository;
        private readonly ITransactionRepository transactionRepository;
        public HomeController(ILogger<HomeController> logger, IBalanceHistoryRepository balanceHistory, IAccountRepository accountRepository, TrustNodeRepository trustNodeRepository, ITransactionRepository transactionRepository)
        {
            _logger = logger;
            this.accountRepository = accountRepository;
            this._balanceHistory = balanceHistory;
            this.trustNodeRepository = trustNodeRepository;
            this.transactionRepository = transactionRepository;
        }

        public IActionResult Index()
        {
            try
            {
                ViewBag.trustNodes = trustNodeRepository.GetAll();
                ViewBag.trustedWalletAddress = trustNodeRepository.GetTrustedWalletAddress();

                ViewBag.trustNodesActive = trustNodeRepository.GetAllActive();
                ViewBag.trustedWalletAddressActive = trustNodeRepository.GetTrustedWalletAddressActive();
            }
            catch (Exception)
            {
                Redirect("/");
            }

            return View();
        }
        public IActionResult TrustedNode()
        {

            return View();
        }
        [HttpGet]
        public JsonResult GetChartByAddress(string address)
        {
            return Json(_balanceHistory.GetChartByAddress(address));
        }
        [HttpGet]
        public JsonResult GetTrustedNodeList()
        {
            return Json(trustNodeRepository.GetTrustedWalletAddress());
        }
        [HttpGet]
        public JsonResult GetNodeIPAndLocation()
        {
            return Json(trustNodeRepository.GetNodeIPAndLocation());
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public JsonResult Test()
        {
            try
            {

                {
                    HttpClient http = new HttpClient();
                    List<LatAndLon> latAndLons = new List<LatAndLon>();
                    var ipList = trustNodeRepository.GetAllActive().Where(x => x.Lat == null && x.IPAddressList != null && x.IPAddressList.Length > 4).Select(x => x.IPAddressList).Take(10).ToList();
                    foreach (var ip in ipList)
                    {
                        var api = $@"http://ip-api.com/json/{ip}";
                        var result = http.GetAsync(api).Result;
                        var data = result.Content.ReadAsStringAsync().Result;
                        var parsedData = JsonConvert.DeserializeObject<LatAndLon>(data);
                        latAndLons.Add(parsedData);
                    }
                    trustNodeRepository.UpdateLatAndLon(latAndLons);



                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(0);
        }
        [HttpPost]
        public JsonResult AddAccount(Account account)
        {
            account.Address = account.Address.ToLower();
            account.Address = "0x" + account.Address.Replace("0x", "");
            var result = accountRepository.Add(account);
            return Json(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public JsonResult StoreTrustNode(string info)
        {
            string remoteIpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                remoteIpAddress = Request.Headers["X-Forwarded-For"];
            this.trustNodeRepository.Store(info, remoteIpAddress);
            return Json(1);
        }
        public JsonResult GetBaiduNodeList()
        {
            var baiduNodeList = trustNodeRepository.GetBaiduNodeList();
            object[] formatedObj = new object[] { baiduNodeList };
            return Json(formatedObj);
        }
        public JsonResult GetNFTList()
        {
            return Json(new { Count = transactionRepository.GetNFT().Count, List = transactionRepository.GetNFT(), });
        }
    }
}
