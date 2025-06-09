using Insurance.Areas.Admins.ViewModels;
using Insurance.Areas.FinanceSys.Interface;
using Insurance.Models;
using Insurance.Services;
using Insurance.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Areas.FinanceSys.Repository
{
    public class PrayogKartaRepository : IPrayogKarta
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<PrayogKartaRepository> _logger;

        public PrayogKartaRepository(UserManager<ApplicationUser> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager, ILogger<PrayogKartaRepository> logger)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<ResponseModel> Create(PrayogKartaViewModel prayogKarta)
        {
            ResponseModel responseModel = new();

            var users = await _userManager.Users.ToListAsync();
            int anyOldUser = users.Where(x => x.UserName == prayogKarta.Email && x.Id != prayogKarta.Id).Count();
            if (anyOldUser > 0)
            {
                responseModel.Status = false;
                responseModel.Message = "AlreadyHaveUser";
                return responseModel;
            }

            if (prayogKarta.Id != null)
            {
                var existingUser = await _userManager.FindByIdAsync(prayogKarta.Id);

                if (existingUser != null)
                {
                    existingUser.FullName = prayogKarta.FullName;
                    existingUser.PhoneNumber = prayogKarta.PhoneNumber;
                    existingUser.Department = prayogKarta.Department;
                    existingUser.Email = prayogKarta.Email;
                    existingUser.PrayogkartaName = prayogKarta.PrayogkartaName;
                    var updateResult = await _userManager.UpdateAsync(existingUser);

                    if (updateResult.Succeeded)
                    {
                        // Remove existing roles and add the new role
                        var existingRoles = await _userManager.GetRolesAsync(existingUser);
                        await _userManager.RemoveFromRolesAsync(existingUser, existingRoles);
                        await _userManager.AddToRoleAsync(existingUser, prayogKarta.Role);

                        responseModel.Status = true;
                        return responseModel;
                    }
                    else
                    {
                        responseModel.Status = false;
                    }
                }
            }
            else
            {
                var newUser = new ApplicationUser
                {
                    FullName = prayogKarta.FullName,
                    PrayogkartaName = prayogKarta.PrayogkartaName,
                    Department= prayogKarta.Department,
                    Email = prayogKarta.Email,
                    PhoneNumber = prayogKarta.PhoneNumber,
                    HasLoggedIn = false,
                    LastLoginDate = null,
                    UserName = prayogKarta.Email.ToUpper(),
                };

                var createResult = await _userManager.CreateAsync(newUser, prayogKarta.Password);

                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, prayogKarta.Role);

                    responseModel.Status = true;
                    return responseModel;
                }
                else
                {
                    responseModel.Status = false;
                }
            }

            return responseModel;
        }


        public async Task<ResponseModel> Delete(string id)
        {
            ResponseModel responseModel = new();
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    responseModel.Status = true;
                    return responseModel;
                }
            }
            responseModel.Status = false;
            return responseModel;
        } 
        public async Task<ResponseModel> DeleteRole(string id)
        {
            ResponseModel responseModel = new();
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    responseModel.Status = true;
                    return responseModel;
                }
            }
            responseModel.Status = false;
            return responseModel;
        }

        public async Task<bool> ResetPass(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(user, "NewPass@123");
                    if (result.Succeeded)
                    {
                        return true;
                    }
                }
            }
           
            return false;
        }

        public async Task<List<PrayogKartaViewModel>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            List<PrayogKartaViewModel> kartas = new();

            foreach (var item in users)
            {
                var roles = await _userManager.GetRolesAsync(item);

                var karta = new PrayogKartaViewModel
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Department = item.Department,
                    PrayogkartaName = item.PrayogkartaName,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    HasLogged = item.HasLoggedIn,
                    LastLogged = item.LastLoginDate,
                    Role = roles.FirstOrDefault()
                };
                kartas.Add(karta);
            }
            return kartas;
        }

        public async Task<PrayogKartaViewModel> GetById(string id)
        {
            var users = await _userManager.FindByIdAsync(id);
            if (users != null)
            {
                var roles = await _userManager.GetRolesAsync(users);
                PrayogKartaViewModel prayogKarta = new()
                {
                    Id = users.Id,
                    FullName = users.FullName,
                    Department = users.Department,
                    PrayogkartaName = users.PrayogkartaName,
                    Email = users.Email,
                    PhoneNumber = users.PhoneNumber,
                    Role = roles.FirstOrDefault()

                };
                return prayogKarta;
            }
            return null;
        }

        public async  Task<List<UserRolesModel>> GetRoles()
        {
            var roles = _roleManager.Roles;
            var roleModels = roles.Select(role => new UserRolesModel
            {
                Id = role.Id,
                RoleName = role.NormalizedName,
            }).ToList();

            return await Task.FromResult(roleModels);
        }

        public async  Task<UserRolesModel> GetRolesById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                UserRolesModel roles = new UserRolesModel();
                roles.Id = role.Id;
                roles.RoleName = role.Name;
                return roles;
            }
            return null;
        }

        public async Task<bool> CreateRole(UserRolesModel model)
        {
            var existingRole = await _roleManager.FindByNameAsync(model.RoleName);
            if (existingRole != null)
            {
                _logger.LogInformation("Failed to Create a new role for " + model.RoleName + " because it is already created.");
                return false;
            }

            var newRole = new IdentityRole(model.RoleName);
            var result = await _roleManager.CreateAsync(newRole);

            return true;
        }
    }
}
