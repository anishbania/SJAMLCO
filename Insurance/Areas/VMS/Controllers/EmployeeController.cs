using ClosedXML.Excel;
using Insurance.Areas.VMS.Interface;
using Insurance.Areas.VMS.ViewModels;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Insurance.Areas.VMS.Controllers
{
    [Authorize(Roles = "IT,Admin,User")]
    [Area("VMS")]
    public class EmployeeController : Controller
    {
        private readonly IEmployee _employeeService;
        public EmployeeController(IEmployee employeeService)
        {
            _employeeService=employeeService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _employeeService.GetAllEmployeesAsync());
        }
        public async Task<IActionResult>CreateOrEdit(int? id)
        {
            if (id.HasValue)
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
                return View(employee);
            }
            return View(new EmployeeViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrEdit(EmployeeViewModel model)
        {
            ResponseModel response = await _employeeService.AddOrUpdateAsync(model);
            if (response.Status)
            {
                TempData["success"] = response.Message;
                return RedirectToAction("Index", "Employee", new { area = "VMS" });
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View("CreateOrEdit", model);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseModel response = await _employeeService.DeleteAsync(id);
            if (response.Status)
            {
                TempData["success"] = response.Message;
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return RedirectToAction("Index", "Employee", new { area = "VMS" });
        }
    }
}
