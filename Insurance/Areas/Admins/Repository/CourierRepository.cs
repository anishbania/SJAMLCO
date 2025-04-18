using Insurance.Areas.Admins.Interface;
using Insurance.Areas.Admins.Models;
using Insurance.Areas.Admins.ViewModels;
using Insurance.Repositories;
using Insurance.Services;
using Insurance.Utilities;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using System.Security.Cryptography;
//using System.Web.Providers.Entities;

namespace Insurance.Areas.Admins.Repository
{
    public class CourierRepository : ICourier
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CourierRepository> _logger;
        private readonly IAllCommonRepository _commonRepository;
        private readonly string userId = null;
        private readonly string userRole = null;
        private readonly IUtility _utility;
        public CourierRepository(AppDbContext context, IWebHostEnvironment webHostEnvironment, IAllCommonRepository commonRepository, ILogger<CourierRepository> logger, IHttpContextAccessor httpContextAccessor, IUtility utility)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _utility = utility;
            _logger = logger;
            _commonRepository = commonRepository;
            _httpContextAccessor = httpContextAccessor;
            userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            userRole = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        }
        public async Task<bool> DeleteCourier(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var dispatch = await _context.LogisticDispatches.Include(d => d.LogisticItems).FirstOrDefaultAsync(x => x.Id == id);
                    if (dispatch != null)
                    {
                        if (dispatch.LogisticItems != null && dispatch.LogisticItems.Any())
                        {
                            _context.LogisticItems.RemoveRange(dispatch.LogisticItems);
                        }
                        _context.LogisticDispatches.Remove(dispatch);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    await transaction.RollbackAsync();
                    return false;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
        public async Task<List<LogisticDispatchViewModel>> GetAllCourier()
        {

            if (userRole == "Admin")
            {
                var CourierDispatchList = await (from x in _context.LogisticDispatches
                                                 select new LogisticDispatchViewModel
                                                 {
                                                     Id = x.Id,
                                                     BranchCode = x.BranchCode,
                                                     VendorID = x.VendorID,
                                                     DispatchDate = x.DispatchDate,
                                                     Status = x.Status,
                                                     SendBy = _context.Users.Where(d => d.Id == x.SendBy).Select(d => d.FullName).FirstOrDefault(),
                                                     ReceivedBy = _context.Users.Where(d => d.Id == x.ReceivedBy).Select(d => d.FullName).FirstOrDefault(),
                                                     ReceivedDate = x.ReceivedDate,
                                                     Remarks = x.Remarks,
                                                     ModifiedBy = x.ModifiedBy,
                                                     ModifiedDate = x.ModifiedDate,
                                                     SupportingFilePath = x.SupportingFilePath,
                                                     ModeOfCourier = x.ModeOfCourier,
                                                     ChalaniNo = x.ChalaniNo,
                                                     DartaNo = x.DartaNo,
                                                     BranchName = _context.Branch.Where(b => b.Id == Convert.ToInt32(x.BranchCode)).Select(b => b.Name).FirstOrDefault(),
                                                     VendorName = _context.CourierVendor.Where(c => c.Id == x.VendorID).Select(c => c.VendorName).FirstOrDefault()
                                                 }).ToListAsync();
                return CourierDispatchList;


            }
            else
            {
                int? userBranchId = await _context.Users.Where(u => u.Id == userId).Select(u => u.BranchId).FirstOrDefaultAsync();
                string branchCode = userBranchId?.ToString();
                var CourierDispatchList = await (from x in _context.LogisticDispatches
                                                 where(x.BranchCode== branchCode)
                                                 select new LogisticDispatchViewModel
                                                 {
                                                     Id = x.Id,
                                                     BranchCode = x.BranchCode,
                                                     VendorID = x.VendorID,
                                                     DispatchDate = x.DispatchDate,
                                                     Status = x.Status,
                                                     SendBy = _context.Users.Where(d => d.Id == x.SendBy).Select(d => d.FullName).FirstOrDefault(),
                                                     ReceivedBy = _context.Users.Where(d => d.Id == x.ReceivedBy).Select(d => d.FullName).FirstOrDefault(),
                                                     ReceivedDate = x.ReceivedDate,
                                                     Remarks = x.Remarks,
                                                     ModifiedBy = x.ModifiedBy,
                                                     ModifiedDate = x.ModifiedDate,
                                                     SupportingFilePath = x.SupportingFilePath,
                                                     ModeOfCourier = x.ModeOfCourier,
                                                     ChalaniNo = x.ChalaniNo,
                                                     DartaNo = x.DartaNo,
                                                     BranchName = _context.Branch.Where(b => b.Id == Convert.ToInt32(x.BranchCode)).Select(b => b.Name).FirstOrDefault(),
                                                     VendorName = _context.CourierVendor.Where(c => c.Id == x.VendorID).Select(c => c.VendorName).FirstOrDefault()
                                                 }).ToListAsync();
                return CourierDispatchList;
            }

        }

        public async Task<LogisticDispatchViewModel> GetCourierId(int id)
        {
            var CourierData = await _context.LogisticDispatches.Where(x => x.Id == id).Select(x => new LogisticDispatchViewModel
            {
                Id = x.Id,
                BranchCode = x.BranchCode,
                BranchName = _context.Branch.Where(b => b.Id == Convert.ToInt32(x.BranchCode)).Select(b => b.Name).FirstOrDefault(),
                VendorName = _context.CourierVendor.Where(c => c.Id == x.VendorID).Select(c => c.VendorName).FirstOrDefault(),
                VendorID = x.VendorID,
                DispatchDate = x.DispatchDate,
                Status = x.Status,
                SendBy = x.SendBy,
                ReceivedBy = x.ReceivedBy,
                ReceivedDate = x.ReceivedDate,
                Remarks = x.Remarks,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
                ModeOfCourier = x.ModeOfCourier,
                ChalaniNo = x.ChalaniNo,
                DartaNo = x.DartaNo,
                Items = _context.LogisticItems.Where(c => c.DispatchId == x.Id).Select(c => new LogisticItemViewModel
                {
                    ID = c.ID,
                    ItemName = c.ItemName,
                    Qty = c.Qty,
                    UnitType = c.UnitType,
                    CategoryID = c.CategoryID,
                    CategoryName = _context.LogisticCategories.Where(cat => cat.Id == c.CategoryID).Select(cat => cat.Category).FirstOrDefault()
                }).ToList(),

            }).FirstOrDefaultAsync() ?? new LogisticDispatchViewModel();
            return CourierData;
        }

        public async Task<bool> InsertCourier(LogisticDispatchViewModel model)
        {
            if (model.SupportingFile != null && model.SupportingFile.Any())
            {
                var filePaths = new List<string>();

                foreach (var file in model.SupportingFile)
                {
                    var path = await _utility.UploadandGetPath("CourierFile", file);
                    if (!string.IsNullOrEmpty(path))
                    {
                        filePaths.Add(path);
                    }
                }
                model.SupportingFilePath = string.Join(",", filePaths);
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (model.Id > 0)
                    {
                        var data = await _context.LogisticDispatches.FirstOrDefaultAsync(x => x.Id == model.Id);
                        if (data != null)
                        {                           
                            data.BranchCode = model.BranchCode;          
                            data.Remarks = model.Remarks;
                            data.ModifiedBy = userId;
                            data.ModifiedDate = DateTime.Now;
                            data.ModeOfCourier= model.ModeOfCourier;
                            data.SupportingFilePath = (string.IsNullOrEmpty(model.SupportingFilePath)) ? data.SupportingFilePath : model.SupportingFilePath;                           
                            if (model.Items.Count > 0)
                            {
                                foreach (var item in model.Items)
                                {
                                    var abc = await _context.LogisticItems.Where(x => x.ID == item.ID).FirstOrDefaultAsync();
                                    {
                                        if (abc != null)
                                        {
                                            abc.DispatchId = data.Id;
                                            abc.ItemName = item.ItemName;
                                            abc.Qty = item.Qty;
                                            abc.UnitType = item.UnitType;
                                            abc.CategoryID = item.CategoryID;
                                            _context.Entry(abc).State = EntityState.Modified;
                                        }
                                        else
                                        {
                                            LogisticItem member = new()
                                            {
                                                DispatchId = data.Id,
                                                ItemName = item.ItemName,
                                                Qty = item.Qty,
                                                UnitType = item.UnitType,
                                                CategoryID = item.CategoryID,
                                            };
                                            await _context.LogisticItems.AddAsync(member);

                                        }
                                    }
                                }
                            }
                            _context.Entry(data).State = EntityState.Modified;
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        int count = await _context.LogisticDispatches.CountAsync() + 1;
                        LogisticDispatch data = new()
                        {
                            BranchCode = model.BranchCode,
                            DispatchDate = DateTime.Now,
                            VendorID = model.VendorID,
                            Status = "NEW",
                            SendBy = userId,                           
                            Remarks = model.Remarks,
                            ModeOfCourier = model.ModeOfCourier,
                            SupportingFilePath = model.SupportingFilePath,
                            SequenceNumber = count  // Store the sequence number.

                        };
                        DateTime refDate = data.DispatchDate != DateTime.MinValue ? data.DispatchDate : DateTime.Now;
                        data.ChalaniNo = await _utility.GenerateChalaniNumber(refDate,data.SequenceNumber); 
                        await _context.LogisticDispatches.AddAsync(data);
                        await _context.SaveChangesAsync();
                        if (model.Items.Count > 0)
                        {
                            foreach (var item in model.Items)
                            {
                                LogisticItem member = new()
                                {
                                    DispatchId = data.Id,
                                    ItemName = item.ItemName,
                                    Qty = item.Qty,
                                    UnitType = item.UnitType,
                                    CategoryID = item.CategoryID,
                                };
                                await _context.LogisticItems.AddAsync(member);
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Courier Dispatch create/update Error User Id = " + userId + " Date : " + DateTime.Now + " Error log : " + ex);
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        //public async Task<bool> Delete(string source, int id)
        //{
        //    string filename = null;

        //    string wwwroot = _webHostEnvironment.WebRootPath;
        //    filename = Path.GetFileName(source);
        //    var filePath = Path.Combine(wwwroot + "/Reports/BelowTwoYears", filename);
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        try
        //        {
        //            if (id != 0)
        //            {
        //                var pics = await _context.BelowTwoYearsFile.Where(x => x.Id == id).FirstOrDefaultAsync();

        //                _context.Remove(pics);
        //                _context.SaveChanges();
        //                File.Delete(filePath);
        //            }
        //            else
        //                return false;
        //        }
        //        catch (Exception ex)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

    }
}
