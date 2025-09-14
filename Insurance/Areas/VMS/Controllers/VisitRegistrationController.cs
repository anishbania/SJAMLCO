using Insurance.Areas.VMS.Interface;
using Insurance.Areas.VMS.Models;
using Insurance.Areas.VMS.ViewModels;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Threading.Tasks;

namespace Insurance.Areas.VMS.Controllers
{
    [Area("VMS")]
    public class VisitRegistrationController : Controller
    {
        private readonly IVisitRegistration _visitRegistration;
        public VisitRegistrationController(IVisitRegistration visitorRegistration)
        {
            _visitRegistration = visitorRegistration;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _visitRegistration.GetAllAsync());
        }
        public async Task<IActionResult> CreateVisit(int? id)
        {
            return View(await _visitRegistration.GetByIdAsync(id));
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateVisit(VisitViewModel model)
        {
            ResponseModel reponse = await _visitRegistration.AddORUpdateAsync(model);
            if (reponse.Status)
            {
                TempData["success"] = reponse.Message;
                return RedirectToAction("Index", "VisitorRegistration", new { area = "VMS" });
            }
            else
            {
                TempData["error"] = reponse.Message;
            }
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult GetVisitPartial(VisitType type)
        {
            var model = new VisitViewModel { VisitType = type };
            string partialViewName = type switch
            {
                VisitType.InPerson => "_InPersonVisitPartial",
                VisitType.DocDelivery => "_DeliveryVisitPartial",
                _ => ""
            };
            if (string.IsNullOrEmpty(partialViewName))
            {
                TempData["error"] = "Invalid visit type.";
            }

            return PartialView(partialViewName, model);
        }
        [AllowAnonymous]
        public IActionResult Success()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult QrCodeForCreateVisit()
        {
            // Build absolute URL to your CreateVisit page (GET)
            var createVisitPath = Url.Action("CreateVisit", "VisitRegistration", new { area = "VMS" });
            string targetUrl = $"{Request.Scheme}://{Request.Host}{createVisitPath}";

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrData = qrGenerator.CreateQrCode(targetUrl, QRCodeGenerator.ECCLevel.Q))
            using (var png = new PngByteQRCode(qrData))
            {
                var bytes = png.GetGraphic(pixelsPerModule: 20);
                return File(bytes, "image/png");
            }
        }

    }
}
