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
        public HomeController(ILogger<HomeController> logger, IBalanceHistoryRepository balanceHistory, IAccountRepository accountRepository)
        {
            _logger = logger;
            this.accountRepository = accountRepository;
            this._balanceHistory = balanceHistory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetChartByAddress(string address)
        {
            return Json(_balanceHistory.GetChartByAddress(address));
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
    }
}
