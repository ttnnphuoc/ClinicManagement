using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ClinicManagement.API.Middleware;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Infrastructure.Repositories;
using ClinicManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    // Role-based policies
    options.AddPolicy(ClinicManagement.API.Constants.Policies.SuperAdminOnly, policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ClinicManagerOnly, policy => policy.RequireRole("ClinicManager"));
    options.AddPolicy(ClinicManagement.API.Constants.Policies.DoctorOnly, policy => policy.RequireRole("Doctor"));
    options.AddPolicy(ClinicManagement.API.Constants.Policies.NurseOnly, policy => policy.RequireRole("Nurse"));
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ReceptionistOnly, policy => policy.RequireRole("Receptionist"));
    options.AddPolicy(ClinicManagement.API.Constants.Policies.AccountantOnly, policy => policy.RequireRole("Accountant"));
    options.AddPolicy(ClinicManagement.API.Constants.Policies.PharmacistOnly, policy => policy.RequireRole("Pharmacist"));

    // Combined policies
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ManageClinic, policy => 
        policy.RequireRole("SuperAdmin", "ClinicManager"));
    
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ManagePatients, policy => 
        policy.RequireRole("SuperAdmin", "ClinicManager", "Doctor", "Nurse", "Receptionist"));
    
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ManageAppointments, policy => 
        policy.RequireRole("SuperAdmin", "ClinicManager", "Doctor", "Nurse", "Receptionist"));
    
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ViewPatientRecords, policy => 
        policy.RequireRole("SuperAdmin", "ClinicManager", "Doctor", "Nurse"));
    
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ManageServices, policy => 
        policy.RequireRole("SuperAdmin", "ClinicManager"));
    
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ManageFinance, policy => 
        policy.RequireRole("SuperAdmin", "ClinicManager", "Accountant"));
    
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ManageInventory, policy => 
        policy.RequireRole("SuperAdmin", "ClinicManager", "Pharmacist"));
    
    options.AddPolicy(ClinicManagement.API.Constants.Policies.ManageStaff, policy => 
        policy.RequireRole("SuperAdmin", "ClinicManager"));
});

builder.Services.AddScoped<IClinicContext, ClinicContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStaffService, StaffService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var authService = services.GetRequiredService<IAuthService>();
    await DbSeeder.SeedAsync(context, authService);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseMiddleware<ClinicMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();