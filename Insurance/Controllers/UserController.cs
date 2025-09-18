using Insurance.Models;
using Insurance.Services;
using Insurance.Utilities;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Claims;

namespace Insurance.Controllers
{
    [Authorize(Roles = "RSuperAdmin,IT,Admin,User")]
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string UserId = null;
        private readonly IUtility _utility;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _manager;

        public UserController(AppDbContext appDbContext, IUtility utility,
            UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _utility = utility;
            _context = appDbContext;
            _manager = userManager;
            UserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        }

        public IActionResult Index()
        {

            return View();
        }

    }
}
