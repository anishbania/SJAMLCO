using Insurance.Areas.Admins.ViewModels;
using Insurance.Models;
using Insurance.ViewModels;

namespace Insurance.Areas.FinanceSys.Interface
{
    public interface IPrayogKarta
    {
        Task<List<PrayogKartaViewModel>> GetAll(); 
        Task<PrayogKartaViewModel> GetById(string id);
        Task<ResponseModel> Create(PrayogKartaViewModel prayogKarta);
        Task<ResponseModel> Delete(string id);
        Task<bool> ResetPass(string id);
    }
}
