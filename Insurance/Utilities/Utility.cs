using Insurance.Models;
using Insurance.Services;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
namespace Insurance.Utilities
{
    public class Utility : IUtility
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly string UserId = null;
        private readonly string UserRole = null;


        public Utility(AppDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _userManager = userManager;
            _signInManager = signInManager;
            UserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserRole = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        }

        public async Task<string> GetUserName()
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId);
            return user.FullName;
        }
        public async Task<string> GetLoggedUserEmail()
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId);
            return user.Email;

        }
        public async Task<string> GetProjectName()
        {
            var ProjectName = await _context.SiteSetting.Select(x => x.Name).FirstOrDefaultAsync();
            return ProjectName;
        }
        public async Task<string> GetPalikaName()
        {
            var ProjectName = await (from x in _context.SiteSetting
                                     join p in _context.Palika on x.Palika equals p.PalikaId
                                     select p.PalikaName_Nep).FirstOrDefaultAsync();

            return ProjectName;
        }

        public async Task<SelectList> GetDistrictSelectListItems()
        {
            return new SelectList(await _context.District.ToListAsync(), "DistrictId", "DistrictName_Nep");
        }
        public async Task<SelectList> GetDistrictSelectListItems(int? stateId)
        {
            return new SelectList(await _context.District.Where(x => (x.StateId == stateId || stateId == null)).ToListAsync(), "DistrictId", "DistrictName_Nep");
        }
        public async Task<SelectList> GetPalikaSelectListItems()
        {
            return new SelectList(await _context.Palika.ToListAsync(), "PalikaId", "PalikaName_Nep");
        }
        public async Task<SelectList> GetPalikaSelectListItems(int? distId)
        {
            return new SelectList(await _context.Palika.Where(x => (x.DistrictId == distId || distId == null)).ToListAsync(), "PalikaId", "PalikaName_Nep");
        }
        public async Task<SelectList> GetStateSelectListItems()
        {
            return new SelectList(await _context.State.ToListAsync(), "StateId", "StateName_Nep");
        }

        public string English_Nepali(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var engValue = new Dictionary<string, string>
                {
                    { "0", "०" },
                    { "1", "१" },
                    { "2", "२" },
                    { "3", "३" },
                    { "4", "४" },
                    { "5", "५" },
                    { "6", "६" },
                    { "7", "७" },
                    { "8", "८" },
                    { "9", "९" }
                };

                var result = "";
                foreach (var item in value)
                {
                    var itemString = item.ToString();
                    if (engValue.ContainsKey(itemString))
                    {
                        result += engValue[itemString];
                    }
                    else
                    {
                        result += item;
                    }
                }
                return result;
            }
            else return "";
        }

        public async Task<SelectList> GetFinanceTypeListItems()
        {
            return new SelectList(await _context.FinanceTypes.ToListAsync(), "Name", "Name");
        }

        public async Task<SelectList> GetMonthSelectListItems()
        {
            return new SelectList(await _context.Mahinas.ToListAsync(), "NepaliName", "NepaliName");
        }

        //new 

        public async Task<SelectList> GetBranchSelectListItems()
        {
            return new SelectList(await _context.Branch.ToListAsync(), "Id", "Name");
        }
         public async Task<SelectList> GetCourierVendorSelectListItems()
        {
            return new SelectList(await _context.CourierVendor.ToListAsync(), "Id", "VendorName");
        } 
        public async Task<SelectList> GetLogisticCategorySelectListItems()
        {
            return new SelectList(await _context.LogisticCategories.ToListAsync(), "Id", "Category");
        }
        public async Task<string> UploadandGetPath(string folderName, IFormFile file)
        {
            try
            {
                string returnPath = null;
                if (file != null)
                {
                    var fileExt = Path.GetExtension(file.FileName).Substring(1);
                    folderName = string.IsNullOrEmpty(folderName) ? "images" : folderName;
                    folderName = (folderName == "images") ? "images/AppImage/" : "images/" + folderName + "/";
                    returnPath = folderName + Guid.NewGuid().ToString() + "." + fileExt;

                    var uploadsFolder = Path.Combine(_env.WebRootPath, folderName);
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(_env.WebRootPath, returnPath);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return "/" + returnPath;
                }
                else
                    return returnPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool RemoveFile(string filePath)
        {
            if (filePath == "0")
            {
                return true;
            }
            try
            {
                string wwwroot = _env.WebRootPath;
                var file = Path.Combine(wwwroot + filePath);
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

      

        public async Task<SelectList> GetAdminUser()
        {
            var user = await _userManager.GetUsersInRoleAsync("Admin");
            return new SelectList(user, "Id", "FullName");
        }


        public async Task<FiscalYearViewModel> GetFiscalYearByIdAsync(int id)
        {
            return await _context.FiscalYears.Where(x => x.Id == id)
                       .Select(x => new FiscalYearViewModel()
                       {
                           Id = x.Id,
                           Name = x.Name,
                           LocalName = x.LocalName,
                           AdStartDate = x.AdStartDate,
                           BsStartDate = x.BsStartDate,
                           AdEndDate = x.AdEndDate,
                           BsEndDate = x.BsEndDate,
                           PreviousFyId = x.PreviousFyId,
                           IsActive = x.IsActive
                       }).FirstOrDefaultAsync() ?? new FiscalYearViewModel();
        }

        public async Task<SelectList> GetAllFiscalYears()
        {
            return new SelectList(await _context.FiscalYears.OrderByDescending(x => x.BsStartDate).ToListAsync(), "Id", "Name");
        }

        public async Task<List<FiscalYearViewModel>> GetAllFiscalYearsAsync()
        {
            return await _context.FiscalYears.Select(x => new FiscalYearViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                LocalName = x.LocalName,
                AdStartDate = x.AdStartDate,
                BsStartDate = x.BsStartDate,
                AdEndDate = x.AdEndDate,
                BsEndDate = x.BsEndDate,
                PreviousFyId = x.PreviousFyId,
                IsActive = x.IsActive
            }).ToListAsync();
        }

        public async Task<SelectList> GetPreviousFiscalYear()
        {
            return new SelectList(await _context.FiscalYears.ToListAsync(), "Id", "Name");
        }

        public async Task<string> GetActiveFiscalYear()
        {
            var data = await _context.FiscalYears.Where(x => x.IsActive == true).FirstOrDefaultAsync();
            return data == null ? "" : data.Name;
        }

        public async Task<bool> InsertUpdateFiscalYearAsync(FiscalYearViewModel model)
        {
            try
            {
                // For only one Fiscal year active
                if (model.IsActive)
                {
                    foreach (var year in _context.FiscalYears.ToList())
                    {
                        year.IsActive = false;

                        _context.Entry(year).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                if (model.Id > 0)
                {
                    var data = _context.FiscalYears.FirstOrDefault(x => x.Id == model.Id);
                    if (data != null)
                    {
                        data.Name = model.Name;
                        data.LocalName = model.LocalName;
                        data.BsStartDate = model.BsStartDate;
                        data.BsEndDate = model.BsEndDate;
                        data.AdStartDate = model.AdStartDate;
                        data.AdEndDate = model.AdEndDate;
                        data.PreviousFyId = model.PreviousFyId;
                        data.IsActive = model.IsActive;
                        data.UpdatedDate = DateTime.Now;
                        data.UpdatedBy = _httpContextAccessor.HttpContext.User.Identity.Name;

                        _context.Entry(data).State = EntityState.Modified;
                    }
                    else
                        return false;
                }
                else
                {
                    FiscalYear fiscal = new FiscalYear()
                    {
                        Name = model.Name,
                        LocalName = model.LocalName,
                        BsStartDate = model.BsStartDate,
                        BsEndDate = model.BsEndDate,
                        AdStartDate = model.AdStartDate,
                        AdEndDate = model.AdEndDate,
                        PreviousFyId = model.PreviousFyId,
                        IsActive = model.IsActive,
                        CreatedDate = DateTime.Now,
                        CreatedBy = _httpContextAccessor.HttpContext.User.Identity.Name
                    };
                    await _context.FiscalYears.AddAsync(fiscal);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<SelectList> GetGenderSelectListItems()
        {
            return new SelectList(await _context.Genders.ToListAsync(), "GenderNepali", "GenderNepali");
        }

    }
}
