using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mytrade.LogProvider;
using mytrade.Models;
using Microsoft.Extensions.Configuration;

namespace mytrade.Controllers
{
    public class LandingController : Controller
    {
        private CustomLoggerDBContext _context;
        private readonly ILogger<LandingController> _logger;

        IConfiguration _configration;        

        public LandingController(ILogger<LandingController> logger, CustomLoggerDBContext context, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configration = configuration;
        }

        [Authorize]
        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            foreach (var c in claims)
            {
                if (c.Value == "client")
                {
                    return RedirectToAction("Client");
                }
                if (c.Value == "tm")
                {
                    return RedirectToAction("TM");
                }
                if (c.Value == "admin")
                {
                    return RedirectToAction("Admin");
                }
            }

            ViewData["Message"] = "...";

            return View();
        }

        [Authorize(Policy = "client")]
        public IActionResult Client()
        {
            _logger.LogInformation((int)LoggingEvents.ACCOUNT_TYPE, "Account type client logged in.");

            ViewData["Message"] = "Client Area";
            return View();
        }

        [Authorize(Policy = "tm")]
        public IActionResult TM()
        {
            _logger.LogInformation((int)LoggingEvents.ACCOUNT_TYPE, "Account type tradesmen logged in.");

            ViewData["Message"] = "Tradesmen Area";
            return View();
        }

        [Authorize(Policy = "admin")]
        public IActionResult Admin()
        {
            _logger.LogInformation((int)LoggingEvents.ACCOUNT_TYPE, "Account type administrator logged in.");

            ViewData["Message"] = "Administrator Area";
            return View();
        }
    }
}