using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using USMS.API.Persistence;
using USMS.Common.Constants;
using USMS.DTOs.Auth;
using USMS.Entities;
using USMS.Persistence;

namespace USMS.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, AuthResponseDto? Data)> RegisterStudentAsync(RegisterDto dto);
    Task<(bool Success, string Message, AuthResponseDto? Data)> LoginAsync(LoginDto dto);
}

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _context = context;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Message, AuthResponseDto? Data)> RegisterStudentAsync(RegisterDto dto)
    {
        var userExists = await _userManager.FindByEmailAsync(dto.Email);
        if (userExists != null)
        {
            return (false, "Email is already in use.", null);
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            AccountStatus = "Active"
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var firstError = result.Errors.FirstOrDefault()?.Description ?? "Registration failed.";
            return (false, firstError, null);
        }

        await _userManager.AddToRoleAsync(user, AppRoles.Student);

        var student = new Student
        {
            UserId = user.Id,
            StudentNumber = dto.StudentNumber,
            FullName = dto.FullName
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var token = GenerateToken(user, new List<string> { AppRoles.Student });

        var responseData = new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Roles = new List<string> { AppRoles.Student },
            Token = token,
            ExpiresOn = DateTime.UtcNow.AddDays(7)
        };

        return (true, "Registration completed successfully.", responseData);
    }

    public async Task<(bool Success, string Message, AuthResponseDto? Data)> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return (false, "Invalid email or password.", null);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            return (false, "Invalid email or password.", null);
        }

        if (user.AccountStatus != "Active")
        {
            return (false, "Account is deactivated. Please contact support.", null);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateToken(user, roles);

        var responseData = new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList(),
            Token = token,
            ExpiresOn = DateTime.UtcNow.AddDays(7)
        };

        return (true, "Login successful.", responseData);
    }

    private string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var secretKey = _configuration["Jwt:Key"] ?? "USMS_Default_Secret_Key_For_Development_Only_2026";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}