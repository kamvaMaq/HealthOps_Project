using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HealthOps_Project.Data;
using HealthOps_Project.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// ✅ FIX: Use AddIdentity instead of AddDefaultIdentity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<HealthOps_Project.Services.IEmailService, HealthOps_Project.Services.EmailService>();
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<HealthOps_Project.Hubs.ChatHub>("/chatHub");

// ✅ Seed roles and admin
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var ctx = services.GetRequiredService<ApplicationDbContext>();

    string[] roles = new[] { "Admin", "Doctor", "Nurse", "Patient", "ScriptManager", "StockManager" };
    foreach (var r in roles)
    {
        if (!roleManager.RoleExistsAsync(r).Result)
            roleManager.CreateAsync(new IdentityRole(r)).Wait();
    }

    var adminEmail = "admin@example.com";
    var admin = userManager.FindByEmailAsync(adminEmail).Result;
    if (admin == null)
    {
        admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, FullName = "System Admin" };
        var res = userManager.CreateAsync(admin, "Admin@123").Result;
        if (res.Succeeded)
            userManager.AddToRoleAsync(admin, "Admin").Wait();
    }

    // ✅ Seed InsuranceProviders if none exist
    if (!ctx.InsuranceProviders.Any())
    {
        ctx.InsuranceProviders.AddRange(
            new InsuranceProvider { Name = "DefaultCare" },
            new InsuranceProvider { Name = "HealthPlus" },
            new InsuranceProvider { Name = "MediCover" }
        );
        ctx.SaveChanges();
    }
}

app.Run();
