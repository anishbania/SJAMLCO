using Insurance.Areas.Admins.ViewModels;

namespace Insurance.Areas.Admins.Interface
{
    public interface ICourier
    {
        public Task<List<LogisticDispatchViewModel>> GetAllCourier();

        public Task<bool> InsertCourier(LogisticDispatchViewModel model);

        public Task<LogisticDispatchViewModel> GetCourierId(int id);
        public Task<bool> DeleteCourier(int id);

    }
}
