using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mytrade.Controllers
{
    public class LandingController : Controller
    {
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
            ViewData["Message"] = "Client Area";
            return View();
        }

        [Authorize(Policy = "tm")]
        public IActionResult TM()
        {
            ViewData["Message"] = "Tradesmen Area";
            return View();
        }

        [Authorize(Policy = "admin")]
        public IActionResult Admin()
        {
            ViewData["Message"] = "Administrator Area";
            return View();
        }
    }
}