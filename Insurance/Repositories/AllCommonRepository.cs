using Insurance.Areas.Admins.Models;
using Insurance.Areas.Identity.Pages.Account;
using Insurance.Models;
using Insurance.Services;
using Insurance.Utilities;
using Insurance.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using static Insurance.ViewModels.AllCommonViewModel;

namespace Insurance.Repositories
{
    public class AllCommonRepository : IAllCommonRepository
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IUtility _utility;
        private readonly string userId = null;
        private readonly string userRole = null;


        public AllCommonRepository(AppDbContext context, ILogger<RegisterModel> logger, IWebHostEnvironment webHost, IHttpContextAccessor httpContextAccessor, IUtility utility)
        {
            _context = context;
            _webHostEnvironment = webHost;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _utility = utility;
            userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            userRole = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);


        }
        public async Task<bool> DeleteSiteSettingLogoAsync(string source, int id)
        {
            string filename = null;
            // using the method 
            string wwwroot = _webHostEnvironment.WebRootPath;
            filename = Path.GetFileName(source);
            var filePath = Path.Combine(wwwroot + "/images/AppImage", filename);
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    if (id != 0)
                    {
                        var pics = await _context.SiteSetting.Where(x => x.Id == id).FirstOrDefaultAsync();

                        pics.LogoPath = null;
                        _context.SaveChanges();
                        File.Delete(filePath);
                    }
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    return false;

                }
            }
            return true;
        }

        public async Task<List<AllCommonViewModel.SiteSettingViewModel>> GetAllSiteSettings()
        {
            return await _context.SiteSetting
                .Select(x => new SiteSettingViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Address = x.Address,
                    Address2 = x.Address,
                    ContactName = x.ContactName,
                    ContactNumber = x.ContactNumber,
                    MobileNumber = x.MobileNumber,
                    FaxNo = x.FaxNo,
                    Website = x.Website,
                    Slogan = x.Slogan,
                    LogoPath = x.LogoPath,
                }).ToListAsync();
        }

        public async Task<AllCommonViewModel.SiteSettingViewModel> GetSiteSettingByIdAsync(int id)
        {
            var data = await _context.SiteSetting
                       .Select(x => new SiteSettingViewModel()
                       {
                           Id = x.Id,
                           Name = x.Name,
                           Email = x.Email,
                           State = x.State,
                           District = x.District,
                           Palika = x.Palika,
                           Ward = x.Ward,
                           Address = x.Address,
                           Address2 = x.Address,
                           ContactName = x.ContactName,
                           ContactNumber = x.ContactNumber,
                           MobileNumber = x.MobileNumber,
                           Slogan = x.Slogan,
                           FaxNo = x.FaxNo,
                           Website = x.Website,
                           LogoPath = x.LogoPath
                       }).FirstOrDefaultAsync() ?? new SiteSettingViewModel();
            return data;
        }

        public async Task<bool> InsertUpdateSiteSettingYearAsync(AllCommonViewModel.SiteSettingViewModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var data = _context.SiteSetting.FirstOrDefault(x => x.Id == model.Id);
                    if (data != null)
                    {
                        data.Name = model.Name;
                        data.Slogan = model.Slogan;
                        data.Email = model.Email;
                        data.ContactName = model.ContactName;
                        data.ContactNumber = model.ContactNumber;
                        data.MobileNumber = model.MobileNumber;
                        data.Address = model.Address;
                        data.Address2 = model.Address2;
                        data.FaxNo = model.FaxNo;
                        data.State = model.State;
                        data.District = model.District;
                        data.Palika = model.Palika;
                        data.Ward = model.Ward;
                        data.Website = model.Website;
                        data.LogoPath = (model.ProfileImage == null) ? data.LogoPath : await UploadImgAsync(null, model.ProfileImage);

                        _context.Entry(data).State = EntityState.Modified;
                    }
                    else
                        return false;
                }
                else
                {
                    var siteSetting = new SiteSetting()
                    {
                        Name = model.Name,
                        Slogan = model.Slogan,
                        Email = model.Email,
                        State = model.State,
                        District = model.District,
                        Palika = model.Palika,
                        Ward = model.Ward,
                        ContactName = model.ContactName,
                        ContactNumber = model.ContactNumber,
                        MobileNumber = model.MobileNumber,
                        Address = model.Address,
                        Address2 = model.Address2,
                        FaxNo = model.FaxNo,
                        Website = model.Website,

                    };
                    siteSetting.LogoPath = await UploadImgAsync(null, model.ProfileImage);
                    await _context.SiteSetting.AddAsync(siteSetting);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> UploadImgAsync(string folderName, IFormFile file)
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

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);// if Path not present than create

                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, returnPath);
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


        public async Task<FileUploadModel> UploadImgReturnPathAndName(string folderName, IFormFile img)
        {
            try
            {
                FileUploadModel model = new FileUploadModel();
                string returnPath = null;
                if (img != null)
                {
                    var fileExt = Path.GetExtension(img.FileName).Substring(1);
                    folderName = string.IsNullOrEmpty(folderName) ? "Reports" : folderName;
                    folderName = (folderName == "Reports") ? "Reports" : folderName + "/";
                    model.FileName = Guid.NewGuid().ToString() + "." + fileExt;
                    returnPath = folderName + model.FileName;

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);// if Path not present than create

                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, returnPath);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await img.CopyToAsync(fileStream);
                    }
                    model.FilePath = "/" + returnPath;
                    return model;
                }
                else
                    return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
