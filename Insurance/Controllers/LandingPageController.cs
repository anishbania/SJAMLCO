using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.Controllers
{
    public class LandingPageController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
    }
}
