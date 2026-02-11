using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UsalClinic.Application.Interfaces;
using UsalClinic.Application.Mapper;
using UsalClinic.Application.Models;
using UsalClinic.Application.Services;
using UsalClinic.Core;
using UsalClinic.Core.Entities;
using UsalClinic.Core.Repositories;
using UsalClinic.Infrastructure;
using UsalClinic.Infrastructure.Repository;
using UsalClinic.Infrastructure.Services;
using UsalClinic.Web.Data;
using UsalClinic.Web.MappingProfiles;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Connection string and DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Add this if you're using Razor Pages

// AutoMapper registrations
builder.Services.AddAutoMapper(typeof(UsalClinicDtoMapper).Assembly);
builder.Services.AddAutoMapper(typeof(ViewModelMappingProfile));

// Repositories
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorDepartmentRepository, DoctorDepartmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IFAQEntryRepository, FAQEntryRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<INurseRepository, NurseRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IPatientRequestRepository, PatientRequestRepository>();

// Application Services
builder.Services.AddScoped<FAQEntryService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<MedicalRecordService>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<PrescriptionService>();
builder.Services.AddScoped<NurseService>();
builder.Services.AddScoped<ShiftService>();
builder.Services.AddScoped<PatientRequestService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Infrastructure Service
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


// Identity configuration (use IdentityRole for role support)
// builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
// {
//     options.SignIn.RequireConfirmedAccount = true;
// })
// .AddEntityFrameworkStores<ApplicationDbContext>()
// .AddDefaultTokenProviders();

// Configure cookie policy for security
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    //options.Secure = CookieSecurePolicy.Always;
    options.Secure = CookieSecurePolicy.SameAsRequest;
    options.MinimumSameSitePolicy = SameSiteMode.Strict; // Prevent CSRF attacks
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");

    // Optional: basic Content-Security-Policy (adjust as needed)
    //context.Response.Headers.Add("Content-Security-Policy",
    //    "default-src 'self';");

    await next();
});

app.UseCookiePolicy();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Make sure this is before UseAuthorization
app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Needed if you?re using Identity UI or Razor Pages

async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { "Admin", "Doctor", "Patient", "Nurse" };

    foreach (var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var adminEmail = "admin@clinic.com";
    var adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Administrator",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        // Apply any pending migrations. This ensures the database exists and schema is up-to-date
        // before we attempt to use Identity services for seeding roles/users.
        var db = services.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        await SeedRolesAsync(services);
        await SeedAdminUserAsync(services);
    }
    catch (Exception ex)
    {
        // Log the detailed error and continue. This prevents the app from crashing on transient DB errors
        // such as localdb not available or login failures during development.
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();
