using Insurance.Areas.Risk.Interface;
using Insurance.Areas.Risk.Models;
using Insurance.Areas.Risk.ViewModels;
using Insurance.Services;
using Insurance.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Insurance.Areas.Risk.Repository
{
    public class RiskRegisterRepository : IRiskRegister
    {
        private readonly ILogger<RiskRegisterRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly string userId = null;
        public RiskRegisterRepository(ILogger<RiskRegisterRepository> logger, IConfiguration configuration, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _context = context;
            userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseModel> AddUpdateRiskRegisterAsync(RiskRegisterViewModel model)
        {
            ResponseModel response = new();
            var username = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name) ?? _httpContextAccessor.HttpContext.User.Identity.Name;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.ID > 0)
                    {
                        var data = await _context.RiskRegisters.FirstOrDefaultAsync(x => x.ID == model.ID);
                        if (data != null)
                        {
                            data.RiskID = model.RiskID;
                            data.RegisterDate = model.RegisterDate;
                            data.RiskDescription = model.RiskDescription;
                            data.Department = model.Department;
                            data.PrimaryRisk = model.PrimaryRisk;
                            data.SecondaryRisk = model.SecondaryRisk;
                            data.LikeHood = model.LikeHood;
                            data.Impact = model.Impact;
                            data.RiskOwner = model.RiskOwner;
                            data.MitigationAction = model.MitigationAction;
                            data.RiskStatus = model.RiskStatus;
                            data.ClosedDate = model.ClosedDate;
                            data.Quantification = model.Quantification;
                            data.UpdatedDate = DateTime.Now;
                            data.UpdatedBy = username;
                            data.Remarks = model.Remarks;
                            data.RiskResponse = model.RiskResponse;


                            _context.Entry(data).State = EntityState.Modified;
                        }
                        else
                        {
                            response.Message = "Risk Register not found.";
                            response.Status = false;
                            return response;
                        }
                    }
                    else
                    {
                        RiskRegister abc = new()
                        {
                            RiskID = model.RiskID,
                            RegisterDate = model.RegisterDate,
                            RiskDescription = model.RiskDescription,
                            Department = model.Department,
                            PrimaryRisk = model.PrimaryRisk,
                            SecondaryRisk = model.SecondaryRisk,
                            LikeHood = model.LikeHood,
                            Impact = model.Impact,
                            RiskOwner = model.RiskOwner,
                            MitigationAction = model.MitigationAction,
                            RiskStatus = model.RiskStatus,
                            ClosedDate = model.ClosedDate,
                            Quantification = model.Quantification,
                            Remarks = model.Remarks,
                            RiskResponse = model.RiskResponse,
                            CreadtedBy=username,
                            CreatedDate = DateTime.Now,

                        };
                        await _context.RiskRegisters.AddAsync(abc);
                        response.PrimaryId = model.ID > 0 ? model.ID : abc.ID;
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    response.Status = true;
                    response.Message = model.ID > 0 ? "Risk Register updated successfully." : "Risk Register added successfully.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in AddUpdateRiskRegisterAsync");
                    await transaction.RollbackAsync();
                    response.Message = "An error occurred while processing your request.";
                }
            }
            return response;
        }

        public async Task<ResponseModel> DeleteRiskRegisterAsync(int id)
        {
            var response = new ResponseModel();
            try
            {
                var riskRegister = await _context.RiskRegisters.FirstOrDefaultAsync(x => x.ID == id);
                if (riskRegister != null)
                {
                    _context.RiskRegisters.Remove(riskRegister);
                    await _context.SaveChangesAsync();
                    response.PrimaryId = riskRegister.ID;
                    response.Message = "Risk Register deleted successfully.";
                    response.Status = true;
                }
                else
                {
                    response.Message = "Risk Register not found.";
                    response.Status = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Risk Register with ID: {Id}", id);
                response.Message = "An error occurred while deleting the Risk Register.";
                response.Status = false;
            }
            return response;
        }

        public async Task<List<RiskRegisterViewModel>> GetAllRiskRegistersAsync()
        {
            var result = await (from x in _context.RiskRegisters
                                select new RiskRegisterViewModel()
                                {
                                    ID = x.ID,
                                    RiskID = x.RiskID,
                                    RegisterDate = x.RegisterDate,
                                    RiskDescription = x.RiskDescription,
                                    Department = x.Department,
                                    PrimaryRisk = x.PrimaryRisk,
                                    SecondaryRisk = x.SecondaryRisk,
                                    LikeHood = x.LikeHood,
                                    Impact = x.Impact,
                                    RiskOwner = x.RiskOwner,
                                    MitigationAction = x.MitigationAction,
                                    RiskStatus = x.RiskStatus,
                                    ClosedDate = x.ClosedDate,
                                    Quantification = x.Quantification,
                                    UpdatedDate = x.UpdatedDate,
                                    Remarks = x.Remarks,
                                    RiskResponse = x.RiskResponse
                                }).ToListAsync() ?? new List<RiskRegisterViewModel>();
            return result;
        }

        public async Task<RiskRegisterViewModel> GetRiskRegisterByIdAsync(int id)
        {
            var result = await _context.RiskRegisters.AsNoTracking().Where(x => x.ID == id)
                .Select(x => new RiskRegisterViewModel()
                {
                    ID = x.ID,
                    RiskID = x.RiskID,
                    RegisterDate = x.RegisterDate,
                    RiskDescription = x.RiskDescription,
                    Department = x.Department,
                    PrimaryRisk = x.PrimaryRisk,
                    SecondaryRisk = x.SecondaryRisk,
                    LikeHood = x.LikeHood,
                    Impact = x.Impact,
                    RiskOwner = x.RiskOwner,
                    MitigationAction = x.MitigationAction,
                    RiskStatus = x.RiskStatus,
                    ClosedDate = x.ClosedDate,
                    Quantification = x.Quantification,
                    UpdatedDate = x.UpdatedDate,
                    Remarks = x.Remarks,
                    RiskResponse = x.RiskResponse
                }).FirstOrDefaultAsync() ?? new RiskRegisterViewModel();
            return result;
        }
    }
}
