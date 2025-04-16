using Insurance.Models;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurance.Utilities
{
    public interface IUtility
    {
        Task<string> GetUserName();
        Task<string> GetLoggedUserEmail();
        Task<SelectList> GetStateSelectListItems();
        Task<SelectList> GetDistrictSelectListItems();
        Task<SelectList> GetDistrictSelectListItems(int? id);
        Task<SelectList> GetPalikaSelectListItems();
        Task<SelectList> GetPalikaSelectListItems(int? id);
        Task<SelectList> GetGenderSelectListItems();
        Task<SelectList> GetMonthSelectListItems();
        Task<SelectList> GetBranchSelectListItems();
        Task<SelectList> GetCourierVendorSelectListItems();
        Task<SelectList> GetLogisticCategorySelectListItems();
        string English_Nepali(string value);

        Task<string> UploadandGetPath(string folderName, IFormFile file);
        Task<string> GetProjectName();
        Task<string> GetPalikaName();

        Task<SelectList> GetPreviousFiscalYear();
        Task<string> GetActiveFiscalYear();

        Task<List<FiscalYearViewModel>> GetAllFiscalYearsAsync();
        Task<SelectList> GetAllFiscalYears();
        Task<FiscalYearViewModel> GetFiscalYearByIdAsync(int id);
        Task<bool> InsertUpdateFiscalYearAsync(FiscalYearViewModel model);
        bool RemoveFile(string filePath);
        Task<string> GenerateChalaniNumber(DateTime referenceDate,int SequenceNumber);
        Task<string> GenerateDartaNumber(DateTime referenceDate,int SequenceNumber);



    }
}
