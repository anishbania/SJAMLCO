using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Insurance.Areas.VMS.Interface;
using Insurance.Areas.VMS.Models;
using Insurance.Areas.VMS.ViewModels;
using Insurance.Services;
using Insurance.ViewModels;
using Microsoft.Build.Logging;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Insurance.Areas.VMS.Repositories
{
    public class VisitRegistrationRepository : IVisitRegistration
    {
        private readonly AppDbContext _context;
        private readonly string userId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        public VisitRegistrationRepository(AppDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
        }

        public async Task<ResponseModel> AddORUpdateAsync(VisitViewModel model)
        {
            var response = new ResponseModel();

            if (model == null)
            {
                return new ResponseModel
                {
                    Status = false,
                    Message = "Invalid payload."
                };
            }

            // Begin transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {                
                // 1) Upsert Visitor
                int visitorId = model.VisitorId;
                if (model.VisitorDetails != null)
                {
                    var vm = model.VisitorDetails;
                    if (vm.Id > 0)
                    {
                        // update existing visitor
                        var existingVisitor = await _context.Visitors.FindAsync(vm.Id);
                        if (existingVisitor == null)
                        {
                            response.Status = false;
                            response.Message = "Visitor not found for update.";
                            return response;
                        }

                        existingVisitor.FullName = vm.FullName;
                        existingVisitor.PhoneNumber = vm.PhoneNumber;
                        existingVisitor.EmailAddress = vm.EmailAddress;
                        existingVisitor.Company = vm.Company;
                        existingVisitor.CreatedDate = vm.CreatedDate ?? existingVisitor.CreatedDate;

                        _context.Visitors.Update(existingVisitor);
                        visitorId = existingVisitor.Id;
                    }
                    else
                    {
                        // create new visitor
                        var newVisitor = new Visitor
                        {
                            FullName = vm.FullName,
                            PhoneNumber = vm.PhoneNumber,
                            EmailAddress = vm.EmailAddress,
                            Company = vm.Company,
                            CreatedDate = vm.CreatedDate ?? DateTimeOffset.UtcNow
                        };

                        await _context.Visitors.AddAsync(newVisitor);
                        // Save here so that newVisitor.Id is available below
                        await _context.SaveChangesAsync();
                        visitorId = newVisitor.Id;
                    }
                }
                else if (model.VisitorId <= 0)
                {
                    response.Status = false;
                    response.Message = "Visitor information is required.";
                    return response;
                }

                // -------------------------
                // 2) Create or update Visit
                // -------------------------
                if (model.Id > 0)
                {
                    // UPDATE existing visit
                    var visit = await _context.Visits.FirstOrDefaultAsync(v => v.Id == model.Id);

                    if (visit == null)
                    {
                        response.Status = false;
                        response.Message = "Visit not found for update.";
                        return response;
                    }

                    visit.VisitorId = visitorId;
                    visit.EmployeeId = model.EmployeeId;
                    visit.Purpose = model.Purpose;
                    visit.Department = model.Department;


                    visit.LengthOfMeeting = model.LengthOfMeeting;
                    visit.CheckInTime = model.CheckInTime;
                    visit.CheckOutTime = model.CheckOutTime;
                    visit.VisitType = model.VisitType;
                    visit.Notes = model.Notes;

                    //visit.ModifiedBy = userId;
                    //visit.ModifiedAt = DateTime.UtcNow;

                    _context.Visits.Update(visit);
                }
                else
                {
                    // CREATE new visit
                    var newVisit = new Visit
                    {
                        VisitorId = visitorId,
                        EmployeeId = model.EmployeeId,
                        Purpose = model.Purpose,
                        Department = model.Department,
                        LengthOfMeeting = model.LengthOfMeeting,
                        // If user provided CheckInTime use it, otherwise set current UTC time as TimeOnly
                        CheckInTime = model.CheckInTime ?? TimeOnly.FromDateTime(DateTime.UtcNow),
                        CheckInDate = model.CheckInDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                        Status = VisitStatus.CheckedIn,
                        VisitType = model.VisitType,
                        Notes = model.Notes,
                        CreatedBy = (await _context.Users.Where(x => x.Id == userId).Select(a => a.UserName).FirstOrDefaultAsync()) ?? userId ?? "system",
                        CreatedAt = DateTime.UtcNow,
                        // create a short public token (useful for anonymous check-in / QR)
                        PublicToken = Guid.NewGuid().ToString("N")
                    };

                    // If model contains visit documents (IFormFile), handle them here or in a service - example below
                    if (model.VisitDocument != null && model.VisitDocument.File != null && model.VisitDocument.File.Length > 0)
                    {
                        // TODO: Move file storage to a service for testability. Example below saves to wwwroot/uploads/visitdocs.
                        var file = model.VisitDocument.File; // IFormFile
                        var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "visitdocs");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueName = $"{DateTime.UtcNow.Ticks}_{Path.GetRandomFileName()}{Path.GetExtension(file.FileName)}";
                        var fullPath = Path.Combine(uploadsFolder, uniqueName);
                        await using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var visitDoc = new VisitDocument
                        {
                            DocumentName = Path.GetFileName(file.FileName),
                            DocumentPath = Path.Combine("/uploads/visitdocs", uniqueName).Replace("\\", "/"),
                            DocumentType = file.ContentType,
                            FileSize = file.Length
                        };
                        //newVisit.VisitDocument.Add(visitDoc);
                    }

                    await _context.Visits.AddAsync(newVisit);
                    // ensure id is available for subsequent operations (e.g., QR filename)
                    await _context.SaveChangesAsync();

                    ;
                }

                // 3) Commit transaction
              
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                response.Status = true;
                response.Message = model.Id > 0 ? "Visit updated successfully." : "Visit created successfully.";
                return response;
            }
            catch (Exception ex)
            {
                // rollback on any failure
                try { await transaction.RollbackAsync(); } catch { }

                // Log actual exception using your logger (not shown here)
                response.Status = false;
                response.Message = ex.Message;
                return response;
            }
        }


        //public async Task<ResponseModel> AddORUpdateAsync(VisitViewModel model)
        //{
        //    ResponseModel response = new();
        //    if (model == null)
        //    {
        //        return new ResponseModel
        //        {
        //            Status = false,
        //            Message = "Invalid payload."
        //        };
        //    }
        //    using (var transaction = await _context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // 1) Upsert Visitor
        //            int visitorId = model.VisitorId;
        //            if (model.VisitorDetails != null)
        //            {
        //                var vm = model.VisitorDetails;
        //                if (vm.Id > 0)
        //                {
        //                    // update existing visitor
        //                    var v = await _context.Visitors.FindAsync(vm.Id);
        //                    if (v == null)
        //                    {
        //                        // choose to create new or fail; we'll fail for safety
        //                        response.Status = false;
        //                        response.Message = "Visitor not found for update.";
        //                        return response;
        //                    }
        //                    v.FullName = vm.FullName;
        //                    v.PhoneNumber = vm.PhoneNumber;
        //                    v.EmailAddress = vm.EmailAddress;
        //                    v.Company = vm.Company;
        //                    v.CreatedDate = vm.CreatedDate ?? v.CreatedDate;
        //                    _context.Visitors.Update(v);
        //                    visitorId = v.Id;
        //                }
        //                else
        //                {
        //                    // create new visitor
        //                    var newVisitor = new Visitor
        //                    {
        //                        FullName = vm.FullName,
        //                        PhoneNumber = vm.PhoneNumber,
        //                        EmailAddress = vm.EmailAddress,
        //                        Company = vm.Company,
        //                        CreatedDate = vm.CreatedDate ?? DateTimeOffset.UtcNow
        //                    };
        //                    await _context.Visitors.AddAsync(newVisitor);
        //                    await _context.SaveChangesAsync(); // so newVisitor.Id is populated
        //                    visitorId = newVisitor.Id;
        //                }
        //            }
        //            else if (model.VisitorId <= 0)
        //            {
        //                response.Status = false;
        //                response.Message = "Visitor information is required.";
        //                return response;
        //            }

        //            // 3) Create or update Visit
        //            if (model.Id > 0)
        //            {
        //                // UPDATE existing visit
        //                var visit = await _context.Visits.Include(v => v.Visitor).FirstOrDefaultAsync(v => v.Id == model.Id);

        //                if (visit == null)
        //                {
        //                    response.Status = false;
        //                    response.Message = "Visit not found for update.";
        //                    return response;
        //                }

        //                visit.VisitorId = visitorId;
        //                visit.EmployeeId = model.EmployeeId;
        //                visit.Purpose = model.Purpose;
        //                visit.Department = model.Department;
        //                visit.CheckInTime = model.CheckInTime;
        //                visit.CheckOutTime = model.CheckOutTime;
        //                visit.VisitType = model.VisitType;
        //                visit.Notes = model.Notes;

        //                _context.Visits.Update(visit);
        //            }
        //            else
        //            {
        //                // CREATE new visit
        //                var newVisit = new Visit
        //                {
        //                    VisitorId = visitorId,
        //                    EmployeeId = model.EmployeeId,
        //                    Purpose = model.Purpose,
        //                    Department = model.Department,
        //                    LengthOfMeeting=model.LengthOfMeeting,
        //                    CheckInTime = TimeOnly.FromDateTime(DateTime.Now),
        //                    CheckInDate= DateOnly.FromDateTime(DateTime.UtcNow),
        //                    Status = VisitStatus.CheckedIn,
        //                    VisitType = model.VisitType,
        //                    Notes = model.Notes,
        //                    CreatedBy = await _context.Users.Where(x => x.Id == userId).Select(a => a.UserName).FirstOrDefaultAsync() ?? "system",
        //                    CreatedAt = DateTime.UtcNow
        //                };

        //                await _context.Visits.AddAsync(newVisit);
        //            }

        //            // Save all changes & commit
        //            await _context.SaveChangesAsync();

        //            response.Status = true;
        //            response.Message = model.Id > 0 ? "Visit updated successfully." : "Visit created successfully.";
        //            return response;
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Status = false;
        //            response.Message = ex.Message;
        //            await transaction.RollbackAsync();
        //        }
        //        finally
        //        {
        //            await transaction.CommitAsync();
        //        }
        //    }
        //    return response;
        //}

        public async Task<List<VisitViewModel>> GetAllAsync()
        {
            return (await _context.Visits
                .Select(e => new VisitViewModel
                {
                    VisitorId = e.Id,
                    EmployeeId = e.EmployeeId,
                    Purpose = e.Purpose,
                    Department = e.Department,
                    CheckInTime = e.CheckInTime,
                    CheckOutTime = e.CheckOutTime,
                    Status = e.Status,
                    VisitType = e.VisitType,
                    Notes = e.Notes,
                    CreatedBy = e.CreatedBy,
                    CreatedAt = e.CreatedAt,
                    VisitorDetails = new VisitorViewModel
                    {
                        Id = e.Visitor.Id,
                        FullName = e.Visitor.FullName,
                        EmailAddress = e.Visitor.EmailAddress,
                        PhoneNumber = e.Visitor.PhoneNumber,
                        Company = e.Visitor.Company,
                        CreatedDate = e.Visitor.CreatedDate,
                    },
                }).ToListAsync()) ?? new List<VisitViewModel>();
        }

        public async Task<VisitViewModel> GetByIdAsync(int? id)
        {
            return (await _context.Visits
                .Where(x => x.Id == id)
                .Select(e => new VisitViewModel
                {
                    VisitorId = e.Id,
                    EmployeeId = e.EmployeeId,
                    Purpose = e.Purpose,
                    Department = e.Department,
                    CheckInTime = e.CheckInTime,
                    CheckOutTime = e.CheckOutTime,
                    Status = e.Status,
                    VisitType = e.VisitType,
                    Notes = e.Notes,
                    CreatedBy = e.CreatedBy,
                    CreatedAt = e.CreatedAt,
                    VisitorDetails = new VisitorViewModel
                    {
                        Id = e.Visitor.Id,
                        FullName = e.Visitor.FullName,
                        EmailAddress = e.Visitor.EmailAddress,
                        PhoneNumber = e.Visitor.PhoneNumber,
                        Company = e.Visitor.Company,
                        CreatedDate = e.Visitor.CreatedDate,
                    },
                }).FirstOrDefaultAsync()) ?? new VisitViewModel();
        }


    }
}
