using Insurance.Areas.Admins.Interface;
using Insurance.Areas.Admins.ViewModels;
using Insurance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Insurance.Areas.Admins.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [Area("Admins")]
    public class CourierController : Controller
    {
        private readonly ILogger<CourierController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICourier _courier;
        private readonly AppDbContext _context;
        private readonly string userId = null;
        public CourierController(ICourier courier, ILogger<CourierController> logger, AppDbContext context, IHttpContextAccessor httpContextAccessor    )
        {
            _courier = courier;
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _courier.GetAllCourier());
        }
        public async Task<IActionResult> Create(int id = 0)
        {
            return View(await _courier.GetCourierId(id));
        }
        [HttpPost]
        public async Task<IActionResult> Create(LogisticDispatchViewModel model)
        {
            var result = await _courier.InsertCourier(model);
            if (result)
            {
                TempData["success"] = "Courirer Dispatched Successfully";
                return RedirectToAction("Index");
            }
            //if (ModelState.IsValid)
            //{
               
            //}
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateStatus(int id, string status)
        {
            var dispatch = await _context.LogisticDispatches.FirstOrDefaultAsync(d => d.Id == id);
            if (dispatch != null)
            {
                dispatch.Status = status;
                dispatch.ReceivedBy = userId;
                dispatch.ReceivedDate = DateTime.Now;

                _context.Update(dispatch);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public async Task<IActionResult>FileUpload(int id)
        {
            var filePathData = await _context.LogisticDispatches .Where(x => x.Id == id).Select(x => x.SupportingFilePath) .FirstOrDefaultAsync();
            // Create a new view model instance and assign the file paths to the property
            var model = new LogisticDispatchViewModel
            {
                SupportingFilePath = filePathData
            };

            return View(model);
        }       
        public async Task<IActionResult> Details(int id)
        {
            return View(await _courier.GetCourierId(id));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _courier.DeleteCourier(id);
            if (result)
            {
                TempData["success"] = "Courier Deleted Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult AddNewCourierItem() => PartialView("~/Areas/Admins/Views/Courier/_AddItems.cshtml", new LogisticItemViewModel());

    }
}
