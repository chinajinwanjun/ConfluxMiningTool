using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConfluxMiningTool.Models;

namespace ConfluxMiningTool.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IBalanceHistoryRepository _balanceHistory;
        private readonly IAccountRepository accountRepository;
        private readonly ITrustNodeRepository trustNodeRepository;
        public HomeController(ILogger<HomeController> logger, IBalanceHistoryRepository balanceHistory, IAccountRepository accountRepository, ITrustNodeRepository trustNodeRepository)
        {
            _logger = logger;
            this.accountRepository = accountRepository;
            this._balanceHistory = balanceHistory;
            this.trustNodeRepository = trustNodeRepository;
        }

        public IActionResult Index()
        {
            ViewBag.trustNodes = trustNodeRepository.GetAll();
            ViewBag.trustedWalletAddress = trustNodeRepository.GetTrustedWalletAddress();

            ViewBag.trustNodesActive = trustNodeRepository.GetAllActive();
            ViewBag.trustedWalletAddressActive = trustNodeRepository.GetTrustedWalletAddressActive();
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
        public IActionResult Register()
        {
            return View();
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
    }
}
