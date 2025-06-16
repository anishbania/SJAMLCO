using Insurance.Models;
using Insurance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Insurance.ViewModels;
using Microsoft.EntityFrameworkCore;
using Insurance.Utilities;

namespace Insurance.Controllers
{
    [Authorize(Roles = "RSuperAdmin,RAdmin,IT,Admin,User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _manager;
        private readonly IUtility _utility;

        public HomeController(ILogger<HomeController> logger, IUtility utility, AppDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = dbContext;
            _manager = userManager;
            _utility = utility;
        }
        public IActionResult Index()
        {           
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult RedirectRought()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "LandingPage", new { area = "" });
            }
            if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", "User", new { area = "" });
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        [Authorize(Roles = "SuperAdmin,Admin,IT")]
        public async Task<IActionResult> IndexFiscalYear()
        {
            return View(await _utility.GetAllFiscalYearsAsync());
        }
        [Authorize(Roles = "SuperAdmin,Admin,IT")]
        public async Task<IActionResult> Create(int id = 0)
        {
            ViewBag.FiscalYears = await _utility.GetPreviousFiscalYear();

            return View(await _utility.GetFiscalYearByIdAsync(id));
        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin,IT")]
        public async Task<IActionResult> Create(FiscalYearViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _utility.InsertUpdateFiscalYearAsync(model))
                {
                    return RedirectToAction("IndexFiscalYear");
                }
            }
            return View(model);
        }
    }
}
