using Insurance.Areas.VMS.Interface;
using Insurance.Areas.VMS.Models;
using Insurance.Areas.VMS.ViewModels;
using Insurance.Services;
using Insurance.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Areas.VMS.Repositories
{
    public class EmployeeRepository : IEmployee
    {
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<ResponseModel> AddOrUpdateAsync(EmployeeViewModel model)
        {
            ResponseModel response = new ResponseModel();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (model == null)
                    {
                        response.Message = "Model cannot be null";
                        response.Status = false;
                        return response;
                    }
                    if (string.IsNullOrEmpty(model.FullName))
                    {
                        response.Message = "Name cannot be empty";
                        response.Status = false;
                        return response;
                    }
                    if (string.IsNullOrEmpty(model.Email))
                    {
                        response.Message = "Email cannot be empty";
                        response.Status = false;
                        return response;
                    }
                    if (model.Id > 0)
                    {
                        // Update existing employee
                        var existingEmployee = await _context.Employee.FindAsync(model.Id);
                        if (existingEmployee == null)
                        {
                            response.Message = "Employee not found";
                            response.Status = false;
                            return response;
                        }
                        existingEmployee.FullName = model.FullName;
                        existingEmployee.Email = model.Email;
                        existingEmployee.PhoneNumber = model.PhoneNumber;
                        existingEmployee.Department = model.Department;

                        _context.Employee.Update(existingEmployee);
                    }
                    else
                    {
                        // Add new employee
                        var newEmployee = new Employee
                        {
                            FullName = model.FullName,
                            Email = model.Email,
                            Department = model.Department,
                            PhoneNumber = model.PhoneNumber,
                        };
                        await _context.Employee.AddAsync(newEmployee);
                    }
                    await _context.SaveChangesAsync();

                    response.Status = true;
                    response.Message = model.Id > 0 ? "Employee updated successfully." : "Employee created successfully.";
                    return response;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Status = false;
                    await transaction.RollbackAsync();
                }
                finally
                {
                    await transaction.CommitAsync();
                }
            }
            return response;
        }

        public async Task<ResponseModel> DeleteAsync(int id)
        {
            ResponseModel response = new ResponseModel();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var employee = await _context.Employee.FindAsync(id);
                    if (employee == null)
                    {
                        response.Message = "Employee not found";
                        response.Status = false;
                        return response;
                    }
                    _context.Employee.Remove(employee);
                    await _context.SaveChangesAsync();
                    response.PrimaryId = id;
                    response.Message = "Employee deleted successfully";
                    response.Status = true;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.Status = false;
                    await transaction.RollbackAsync();
                }
                finally
                {
                    await transaction.CommitAsync();
                }
            }
            return response;
        }

        public async Task<List<EmployeeViewModel>> GetAllEmployeesAsync()
        {
            return await _context.Employee
                .Select(e => new EmployeeViewModel
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Department = e.Department
                }).ToListAsync() ?? new List<EmployeeViewModel>();
        }

        public async Task<EmployeeViewModel> GetEmployeeByIdAsync(int id)
        {
            return (await _context.Employee
                .Where(e => e.Id == id)
                .Select(e => new EmployeeViewModel
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Department = e.Department
                }).FirstOrDefaultAsync()) ?? new EmployeeViewModel();
        }
    }
}
