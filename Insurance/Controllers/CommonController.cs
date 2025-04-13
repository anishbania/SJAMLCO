using Insurance.Services;
using Microsoft.AspNetCore.Mvc;
using static Insurance.ViewModels.AllCommonViewModel;
using Insurance.Repositories;
using Insurance.Areas.Admins.Models;
using Insurance.Areas.Admins.Interface;

namespace Insurance.Controllers
{
    public class CommonController : Controller
    {
        private readonly IAllCommonRepository _commonRepository = null;
        private readonly AppDbContext _context;
        public CommonController(IAllCommonRepository commonRepository, AppDbContext context)
        {
            _commonRepository = commonRepository;
            _context = context;
        }
        public async Task<IActionResult> SiteSetting()
        {
            return View(await _commonRepository.GetAllSiteSettings());
        }
        [HttpPost]
        public async Task<JsonResult> DeleteSiteSettingLogo(string source, int id)
        {
            return Json(await _commonRepository.DeleteSiteSettingLogoAsync(source, id));
        }
        public async Task<IActionResult> CreateSiteSetting(int id = 0)
        {
            return View(await _commonRepository.GetSiteSettingByIdAsync(id));
        }
        [HttpPost]

        public async Task<IActionResult> CreateSiteSetting(SiteSettingViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _commonRepository.InsertUpdateSiteSettingYearAsync(model))
                {
                    return RedirectToAction("SiteSetting");
                }
            }
            return View(model);
        }

    }
}
