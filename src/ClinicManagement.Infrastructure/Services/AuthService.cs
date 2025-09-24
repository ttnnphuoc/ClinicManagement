using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IStaffRepository _staffRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IStaffRepository staffRepository, IConfiguration configuration)
    {
        _staffRepository = staffRepository;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Token, string FullName, string Email, string Role, List<Guid> ClinicIds)> LoginAsync(string emailOrPhone, string password)
    {
        var staff = await _staffRepository.GetByEmailOrPhoneWithClinicsAsync(emailOrPhone);

        if (staff == null)
        {
            return (false, string.Empty, string.Empty, string.Empty, string.Empty, new List<Guid>());
        }

        if (!VerifyPassword(password, staff.PasswordHash))
        {
            return (false, string.Empty, string.Empty, string.Empty, string.Empty, new List<Guid>());
        }

        var clinicIds = staff.StaffClinics
            .Where(sc => sc.IsActive)
            .Select(sc => sc.ClinicId)
            .ToList();

        var token = GenerateJwtToken(staff.Id.ToString(), staff.Email, staff.Role);
        return (true, token, staff.FullName, staff.Email, staff.Role, clinicIds);
    }

    public async Task<(bool Success, string Token, string FullName, string Email, string Role, Guid ClinicId)> LoginWithClinicAsync(string emailOrPhone, string password, Guid clinicId)
    {
        var staff = await _staffRepository.GetByEmailOrPhoneAsync(emailOrPhone);

        if (staff == null)
        {
            return (false, string.Empty, string.Empty, string.Empty, string.Empty, Guid.Empty);
        }

        if (!VerifyPassword(password, staff.PasswordHash))
        {
            return (false, string.Empty, string.Empty, string.Empty, string.Empty, Guid.Empty);
        }

        var hasAccess = await _staffRepository.HasAccessToClinicAsync(staff.Id, clinicId);
        if (!hasAccess)
        {
            return (false, string.Empty, string.Empty, string.Empty, string.Empty, Guid.Empty);
        }

        var token = GenerateJwtToken(staff.Id.ToString(), staff.Email, staff.Role, clinicId.ToString());
        return (true, token, staff.FullName, staff.Email, staff.Role, clinicId);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private string GenerateJwtToken(string userId, string email, string role, string? clinicId = null)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(clinicId))
        {
            claims.Add(new Claim("ClinicId", clinicId));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}