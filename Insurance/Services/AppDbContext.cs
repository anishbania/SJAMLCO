using Insurance.Areas.Admins.Models;
using Insurance.Areas.Risk.Models;
using Insurance.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Insurance.Services
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        #region logistic
        public DbSet<LogisticCategory> LogisticCategories { get; set; }
        public DbSet<CourierVendor> CourierVendor { get; set; }
        public DbSet<LogisticDispatch> LogisticDispatches { get; set; }
        public DbSet<LogisticItem> LogisticItems { get; set; }
        //public DbSet<CourierSupportingFile> CourierSupportingFiles { get; set; }
        #endregion 

        public DbSet<FiscalYear> FiscalYears { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Palika> Palika { get; set; }
        public DbSet<FinanceType> FinanceTypes { get; set; }
        public DbSet<Mahina> Mahinas { get; set; }
        public DbSet<SiteSetting> SiteSetting { get; set; }

        #region Risk Register 
        public DbSet<RiskRegister> RiskRegisters { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<PrimaryRisk> PrimaryRisk { get; set; }
        public DbSet<SecondaryRisk> SecondaryRisk { get; set; }
        public DbSet<Likehood> Likehoods { get; set; }
        public DbSet<Impact> Impacts { get; set; }
        public DbSet<RiskStatus> RiskStatus { get; set; } 
        #endregion

    }
}

