using Insurance.Areas.Risk.Models;
using Insurance.Areas.Risk.ViewModels;
using Insurance.ViewModels;

namespace Insurance.Areas.Risk.Interface
{
    public interface IRiskRegister
    {
        Task<List<RiskRegisterViewModel>> GetAllRiskRegistersAsync();
        Task<RiskRegisterViewModel> GetRiskRegisterByIdAsync(int id);
        Task <ResponseModel>AddUpdateRiskRegisterAsync(RiskRegisterViewModel model);
        Task<ResponseModel> DeleteRiskRegisterAsync(int id);
        Task<ImportResult> ImportFromExcelAsync(IFormFile excelFile);


    }
}
