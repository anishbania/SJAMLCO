using ClosedXML.Excel;
using Insurance.Areas.Risk.Interface;
using Insurance.Areas.Risk.Models;
using Insurance.Areas.Risk.ViewModels;
using Insurance.Models;
using Insurance.Services;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace Insurance.Areas.Risk.Repository
{
    public class RiskRegisterRepository : IRiskRegister
    {
        private readonly ILogger<RiskRegisterRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly string userId = null;
        private readonly string userRole = null;

        public RiskRegisterRepository(ILogger<RiskRegisterRepository> logger, UserManager<ApplicationUser> userManager, IConfiguration configuration, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _context = context;
            _userManager = userManager;
            userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            userRole = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseModel> AddUpdateRiskRegisterAsync(RiskRegisterViewModel model)
        {
            ResponseModel response = new();
            var username = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name) ?? _httpContextAccessor.HttpContext.User.Identity.Name;
            var departmentPrefixes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ADMIN"] = "ADM",
                ["AGENCY"] = "AGY",
                ["BOD"] = "BOD",
                ["BRANDING"] = "BRD",
                ["CLAIM AND LEGAL"] = "CLG",
                ["CORPORATE SALES"] = "CPS",
                ["CUSTOMER SERVICE"] = "CUS",
                ["FINANCE"] = "FIN",
                ["HUMAN RESOURCE"] = "HR",
                ["INTERNAL AUDIT"] = "IAD",
                ["IT"] = "IT",
                ["REINSURANCE"] = "RI",
                ["TRAINING"] = "TRN",
                ["UNDERWRITING"] = "UND",
                ["RISK DEPARTMENT"] = "RISK`",
            };           
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.ID > 0)
                    {
                        var data = await _context.RiskRegisters.FirstOrDefaultAsync(x => x.ID == model.ID);
                        if (data != null)
                        {
                            data.RegisterDate = model.RegisterDate;
                            data.RiskDescription = model.RiskDescription;
                            data.Username = model.Department;
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
                            data.LikehoodId = await _context.Likehoods.Where(x => x.Name == model.LikeHood).Select(x => x.Id).FirstOrDefaultAsync();
                            data.ImpactId = await _context.Impacts.Where(x => x.Name == model.Impact).Select(x => x.Id).FirstOrDefaultAsync();


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
                        // 2) Determine the prefix for this department
                        if (!departmentPrefixes.TryGetValue(model.Department, out var prefix))
                        {
                            // fallback or error if department unknown
                            return new ResponseModel
                            {
                                Status = false,
                                Message = $"Unknown department '{model.Department}'. cannot assign RiskID."
                            };
                        }

                        // 3) Fetch existing RiskIDs for that prefix, pick the max numeric suffix
                        //    e.g. existing: FIN-0012, FIN-0013 → we want suffix=13
                        var latest = await _context.RiskRegisters
                            .Where(r => r.RiskID.StartsWith(prefix + "-"))
                            .OrderByDescending(r => r.RiskID)
                            .Select(r => r.RiskID)
                            .FirstOrDefaultAsync();

                        int nextNumber = 1;
                        if (!string.IsNullOrEmpty(latest))
                        {
                            // parse the suffix after the dash
                            var parts = latest.Split('-');
                            if (parts.Length == 2 && int.TryParse(parts[1], out var lastNum))
                            {
                                nextNumber = lastNum + 1;
                            }
                        }

                        // 4) Build new RiskID — e.g. "FIN-1" 
                        var newRiskId = $"{prefix}-{nextNumber}";
                        RiskRegister abc = new()
                        {

                            RiskID =newRiskId,
                            Username=model.Department,
                            LikehoodId = await _context.Likehoods.Where(x=>x.Name==model.LikeHood).Select(x=>x.Id).FirstOrDefaultAsync(),
                            ImpactId = await _context.Impacts.Where(x => x.Name == model.Impact).Select(x => x.Id).FirstOrDefaultAsync(),
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
                            CreatedBy=username,
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
                    response.Message =ex.Message;
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
            
            if (userRole == "IT" || userRole == "RSuperAdmin")
            {
                var result = await (from x in _context.RiskRegisters
                                    select new RiskRegisterViewModel()
                                    {
                                        ID = x.ID,
                                        RiskID = x.RiskID,
                                        Username = x.Username,
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
                                        RiskResponse = x.RiskResponse,
                                        LikehoodId = x.LikehoodId,
                                        ImpactId = x.ImpactId,
                                        LikeHoodScore = x.Likehood != null ? x.Likehood.NumScore : 0,
                                        ImpactScore = x.Impacts != null ? x.Impacts.NumScore : 0,
                                    }).ToListAsync() ?? new List<RiskRegisterViewModel>();
                return result;
            }
            else
            {
                var department = await _context.Users.AsNoTracking().Where(x => x.Id == userId).Select(x => x.Department).FirstOrDefaultAsync();
                var result = await (from x in _context.RiskRegisters
                                    where x.Department == department
                                    select new RiskRegisterViewModel()
                                    {
                                        ID = x.ID,
                                        RiskID = x.RiskID,
                                        Username = x.Username,
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
                                        RiskResponse = x.RiskResponse,
                                        LikehoodId = x.LikehoodId,
                                        ImpactId = x.ImpactId,
                                        LikeHoodScore = x.Likehood != null ? x.Likehood.NumScore : 0,
                                        ImpactScore = x.Impacts != null ? x.Impacts.NumScore : 0,
                                    }).ToListAsync() ?? new List<RiskRegisterViewModel>();
                return result;
            }
        }

        public async Task<RiskRegisterViewModel> GetRiskRegisterByIdAsync(int id)
        {
           

            var result = await _context.RiskRegisters.AsNoTracking().Where(x => x.ID == id)
                .Select(x => new RiskRegisterViewModel()
                {
                    ID = x.ID,
                    RiskID = x.RiskID,
                    Username = x.Username,
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
                    RiskResponse = x.RiskResponse,
                    LikehoodId = x.LikehoodId,
                    ImpactId = x.ImpactId,
                    LikeHoodScore = x.Likehood != null ? x.Likehood.NumScore : 0,
                    ImpactScore = x.Impacts != null ? x.Impacts.NumScore : 0,
                }).FirstOrDefaultAsync() ?? new RiskRegisterViewModel();
            return result;
        }
        public async Task<ImportResult> ImportFromExcelAsync(IFormFile excelFile)
        {
            var result = new ImportResult();

            if (excelFile == null || excelFile.Length == 0)
            {
                result.Errors.Add("No file provided.");
                return result;
            }

            using var stream = excelFile.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheets.FirstOrDefault();
            if (sheet == null)
            {
                result.Errors.Add("Workbook contains no worksheet.");
                return result;
            }

            int row = 2;
            while (true)
            {
                var idCell = sheet.Cell(row, "B");
                var riskId = idCell.GetString().Trim();
                if (string.IsNullOrWhiteSpace(riskId))
                    break;

                try
                {
                    var username = sheet.Cell(row, "A").GetString().Trim();

                    // RegisterDate (nullable DateTime)
                    DateTime? registerDate = null;
                    if (!sheet.Cell(row, "C").IsEmpty())
                        registerDate = sheet.Cell(row, "C").GetDateTime();

                    // Text fields (may be empty)
                    var description = sheet.Cell(row, "D").GetString().Trim();
                    var department = sheet.Cell(row, "E").GetString().Trim();
                    var primaryRisk = sheet.Cell(row, "F").GetString().Trim();
                    var secondaryRisk = sheet.Cell(row, "G").GetString().Trim();
                    var riskOwner = sheet.Cell(row, "M").GetString().Trim();
                    var mitigationAction = sheet.Cell(row, "N").GetString().Trim();
                    var riskStatus = sheet.Cell(row, "O").GetString().Trim();
                    var riskResponse = sheet.Cell(row, "Q").GetString().Trim();
                    var remarks = sheet.Cell(row, "R").GetString().Trim();

                    // Numeric fields (nullable int)
                    int? likeHoodId = null;
                    if (!sheet.Cell(row, "H").IsEmpty())
                        likeHoodId = sheet.Cell(row, "H").GetValue<int>();

                    int? impactId = null;
                    if (!sheet.Cell(row, "I").IsEmpty())
                        impactId = sheet.Cell(row, "I").GetValue<int>();

                    int? quantification = null;
                    if (!sheet.Cell(row, "L").IsEmpty())
                        quantification = sheet.Cell(row, "L").GetValue<int>();

                    // ClosedDate (nullable DateTime)
                    DateTime? closedDate = null;
                    if (!sheet.Cell(row, "P").IsEmpty())
                        closedDate = sheet.Cell(row, "P").GetDateTime();

                    // UpdatedDate (nullable DateTime)
                    DateTime? updatedDate = null;
                    if (!sheet.Cell(row, "S").IsEmpty())
                        updatedDate = sheet.Cell(row, "S").GetDateTime();

                    // Map into entity (make sure these props are nullable)
                    var entity = new RiskRegister
                    {
                        RiskID = riskId,
                        Username= username,
                        RegisterDate = registerDate,
                        RiskDescription = description,
                        Department = department,
                        PrimaryRisk = primaryRisk,
                        SecondaryRisk = secondaryRisk,
                        LikehoodId = likeHoodId,
                        ImpactId = impactId,
                        Quantification = quantification,
                        RiskOwner = riskOwner,
                        MitigationAction = mitigationAction,
                        RiskStatus = riskStatus,
                        ClosedDate = closedDate,
                        RiskResponse = riskResponse,
                        Remarks = remarks,
                        CreatedBy = "import",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = updatedDate
                    };

                    _context.RiskRegisters.Add(entity);
                    result.RowsImported++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Row {row}: {ex.Message}");
                }

                row++;
            }

            if (result.RowsImported > 0)
                await _context.SaveChangesAsync();

            result.Success = result.Errors.Count == 0;
            return result;
        }
    }
}
