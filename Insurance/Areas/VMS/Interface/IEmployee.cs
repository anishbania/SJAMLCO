using Insurance.Areas.VMS.ViewModels;
using Insurance.ViewModels;

namespace Insurance.Areas.VMS.Interface
{
    public interface IEmployee
    {
        Task<List<EmployeeViewModel>> GetAllEmployeesAsync();
        Task<EmployeeViewModel> GetEmployeeByIdAsync(int id);   
        Task<ResponseModel> AddOrUpdateAsync(EmployeeViewModel model);
        Task<ResponseModel> DeleteAsync(int id);
    }
}
