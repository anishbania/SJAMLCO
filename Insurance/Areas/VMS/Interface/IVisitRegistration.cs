using Insurance.Areas.VMS.ViewModels;
using Insurance.ViewModels;

namespace Insurance.Areas.VMS.Interface
{
    public interface IVisitRegistration
    {
        Task<List<VisitViewModel>> GetAllAsync();
        Task<VisitViewModel> GetByIdAsync(int? id);
        Task<ResponseModel> AddORUpdateAsync(VisitViewModel model);
    }
}
