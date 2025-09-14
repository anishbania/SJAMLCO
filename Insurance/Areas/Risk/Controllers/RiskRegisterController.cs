using Insurance.Areas.Risk.Interface;
using Insurance.Areas.Risk.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Insurance.Areas.Risk.Controllers
{
    [Authorize(Roles = "IT,RAdmin,RSuperAdmin")]
    [Area("Risk")]
    public class RiskRegisterController : Controller
    {
        private readonly IRiskRegister _riskRegister;
        private readonly IWebHostEnvironment _env;

        public RiskRegisterController(IRiskRegister riskRegister, IWebHostEnvironment env)
        {
            _riskRegister = riskRegister;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _riskRegister.GetAllRiskRegistersAsync());
        }
        public async Task<IActionResult> CreateUpdate(int? id)
        {
            return View(await _riskRegister.GetRiskRegisterByIdAsync(id ?? 0));
        }
        [HttpPost]
        public async Task<IActionResult> CreateUpdate(RiskRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _riskRegister.AddUpdateRiskRegisterAsync(model);
                if (response.Status)
                {
                    TempData["success"] = response.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _riskRegister.DeleteRiskRegisterAsync(id);
            if (response.Status)
            {
                TempData["success"] = response.Message;
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return RedirectToAction("Index");
        }
        public IActionResult DownloadSampleFormat()
        {
            var sampleFolder = Path.Combine(_env.WebRootPath, "sample");
            var filePath = Path.Combine(sampleFolder, "sample_format.xlsx");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("SampleFormat.xlsx not found on server.");
            }

            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return PhysicalFile(filePath, contentType, "sample_format.xlsx");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile excelFile)
        {
            if (excelFile == null)
            {
                ModelState.AddModelError("", "Please select an Excel file to upload.");
            }

            var result = await _riskRegister.ImportFromExcelAsync(excelFile);

            if (!result.Success)
            {
                // show errors back on the form
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err);
            }

            TempData["success"] = $"{result.RowsImported} rows imported successfully.";
            return RedirectToAction(nameof(Index));  // or wherever you list
        }
        public async Task<IActionResult> Details(int id)
        {
            var riskRegister = await _riskRegister.GetRiskRegisterByIdAsync(id);
            if (riskRegister == null)
            {
                TempData["error"] = "Risk Register not found.";
            }
            return View(riskRegister);

        }

        public IActionResult RiskReportingManual()
        {
            return View();
        }
    }
}
