using Insurance.Areas.VMS.Interface;
using Insurance.Areas.VMS.Models;
using Insurance.Areas.VMS.ViewModels;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using System.Globalization;

namespace Insurance.Areas.VMS.Controllers
{
    [Authorize(Roles = "IT,Admin,User")]
    [Area("VMS")]
    public class VisitRegistrationController : Controller
    {
        private readonly IVisitRegistration _visitRegistration;
        private readonly IDataProtector _protector;

        // Protect tokens with this purpose
        private const string ProtectorPurpose = "VisitRegistration.QrTokens";

        // 10 minute lifetime
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(10);

        public VisitRegistrationController(IVisitRegistration visitorRegistration, IDataProtectionProvider dataProtectionProvider)
        {
            _visitRegistration = visitorRegistration;
            _protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _visitRegistration.GetAllAsync());
        }

        // Allow anonymous if token valid; otherwise show "QR expired" page
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CreateVisit(int? id, string token = null)
        {
            // Authenticated users proceed normally
            if (User?.Identity != null && User.Identity.IsAuthenticated)
                return View(await _visitRegistration.GetByIdAsync(id));

            // No token provided -> show expired / request to rescan
            if (string.IsNullOrEmpty(token))
            {
                return View("QrExpired");
            }

            // Validate token: unprotect -> parse expiry -> check expiry
            try
            {
                var plain = _protector.Unprotect(token);
                // plain format: "<expiryIso>" e.g. "2025-09-16T12:34:56.0000000Z"
                // (we are keeping payload minimal - just expiry timestamp)
                if (string.IsNullOrEmpty(plain))
                    return View("QrExpired");

                var expiry = DateTime.Parse(plain, null, DateTimeStyles.RoundtripKind);
                if (expiry < DateTime.UtcNow)
                {
                    // expired
                    return View("QrExpired");
                }

                // token valid -> allow anonymous access
                return View(await _visitRegistration.GetByIdAsync(id));
            }
            catch
            {
                // invalid / tampered token
                return View("QrExpired");
            }
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

        // Generate QR that encodes CreateVisit?token=PROTECTED_TOKEN
        // Token contains expiry timestamp and is protected by DataProtection.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult QrCodeForCreateVisit()
        {
            // compute expiry UTC
            var expiryUtc = DateTime.UtcNow.Add(TokenLifetime);

            // Payload is expiry ISO string only (keeps token short)
            var payload = expiryUtc.ToString("o"); // round-trip ISO

            // Protect the payload (encrypt + sign) -> string safe for query param
            var token = _protector.Protect(payload);

            // Build absolute URL to CreateVisit including token query param
            var createVisitPath = Url.Action("CreateVisit", "VisitRegistration", new { area = "VMS", token = token });
            string targetUrl = $"{Request.Scheme}://{Request.Host}{createVisitPath}";

            // Generate PNG QR bytes using QRCoder
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrData = qrGenerator.CreateQrCode(targetUrl, QRCodeGenerator.ECCLevel.Q))
            using (var png = new PngByteQRCode(qrData))
            {
                var bytes = png.GetGraphic(pixelsPerModule: 20);
                return File(bytes, "image/png");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult QrCode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return BadRequest("text is required");

            // If text comes from query use Uri.UnescapeDataString if needed
            string payload = text;

            using (var qrGenerator = new QRCodeGenerator())
            using (var qrData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q))
            using (var png = new PngByteQRCode(qrData))
            {
                var bytes = png.GetGraphic(20);
                return File(bytes, "image/png");
            }
        }
    }
}
