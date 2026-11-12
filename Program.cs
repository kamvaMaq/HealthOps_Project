using HealthOps_Project.Data;
using HealthOps_Project.Hubs;
using HealthOps_Project.Models;
using HealthOps_Project.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// --- DbContext ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- Identity ---
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// --- Services ---
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register existing services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();
builder.Services.AddScoped<IPdfService, QuestPdfService>();

// Register OpenAI Service with HttpClient
builder.Services.AddHttpClient<IOpenAIService, OpenAIService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "HealthOps-Project/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IOpenAIService, OpenAIService>();

// SignalR
builder.Services.AddSignalR();

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// --- Middleware ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// --- Routes ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<HealthOps_Project.Hubs.ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");

//// --- Seed Data ---
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await SeedData.InitializeAsync(services);
//}

app.Run();