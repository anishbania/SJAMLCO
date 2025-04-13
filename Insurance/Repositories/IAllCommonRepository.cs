using Insurance.Areas.Admins.Models;
using Insurance.Models;
using static Insurance.ViewModels.AllCommonViewModel;

namespace Insurance.Repositories
{
    public interface IAllCommonRepository
    {
        Task<List<SiteSettingViewModel>> GetAllSiteSettings();
        Task<SiteSettingViewModel> GetSiteSettingByIdAsync(int id);
        Task<bool> InsertUpdateSiteSettingYearAsync(SiteSettingViewModel model);
        Task<bool> DeleteSiteSettingLogoAsync(string source, int id);
        Task<FileUploadModel> UploadImgReturnPathAndName(string folderName, IFormFile img);        
    }
}
