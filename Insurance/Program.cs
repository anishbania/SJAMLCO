using Insurance.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Insurance.Models;
using Insurance.Areas.FinanceSys.Interface;
using Insurance.Areas.FinanceSys.Repository;
using Insurance.Utilities;
using System.Security.Claims;
using Insurance.Repositories;
using Insurance.Areas.Admins.Interface;
using Insurance.Areas.Risk.Interface;
using Insurance.Areas.Risk.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/RedirectRought";
    options.Cookie.Name = "FinancePalikaCookieName";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true; // Extend expiration with each request
});

//services scoped
builder.Services.AddScoped<IUtility, Utility>();
builder.Services.AddScoped<IPrayogKarta, PrayogKartaRepository>();
builder.Services.AddScoped<IAllCommonRepository, AllCommonRepository>();
//builder.Services.AddScoped<ICourier, CourierRepository>();
builder.Services.AddScoped<IRiskRegister, RiskRegisterRepository>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
            "Cache-Control", "public,max-age=600");
    }
});

app.UseRouting();

// Apply CORS policy
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", ctx =>
    {
        ctx.Response.Redirect("/Identity/Account/Login");
        return Task.CompletedTask;
    });

    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=LandingPage}/{action=Index}/{id?}"
    );
    endpoints.MapRazorPages();

});

app.MapRazorPages();

await InitializeDatabaseAsync(app);

// Start the application
app.Run();

// Method to handle async initialization
static async Task InitializeDatabaseAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await DbInitializer.SeedDataAsync(userManager, roleManager);
        await DbInitializer.SeedMonths(context);
        await DbInitializer.SeedGender(context);
        await DbInitializer.SeedCourierVendor(context);
        await DbInitializer.SeedLogisticCategories(context);
        await DbInitializer.SeedDepartments(context);
        await DbInitializer.SeedPrimaryRisk(context);
        await DbInitializer.SeedSecondaryRisk(context);
        await DbInitializer.SeedLikehood(context);
        await DbInitializer.SeedImpact(context);
        await DbInitializer.SeedRiskStatus(context);
        DbInitializer.SeedProDisPal(context);
       
    }
}
