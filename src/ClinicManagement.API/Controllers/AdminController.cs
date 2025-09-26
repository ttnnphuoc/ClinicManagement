using Microsoft.AspNetCore.Mvc;
using ClinicManagement.API.DTOs;
using ClinicManagement.Core.Interfaces;
using ClinicManagement.Core.Enums;

namespace ClinicManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IStaffRepository _staffRepository;

    public AdminController(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    [HttpGet("check-user-role/{email}")]
    public async Task<IActionResult> CheckUserRole(string email)
    {
        try
        {
            var staff = await _staffRepository.GetByEmailAsync(email);
            if (staff == null)
            {
                return NotFound($"Staff with email {email} not found");
            }

            return Ok(new {
                Email = staff.Email,
                FullName = staff.FullName,
                Role = Enum.GetName<UserRole>(staff.Role),
                RoleValue = (int)staff.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error checking role: {ex.Message}");
        }
    }

    [HttpGet("check-user-role-by-id/{id}")]
    public async Task<IActionResult> CheckUserRoleById(Guid id)
    {
        try
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null)
            {
                return NotFound($"Staff with ID {id} not found");
            }

            return Ok(new {
                Id = staff.Id,
                Email = staff.Email,
                FullName = staff.FullName,
                Role = Enum.GetName<UserRole>(staff.Role),
                RoleValue = (int)staff.Role
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error checking role: {ex.Message}");
        }
    }
}