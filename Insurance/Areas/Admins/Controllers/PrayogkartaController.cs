using Insurance.Areas.Admins.ViewModels;
using Insurance.Areas.FinanceSys.Interface;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.Areas.FinanceSys.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admins")]
    public class PrayogkartaController : Controller
    {
        private readonly IPrayogKarta _prayogKarta;

        public PrayogkartaController(IPrayogKarta prayogKarta)
        {
            _prayogKarta = prayogKarta;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _prayogKarta.GetAll());
        }

        public async Task<IActionResult> Create(string? id)
        {
            return View(await _prayogKarta.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrayogKartaViewModel prayogKarta)
        {
            ResponseModel response = await _prayogKarta.Create(prayogKarta);
            if (response.Status == true)
            {
                return RedirectToAction("Index");
            }
            if (response.Status == false && response.Message == "AlreadyHaveUser")
            {
                TempData["error"] = "यो सहकारीको प्रयोगकर्ता बनिसकेको छ";
                return RedirectToAction("Create", "PrayogKarta", new { id = prayogKarta.Id });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            ResponseModel response = await _prayogKarta.Delete(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ResetPass(string id)
        {
            await _prayogKarta.ResetPass(id);
            return Json(await _prayogKarta.ResetPass(id));
        }
    }
}
