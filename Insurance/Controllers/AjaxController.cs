using Insurance.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.Controllers
{
    public class AjaxController : Controller
    {
        private readonly IUtility _utility;

        public AjaxController(IUtility utility)
        {
            _utility = utility;
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetDistrictByStateId(int id)
        {
            return Json(await _utility.GetDistrictSelectListItems(id));
        }
        [AllowAnonymous]
        public async Task<JsonResult> GetPalikaByDistId(int id)
        {
            return Json(await _utility.GetPalikaSelectListItems(id));
        }
    }
}
